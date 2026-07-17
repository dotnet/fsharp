#nowarn "57"

namespace FSharp.Compiler.Service.Tests.HotReload

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.IO
open System.Reflection
open System.Reflection.Emit
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Reflection.PortableExecutable
open System.Text
open Xunit

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditAndContinue
open FSharp.Compiler.HotReload
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Test
open FSharp.Test.Utilities

open FSharp.Compiler.Service.Tests.Common

/// Tests for the FSharpHotReloadSession entity (the F# DebuggingSession analogue):
/// independent session instances, per-project committed baselines and generation chains
/// inside one session, solution-wide commit/discard, and session-wide
/// capabilities/active statements.
[<Collection(nameof NotThreadSafeResourceCollection)>]
module HotReloadSessionTests =

    let private createChecker () =
        FSharpChecker.Create(
            keepAssemblyContents = true,
            keepAllBackgroundResolutions = false,
            keepAllBackgroundSymbolUses = false,
            enableBackgroundItemKeyStoreAndSemanticClassification = false,
            enablePartialTypeChecking = false,
            captureIdentifiersWhenParsing = false,
            useTransparentCompiler = CompilerAssertHelpers.UseTransparentCompiler
        )

    /// Project options with the output passed as `-o:` so the project identity
    /// (FSharpProjectIdentifier = projectFileName * "-o:" output) carries the output path.
    let private prepareProjectOptions
        (checker: FSharpChecker)
        (fsPath: string)
        (dllPath: string)
        (source: string)
        (extraOptions: string list)
        =
        let projectOptions, _ =
            checker.GetProjectOptionsFromScript(
                fsPath,
                SourceText.ofString source,
                assumeDotNetFramework = false,
                useSdkRefs = true,
                useFsiAuxLib = false
            )
            |> Async.RunImmediate

        { projectOptions with
            SourceFiles = [| fsPath |]
            OtherOptions =
                projectOptions.OtherOptions
                |> Array.append (List.toArray extraOptions)
                |> Array.append
                    [| "--target:library"
                       "--langversion:preview"
                       "--optimize-"
                       "--debug:portable"
                       "--deterministic"
                       "--test:HotReloadDeltas"
                       $"-o:{dllPath}" |] }

    let private compileProject
        (checker: FSharpChecker)
        (projectOptions: FSharpProjectOptions)
        (includeHotReloadCapture: bool)
        =
        let options =
            if includeHotReloadCapture then
                projectOptions.OtherOptions
            else
                projectOptions.OtherOptions
                |> Array.filter (fun opt ->
                    not (opt.StartsWith("--test:HotReloadDeltas", StringComparison.OrdinalIgnoreCase)))

        let argv =
            Array.concat [ [| "fsc.exe" |]; options; projectOptions.SourceFiles ]

        let diagnostics, exOpt = checker.Compile(argv) |> Async.RunImmediate

        let errors =
            diagnostics
            |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

        match errors, exOpt with
        | [||], None -> ()
        | errs, _ -> failwithf "Compilation failed: %A" (errs |> Array.map (fun d -> d.Message))

    let private createProjectSnapshot (projectOptions: FSharpProjectOptions) =
        FSharpProjectSnapshot.FromOptions(projectOptions, DocumentSource.FileSystem)
        |> Async.RunImmediate

    let private prepareMultiFileProjectOptions
        (checker: FSharpChecker)
        (fsPaths: string array)
        (dllPath: string)
        (firstSource: string)
        (extraOptions: string list)
        =
        let options = prepareProjectOptions checker fsPaths[0] dllPath firstSource extraOptions
        { options with SourceFiles = fsPaths }

    let private withProjectDir (testName: string) (action: string -> unit) =
        let projectDir =
            Path.Combine(Path.GetTempPath(), testName, Guid.NewGuid().ToString("N"))

        Directory.CreateDirectory(projectDir) |> ignore

        try
            action projectDir
        finally
            try
                Directory.Delete(projectDir, true)
            with _ ->
                ()

    let private withProjectDirReturning (testName: string) (action: string -> 'T) =
        let projectDir =
            Path.Combine(Path.GetTempPath(), testName, Guid.NewGuid().ToString("N"))

        Directory.CreateDirectory(projectDir) |> ignore

        try
            action projectDir
        finally
            try
                Directory.Delete(projectDir, true)
            with _ ->
                ()

    let private writeAndCompile (checker: FSharpChecker) (fsPath: string) (options: FSharpProjectOptions) (source: string) capture =
        File.WriteAllText(fsPath, source)
        checker.NotifyFileChanged(fsPath, options) |> Async.RunImmediate
        compileProject checker options capture

    let private withEnvVar name value action =
        let previous = Environment.GetEnvironmentVariable name

        try
            Environment.SetEnvironmentVariable(name, value)
            action ()
        finally
            Environment.SetEnvironmentVariable(name, previous)

    let private assertPortablePdbWithMethodDebugInfo (pdbBytes: byte[] option) =
        let bytes =
            match pdbBytes with
            | Some bytes -> bytes
            | None -> failwith "Expected portable PDB delta."

        use provider = MetadataReaderProvider.FromPortablePdbImage(ImmutableArray.CreateRange bytes)
        let reader = provider.GetMetadataReader()
        Assert.True(reader.MethodDebugInformation.Count > 0, "Expected method debug information in portable PDB delta.")

    type private G1LdstrOperand =
        {
            InstructionOffset: int
            Token: int
            Offset: int
        }

    type private G1UserStringEntry =
        {
            RelativeOffset: int
            AbsoluteOffset: int
            Value: string
            Bytes: string
        }

    type private G1BaselineDecode =
        {
            UserStringHeapSize: int
            UpdatedMethod: string
            Ldstrs: G1LdstrOperand list
            UserStrings: (int * string) list
        }

    type private G1DeltaDecode =
        {
            EncLog: (int * int) list
            EncMap: int list
            MethodRows: (int * string) list
            UserStrings: G1UserStringEntry list
            Ldstrs: G1LdstrOperand list
            UserStringUpdates: (int * int * string) list
        }

    type private G1CaseDecode =
        {
            Label: string
            UpdatedMethods: int list
            Baseline: G1BaselineDecode
            Delta: G1DeltaDecode
        }

    let private formatToken token = sprintf "0x%08X" token

    let private opcodeOperandTypes =
        lazy
            let table = Dictionary<int, OperandType>()

            for field in typeof<OpCodes>.GetFields(BindingFlags.Public ||| BindingFlags.Static) do
                match field.GetValue null with
                | :? OpCode as opCode ->
                    let key = (int opCode.Value) &&& 0xFFFF
                    table.[key] <- opCode.OperandType
                | _ -> ()

            table

    let private findLdstrOperands (ilBytes: byte[]) =
        let operands = ResizeArray<G1LdstrOperand>()
        let mutable offset = 0

        let advance count =
            offset <- min ilBytes.Length (offset + count)

        while offset < ilBytes.Length do
            let instructionOffset = offset
            let first = int ilBytes.[offset]
            offset <- offset + 1

            let opcode =
                if first = 0xFE && offset < ilBytes.Length then
                    let second = int ilBytes.[offset]
                    offset <- offset + 1
                    0xFE00 ||| second
                else
                    first

            let operandType =
                match opcodeOperandTypes.Value.TryGetValue opcode with
                | true, operandType -> operandType
                | _ -> OperandType.InlineNone

            let operandOffset = offset

            match operandType with
            | OperandType.InlineNone -> ()
            | OperandType.ShortInlineI
            | OperandType.ShortInlineBrTarget
            | OperandType.ShortInlineVar -> advance 1
            | OperandType.InlineVar -> advance 2
            | OperandType.InlineI
            | OperandType.ShortInlineR
            | OperandType.InlineBrTarget
            | OperandType.InlineField
            | OperandType.InlineMethod
            | OperandType.InlineSig
            | OperandType.InlineTok
            | OperandType.InlineType -> advance 4
            | OperandType.InlineI8
            | OperandType.InlineR -> advance 8
            | OperandType.InlineString ->
                let token = BitConverter.ToInt32(ilBytes, operandOffset)
                let offsetValue = token &&& 0x00FFFFFF
                operands.Add(
                    {
                        InstructionOffset = instructionOffset
                        Token = token
                        Offset = offsetValue
                    })

                advance 4
            | OperandType.InlineSwitch ->
                if operandOffset + 4 <= ilBytes.Length then
                    let count = BitConverter.ToInt32(ilBytes, operandOffset)
                    advance (4 + count * 4)
                else
                    advance 4
            | _ -> ()

        operands |> Seq.toList

    let private readCompressedUInt (bytes: byte[]) offset =
        let b0 = int bytes.[offset]

        if (b0 &&& 0x80) = 0 then
            b0, 1
        elif (b0 &&& 0xC0) = 0x80 then
            ((b0 &&& 0x3F) <<< 8) ||| int bytes.[offset + 1], 2
        else
            ((b0 &&& 0x1F) <<< 24)
            ||| (int bytes.[offset + 1] <<< 16)
            ||| (int bytes.[offset + 2] <<< 8)
            ||| int bytes.[offset + 3],
            4

    let private decodeUserStringHeapEntries baselineUserStringHeapSize (heapBytes: byte[]) =
        let entries = ResizeArray<G1UserStringEntry>()
        let mutable offset = 1

        while offset < heapBytes.Length do
            if heapBytes.[offset] = 0uy then
                offset <- heapBytes.Length
            else
                let length, prefixLength = readCompressedUInt heapBytes offset
                let textByteLength = max 0 (length - 1)
                let textOffset = offset + prefixLength
                let nextOffset = textOffset + length

                if nextOffset <= heapBytes.Length then
                    let value = Encoding.Unicode.GetString(heapBytes, textOffset, textByteLength)
                    let encodedLength = prefixLength + length

                    let encodedBytes =
                        heapBytes.[offset .. offset + encodedLength - 1]
                        |> Array.map (sprintf "%02X")
                        |> String.concat " "

                    entries.Add(
                        {
                            RelativeOffset = offset
                            AbsoluteOffset = baselineUserStringHeapSize + offset
                            Value = value
                            Bytes = encodedBytes
                        })

                    offset <- offset + encodedLength
                else
                    offset <- heapBytes.Length

        entries |> Seq.toList

    let private tryGetMetadataStreamBytes streamName (metadata: byte[]) =
        use stream = new MemoryStream(metadata, false)
        use reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen = true)

        let align4 () =
            while stream.Position % 4L <> 0L do
                reader.ReadByte() |> ignore

        let readStreamName () =
            let bytes = ResizeArray<byte>()
            let mutable b = reader.ReadByte()

            while b <> 0uy do
                bytes.Add b
                b <- reader.ReadByte()

            align4 ()
            Encoding.UTF8.GetString(bytes.ToArray())

        try
            let signature = reader.ReadUInt32()

            if signature <> 0x424A5342u then
                None
            else
                reader.ReadUInt16() |> ignore
                reader.ReadUInt16() |> ignore
                reader.ReadUInt32() |> ignore
                let versionLength = int (reader.ReadUInt32())
                reader.ReadBytes(versionLength) |> ignore
                align4 ()
                reader.ReadUInt16() |> ignore
                let streamCount = int (reader.ReadUInt16())
                let mutable found = None

                for _ = 1 to streamCount do
                    let offset = int (reader.ReadUInt32())
                    let size = int (reader.ReadUInt32())
                    let name = readStreamName ()

                    if name = streamName && offset >= 0 && size >= 0 && offset + size <= metadata.Length then
                        found <- Some(Array.sub metadata offset size)

                found
        with _ ->
            None

    let private methodDisplayName (reader: MetadataReader) (handle: MethodDefinitionHandle) =
        let methodDef = reader.GetMethodDefinition handle
        let declaringType = reader.GetTypeDefinition(methodDef.GetDeclaringType())
        let ns = reader.GetString declaringType.Namespace
        let typeName = reader.GetString declaringType.Name
        let methodName = reader.GetString methodDef.Name

        if String.IsNullOrEmpty ns then
            $"{typeName}::{methodName}"
        else
            $"{ns}.{typeName}::{methodName}"

    let private readTypeNames (assemblyBytes: byte[]) =
        use stream = new MemoryStream(assemblyBytes, false)
        use peReader = new PEReader(stream)
        let reader = peReader.GetMetadataReader()

        let rec buildName (handle: TypeDefinitionHandle) =
            let typeDef = reader.GetTypeDefinition handle
            let name = reader.GetString typeDef.Name

            let visibility = typeDef.Attributes &&& TypeAttributes.VisibilityMask

            let isNested =
                match visibility with
                | TypeAttributes.NestedPublic
                | TypeAttributes.NestedPrivate
                | TypeAttributes.NestedFamily
                | TypeAttributes.NestedAssembly
                | TypeAttributes.NestedFamORAssem
                | TypeAttributes.NestedFamANDAssem -> true
                | _ -> false

            if isNested then
                $"{buildName (typeDef.GetDeclaringType())}+{name}"
            else
                let ns = reader.GetString typeDef.Namespace

                if String.IsNullOrEmpty ns then
                    name
                else
                    $"{ns}.{name}"

        [ for handle in reader.TypeDefinitions -> buildName handle ]

    let private assertMatchingTypeNames testLabel familyName predicate baselineTypeNames freshTypeNames =
        let filteredBaseline: string list =
            baselineTypeNames
            |> List.filter predicate
            |> List.sort

        let filteredFresh: string list =
            freshTypeNames
            |> List.filter predicate
            |> List.sort

        let format names = names |> String.concat "; "

        Assert.NotEmpty filteredBaseline
        Assert.Equal<string list>(filteredBaseline, filteredFresh)
        printfn "[%s] %s synthesized TypeDefs: %s" testLabel familyName (format filteredBaseline)

    let private assertReplayEndpointTypeNames testLabel (typeNames: string list) =
        let endpointNames =
            typeNames
            |> List.filter (fun name -> name.Contains("endpoints@hotreload"))
            |> List.sort

        Assert.NotEmpty endpointNames

        Assert.Contains(
            endpointNames,
            fun name ->
                name.Contains("endpoints@hotreload")
                && not (name.Contains("@hotreload#g0_o")))

        printfn "[%s] replay endpoint synthesized TypeDefs: %s" testLabel (endpointNames |> String.concat "; ")
        endpointNames

    let private assertMixedEndpointTypeNames testLabel (typeNames: string list) =
        let endpointNames =
            typeNames
            |> List.filter (fun name -> name.Contains("endpoints@hotreload"))
            |> List.sort

        Assert.NotEmpty endpointNames

        Assert.Contains(
            endpointNames,
            fun name -> name.Contains("@hotreload#g0_o"))

        Assert.Contains(
            endpointNames,
            fun name ->
                name.Contains("endpoints@hotreload")
                && not (name.Contains("@hotreload#g0_o")))

        printfn "[%s] mixed endpoint synthesized TypeDefs: %s" testLabel (endpointNames |> String.concat "; ")
        endpointNames

    let private expectedMixedEndpointSnapshotNames =
        [|
            "endpoints@hotreload"
            "endpoints@hotreload#g0_o0"
            "endpoints@hotreload-2"
            "endpoints@hotreload#g0_o1"
            "endpoints@hotreload-4"
            "endpoints@hotreload#g0_o2"
            "endpoints@hotreload#g0_o3"
            "endpoints@hotreload#g0_o4"
        |]

    let private assertMixedEndpointSnapshot testLabel (snapshot: Map<string, string[]>) =
        let bucketCount = Map.count snapshot
        Assert.True(bucketCount > 3, $"[%s{testLabel}] expected more than the old three recorded buckets, got %d{bucketCount}")

        match Map.tryFind "endpoints" snapshot with
        | Some endpointNames ->
            Assert.Equal<string[]>(expectedMixedEndpointSnapshotNames, endpointNames)
            printfn "[%s] mixed endpoint snapshot bucket: %s" testLabel (endpointNames |> String.concat "; ")
            endpointNames
        | None ->
            let keys = snapshot |> Map.toList |> List.map fst |> String.concat "; "
            failwithf "[%s] expected an endpoints synthesized-name snapshot bucket; keys: %s" testLabel keys

    let private assertSynthesizedNameSnapshotsEqual
        testLabel
        (expected: Map<string, string[]>)
        (actual: Map<string, string[]>)
        =
        let expectedKeys = expected |> Map.toArray |> Array.map fst |> Array.sort
        let actualKeys = actual |> Map.toArray |> Array.map fst |> Array.sort

        Assert.Equal<string[]>(expectedKeys, actualKeys)

        for key in expectedKeys do
            Assert.Equal<string[]>(Map.find key expected, Map.find key actual)

        printfn "[%s] synthesized-name snapshot buckets: %d" testLabel expectedKeys.Length

    let private readRecordedSynthesizedNameSnapshot testLabel pdbPath =
        Assert.True(File.Exists pdbPath, $"[%s{testLabel}] expected portable PDB at %s{pdbPath}")

        match
            FSharp.Compiler.EncMethodDebugInformation.readSynthesizedNameSnapshotFromPortablePdb (File.ReadAllBytes pdbPath)
        with
        | Some snapshot -> snapshot
        | None -> failwithf "[%s] expected synthesized-name snapshot CDI in %s" testLabel pdbPath

    let private decodeBaselineMethod (assemblyBytes: byte[]) methodToken =
        use stream = new MemoryStream(assemblyBytes, false)
        use peReader = new PEReader(stream)
        let reader = peReader.GetMetadataReader()
        let userStringHeapSize = reader.GetHeapSize HeapIndex.UserString
        let methodHandle = MetadataTokens.MethodDefinitionHandle(methodToken &&& 0x00FFFFFF)
        let methodDef = reader.GetMethodDefinition methodHandle
        let methodBody = peReader.GetMethodBody methodDef.RelativeVirtualAddress

        let ldstrs =
            methodBody.GetILBytes()
            |> findLdstrOperands

        let userStrings =
            ldstrs
            |> List.map (fun operand ->
                operand.Offset,
                reader.GetUserString(MetadataTokens.UserStringHandle operand.Offset))

        {
            UserStringHeapSize = userStringHeapSize
            UpdatedMethod = methodDisplayName reader methodHandle
            Ldstrs = ldstrs
            UserStrings = userStrings
        }

    let private extractDeltaMethodIl (delta: FSharpHotReloadDelta) (methodInfo: FSharpAddedOrChangedMethodInfo) =
        let headerOffset = methodInfo.CodeOffset
        let first = delta.IL.[headerOffset]
        let format = first &&& 0x03uy

        if format = 0x02uy then
            let codeSize = int (first >>> 2)
            Array.sub delta.IL (headerOffset + 1) codeSize
        elif format = 0x03uy then
            let headerSize = int (delta.IL.[headerOffset + 1] >>> 4) * 4
            let codeSize = BitConverter.ToInt32(delta.IL, headerOffset + 4)
            Assert.Equal(methodInfo.CodeLength, codeSize)
            Array.sub delta.IL (headerOffset + headerSize) codeSize
        else
            failwithf "Unsupported method body header format 0x%02X at delta IL offset %d" format headerOffset

    let private decodeDelta baselineUserStringHeapSize (delta: FSharpHotReloadDelta) =
        let methodInfo = Assert.Single(delta.AddedOrChangedMethods)

        use provider =
            MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte> delta.Metadata)

        let reader = provider.GetMetadataReader()

        let encLog =
            reader.GetEditAndContinueLogEntries()
            |> Seq.map (fun entry -> MetadataTokens.GetToken entry.Handle, int entry.Operation)
            |> Seq.toList

        let encMap =
            reader.GetEditAndContinueMapEntries()
            |> Seq.map MetadataTokens.GetToken
            |> Seq.toList

        let methodRows =
            reader.MethodDefinitions
            |> Seq.map (fun handle -> MetadataTokens.GetToken(EntityHandle.op_Implicit handle), "<delta MethodDef row>")
            |> Seq.toList

        let userStringHeapBytes =
            tryGetMetadataStreamBytes "#US" delta.Metadata
            |> Option.defaultValue Array.empty

        let methodIl = extractDeltaMethodIl delta methodInfo

        {
            EncLog = encLog
            EncMap = encMap
            MethodRows = methodRows
            UserStrings = decodeUserStringHeapEntries baselineUserStringHeapSize userStringHeapBytes
            Ldstrs = findLdstrOperands methodIl
            UserStringUpdates = delta.UserStringUpdates |> List.map (fun struct (oldToken, newToken, value) -> oldToken, newToken, value)
        }

    let private formatLdstrs ldstrs =
        ldstrs
        |> List.map (fun operand ->
            sprintf "il+0x%04X token=%s offset=%d" operand.InstructionOffset (formatToken operand.Token) operand.Offset)
        |> String.concat "; "

    let private printG1CaseDecode decode =
        printfn "[g1] case=%s" decode.Label
        printfn "[g1] updatedMethods=%s" (decode.UpdatedMethods |> List.map formatToken |> String.concat ", ")
        printfn "[g1] baselineMethod=%s" decode.Baseline.UpdatedMethod
        printfn "[g1] baselineUSSize=%d" decode.Baseline.UserStringHeapSize
        printfn "[g1] baselineLdstr=%s" (formatLdstrs decode.Baseline.Ldstrs)

        for offset, value in decode.Baseline.UserStrings do
            printfn "[g1] baselineUserString offset=%d value=%A" offset value

        printfn
            "[g1] deltaEncLog=%s"
            (decode.Delta.EncLog
             |> List.map (fun (token, op) -> sprintf "(%s, op=%d)" (formatToken token) op)
             |> String.concat ", ")

        printfn
            "[g1] deltaEncMap=%s"
            (decode.Delta.EncMap |> List.map formatToken |> String.concat ", ")

        for token, name in decode.Delta.MethodRows do
            printfn "[g1] deltaMethodRow token=%s name=%s" (formatToken token) name

        for entry in decode.Delta.UserStrings do
            printfn
                "[g1] deltaUserString relative=%d absolute=%d value=%A bytes=%s"
                entry.RelativeOffset
                entry.AbsoluteOffset
                entry.Value
                entry.Bytes

        printfn "[g1] deltaLdstr=%s" (formatLdstrs decode.Delta.Ldstrs)

        for oldToken, newToken, value in decode.Delta.UserStringUpdates do
            printfn "[g1] userStringUpdate original=%s new=%s value=%A" (formatToken oldToken) (formatToken newToken) value

    let private checkProjectOrFail (checker: FSharpChecker) (options: FSharpProjectOptions) =
        let results =
            checker.ParseAndCheckProject(options)
            |> Async.RunImmediate

        let errors =
            results.Diagnostics
            |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

        if results.HasCriticalErrors || errors.Length > 0 then
            failwithf "Project check failed: %A" (errors |> Array.map (fun d -> d.Message))

        results

    let private compileFromCheckedProjectAndReadBytes (checker: FSharpChecker) (results: FSharpCheckProjectResults) (outfile: string) =
        checker.CompileFromCheckedProject(results, outfile, HotReloadEmitNaming.ClearForLineBasedBaseline)
        |> Async.RunImmediate
        |> ignore

        File.ReadAllBytes(outfile)

    let private addProjectOrFail (session: FSharpHotReloadSession) snapshot =
        match session.AddProject(snapshot) |> Async.RunImmediate with
        | Ok() -> ()
        | Error error -> failwithf "AddProject failed: %A" error

    let private emitOrFail (session: FSharpHotReloadSession) snapshot =
        match session.EmitDelta(snapshot) |> Async.RunImmediate with
        | Ok delta -> delta
        | Error error -> failwithf "EmitDelta failed: %A" error

    let private runG1SessionCase (label: string) (baselineSource: string) (editedSource: string) =
        withProjectDirReturning $"fcs-hotreload-g1-{label}" (fun projectDir ->
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")
            let checker = createChecker ()
            let trimmedBaselineSource = baselineSource.TrimStart()
            let trimmedEditedSource = editedSource.TrimStart()

            File.WriteAllText(fsPath, trimmedBaselineSource)
            let options = prepareProjectOptions checker fsPath dllPath trimmedBaselineSource []

            checker.InvalidateAll()
            compileProject checker options true

            let baselineBytes = File.ReadAllBytes dllPath

            use session = checker.CreateHotReloadSession()
            addProjectOrFail session (createProjectSnapshot options)

            let delta =
                withEnvVar "FSHARP_HOTRELOAD_INPROCESS_COMPILE" "1" (fun () ->
                    File.WriteAllText(fsPath, trimmedEditedSource)
                    checker.NotifyFileChanged(fsPath, options) |> Async.RunImmediate
                    emitOrFail session (createProjectSnapshot options))

            Assert.Equal<string list>([ "Baseline" ], delta.RequiredCapabilities)
            Assert.Equal(1, delta.UpdatedMethods.Length)
            let updatedMethod = Assert.Single(delta.UpdatedMethods)
            let baseline = decodeBaselineMethod baselineBytes updatedMethod
            let deltaDecode = decodeDelta baseline.UserStringHeapSize delta

            let decode =
                {
                    Label = label
                    UpdatedMethods = delta.UpdatedMethods
                    Baseline = baseline
                    Delta = deltaDecode
                }

            printG1CaseDecode decode
            decode)

    let private projectViewOrFail (session: FSharpHotReloadSession) snapshot =
        match session.TryGetProjectView(snapshot) with
        | ValueSome view -> view
        | ValueNone -> failwith "Expected a session view for the project."

    let private implFiles (CheckedAssemblyAfterOptimization files) =
        files |> List.map (fun file -> file.ImplFile)

    let private libSource (generation: int) =
        $"""
module SessionLib

let libValue () = "lib generation {generation}"
"""

    let private appSource (generation: int) =
        $"""
module SessionApp

let appValue () = "app generation {generation}"
"""

    let private generatedIncrementalEmitSources generation =
        [|
            "File01_Common.fs",
            """
namespace IncEmit

module Common =
    [<Literal>]
    let LiteralValue = 7

    let addLiteral value = value + LiteralValue
"""
            "File02_AnonProducer.fs",
            """
namespace IncEmit

module AnonProducer =
    let makeAnon name count = {| Name = name; Count = count |}

    let pipeHeavy values =
        values
        |> List.map (fun value -> value + Common.LiteralValue)
        |> List.filter (fun value -> value % 2 = 0)
        |> List.collect (fun value -> [ value; value * 2 ])
        |> List.fold (fun state value -> state + value) 0
"""
            "File03_AnonConsumer.fs",
            """
namespace IncEmit

module AnonConsumer =
    let localAnon count = {| Name = "local"; Count = count |}

    let consume () =
        let produced = AnonProducer.makeAnon "shared" 3
        let local = localAnon 4
        produced.Count + local.Count + Common.addLiteral 1
"""
            "File04_Chain01.fs",
            """
namespace IncEmit

module Chain01 =
    let value () = AnonConsumer.consume ()
"""
            "File05_Chain02.fs",
            """
namespace IncEmit

module Chain02 =
    let value () = Chain01.value () + AnonProducer.pipeHeavy [ 1; 2; 3; 4 ]
"""
            "File06_Chain03.fs",
            """
namespace IncEmit

module Chain03 =
    let value () = Chain02.value () + Common.addLiteral 5
"""
            "File07_Chain04.fs",
            """
namespace IncEmit

module Chain04 =
    let choose f x = f x
    let value () = choose (fun value -> value + 1) (Chain03.value ())
"""
            "File08_Chain05.fs",
            """
namespace IncEmit

module Chain05 =
    let value () =
        [ 1 .. 5 ]
        |> List.map (fun n -> n * Chain04.value ())
        |> List.sum
"""
            "File09_Chain06.fs",
            """
namespace IncEmit

module Chain06 =
    let value () = Chain05.value () / Common.LiteralValue
"""
            "File10_Chain07.fs",
            """
namespace IncEmit

module Chain07 =
    let value () =
        let anon = {| Name = "consumer"; Count = Chain06.value () |}
        anon.Count + anon.Name.Length
"""
            "File11_Chain08.fs",
            """
namespace IncEmit

module Chain08 =
    let value () = Chain07.value () + Chain06.value ()
"""
            "File12_Entry.fs",
            $"""
namespace IncEmit

module Entry =
    let mutable sink = 0

    let run () =
        let value = Chain08.value ()
        sink <- value
        sprintf "generation {generation} value %%d" value
"""
        |]

    let private writeIncrementalEmitSources projectDir generation =
        generatedIncrementalEmitSources generation
        |> Array.map (fun (name, source) ->
            let path = Path.Combine(projectDir, name)
            File.WriteAllText(path, source.TrimStart())
            path)

    [<Fact>]
    let ``CompileFromCheckedProject incremental emit cache matches batch optimization bytes after one file edit`` () =
        withProjectDir "fcs-hotreload-incremental-emit-equivalence" (fun projectDir ->
            let checker = createChecker ()
            let fsPaths = writeIncrementalEmitSources projectDir 0
            let dllPath = Path.Combine(projectDir, "IncrementalEmit.dll")
            let options = prepareMultiFileProjectOptions checker fsPaths dllPath (snd (generatedIncrementalEmitSources 0).[0]) []

            checker.InvalidateAll()

            let warmResults = checkProjectOrFail checker options

            withEnvVar "FSHARP_HOTRELOAD_INCREMENTAL_EMIT" "1" (fun () ->
                compileFromCheckedProjectAndReadBytes checker warmResults dllPath |> ignore)

            let _, updatedEntry = (generatedIncrementalEmitSources 1).[11]
            File.WriteAllText(fsPaths[11], updatedEntry.TrimStart())
            checker.NotifyFileChanged(fsPaths[11], options) |> Async.RunImmediate

            let editedResults = checkProjectOrFail checker options

            let incrementalBytes =
                withEnvVar "FSHARP_HOTRELOAD_INCREMENTAL_EMIT" "1" (fun () ->
                    compileFromCheckedProjectAndReadBytes checker editedResults dllPath)

            let batchBytes =
                withEnvVar "FSHARP_HOTRELOAD_INCREMENTAL_EMIT" null (fun () ->
                    compileFromCheckedProjectAndReadBytes checker editedResults dllPath)

            Assert.Equal<byte>(batchBytes, incrementalBytes))

    [<Fact>]
    let ``CompileFromCheckedProject pins deterministic sequential codegen for cached FCS configuration`` () =
        withProjectDir "fcs-hotreload-inprocess-determinism" (fun projectDir ->
            let firstPath = Path.Combine(projectDir, "First.fs")
            let secondPath = Path.Combine(projectDir, "Second.fs")
            let dllPath = Path.Combine(projectDir, "Deterministic.dll")

            let firstSource =
                """
module First

let map values = values |> List.map (fun value -> value + 1)
"""

            let secondSource =
                """
module Second

let map values = values |> List.map (fun value -> value + 2)
"""

            File.WriteAllText(firstPath, firstSource.TrimStart())
            File.WriteAllText(secondPath, secondSource.TrimStart())

            let checker = createChecker ()

            let options =
                prepareMultiFileProjectOptions checker [| firstPath; secondPath |] dllPath firstSource []

            // FCS materializes its immutable TcConfig before fsc's hot reload pin runs. Keep
            // that real configuration here so this test exercises the in-process override.
            let options =
                { options with
                    OtherOptions =
                        options.OtherOptions
                        |> Array.filter (fun option -> option <> "--deterministic")
                }

            let results = checkProjectOrFail checker options
            let tcConfig, _, _, _, _, _, _, _ = results.CompilationData
            Assert.True(tcConfig.parallelIlxGen, "Expected the cached FCS configuration to retain parallel IlxGen.")

            let firstAssembly = compileFromCheckedProjectAndReadBytes checker results dllPath
            let firstPdb = File.ReadAllBytes(Path.ChangeExtension(dllPath, ".pdb"))
            let secondAssembly = compileFromCheckedProjectAndReadBytes checker results dllPath
            let secondPdb = File.ReadAllBytes(Path.ChangeExtension(dllPath, ".pdb"))

            Assert.Equal<byte>(firstAssembly, secondAssembly)
            Assert.Equal<byte>(firstPdb, secondPdb))

    [<Fact>]
    let ``EmitDelta reuses checked implementation files for unchanged earlier files`` () =
        withProjectDir "fcs-hotreload-session-reference-equality" (fun projectDir ->
            let fileAPath = Path.Combine(projectDir, "FileA.fs")
            let fileBPath = Path.Combine(projectDir, "FileB.fs")
            let fileCPath = Path.Combine(projectDir, "FileC.fs")
            let dllPath = Path.Combine(projectDir, "ReferenceEquality.dll")

            let fileASource =
                """
module FileA

let baseValue = 41
"""

            let fileBSource =
                """
module FileB

let derived () = FileA.baseValue + 1
"""

            let fileCSource generation =
                $"""
module FileC

let current () = FileB.derived () + {generation}
"""

            File.WriteAllText(fileAPath, fileASource)
            File.WriteAllText(fileBPath, fileBSource)
            File.WriteAllText(fileCPath, fileCSource 0)

            let checker = createChecker ()
            let sourcePaths = [| fileAPath; fileBPath; fileCPath |]
            let options = prepareMultiFileProjectOptions checker sourcePaths dllPath fileASource []
            let snapshot = createProjectSnapshot options

            checker.InvalidateAll()
            compileProject checker options true

            use session = checker.CreateHotReloadSession()
            addProjectOrFail session snapshot

            let baselineFiles =
                (projectViewOrFail session snapshot).ImplementationFiles |> implFiles

            let previousFlag =
                Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_INPROCESS_COMPILE")

            try
                Environment.SetEnvironmentVariable("FSHARP_HOTRELOAD_INPROCESS_COMPILE", "1")

                File.WriteAllText(fileCPath, fileCSource 1)
                checker.NotifyFileChanged(fileCPath, options) |> Async.RunImmediate

                let delta = emitOrFail session (createProjectSnapshot options)
                Assert.NotEmpty(delta.UpdatedMethods)

                let freshFiles =
                    match (projectViewOrFail session snapshot).PendingUpdate with
                    | Some(FSharp.Compiler.HotReloadState.Delta pending) ->
                        match pending.ImplementationFiles with
                        | Some implementationFiles -> implFiles implementationFiles
                        | None -> failwith "Expected pending implementation files after EmitDelta."
                    | Some(FSharp.Compiler.HotReloadState.LineOnly _) ->
                        failwith "Expected a metadata delta pending update after EmitDelta."
                    | None -> failwith "Expected a pending update after EmitDelta."

                let equalityPairs =
                    List.zip baselineFiles freshFiles
                    |> List.map (fun (baselineFile, freshFile) -> obj.ReferenceEquals(baselineFile, freshFile))

                let referenceEqualCount = equalityPairs |> List.filter id |> List.length
                printfn "[fsharp-hotreload][typed-tree-reference-equality] unchanged-file hit rate: %d/%d" referenceEqualCount equalityPairs.Length

                Assert.Equal<bool list>([ true; true; false ], equalityPairs)
                session.Discard()
            finally
                Environment.SetEnvironmentVariable("FSHARP_HOTRELOAD_INPROCESS_COMPILE", previousFlag))

    [<Fact>]
    let ``Two independent sessions emit deltas without interference`` () =
        withProjectDir "fcs-hotreload-session-independent" (fun projectDir ->
            let fsPathA = Path.Combine(projectDir, "LibraryA.fs")
            let dllPathA = Path.Combine(projectDir, "LibraryA.dll")
            let fsPathB = Path.Combine(projectDir, "LibraryB.fs")
            let dllPathB = Path.Combine(projectDir, "LibraryB.dll")

            File.WriteAllText(fsPathA, libSource 0)
            File.WriteAllText(fsPathB, appSource 0)

            let checker = createChecker ()
            let optionsA = prepareProjectOptions checker fsPathA dllPathA (libSource 0) []
            let optionsB = prepareProjectOptions checker fsPathB dllPathB (appSource 0) []

            checker.InvalidateAll()
            compileProject checker optionsA true
            compileProject checker optionsB true

            use sessionA = checker.CreateHotReloadSession()
            use sessionB = checker.CreateHotReloadSession()

            let snapshotA = createProjectSnapshot optionsA
            let snapshotB = createProjectSnapshot optionsB
            addProjectOrFail sessionA snapshotA
            addProjectOrFail sessionB snapshotB

            // Each session tracks exactly its own project, keyed by the snapshot identity.
            Assert.Equal<FSharpProjectIdentifier list>([ snapshotA.Identifier ], sessionA.ProjectIdentifiers)
            Assert.Equal<FSharpProjectIdentifier list>([ snapshotB.Identifier ], sessionB.ProjectIdentifiers)

            // Edit + emit on session A.
            writeAndCompile checker fsPathA optionsA (libSource 1) false
            let deltaA1 = emitOrFail sessionA (createProjectSnapshot optionsA)
            Assert.NotEmpty(deltaA1.Metadata)
            Assert.NotEmpty(deltaA1.UpdatedMethods)
            sessionA.Commit()

            // Edit + emit on session B, unaffected by session A's activity.
            writeAndCompile checker fsPathB optionsB (appSource 1) false
            let deltaB1 = emitOrFail sessionB (createProjectSnapshot optionsB)
            Assert.NotEmpty(deltaB1.Metadata)
            Assert.NotEmpty(deltaB1.UpdatedMethods)
            sessionB.Commit()

            // Session A chains its own generations: the next delta builds on A's gen-1 id even
            // though session B emitted in between.
            writeAndCompile checker fsPathA optionsA (libSource 2) false
            let deltaA2 = emitOrFail sessionA (createProjectSnapshot optionsA)
            Assert.Equal(deltaA1.GenerationId, deltaA2.BaseGenerationId)
            sessionA.Commit())

    [<Fact>]
    let ``Disposing a session ends it without affecting other sessions`` () =
        withProjectDir "fcs-hotreload-session-dispose" (fun projectDir ->
            let fsPathA = Path.Combine(projectDir, "LibraryA.fs")
            let dllPathA = Path.Combine(projectDir, "LibraryA.dll")
            let fsPathB = Path.Combine(projectDir, "LibraryB.fs")
            let dllPathB = Path.Combine(projectDir, "LibraryB.dll")

            File.WriteAllText(fsPathA, libSource 0)
            File.WriteAllText(fsPathB, appSource 0)

            let checker = createChecker ()
            let optionsA = prepareProjectOptions checker fsPathA dllPathA (libSource 0) []
            let optionsB = prepareProjectOptions checker fsPathB dllPathB (appSource 0) []

            checker.InvalidateAll()
            compileProject checker optionsA true
            compileProject checker optionsB true

            let sessionA = checker.CreateHotReloadSession()
            use sessionB = checker.CreateHotReloadSession()

            addProjectOrFail sessionA (createProjectSnapshot optionsA)
            addProjectOrFail sessionB (createProjectSnapshot optionsB)

            (sessionA :> IDisposable).Dispose()

            // The disposed session refuses further work...
            Assert.Throws<ObjectDisposedException>(fun () -> sessionA.Commit()) |> ignore
            Assert.Throws<ObjectDisposedException>(fun () -> sessionA.ProjectIdentifiers |> ignore)
            |> ignore

            Assert.Throws<ObjectDisposedException>(fun () ->
                sessionA.AddProject(createProjectSnapshot optionsA) |> Async.RunSynchronously |> ignore)
            |> ignore

            // ...while the other session keeps emitting.
            writeAndCompile checker fsPathB optionsB (appSource 1) false
            let deltaB = emitOrFail sessionB (createProjectSnapshot optionsB)
            Assert.NotEmpty(deltaB.Metadata)
            sessionB.Commit())

    [<Fact>]
    let ``EmitDelta for an untracked project returns NoActiveSession`` () =
        withProjectDir "fcs-hotreload-session-untracked" (fun projectDir ->
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")

            File.WriteAllText(fsPath, libSource 0)

            let checker = createChecker ()
            let options = prepareProjectOptions checker fsPath dllPath (libSource 0) []

            checker.InvalidateAll()
            compileProject checker options true

            use session = checker.CreateHotReloadSession()

            // No AddProject: emitting is unrepresentable as anything but an error.
            match session.EmitDelta(createProjectSnapshot options) |> Async.RunImmediate with
            | Error FSharpHotReloadError.NoActiveSession -> ()
            | Error other -> failwithf "Expected NoActiveSession, got %A" other
            | Ok _ -> failwith "Expected EmitDelta to fail for a project the session does not track.")

    [<Fact>]
    let ``Multi-project session keeps independent baselines and generation chains`` () =
        withProjectDir "fcs-hotreload-session-multiproject" (fun projectDir ->
            let libFsPath = Path.Combine(projectDir, "SessionLib.fs")
            let libDllPath = Path.Combine(projectDir, "SessionLib.dll")
            let appFsPath = Path.Combine(projectDir, "SessionApp.fs")
            let appDllPath = Path.Combine(projectDir, "SessionApp.dll")

            let appReferencingSource (generation: int) =
                $"""
module SessionApp

let appValue () = "app generation {generation}: " + SessionLib.libValue ()
"""

            File.WriteAllText(libFsPath, libSource 0)
            File.WriteAllText(appFsPath, appReferencingSource 0)

            let checker = createChecker ()
            let libOptions = prepareProjectOptions checker libFsPath libDllPath (libSource 0) []

            // The app project references the library's built output on disk.
            let appOptions =
                prepareProjectOptions checker appFsPath appDllPath (appReferencingSource 0) [ $"-r:{libDllPath}" ]

            checker.InvalidateAll()
            compileProject checker libOptions true
            compileProject checker appOptions true

            use session = checker.CreateHotReloadSession()

            // One session, two projects (AddProject twice).
            addProjectOrFail session (createProjectSnapshot libOptions)
            addProjectOrFail session (createProjectSnapshot appOptions)
            Assert.Equal(2, session.ProjectIdentifiers.Length)

            // Edit each project; each delta is emitted against ITS OWN baseline.
            writeAndCompile checker libFsPath libOptions (libSource 1) false
            let libDelta1 = emitOrFail session (createProjectSnapshot libOptions)
            Assert.NotEmpty(libDelta1.UpdatedMethods)

            writeAndCompile checker appFsPath appOptions (appReferencingSource 1) false
            let appDelta1 = emitOrFail session (createProjectSnapshot appOptions)
            Assert.NotEmpty(appDelta1.UpdatedMethods)

            // Distinct projects, distinct generation ids and base chains.
            Assert.NotEqual<Guid>(libDelta1.GenerationId, appDelta1.GenerationId)
            Assert.NotEqual<Guid>(libDelta1.GenerationId, appDelta1.BaseGenerationId)

            // Solution-wide commit advances BOTH pending project updates together.
            session.Commit()

            // The library's next generation chains from the library's own gen-1 id,
            // interleaved app edits do not perturb the lib chain (and vice versa).
            writeAndCompile checker libFsPath libOptions (libSource 2) false
            let libDelta2 = emitOrFail session (createProjectSnapshot libOptions)
            Assert.Equal(libDelta1.GenerationId, libDelta2.BaseGenerationId)

            writeAndCompile checker appFsPath appOptions (appReferencingSource 2) false
            let appDelta2 = emitOrFail session (createProjectSnapshot appOptions)
            Assert.Equal(appDelta1.GenerationId, appDelta2.BaseGenerationId)

            session.Commit())

    [<Fact>]
    let ``Discard drops pending updates so the next emit re-diffs against the committed view`` () =
        withProjectDir "fcs-hotreload-session-discard" (fun projectDir ->
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")

            File.WriteAllText(fsPath, libSource 0)

            let checker = createChecker ()
            let options = prepareProjectOptions checker fsPath dllPath (libSource 0) []

            checker.InvalidateAll()
            compileProject checker options true

            use session = checker.CreateHotReloadSession()
            addProjectOrFail session (createProjectSnapshot options)

            writeAndCompile checker fsPath options (libSource 1) false
            let delta1 = emitOrFail session (createProjectSnapshot options)

            // A second emit cannot overwrite the first update while the host still owns it.
            match session.EmitDelta(createProjectSnapshot options) |> Async.RunImmediate with
            | Error(FSharpHotReloadError.UnsupportedEdit diagnostics) ->
                Assert.Contains(diagnostics, fun diagnostic -> diagnostic.Message.Contains("already pending"))
            | Error other -> failwithf "Expected UnsupportedEdit for a second pending emit, got %A" other
            | Ok _ -> failwith "Expected a second emit to be rejected while the first update is pending."

            // The host did not apply the update: discard it. The next emit diffs against the
            // unchanged committed baseline, so it builds on the same base generation.
            session.Discard()

            let delta2 = emitOrFail session (createProjectSnapshot options)
            Assert.Equal(delta1.BaseGenerationId, delta2.BaseGenerationId)

            session.Commit()

            // After the commit the next edit chains from the committed generation.
            writeAndCompile checker fsPath options (libSource 2) false
            let delta3 = emitOrFail session (createProjectSnapshot options)
            Assert.Equal(delta2.GenerationId, delta3.BaseGenerationId)

            session.Commit())

    [<Fact>]
    let ``Capabilities and active statements are session-wide across projects`` () =
        withProjectDir "fcs-hotreload-session-capabilities" (fun projectDir ->
            let genericFsPath = Path.Combine(projectDir, "GenericLib.fs")
            let genericDllPath = Path.Combine(projectDir, "GenericLib.dll")
            let plainFsPath = Path.Combine(projectDir, "PlainLib.fs")
            let plainDllPath = Path.Combine(projectDir, "PlainLib.dll")

            let genericSource (generation: int) =
                $"""
module GenericLib

type Calculator<'T>() =
    member _.Describe(value: 'T) = sprintf "generation {generation}: %%A" value
"""

            File.WriteAllText(genericFsPath, genericSource 0)
            File.WriteAllText(plainFsPath, libSource 0)

            let checker = createChecker ()
            let genericOptions = prepareProjectOptions checker genericFsPath genericDllPath (genericSource 0) []
            let plainOptions = prepareProjectOptions checker plainFsPath plainDllPath (libSource 0) []

            checker.InvalidateAll()
            compileProject checker genericOptions true
            compileProject checker plainOptions true

            // Created without capabilities: Roslyn-conservative BaselineOnly default.
            use session = checker.CreateHotReloadSession()
            addProjectOrFail session (createProjectSnapshot genericOptions)
            addProjectOrFail session (createProjectSnapshot plainOptions)

            // Body-editing a member of a generic type requires the GenericUpdateMethod
            // runtime capability; without it the edit is rude.
            writeAndCompile checker genericFsPath genericOptions (genericSource 1) false

            match session.EmitDelta(createProjectSnapshot genericOptions) |> Async.RunImmediate with
            | Error(FSharpHotReloadError.UnsupportedEdit _) -> ()
            | Error other -> failwithf "Expected UnsupportedEdit without GenericUpdateMethod, got %A" other
            | Ok _ -> failwith "Expected generic method edit to be rude under BaselineOnly capabilities."

            // One session-wide capability update unblocks the edit...
            session.UpdateCapabilities [ "GenericUpdateMethod" ]

            let genericDelta = emitOrFail session (createProjectSnapshot genericOptions)
            Assert.NotEmpty(genericDelta.UpdatedMethods)
            Assert.Equal<string list>([ "Baseline"; "GenericUpdateMethod" ], genericDelta.RequiredCapabilities)
            session.Commit()

            // ...and is visible through EVERY project's session view (session-wide state).
            let genericView =
                match session.TryGetProjectView(createProjectSnapshot genericOptions) with
                | ValueSome view -> view
                | ValueNone -> failwith "Expected a session view for the generic project."

            let plainView =
                match session.TryGetProjectView(createProjectSnapshot plainOptions) with
                | ValueSome view -> view
                | ValueNone -> failwith "Expected a session view for the plain project."

            Assert.True(genericView.Capabilities.Supports EditAndContinueCapability.GenericUpdateMethod)
            Assert.Equal<EditAndContinueCapabilities>(genericView.Capabilities, plainView.Capabilities)

            // The host pushes one session-wide batch, but each project view receives only
            // statements owned by that module MVID. MethodDef tokens collide across modules.
            let statement moduleId =
                {
                    ActiveInstruction =
                        {
                            Method = { ModuleId = moduleId; Token = 0x06000001; Version = 1 }
                            ILOffset = 0
                        }
                    DocumentName = None
                    SourceSpan =
                        {
                            StartLine = 0
                            StartColumn = 0
                            EndLine = 0
                            EndColumn = 0
                        }
                    Flags =
                        {
                            FrameKind = FSharpActiveStatementFrameKind.Leaf
                            IsMethodUpToDate = true
                            IsPartiallyExecuted = false
                            IsNonUserCode = false
                            IsStale = false
                        }
                }

            session.SetActiveStatements
                [ statement genericView.Baseline.ModuleId
                  statement plainView.Baseline.ModuleId ]

            let genericView =
                match session.TryGetProjectView(createProjectSnapshot genericOptions) with
                | ValueSome view -> view
                | ValueNone -> failwith "Expected a session view for the generic project."

            let plainView =
                match session.TryGetProjectView(createProjectSnapshot plainOptions) with
                | ValueSome view -> view
                | ValueNone -> failwith "Expected a session view for the plain project."

            Assert.Equal(1, genericView.ActiveStatements.Length)
            Assert.Equal(1, plainView.ActiveStatements.Length)
            Assert.Equal(genericView.Baseline.ModuleId, genericView.ActiveStatements.Head.ActiveInstruction.Method.ModuleId)
            Assert.Equal(plainView.Baseline.ModuleId, plainView.ActiveStatements.Head.ActiveInstruction.Method.ModuleId)

            // Clearing replaces the whole session-wide set.
            session.SetActiveStatements []

            let clearedView =
                match session.TryGetProjectView(createProjectSnapshot plainOptions) with
                | ValueSome view -> view
                | ValueNone -> failwith "Expected a session view for the plain project."

            Assert.Empty(clearedView.ActiveStatements))

    /// Experimental (dotnet/fsharp#19941 dev-loop track): FSHARP_HOTRELOAD_INPROCESS_COMPILE lets
    /// EmitDelta recompile the output assembly in-process from the just-produced check results,
    /// instead of requiring an external `dotnet build` before every delta. Covered here:
    /// a body-only (string literal) edit under the flag; a body edit + line shift of an UNEDITED
    /// sibling function under the flag (the shift surfaces as a sequence-point line update only
    /// if the in-process compile wrote a FRESH sibling PDB - the stale external-build PDB still
    /// carries the unshifted lines, so line-shift detection would stay silent); and, with the
    /// flag off (the default), the existing stale-build-output refusal staying unaffected.
    [<Fact>]
    let ``G1 same-line Giraffe-shaped handler string edit decodes delta user string operand`` () =
        let source literal =
            $"""
module Sample.G1

type HttpFunc = string -> string

let handler1 (_: HttpFunc) (ctx: string) = "{literal}"

let endpointA: (HttpFunc -> string -> string) = fun (_: HttpFunc) (ctx: string) -> "Endpoint A"
let endpointB: (HttpFunc -> string -> string) = fun (_: HttpFunc) (ctx: string) -> "Endpoint B"
let endpointC: (HttpFunc -> string -> string) = fun (_: HttpFunc) (ctx: string) -> "Endpoint C"
"""

        let decode =
            runG1SessionCase
                "giraffe-handler"
                (source "Hello World")
                (source "Hello World EDITED")

        Assert.Equal(1, decode.UpdatedMethods.Length)
        Assert.Single(decode.Baseline.Ldstrs) |> ignore
        Assert.Single(decode.Delta.Ldstrs) |> ignore
        Assert.Contains(decode.Baseline.UserStrings, fun (_, value) -> value = "Hello World")
        Assert.Contains(decode.Delta.UserStrings, fun entry -> entry.Value = "Hello World EDITED")

    [<Fact>]
    let ``G1 simple module string edit decodes working contrast delta user string operand`` () =
        let source generation =
            $"""
module SessionLib

let libValue () = "lib generation {generation}"

let probe () = libValue ()
"""

        let decode =
            runG1SessionCase
                "simple-module"
                (source 0)
                (source 1)

        Assert.Equal(1, decode.UpdatedMethods.Length)
        Assert.Single(decode.Baseline.Ldstrs) |> ignore
        Assert.Single(decode.Delta.Ldstrs) |> ignore
        Assert.Contains(decode.Baseline.UserStrings, fun (_, value) -> value = "lib generation 0")
        Assert.Contains(decode.Delta.UserStrings, fun entry -> entry.Value = "lib generation 1")

    [<Fact>]
    let ``G2 flag-on in-process line edit above sibling lambdas replays stable closure names`` () =
        withProjectDir "fcs-hotreload-session-g2-closure-replay" (fun projectDir ->
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")

            let source literal includeInsertedLine =
                let insertedLine =
                    if includeInsertedLine then
                        "    // inserted line shifts the sibling lambdas below"
                    else
                        ""

                let insertedTopLevelLine =
                    if includeInsertedLine then
                        "// inserted line shifts the pipe and endpoint closures below"
                    else
                        ""

                $"""
module SessionG2

let makePair input =
    let message = "{literal}"
{insertedLine}
    let mapperA = fun value -> value + input
    let mapperB = fun value -> value * input
    message, mapperA, mapperB

{insertedTopLevelLine}
let transform (input: int list) =
    let bias = 1
    input
    |> List.map (fun value -> value + bias)
    |> List.filter (fun value -> value > 1)
    |> List.map (fun value -> value * 2)

let handler input =
    input + 1

let endpoints : (int -> int) list =
    [
        fun value -> handler value
        fun value -> handler (value + 1)
        fun value -> handler (value + 2)
    ]

let probe input =
    let transformed = transform [ input; input + 1; input + 2 ] |> List.sum
    let endpointTotal = endpoints |> List.sumBy (fun endpoint -> endpoint input)
    transformed + endpointTotal
"""

            let baselineSource = source "baseline" false
            let editedSource = source "edited" true
            let baselineSource = baselineSource.TrimStart()
            let editedSource = editedSource.TrimStart()

            File.WriteAllText(fsPath, baselineSource)

            let checker = createChecker ()
            let options = prepareProjectOptions checker fsPath dllPath baselineSource []

            checker.InvalidateAll()
            compileProject checker options true

            let baselineBytes = File.ReadAllBytes dllPath

            use session = checker.CreateHotReloadSession()
            addProjectOrFail session (createProjectSnapshot options)

            let baselineView = projectViewOrFail session (createProjectSnapshot options)
            Assert.False(Map.isEmpty baselineView.Baseline.EncClosureNames, "Expected a flag-on baseline with closure-name tables.")

            let delta =
                withEnvVar "FSHARP_HOTRELOAD_INPROCESS_COMPILE" "1" (fun () ->
                    File.WriteAllText(fsPath, editedSource)
                    checker.NotifyFileChanged(fsPath, options) |> Async.RunImmediate
                    emitOrFail session (createProjectSnapshot options))

            let userUpdatedNames =
                let userMethodTokenByToken =
                    baselineView.Baseline.MethodTokens
                    |> Map.toSeq
                    |> Seq.choose (fun (key, token) ->
                        if key.DeclaringType.Contains("@") then
                            None
                        else
                            Some(token, $"{key.DeclaringType}::{key.Name}"))
                    |> Map.ofSeq

                delta.AddedOrChangedMethods
                |> List.choose (fun methodInfo -> userMethodTokenByToken |> Map.tryFind methodInfo.MethodToken)
                |> List.sort

            Assert.Equal<string list>([ "SessionG2::makePair" ], userUpdatedNames)

            let freshBytes = File.ReadAllBytes dllPath

            let outputIsUnchanged =
                baselineBytes.Length = freshBytes.Length
                && Array.forall2 (=) baselineBytes freshBytes

            Assert.False(outputIsUnchanged, "Expected the in-process compile to refresh the output assembly.")

            let baselineTypeNames = readTypeNames baselineBytes
            let freshTypeNames = readTypeNames freshBytes

            assertMatchingTypeNames
                "G2 closure replay"
                "pipeline"
                (fun name -> name.Contains("transform@hotreload"))
                baselineTypeNames
                freshTypeNames

            let freshEndpointNames =
                freshTypeNames
                |> List.filter (fun name -> name.Contains("endpoints@"))

            Assert.NotEmpty freshEndpointNames)

    [<Fact>]
    let ``In-process edit applies with unrelated inline computation-expression helpers`` () =
        withProjectDir "fcs-hotreload-session-inprocess-unrelated-helpers" (fun projectDir ->
            let markupPath = Path.Combine(projectDir, "Markup.fs")
            let layoutPath = Path.Combine(projectDir, "Layout.fs")
            let dllPath = Path.Combine(projectDir, "UnrelatedHelpers.dll")

            // A full in-process emit regenerates helpers from every file, including this
            // unchanged computation expression. Its synthesized-name aliases must remain a
            // one-to-one mapping even though the semantic edit is in the following file.
            let markupSource =
                """
namespace DslIsolation

module Markup =
    open System.Runtime.CompilerServices

    type FragmentBuilder() =
        member _.Add(_text: string) = ()

        member inline _.Combine
            ([<InlineIfLambda>] first: FragmentBuilder -> unit,
             [<InlineIfLambda>] second: FragmentBuilder -> unit)
            : FragmentBuilder -> unit =
            fun builder ->
                first builder
                second builder

        member inline _.Zero() : FragmentBuilder -> unit = ignore

        member inline _.Delay([<InlineIfLambda>] delay: unit -> FragmentBuilder -> unit) : FragmentBuilder -> unit =
            delay()

        member inline this.Yield(text: string) : FragmentBuilder -> unit = fun _ -> this.Add text

        member inline this.Run([<InlineIfLambda>] runExpr: FragmentBuilder -> unit) =
            runExpr this
            this

    let render value =
        let id = value |> string

        FragmentBuilder() {
            "Value: " + (value |> string)
            [ value; value + 1 ] |> List.map string |> String.concat ","
        }

        |> ignore

        id

    let renderDetails value =
        FragmentBuilder() {
            "Details: " + (value |> string)
            [ value; value + 1; value + 2 ]
            |> List.filter (fun item -> item > 0)
            |> List.map string
            |> String.concat ","
        }
        |> ignore

        value |> string

    let handlers : (int -> string) list =
        [
            fun value -> render value
            fun value -> renderDetails (value + 1)
        ]
"""

            let layoutSource heading =
                $"""
namespace DslIsolation

module Layout =
    let heading () = "{heading}"
"""

            let baselineLayout = layoutSource "original"
            let editedLayout = layoutSource "updated"

            File.WriteAllText(markupPath, markupSource.TrimStart())
            File.WriteAllText(layoutPath, baselineLayout.TrimStart())

            let checker = createChecker ()

            let options =
                prepareMultiFileProjectOptions
                    checker
                    [| markupPath; layoutPath |]
                    dllPath
                    markupSource
                    []

            checker.InvalidateAll()
            compileProject checker options true

            use session = checker.CreateHotReloadSession()
            addProjectOrFail session (createProjectSnapshot options)

            let headingToken =
                let baselineView = projectViewOrFail session (createProjectSnapshot options)

                baselineView.Baseline.MethodTokens
                |> Map.toSeq
                |> Seq.pick (fun (methodKey, token) ->
                    if methodKey.Name = "heading" then Some token else None)

            let delta =
                withEnvVar "FSHARP_HOTRELOAD_INPROCESS_COMPILE" "1" (fun () ->
                    File.WriteAllText(layoutPath, editedLayout.TrimStart())
                    checker.NotifyFileChanged(layoutPath, options) |> Async.RunImmediate
                    emitOrFail session (createProjectSnapshot options))

            Assert.Contains(headingToken, delta.UpdatedMethods))

    [<Fact>]
    let ``G2 flag-on in-process line edit above mixed endpoint bucket preserves closure names`` () =
        let routingLibrarySource =
            """
module SessionRoutingLike

open System

type HttpFunc = string -> string
type HttpHandler = HttpFunc -> HttpFunc
type ConfigureEndpoint = int -> int

type HttpVerb =
    | GET
    | POST
    | HEAD
    | NotSpecified

type Endpoint =
    | SimpleEndpoint of HttpVerb * string * HttpHandler * ConfigureEndpoint
    | TemplateEndpoint of HttpVerb * string * (string * char) list * (obj -> HttpHandler) * ConfigureEndpoint
    | NestedEndpoint of string * Endpoint list * ConfigureEndpoint
    | MultiEndpoint of Endpoint list

let compose (handler1: HttpHandler) (handler2: HttpHandler) : HttpHandler =
    fun (final: HttpFunc) ->
        let func = final |> handler2 |> handler1
        fun ctx -> func ctx

let (>=>) = compose

let text (str: string) : HttpHandler =
    let bytes = str.ToCharArray()

    fun (_: HttpFunc) (ctx: string) ->
        String(bytes) + ctx

let rec private applyHttpVerbToEndpoint (verb: HttpVerb) (endpoint: Endpoint) : Endpoint =
    match endpoint with
    | SimpleEndpoint(_, routeTemplate, requestDelegate, metadata) ->
        SimpleEndpoint(verb, routeTemplate, requestDelegate, metadata)
    | TemplateEndpoint(_, routeTemplate, mappings, requestDelegate, metadata) ->
        TemplateEndpoint(verb, routeTemplate, mappings, requestDelegate, metadata)
    | NestedEndpoint(routeTemplate, endpoints, metadata) ->
        NestedEndpoint(routeTemplate, endpoints |> List.map (applyHttpVerbToEndpoint verb), metadata)
    | MultiEndpoint endpoints ->
        endpoints |> List.map (applyHttpVerbToEndpoint verb) |> MultiEndpoint

let rec private applyHttpVerbToEndpoints (verb: HttpVerb) (endpoints: Endpoint list) : Endpoint =
    endpoints
    |> List.map (fun endpoint ->
        match endpoint with
        | SimpleEndpoint(_, routeTemplate, requestDelegate, metadata) ->
            SimpleEndpoint(verb, routeTemplate, requestDelegate, metadata)
        | TemplateEndpoint(_, routeTemplate, mappings, requestDelegate, metadata) ->
            TemplateEndpoint(verb, routeTemplate, mappings, requestDelegate, metadata)
        | NestedEndpoint(routeTemplate, endpoints, metadata) ->
            NestedEndpoint(routeTemplate, endpoints |> List.map (applyHttpVerbToEndpoint verb), metadata)
        | MultiEndpoint endpoints ->
            applyHttpVerbToEndpoints verb endpoints)
    |> MultiEndpoint

let rec private applyHttpVerbsToEndpoints (verbs: HttpVerb list) (endpoints: Endpoint list) : Endpoint =
    endpoints
    |> List.map (fun endpoint ->
        match endpoint with
        | SimpleEndpoint(_, routeTemplate, requestDelegate, metadata) ->
            verbs
            |> List.map (fun verb -> SimpleEndpoint(verb, routeTemplate, requestDelegate, metadata))
            |> MultiEndpoint
        | TemplateEndpoint(_, routeTemplate, mappings, requestDelegate, metadata) ->
            verbs
            |> List.map (fun verb -> TemplateEndpoint(verb, routeTemplate, mappings, requestDelegate, metadata))
            |> MultiEndpoint
        | NestedEndpoint(routeTemplate, endpoints, metadata) ->
            verbs
            |> List.map (fun verb ->
                NestedEndpoint(routeTemplate, endpoints |> List.map (applyHttpVerbToEndpoint verb), metadata))
            |> MultiEndpoint
        | MultiEndpoint endpoints ->
            verbs
            |> List.map (fun verb -> applyHttpVerbToEndpoints verb endpoints)
            |> MultiEndpoint)
    |> MultiEndpoint

let GET_HEAD = applyHttpVerbsToEndpoints [ GET; HEAD ]
let GET = applyHttpVerbToEndpoints GET
let POST = applyHttpVerbToEndpoints POST

let routeWithExtensions (configureEndpoint: ConfigureEndpoint) (path: string) (handler: HttpHandler) : Endpoint =
    SimpleEndpoint(HttpVerb.NotSpecified, path, handler, configureEndpoint)

let route (path: string) (handler: HttpHandler) : Endpoint =
    routeWithExtensions id path handler

let routefWithExtensions
    (configureEndpoint: ConfigureEndpoint)
    (path: PrintfFormat<_, _, _, _, 'T>)
    (routeHandler: 'T -> HttpHandler)
    : Endpoint =
    let template = path.Value
    let mappings = []

    let boxedHandler (o: obj) =
        let t = o :?> 'T
        routeHandler t

    TemplateEndpoint(HttpVerb.NotSpecified, template, mappings, boxedHandler, configureEndpoint)

let routef (path: PrintfFormat<_, _, _, _, 'T>) (routeHandler: 'T -> HttpHandler) : Endpoint =
    routefWithExtensions id path routeHandler

let subRouteWithExtensions (configureEndpoint: ConfigureEndpoint) (path: string) (endpoints: Endpoint list) : Endpoint =
    NestedEndpoint(path, endpoints, configureEndpoint)

let subRoute (path: string) (endpoints: Endpoint list) : Endpoint =
    subRouteWithExtensions id path endpoints
"""

        let source literal includeInsertedLine =
            let insertedLine =
                if includeInsertedLine then
                    "    // inserted line shifts the mixed endpoint bucket below"
                else
                    ""

            $"""
module SessionMixedEndpoints

open SessionRoutingLike

let editedValue () =
{insertedLine}
    "{literal}"

let handler1: HttpHandler =
    fun (_: HttpFunc) (ctx: string) -> "Hello World" + ctx

let handler2 (firstName: string, age: int) : HttpHandler =
    fun (_: HttpFunc) (ctx: string) ->
        sprintf "Hello %%s, you are %%i years old." firstName age + ctx

let handler3 (a: string, b: string, c: string, d: int) : HttpHandler =
    fun (_: HttpFunc) (ctx: string) -> sprintf "Hello %%s %%s %%s %%i" a b c d + ctx

let handlerNamed (petId: int) : HttpHandler =
    fun (_: HttpFunc) (ctx: string) -> sprintf "PetId: %%i" petId + ctx

let jsonHandler: HttpHandler =
    fun (next: HttpFunc) (ctx: string) -> next ("json:" + ctx)

let endpoints =
    [
        subRoute "/foo" [ GET [ route "/bar" (text "Aloha!") ] ]
        GET [
            route "/" (text "Hello World")
            routef "/%%s/%%i" handler2
            routef "/%%s/%%s/%%s/%%i" handler3
            routef "/pet/%%i:petId" handlerNamed
        ]
        GET_HEAD [
            route "/foo" (text "Bar")
            route "/x" (text "y")
            route "/abc" (text "def")
            route "/123" (text "456")
        ]
        subRoute "/sub" [ route "/test" handler1 ]
        POST [ route "/json" jsonHandler ]
    ]

let probe () =
    endpoints.Length + editedValue().Length
"""

        let runCase caseLabel resetBeforeAddProject =
            withProjectDir $"fcs-hotreload-session-g2-mixed-endpoint-bucket-{caseLabel}" (fun projectDir ->
                FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

                let routingPath = Path.Combine(projectDir, "SessionRoutingLike.fs")
                let routingDllPath = Path.Combine(projectDir, "SessionRoutingLike.dll")
                let fsPath = Path.Combine(projectDir, "Library.fs")
                let dllPath = Path.Combine(projectDir, "Library.dll")
                let baselineSource = (source "baseline" false).TrimStart()
                let editedSource = (source "edited" true).TrimStart()
                let checker = createChecker ()

                File.WriteAllText(routingPath, routingLibrarySource.TrimStart())

                let routingOptions = prepareProjectOptions checker routingPath routingDllPath routingLibrarySource []
                checker.InvalidateAll()
                compileProject checker routingOptions false

                File.WriteAllText(fsPath, baselineSource)

                let options =
                    prepareProjectOptions checker fsPath dllPath baselineSource [ $"-r:{routingDllPath}" ]

                checker.InvalidateAll()
                compileProject checker options true

                let baselineBytes = File.ReadAllBytes dllPath
                let baselinePdbSnapshot =
                    readRecordedSynthesizedNameSnapshot
                        $"G2 mixed endpoint baseline PDB {caseLabel}"
                        (Path.ChangeExtension(dllPath, ".pdb"))

                assertMixedEndpointSnapshot $"G2 mixed endpoint baseline PDB {caseLabel}" baselinePdbSnapshot
                |> ignore

                let baselineTypeNames = readTypeNames baselineBytes
                assertMixedEndpointTypeNames $"G2 mixed endpoint baseline {caseLabel}" baselineTypeNames
                |> ignore

                if resetBeforeAddProject then
                    FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

                use session = checker.CreateHotReloadSession()
                addProjectOrFail session (createProjectSnapshot options)

                let baselineView = projectViewOrFail session (createProjectSnapshot options)
                printfn
                    "[G2 mixed endpoint %s] reconstructed closure-name tables=%d"
                    caseLabel
                    (Map.count baselineView.Baseline.EncClosureNames)

                assertMixedEndpointSnapshot $"G2 mixed endpoint baseline snapshot {caseLabel}" baselineView.Baseline.SynthesizedNameSnapshot
                |> ignore

                assertSynthesizedNameSnapshotsEqual
                    $"G2 mixed endpoint baseline PDB/session snapshot {caseLabel}"
                    baselinePdbSnapshot
                    baselineView.Baseline.SynthesizedNameSnapshot

                let delta =
                    withEnvVar "FSHARP_HOTRELOAD_INPROCESS_COMPILE" "1" (fun () ->
                        File.WriteAllText(fsPath, editedSource)
                        checker.NotifyFileChanged(fsPath, options) |> Async.RunImmediate
                        emitOrFail session (createProjectSnapshot options))

                let userUpdatedNames =
                    let userMethodTokenByToken =
                        baselineView.Baseline.MethodTokens
                        |> Map.toSeq
                        |> Seq.choose (fun (key, token) ->
                            if key.DeclaringType.Contains("@") then
                                None
                            else
                                Some(token, $"{key.DeclaringType}::{key.Name}"))
                        |> Map.ofSeq

                    delta.AddedOrChangedMethods
                    |> List.choose (fun methodInfo -> userMethodTokenByToken |> Map.tryFind methodInfo.MethodToken)
                    |> List.sort

                Assert.Equal<string list>([ "SessionMixedEndpoints::editedValue" ], userUpdatedNames)

                let freshBytes = File.ReadAllBytes dllPath
                let freshPdbSnapshot =
                    readRecordedSynthesizedNameSnapshot
                        $"G2 mixed endpoint fresh PDB {caseLabel}"
                        (Path.ChangeExtension(dllPath, ".pdb"))

                assertMixedEndpointSnapshot $"G2 mixed endpoint fresh PDB {caseLabel}" freshPdbSnapshot
                |> ignore

                let freshTypeNames = readTypeNames freshBytes
                assertMixedEndpointTypeNames $"G2 mixed endpoint fresh {caseLabel}" freshTypeNames
                |> ignore)

        runCase "in-memory-capture" false
        runCase "disk-started" true

    [<Fact>]
    let ``G2 flag-on replay endpoint bucket uses recorded snapshot and old baselines fail closed`` () =
        let runCase caseLabel disableSnapshotCdi =
            withProjectDir $"fcs-hotreload-session-g2-replay-endpoint-bucket-{caseLabel}" (fun projectDir ->
                let fsPath = Path.Combine(projectDir, "Library.fs")
                let dllPath = Path.Combine(projectDir, "Library.dll")

                let source handlerOffset includeInsertedLine =
                    let insertedLine =
                        if includeInsertedLine then
                            "    // inserted line shifts the endpoint bucket below"
                        else
                            ""

                    $"""
module SessionReplayEndpoints

type Handler = int -> string

let handler2 value =
{insertedLine}
    string (value + {handlerOffset})

let endpoints : Handler list =
    let local prefix value =
        prefix + string value

    [
        fun value -> string value
        local "foo="
        fun value -> string (value + 1)
        local "bar="
    ]

let probe input =
    endpoints |> List.sumBy (fun endpoint -> endpoint input |> String.length)
"""

                let baselineSource = (source 1 false).TrimStart()
                let editedSource = (source 2 true).TrimStart()

                File.WriteAllText(fsPath, baselineSource)

                let checker = createChecker ()
                let options = prepareProjectOptions checker fsPath dllPath baselineSource []

                checker.InvalidateAll()

                if disableSnapshotCdi then
                    withEnvVar "FSHARP_HOTRELOAD_DISABLE_SYNTHESIZED_NAME_SNAPSHOT_CDI" "1" (fun () ->
                        compileProject checker options true)
                else
                    compileProject checker options true

                let baselineBytes = File.ReadAllBytes dllPath
                let baselineTypeNames = readTypeNames baselineBytes
                assertReplayEndpointTypeNames $"G2 replay endpoint baseline {caseLabel}" baselineTypeNames
                |> ignore

                FSharpEditAndContinueLanguageService.Instance.ResetSessionState()
                use session = checker.CreateHotReloadSession()
                addProjectOrFail session (createProjectSnapshot options)

                let baselineView = projectViewOrFail session (createProjectSnapshot options)

                Assert.True(
                    Map.isEmpty baselineView.Baseline.EncClosureNames,
                    "Expected the replay endpoint bucket to have no occurrence-keyed closure-name reconstruction and rely on the synthesized-name snapshot.")

                let expectedSnapshotSource =
                    if disableSnapshotCdi then
                        SynthesizedNameSnapshotSource.Reconstructed
                    else
                        SynthesizedNameSnapshotSource.Recorded

                Assert.Equal(expectedSnapshotSource, baselineView.Baseline.SynthesizedNameSnapshotSource)

                let result =
                    withEnvVar "FSHARP_HOTRELOAD_INPROCESS_COMPILE" "1" (fun () ->
                        File.WriteAllText(fsPath, editedSource)
                        checker.NotifyFileChanged(fsPath, options) |> Async.RunImmediate
                        session.EmitDelta(createProjectSnapshot options) |> Async.RunImmediate)

                if disableSnapshotCdi then
                    match result with
                    | Error(FSharpHotReloadError.UnsupportedEdit edits) ->
                        let message =
                            edits
                            |> List.map (fun edit -> $"{edit.Id}: {edit.Message}")
                            |> String.concat Environment.NewLine

                        Assert.DoesNotContain("DeltaEmissionFailed", message)
                    | Error other -> failwithf "Expected UnsupportedEdit, got %A" other
                    | Ok _ -> failwith "Expected old replay-only baseline to fail closed."
                else
                    match result with
                    | Ok _ ->
                        File.ReadAllBytes dllPath
                        |> readTypeNames
                        |> assertReplayEndpointTypeNames $"G2 replay endpoint fresh {caseLabel}"
                        |> ignore
                    | Error error -> failwithf "Expected recorded replay-only baseline to emit a delta, got %A" error)

        runCase "recorded" false
        runCase "old-baseline-fallback" true

    [<Fact>]
    let ``in-process compilation falls back to fresh external outputs when a reference assembly is required`` () =
        withProjectDir "fcs-hotreload-inprocess-refout-fallback" (fun projectDir ->
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")
            let refPath = Path.Combine(projectDir, "ref", "Library.dll")

            let baselineSource =
                """
module Library

let existing () = 1
"""

            let updatedSource =
                """
module Library

let existing () = 1
let added () = 2
"""

            File.WriteAllText(fsPath, baselineSource.TrimStart())
            let checker = createChecker ()

            let options =
                prepareProjectOptions checker fsPath dllPath baselineSource [ $"--refout:{refPath}" ]

            checker.InvalidateAll()
            compileProject checker options true
            let baselineReferenceBytes = File.ReadAllBytes(refPath)

            use session = checker.CreateHotReloadSession()

            session.UpdateCapabilities
                [ "AddMethodToExistingType"
                  "AddStaticFieldToExistingType" ]

            addProjectOrFail session (createProjectSnapshot options)

            withEnvVar "FSHARP_HOTRELOAD_INPROCESS_COMPILE" "1" (fun () ->
                File.WriteAllText(fsPath, updatedSource.TrimStart())
                checker.NotifyFileChanged(fsPath, options) |> Async.RunImmediate

                // The fast path must refuse to write only the implementation assembly. With no
                // external build available, the normal stale-output guard rejects the fallback.
                match session.EmitDelta(createProjectSnapshot options) |> Async.RunImmediate with
                | Error(FSharpHotReloadError.DeltaEmissionFailed _) -> ()
                | Error other -> failwithf "Expected stale-output failure, got %A" other
                | Ok _ -> failwith "Expected the refout project to require a fresh external build."

                Assert.Equal<byte>(baselineReferenceBytes, File.ReadAllBytes(refPath))

                // Once the regular compiler has refreshed both outputs, the failed fast path
                // safely falls back to those artifacts and emits the supported member addition.
                compileProject checker options false
                Assert.NotEqual<byte>(baselineReferenceBytes, File.ReadAllBytes(refPath))

                let delta = emitOrFail session (createProjectSnapshot options)
                Assert.NotEmpty(delta.UpdatedMethods)
                session.Commit()))

    [<Fact>]
    let ``overlapping in-process emits serialize and preserve the first pending update`` () =
        withProjectDir "fcs-hotreload-inprocess-overlap" (fun projectDir ->
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")

            let source generation =
                $"""
module Library

let value () = {generation}
"""

            File.WriteAllText(fsPath, source 0)
            let checker = createChecker ()
            let options = prepareProjectOptions checker fsPath dllPath (source 0) []

            checker.InvalidateAll()
            compileProject checker options true

            use session = checker.CreateHotReloadSession()
            addProjectOrFail session (createProjectSnapshot options)

            withEnvVar "FSHARP_HOTRELOAD_INPROCESS_COMPILE" "1" (fun () ->
                File.WriteAllText(fsPath, source 1)
                checker.NotifyFileChanged(fsPath, options) |> Async.RunImmediate
                let snapshot = createProjectSnapshot options

                let results =
                    [| session.EmitDelta(snapshot)
                       session.EmitDelta(snapshot) |]
                    |> Async.Parallel
                    |> Async.RunImmediate

                Assert.Equal(1, results |> Array.filter Result.isOk |> Array.length)

                let rejected =
                    results
                    |> Array.choose (function
                        | Error(FSharpHotReloadError.UnsupportedEdit diagnostics) -> Some diagnostics
                        | _ -> None)
                    |> Assert.Single

                Assert.Contains(rejected, fun diagnostic -> diagnostic.Message.Contains("already pending"))
                session.Discard()))

    [<Fact>]
    let ``EmitDelta recompiles in-process under FSHARP_HOTRELOAD_INPROCESS_COMPILE and refuses stale output once the flag is off`` () =
        withProjectDir "fcs-hotreload-session-inprocess-compile" (fun projectDir ->
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")

            // Two functions: libValue takes the body edits; probe is never edited, so a comment
            // line inserted above it shifts its sequence points without re-emitting its body.
            let inprocSource (generation: int) =
                $"""
module SessionLib

let libValue () = "lib generation {generation}"

let probe () = 1
"""

            let inprocSourceShifted (generation: int) =
                $"""
module SessionLib

let libValue () = "lib generation {generation}"

// inserted line: shifts probe down relative to the committed baseline
let probe () = 1
"""

            File.WriteAllText(fsPath, inprocSource 0)

            let checker = createChecker ()
            let options = prepareProjectOptions checker fsPath dllPath (inprocSource 0) []

            checker.InvalidateAll()
            compileProject checker options true

            use session = checker.CreateHotReloadSession()
            addProjectOrFail session (createProjectSnapshot options)

            let previousFlag =
                Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_INPROCESS_COMPILE")

            try
                Environment.SetEnvironmentVariable("FSHARP_HOTRELOAD_INPROCESS_COMPILE", "1")

                // Body-only edit (string literal): notify the checker but deliberately skip the
                // external `dotnet build`/compileProject step. The in-process compile path must
                // produce the updated output assembly by itself.
                File.WriteAllText(fsPath, inprocSource 1)
                checker.NotifyFileChanged(fsPath, options) |> Async.RunImmediate

                let delta = emitOrFail session (createProjectSnapshot options)
                Assert.Equal(1, delta.UpdatedMethods.Length)
                session.Commit()

                // Body edit of libValue + line shift of the unedited probe, still without any
                // external rebuild. The line update for probe is computed by diffing the
                // committed sequence points against the on-disk PDB, so it only appears if the
                // in-process compile wrote a fresh sibling PDB: the stale external-build PDB
                // still carries probe's unshifted lines and detection would emit nothing.
                File.WriteAllText(fsPath, inprocSourceShifted 2)
                checker.NotifyFileChanged(fsPath, options) |> Async.RunImmediate

                let shiftedDelta = emitOrFail session (createProjectSnapshot options)
                Assert.Equal(1, shiftedDelta.UpdatedMethods.Length)
                assertPortablePdbWithMethodDebugInfo shiftedDelta.Pdb

                let updates = Assert.Single(shiftedDelta.SequencePointUpdates)
                let lineUpdate = Assert.Single(updates.LineUpdates)
                Assert.Equal(1, lineUpdate.NewLine - lineUpdate.OldLine)
                session.Commit()
            finally
                Environment.SetEnvironmentVariable("FSHARP_HOTRELOAD_INPROCESS_COMPILE", previousFlag)

            // Flag off (the default): a further body edit without an external recompile must be
            // refused, proving flag-off behavior is unaffected by the in-process compile path.
            File.WriteAllText(fsPath, inprocSourceShifted 3)
            checker.NotifyFileChanged(fsPath, options) |> Async.RunImmediate

            match session.EmitDelta(createProjectSnapshot options) |> Async.RunImmediate with
            | Error(FSharpHotReloadError.DeltaEmissionFailed _) -> ()
            | Error other -> failwithf "Expected a stale-output DeltaEmissionFailed error, got %A" other
            | Ok _ -> failwith "Expected EmitDelta to refuse a delta from an unchanged (stale) build output.")

    [<Fact>]
    let ``EmitDelta splices successive in-process incremental emit generations`` () =
        withProjectDir "fcs-hotreload-session-incremental-emit" (fun projectDir ->
            let supportPath = Path.Combine(projectDir, "Support.fs")
            let entryPath = Path.Combine(projectDir, "Entry.fs")
            let dllPath = Path.Combine(projectDir, "IncrementalSession.dll")

            let supportSource =
                """
module IncrementalSupport

let addOne value = value + 1
"""

            let entrySource generation =
                $"""
module IncrementalEntry

let changed () = IncrementalSupport.addOne {generation}
"""

            File.WriteAllText(supportPath, supportSource.TrimStart())
            File.WriteAllText(entryPath, (entrySource 0).TrimStart())

            let checker = createChecker ()
            let fsPaths = [| supportPath; entryPath |]
            let options = prepareMultiFileProjectOptions checker fsPaths dllPath supportSource []

            checker.InvalidateAll()
            compileProject checker options true

            use session = checker.CreateHotReloadSession()
            addProjectOrFail session (createProjectSnapshot options)

            withEnvVar "FSHARP_HOTRELOAD_INPROCESS_COMPILE" "1" (fun () ->
                withEnvVar "FSHARP_HOTRELOAD_INCREMENTAL_EMIT" "1" (fun () ->
                    File.WriteAllText(entryPath, (entrySource 1).TrimStart())
                    checker.NotifyFileChanged(entryPath, options) |> Async.RunImmediate

                    let firstDelta = emitOrFail session (createProjectSnapshot options)
                    Assert.Equal(1, firstDelta.UpdatedMethods.Length)
                    session.Commit()

                    File.WriteAllText(entryPath, (entrySource 2).TrimStart())
                    checker.NotifyFileChanged(entryPath, options) |> Async.RunImmediate

                    let secondDelta = emitOrFail session (createProjectSnapshot options)
                    Assert.Equal(1, secondDelta.UpdatedMethods.Length)
                    session.Commit())))
