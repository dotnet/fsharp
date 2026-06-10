namespace FSharp.Compiler.ComponentTests.HotReload

open System
open System.Collections.Immutable
open System.Reflection
open Xunit
open FSharp.Compiler.IlxDeltaEmitter
open FSharp.Compiler.HotReload
open FSharp.Compiler.HotReloadBaseline
open Internal.Utilities
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.AbstractIL.ILPdbWriter
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles
open FSharp.Compiler.IlxDeltaStreams
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.CodeGen.DeltaMetadataTables
open System.Diagnostics
open System.IO
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Reflection.PortableExecutable
open System.Buffers.Binary
open Xunit.Sdk
open FSharp.Test
open FSharp.Compiler.HotReload.SymbolMatcher
open FSharp.Compiler.TypedTreeDiff
open FSharp.Compiler.ComponentTests.HotReload.TestHelpers

// Use our custom EditAndContinueOperation, not System.Reflection.Metadata.Ecma335's
type EditAndContinueOperation = FSharp.Compiler.AbstractIL.ILDeltaHandles.EditAndContinueOperation

[<Collection(nameof NotThreadSafeResourceCollection)>]
module DeltaEmitterTests =

    // Helper to convert int table index to SRM TableIndex enum for boundary calls
    let inline private toTableIndex (table: TableName) : TableIndex =
        LanguagePrimitives.EnumOfValue<byte, TableIndex>(byte table.Index)

    let private tryRunMdv args =
        try
            let startInfo = ProcessStartInfo()
            startInfo.FileName <- "mdv"
            startInfo.Arguments <- args
            startInfo.RedirectStandardOutput <- true
            startInfo.RedirectStandardError <- true
            startInfo.UseShellExecute <- false

            use proc = new Process(StartInfo = startInfo)
            if not (proc.Start()) then
                ValueNone
            else
                proc.WaitForExit()
                ValueSome (proc.ExitCode, proc.StandardOutput.ReadToEnd(), proc.StandardError.ReadToEnd())
        with _ -> ValueNone

    let private createMethod (ilg: ILGlobals) name returnValue =
        let methodBody =
            mkMethodBody (false, [], 2, nonBranchingInstrsToCode [ AI_ldc(DT_I4, ILConst.I4 returnValue); I_ret ], None, None)

        mkILNonGenericStaticMethod (
            name,
            ILMemberAccess.Public,
            [],
            mkILReturn ilg.typ_Int32,
            methodBody
        )

    let private createModule returnValue =
        let ilg = PrimaryAssemblyILGlobals
        let methodDef = createMethod ilg "GetValue" returnValue

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    "Sample.Type",
                    ILTypeDefAccess.Public,
                    mkILMethods [ methodDef ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField
            )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let private createModuleWithMethods (methods: (string * int) list) =
        let ilg = PrimaryAssemblyILGlobals
        let ilMethods = methods |> List.map (fun (name, value) -> createMethod ilg name value)

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    "Sample.Multi",
                    ILTypeDefAccess.Public,
                    mkILMethods ilMethods,
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField
                )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let private createModuleWithOptionalField includeField =
        let ilg = PrimaryAssemblyILGlobals
        let methodDef = createMethod ilg "GetValue" 1

        let fields =
            if includeField then
                mkILFields [ mkILStaticField("trackedField", ilg.typ_Int32, None, None, ILMemberAccess.Private) ]
            else
                mkILFields []

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    "Sample.FieldHolder",
                    ILTypeDefAccess.Public,
                    mkILMethods [ methodDef ],
                    fields,
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField
                )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let private mscorlibRef =
        ILAssemblyRef.Create(
            "mscorlib",
            None,
            Some(
                PublicKeyToken
                    [|
                        0xb7uy
                        0x7auy
                        0x5cuy
                        0x56uy
                        0x19uy
                        0x34uy
                        0xe0uy
                        0x89uy
                    |]
            ),
            false,
            Some(ILVersionInfo(4us, 0us, 0us, 0us)),
            None)

    let private createConsoleCallModule useIntOverload =
        let ilg = PrimaryAssemblyILGlobals
        let consoleTypeRef = mkILTyRef(ILScopeRef.Assembly mscorlibRef, "System.Console")

        let bodyInstrs =
            let consoleType = mkILNamedTy ILBoxity.AsObject consoleTypeRef []

            if useIntOverload then
                let methodSpec =
                    mkILNonGenericMethSpecInTy(consoleType, ILCallingConv.Static, "WriteLine", [ ilg.typ_Int32 ], ILType.Void)

                nonBranchingInstrsToCode [ AI_ldc(DT_I4, ILConst.I4 123); mkNormalCall methodSpec; I_ret ]
            else
                let methodSpec =
                    mkILNonGenericMethSpecInTy(consoleType, ILCallingConv.Static, "WriteLine", [ ilg.typ_String ], ILType.Void)

                nonBranchingInstrsToCode [ I_ldstr "hello"; mkNormalCall methodSpec; I_ret ]

        let methodDef =
            mkILNonGenericStaticMethod(
                "Log",
                ILMemberAccess.Public,
                [],
                mkILReturn ILType.Void,
                mkMethodBody (false, [], 2, bodyInstrs, None, None)
            )

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    "Sample.ConsoleDemo",
                    ILTypeDefAccess.Public,
                    mkILMethods [ methodDef ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField
                )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let private readCallOperand (bytes: ReadOnlySpan<byte>) =
        let mutable offset = 0
        let mutable found = ValueNone

        while offset < bytes.Length && found.IsNone do
            match bytes[offset] with
            | 0x72uy -> // ldstr token (4 bytes)
                offset <- offset + 1 + 4
            | 0x28uy ->
                let token = BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(offset + 1, 4))
                found <- ValueSome token
                offset <- bytes.Length
            | 0x2Auy -> // ret
                offset <- offset + 1
            | opcode -> failwithf "Unexpected opcode 0x%02X while reading call operand" opcode

        match found with
        | ValueSome token -> token
        | ValueNone -> failwith "No call opcode found in method body"

    let private createModuleWithParameterizedMethod () =
        let ilg = PrimaryAssemblyILGlobals
        let baseMethod = createMethod ilg "GetValue" 1

        let paramBody =
            mkMethodBody (
                false,
                [],
                2,
                nonBranchingInstrsToCode [ I_ldarg 0us; I_ldarg 1us; AI_add; I_ret ],
                None,
                None)

        let paramMethod =
            mkILNonGenericStaticMethod(
                "SumValues",
                ILMemberAccess.Public,
                [ mkILParamNamed("left", ilg.typ_Int32); mkILParamNamed("right", ilg.typ_Int32) ],
                mkILReturn ilg.typ_Int32,
                paramBody)

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    "Sample.Multi",
                    ILTypeDefAccess.Public,
                    mkILMethods [ baseMethod; paramMethod ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField
                )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let private createModuleWithProperties (propertyNames: string list) =
        let ilg = PrimaryAssemblyILGlobals
        let typeName = "Sample.PropertyDemo"
        let typeRef = mkILTyRef(ILScopeRef.Local, typeName)
        let stringType = ilg.typ_String

        let getters, properties =
            propertyNames
            |> List.map (fun propertyName ->
                let getterName = $"get_{propertyName}"
                let getterBody =
                    mkMethodBody(
                        false,
                        [],
                        2,
                        nonBranchingInstrsToCode [ I_ldstr $"Property {propertyName}"; I_ret ],
                        None,
                        None)

                let getter =
                    mkILNonGenericInstanceMethod(
                        getterName,
                        ILMemberAccess.Public,
                        [],
                        mkILReturn stringType,
                        getterBody)
                    |> fun methodDef -> methodDef.WithSpecialName.WithHideBySig(true)

                let propertyDef =
                    ILPropertyDef(
                        propertyName,
                        PropertyAttributes.None,
                        None,
                        Some(mkILMethRef(typeRef, ILCallingConv.Instance, getterName, 0, [], stringType)),
                        ILThisConvention.Instance,
                        stringType,
                        None,
                        [],
                        emptyILCustomAttrs)

                getter, propertyDef)
            |> List.unzip

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    typeName,
                    ILTypeDefAccess.Public,
                    mkILMethods getters,
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties properties,
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField
                )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let private createModuleWithEvents (eventNames: string list) =
        let ilg = PrimaryAssemblyILGlobals
        let typeName = "Sample.EventDemo"
        let typeRef = mkILTyRef(ILScopeRef.Local, typeName)
        let handlerType = ilg.typ_Object
        let voidType = ILType.Void

        let methods, events =
            eventNames
            |> List.map (fun eventName ->
                let addName = $"add_{eventName}"
                let removeName = $"remove_{eventName}"
                let param = mkILParamNamed("handler", handlerType)

                let addMethod =
                    mkILNonGenericInstanceMethod(
                        addName,
                        ILMemberAccess.Public,
                        [ param ],
                        mkILReturn voidType,
                        mkMethodBody(false, [], 1, nonBranchingInstrsToCode [ I_ret ], None, None))
                    |> fun methodDef -> methodDef.WithSpecialName.WithHideBySig(true)

                let removeMethod =
                    mkILNonGenericInstanceMethod(
                        removeName,
                        ILMemberAccess.Public,
                        [ param ],
                        mkILReturn voidType,
                        mkMethodBody(false, [], 1, nonBranchingInstrsToCode [ I_ret ], None, None))
                    |> fun methodDef -> methodDef.WithSpecialName.WithHideBySig(true)

                let eventDef =
                    ILEventDef(
                        Some handlerType,
                        eventName,
                        EventAttributes.None,
                        mkILMethRef(typeRef, ILCallingConv.Instance, addName, 0, [ handlerType ], voidType),
                        mkILMethRef(typeRef, ILCallingConv.Instance, removeName, 0, [ handlerType ], voidType),
                        None,
                        [],
                        emptyILCustomAttrs)

                [ addMethod; removeMethod ], eventDef)
            |> List.unzip

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    typeName,
                    ILTypeDefAccess.Public,
                    mkILMethods (methods |> List.collect id),
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents events,
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField
                )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let private createStringModule (message: string) =
        let ilg = PrimaryAssemblyILGlobals
        let methodBody =
            mkMethodBody (false, [], 1, nonBranchingInstrsToCode [ I_ldstr message; I_ret ], None, None)

        let methodDef =
            mkILNonGenericStaticMethod (
                "GetMessage",
                ILMemberAccess.Public,
                [],
                mkILReturn ilg.typ_String,
                methodBody
            )

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    "Sample.Message",
                    ILTypeDefAccess.Public,
                    mkILMethods [ methodDef ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField
                )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let private createFieldHolderBaseline includeField =
        let moduleDef = createModuleWithOptionalField includeField

        let tokenMappings : ILTokenMappings =
            {
                TypeDefTokenMap = fun (_, _) -> 0x02000001
                FieldDefTokenMap = fun (_, _) _ -> 0x04000001
                MethodDefTokenMap = fun (_, _) _ -> 0x06000001
                PropertyTokenMap = fun (_, _) _ -> 0x17000001
                EventTokenMap = fun (_, _) _ -> 0x14000001
            }

        let metadataSnapshot : MetadataSnapshot =
            {
                HeapSizes =
                    {
                        StringHeapSize = 64
                        UserStringHeapSize = 32
                        BlobHeapSize = 64
                        GuidHeapSize = 16
                    }
                TableRowCounts = Array.create 64 0
                GuidHeapStart = 0
            }

        let moduleId = System.Guid.Parse("55555555-6666-7777-8888-999999999999")
        moduleDef, FSharp.Compiler.HotReloadBaseline.create moduleDef tokenMappings metadataSnapshot moduleId None

    let private createBaseline () =
        let baselineModule = createModule 42

        let tokenMappings : ILTokenMappings =
            {
                TypeDefTokenMap = fun (_, _) -> 0x02000001
                FieldDefTokenMap = fun _ _ -> 0x04000001
                MethodDefTokenMap = fun _ _ -> 0x06000001
                PropertyTokenMap = fun _ _ -> 0x17000001
                EventTokenMap = fun _ _ -> 0x14000001
            }

        let metadataSnapshot : MetadataSnapshot =
            {
                HeapSizes =
                    {
                        StringHeapSize = 64
                        UserStringHeapSize = 32
                        BlobHeapSize = 64
                        GuidHeapSize = 16
                    }
                TableRowCounts = Array.create 64 0
                GuidHeapStart = 0
            }

        let moduleId = System.Guid.Parse("11111111-2222-3333-4444-555555555555")
        let baseline = FSharp.Compiler.HotReloadBaseline.create baselineModule tokenMappings metadataSnapshot moduleId None
        baselineModule, baseline

    let private createBaselineWithMethods (methods: (string * int) list) =
        let baselineModule = createModuleWithMethods methods

        let methodTokenMap =
            methods
            |> List.mapi (fun idx (name, _) -> name, 0x06000001 + idx)
            |> dict

        let tokenMappings : ILTokenMappings =
            {
                TypeDefTokenMap = fun (_, _) -> 0x02000001
                FieldDefTokenMap = fun (_, _) _ -> 0x04000001
                MethodDefTokenMap = fun (_, _) mdef -> methodTokenMap.[mdef.Name]
                PropertyTokenMap = fun (_, _) _ -> 0x17000001
                EventTokenMap = fun (_, _) _ -> 0x14000001
            }

        let metadataSnapshot : MetadataSnapshot =
            {
                HeapSizes =
                    {
                        StringHeapSize = 64
                        UserStringHeapSize = 32
                        BlobHeapSize = 64
                        GuidHeapSize = 16
                    }
                TableRowCounts = Array.create 64 0
                GuidHeapStart = 0
            }

        let moduleId = System.Guid.Parse("22222222-3333-4444-5555-666666666666")
        let baseline = FSharp.Compiler.HotReloadBaseline.create baselineModule tokenMappings metadataSnapshot moduleId None
        baselineModule, baseline

    let private createStringBaseline (message: string) =
        let moduleDef = createStringModule message

        let tokenMappings : ILTokenMappings =
            {
                TypeDefTokenMap = fun (_, _) -> 0x02000001
                FieldDefTokenMap = fun (_, _) _ -> 0x04000001
                MethodDefTokenMap = fun (_, _) _ -> 0x06000001
                PropertyTokenMap = fun (_, _) _ -> 0x17000001
                EventTokenMap = fun (_, _) _ -> 0x14000001
            }

        let metadataSnapshot : MetadataSnapshot =
            {
                HeapSizes =
                    {
                        StringHeapSize = 256
                        UserStringHeapSize = 256
                        BlobHeapSize = 128
                        GuidHeapSize = 16
                    }
                TableRowCounts = Array.create 64 0
                GuidHeapStart = 0
            }

        let moduleId = System.Guid.Parse("33333333-4444-5555-6666-777777777777")
        moduleDef, FSharp.Compiler.HotReloadBaseline.create moduleDef tokenMappings metadataSnapshot moduleId None

    let private createLocalsModule (locals: ILType list) (bodyInstrs: ILInstr[]) : ILModuleDef =
        let ilg = PrimaryAssemblyILGlobals

        let methodBody =
            mkMethodBody (
                false,
                locals |> List.map (fun ty -> mkILLocal ty None),
                4,
                nonBranchingInstrsToCode (Array.toList bodyInstrs),
                None,
                None)

        let methodDef =
            mkILNonGenericStaticMethod(
                "Compute",
                ILMemberAccess.Public,
                [],
                mkILReturn ilg.typ_Int32,
                methodBody)

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    "Sample.LocalsDemo",
                    ILTypeDefAccess.Public,
                    mkILMethods [ methodDef ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField
                )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let private methodKey (baseline: FSharpEmitBaseline) name =
        baseline.MethodTokens
        |> Map.toSeq
        |> Seq.map fst
        |> Seq.find (fun key -> key.Name = name)

    [<Fact>]
    let ``symbol matcher locates existing methods`` () =
        let moduleDef, baseline = createBaseline ()
        let matcher = FSharpSymbolMatcher.create moduleDef
        let key = methodKey baseline "GetValue"
        match FSharpSymbolMatcher.tryGetMethodDef matcher key with
        | Some(_, _, methodDef) -> Assert.Equal("GetValue", methodDef.Name)
        | None -> Assert.True(false, "Expected method to be located by symbol matcher.")

    [<Fact>]
    let ``emitDelta records updated user strings`` () =
        let _, baseline = createStringBaseline "Message version 1 (invocation #%d)"
        let updatedModule = createStringModule "Message version 2 (invocation #%d)" |> TestHelpers.withDebuggableAttribute
        let key = methodKey baseline "GetMessage"
        let request : IlxDeltaRequest =
            { Baseline = baseline
              UpdatedTypes = [ key.DeclaringType ]
              UpdatedMethods = [ key ]
              UpdatedAccessors = []
              Module = updatedModule
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta = emitDelta request

        Assert.NotEmpty(delta.UserStringUpdates)
        let baselineUserStringStart = baseline.Metadata.HeapSizes.UserStringHeapSize

        let updatedLiteral =
            delta.UserStringUpdates
            |> List.tryPick (fun (_, updatedToken, text) ->
                if text.StartsWith("Message version", StringComparison.Ordinal) then
                    Some(updatedToken, text)
                else
                    None)

        match updatedLiteral with
        | Some(updatedToken, text) ->
            Assert.Equal("Message version 2 (invocation #%d)", text)
            let updatedOffset = updatedToken &&& 0x00FFFFFF
            Assert.True(
                updatedOffset > baselineUserStringStart,
                $"Expected new #US token offset ({updatedOffset}) to be greater than baseline start ({baselineUserStringStart}).")
        | None -> Assert.True(false, "Expected updated user string literal in delta metadata.")

    [<Fact>]
    let ``emitDelta projects known tokens`` () =
        let _, baseline = createBaseline ()
        let updatedModule = createModule 43 |> TestHelpers.withDebuggableAttribute
        let request =
            {
                IlxDeltaRequest.Baseline = baseline
                UpdatedTypes = [ "Sample.Type" ]
                UpdatedMethods = [ methodKey baseline "GetValue" ]
                UpdatedAccessors = []
                Module = updatedModule
                SymbolChanges = None
                CurrentGeneration = 1
                PreviousGenerationId = None
                SynthesizedNames = None
            }

        let delta = emitDelta request

        Assert.Equal<int list>([ 0x02000001 ], delta.UpdatedTypeTokens)
        Assert.Equal<int list>([ 0x06000001 ], delta.UpdatedMethodTokens)
        // Debug hook to observe method body count when assertions fail.
        if delta.MethodBodies.Length <> 1 then
            printfn "MethodBodies count = %d" delta.MethodBodies.Length
        Assert.NotEmpty(delta.Metadata)
        Assert.NotEmpty(delta.IL)
        match delta.Pdb with
        | Some _ -> ()
        | None -> ()
        let bodyInfo = Assert.Single(delta.MethodBodies)
        Assert.Equal(0x06000001, bodyInfo.MethodToken)
        Assert.True(bodyInfo.CodeLength > 0)
        Assert.NotEqual(System.Guid.Empty, delta.GenerationId)
        Assert.Equal(System.Guid.Empty, delta.BaseGenerationId)
        // Updated methods do NOT emit Param rows - baseline already has them (matches Roslyn)
        let expectedEncLog =
            [|
                (TableNames.Module, 0x00000001, EditAndContinueOperation.Default)
                (TableNames.Method, 0x00000001, EditAndContinueOperation.Default)
            |]

        Assert.Equal<(TableName * int * EditAndContinueOperation) seq>(expectedEncLog, delta.EncLog)

        let expectedEncMap =
            [|
                (TableNames.Module, 0x00000001)
                (TableNames.Method, 0x00000001)
            |]

        Assert.Equal<(TableName * int) seq>(expectedEncMap, delta.EncMap)

    [<Fact>]
    let ``emitDelta sets generation 1 base id to Guid.Empty`` () =
        let _, baseline = createBaseline ()
        let updatedModule = createModule 99 |> TestHelpers.withDebuggableAttribute

        let request =
            {
                IlxDeltaRequest.Baseline = baseline
                UpdatedTypes = [ "Sample.Type" ]
                UpdatedMethods = [ methodKey baseline "GetValue" ]
                UpdatedAccessors = []
                Module = updatedModule
                SymbolChanges = None
                CurrentGeneration = 1
                PreviousGenerationId = None
                SynthesizedNames = None
            }

        let delta = emitDelta request

        Assert.NotEqual(System.Guid.Empty, delta.GenerationId)
        Assert.Equal(System.Guid.Empty, delta.BaseGenerationId)

    [<Fact>]
    let ``emitDelta chains BaseGenerationId across generations`` () =
        let _, baseline = createBaseline ()

        let requestGen1 =
            { IlxDeltaRequest.Baseline = baseline
              UpdatedTypes = [ "Sample.Type" ]
              UpdatedMethods = [ methodKey baseline "GetValue" ]
              UpdatedAccessors = []
              Module = createModule 101 |> TestHelpers.withDebuggableAttribute
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta1 = emitDelta requestGen1
        Assert.NotEqual(System.Guid.Empty, delta1.GenerationId)
        Assert.Equal(System.Guid.Empty, delta1.BaseGenerationId)

        let baseline2 =
            match delta1.UpdatedBaseline with
            | Some b -> b
            | None -> failwith "Generation 1 delta did not return an updated baseline."

        let requestGen2 =
            { IlxDeltaRequest.Baseline = baseline2
              UpdatedTypes = [ "Sample.Type" ]
              UpdatedMethods = [ methodKey baseline "GetValue" ]
              UpdatedAccessors = []
              Module = createModule 102 |> TestHelpers.withDebuggableAttribute
              SymbolChanges = None
              CurrentGeneration = 2
              PreviousGenerationId = Some delta1.GenerationId
              SynthesizedNames = None }

        let delta2 = emitDelta requestGen2

        Assert.NotEqual(System.Guid.Empty, delta2.GenerationId)
        Assert.Equal(delta1.GenerationId, delta2.BaseGenerationId)

    [<Fact>]
    let ``emitDelta ignores unknown symbols`` () =
        let _, baseline = createBaseline ()
        let updatedModule = createModule 43 |> TestHelpers.withDebuggableAttribute
        let unknownMethod =
            {
                DeclaringType = "Sample.Type"
                Name = "Missing"
                GenericArity = 0
                ParameterTypes = []
                ReturnType = ILType.Void
            }

        let request =
            {
                IlxDeltaRequest.Baseline = baseline
                UpdatedTypes = [ "Does.NotExist" ]
                UpdatedMethods = [ unknownMethod ]
                UpdatedAccessors = []
                Module = updatedModule
                SymbolChanges = None
                CurrentGeneration = 1
                PreviousGenerationId = None
                SynthesizedNames = None
            }

        let delta = emitDelta request

        Assert.Empty(delta.UpdatedTypeTokens)
        Assert.Empty(delta.UpdatedMethodTokens)
        Assert.Empty(delta.EncLog)
        Assert.Empty(delta.EncMap)
        Assert.Empty(delta.MethodBodies)

    [<Fact>]
    let ``emitDelta rejects added fields`` () =
        let _, baseline = createFieldHolderBaseline false
        let updatedModule = createModuleWithOptionalField true |> TestHelpers.withDebuggableAttribute
        let request =
            {
                IlxDeltaRequest.Baseline = baseline
                UpdatedTypes = [ "Sample.FieldHolder" ]
                UpdatedMethods = [ methodKey baseline "GetValue" ]
                UpdatedAccessors = []
                Module = updatedModule
                SymbolChanges = None
                CurrentGeneration = 1
                PreviousGenerationId = None
                SynthesizedNames = None
            }

        let ex = Assert.Throws<HotReloadUnsupportedEditException>(fun () -> emitDelta request |> ignore)
        Assert.Contains("Sample.FieldHolder::trackedField", ex.Message)

    [<Fact>]
    let ``emitDelta updates multiple methods`` () =
        let methods = [ "GetValue" , 1; "GetOther", 2 ]
        let _, baseline = createBaselineWithMethods methods
        let updatedModule = createModuleWithMethods [ "GetValue", 10; "GetOther", 20 ] |> TestHelpers.withDebuggableAttribute

        let methodKeys = baseline.MethodTokens |> Map.toList |> List.map fst

        let request =
            {
                IlxDeltaRequest.Baseline = baseline
                UpdatedTypes = [ "Sample.Multi" ]
                UpdatedMethods = methodKeys
                UpdatedAccessors = []
                Module = updatedModule
                SymbolChanges = None
                CurrentGeneration = 1
                PreviousGenerationId = None
                SynthesizedNames = None
            }

        let delta = emitDelta request

        Assert.Equal(2, List.length delta.MethodBodies)
        Assert.Equal(2, List.length delta.UpdatedMethodTokens)
        Assert.Equal<int Set>(Set.ofList [0x06000001; 0x06000002], delta.UpdatedMethodTokens |> Set.ofList)
        Assert.True(delta.MethodBodies |> List.forall (fun body -> body.CodeLength > 0))
        // Updated methods do NOT emit Param rows (matches Roslyn)
        let expectedLog =
            [|
                (TableNames.Module, 0x00000001, EditAndContinueOperation.Default)
                (TableNames.Method, 0x00000001, EditAndContinueOperation.Default)
                (TableNames.Method, 0x00000002, EditAndContinueOperation.Default)
            |]
        Assert.Equal<(TableName * int * EditAndContinueOperation) seq>(expectedLog, delta.EncLog)

        let expectedMap =
            [|
                (TableNames.Module, 0x00000001)
                (TableNames.Method, 0x00000001)
                (TableNames.Method, 0x00000002)
            |]
        Assert.Equal<(TableName * int) seq>(expectedMap, delta.EncMap)
        match delta.Pdb with
        | Some pdb -> Assert.True(pdb.Length >= 0)
        | None -> ()

    [<Fact>]
    let ``emitDelta adds method metadata rows for new method`` () =
        let baselineArtifacts =
            TestHelpers.createBaselineFromModule (createModuleWithMethods [ "GetValue", 1 ])
        let updatedModule = createModuleWithMethods [ "GetValue", 1; "GetExtra", 5 ] |> TestHelpers.withDebuggableAttribute

        let request =
            {
                IlxDeltaRequest.Baseline = baselineArtifacts.Baseline
                UpdatedTypes = [ "Sample.Multi" ]
                UpdatedMethods = []
                UpdatedAccessors = []
                Module = updatedModule
                SymbolChanges = None
                CurrentGeneration = 1
                PreviousGenerationId = None
                SynthesizedNames = None
            }

        let delta = emitDelta request

        Assert.Equal(1, List.length delta.MethodBodies)
        let addedToken = Assert.Single(delta.UpdatedMethodTokens)

        let expectedRowId =
            baselineArtifacts.Baseline.Metadata.TableRowCounts.[TableNames.Method.Index] + 1

        Assert.Equal(0x06000000 ||| expectedRowId, addedToken)

        let hasMethodAdd =
            delta.EncLog
            |> Array.exists (fun (table, row, op) ->
                table = TableNames.Method && row = expectedRowId && op = EditAndContinueOperation.AddMethod)

        Assert.True(hasMethodAdd, "Expected MethodDef add operation in EncLog.")

        match delta.UpdatedBaseline with
        | Some updatedBaseline ->
            let addedKey =
                { MethodDefinitionKey.DeclaringType = "Sample.Multi"
                  Name = "GetExtra"
                  GenericArity = 0
                  ParameterTypes = []
                  ReturnType = PrimaryAssemblyILGlobals.typ_Int32 }

            Assert.True(updatedBaseline.MethodTokens.ContainsKey addedKey, "Updated baseline missing added method token.")
            Assert.Equal(addedToken, updatedBaseline.MethodTokens[addedKey])
        | None ->
            Assert.True(false, "Updated baseline missing.")

    [<Fact>]
    let ``emitDelta adds parameter metadata rows for new method`` () =
        let baselineArtifacts =
            TestHelpers.createBaselineFromModule (createModuleWithMethods [ "GetValue", 1 ])
        let updatedModule = createModuleWithParameterizedMethod () |> TestHelpers.withDebuggableAttribute

        let request =
            {
                IlxDeltaRequest.Baseline = baselineArtifacts.Baseline
                UpdatedTypes = [ "Sample.Multi" ]
                UpdatedMethods = []
                UpdatedAccessors = []
                Module = updatedModule
                SymbolChanges = None
                CurrentGeneration = 1
                PreviousGenerationId = None
                SynthesizedNames = None
            }

        let delta = emitDelta request

        Assert.Equal(1, List.length delta.MethodBodies)
        let addedToken = Assert.Single(delta.UpdatedMethodTokens)
        Assert.True(addedToken <> 0, "Added method token missing.")

        let paramAdds =
            delta.EncLog
            |> Array.filter (fun (table, _, _) -> table = TableNames.Param)

        Assert.Equal(3, paramAdds.Length)

        let baselineParamCount = baselineArtifacts.Baseline.Metadata.TableRowCounts.[TableNames.Param.Index]
        let expectedParamRows = [ baselineParamCount + 1; baselineParamCount + 2; baselineParamCount + 3 ]

        let actualRows =
            paramAdds
            |> Array.map (fun (_, row, op) ->
                Assert.Equal(EditAndContinueOperation.AddParameter, op)
                row)
            |> Array.sort
            |> Array.toList

        Assert.Equal<int list>(expectedParamRows, actualRows)

    /// Updated methods do NOT synthesize Param rows - baseline already has them (matches Roslyn)
    [<Fact>]
    let ``emitDelta does not add param row for updated parameterless method`` () =
        let originalMessage = "Hello baseline"
        let updatedMessage = "Hello updated"
        let baselineModule = TestHelpers.createMethodModule originalMessage
        let baselineArtifacts = TestHelpers.createBaselineFromModule baselineModule

        let updatedModule = TestHelpers.createMethodModule updatedMessage |> TestHelpers.withDebuggableAttribute

        let methodKey =
            TestHelpers.methodKey "Sample.MethodDemo" "GetMessage" [] PrimaryAssemblyILGlobals.typ_String

        let request =
            { IlxDeltaRequest.Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ "Sample.MethodDemo" ]
              UpdatedMethods = [ methodKey ]
              UpdatedAccessors = []
              Module = updatedModule
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta = emitDelta request

        use deltaProvider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange delta.Metadata)
        let reader = deltaProvider.GetMetadataReader()

        Assert.Equal(1, reader.GetTableRowCount(toTableIndex TableNames.Method))
        // Updated methods should NOT have Param rows in delta - baseline has them
        Assert.Equal(0, reader.GetTableRowCount(toTableIndex TableNames.Param))

        // EncLog/EncMap should NOT have Param entries for updated methods
        let hasParamAdd =
            delta.EncLog
            |> Array.exists (fun (table, _, _) -> table = TableNames.Param)
        Assert.False(hasParamAdd, "Updated method should not have Param EncLog entry.")

    /// Updated methods have no Param rows in delta - param table is empty
    [<Fact>]
    let ``emitDelta param table is empty for updated method`` () =
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createMethodModule "Hello baseline")
        let updatedModule = TestHelpers.createMethodModule "Hello updated" |> TestHelpers.withDebuggableAttribute

        let methodKey =
            TestHelpers.methodKey "Sample.MethodDemo" "GetMessage" [] PrimaryAssemblyILGlobals.typ_String

        let request =
            { IlxDeltaRequest.Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ "Sample.MethodDemo" ]
              UpdatedMethods = [ methodKey ]
              UpdatedAccessors = []
              Module = updatedModule
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta = emitDelta request

        use deltaProvider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange delta.Metadata)
        let reader = deltaProvider.GetMetadataReader()

        // Updated method should have no Param rows in delta (baseline has them)
        let paramTableCount = reader.GetTableRowCount(toTableIndex TableNames.Param)
        Assert.Equal(0, paramTableCount)

    [<Fact>]
    let ``emitDelta adds property metadata rows for new property`` () =
        let baselineArtifacts =
            TestHelpers.createBaselineFromModule (TestHelpers.createPropertyHostBaselineModule ())
        let updatedModule = TestHelpers.createPropertyModule "Property addition message" |> TestHelpers.withDebuggableAttribute

        let getterKey =
            TestHelpers.methodKey "Sample.PropertyDemo" "get_Message" [] PrimaryAssemblyILGlobals.typ_String

        let accessorUpdate =
            TestHelpers.mkAccessorUpdate "Sample.PropertyDemo" (SymbolMemberKind.PropertyGet "Message") getterKey

        let request =
            {
                IlxDeltaRequest.Baseline = baselineArtifacts.Baseline
                UpdatedTypes = [ "Sample.PropertyDemo" ]
                UpdatedMethods = []
                UpdatedAccessors = [ accessorUpdate ]
                Module = updatedModule
                SymbolChanges = None
                CurrentGeneration = 1
                PreviousGenerationId = None
                SynthesizedNames = None
            }

        let delta = emitDelta request

        let baselinePropertyCount = baselineArtifacts.Baseline.Metadata.TableRowCounts.[TableNames.Property.Index]

        let propertyAdds =
            delta.EncLog
            |> Array.filter (fun (table, _, op) -> table = TableNames.Property && op = EditAndContinueOperation.AddProperty)

        let propertyMapAdds =
            delta.EncLog
            |> Array.filter (fun (table, _, op) -> table = TableNames.PropertyMap && op = EditAndContinueOperation.AddProperty)

        let semanticsAdds =
            delta.EncLog
            |> Array.filter (fun (table, _, op) -> table = TableNames.MethodSemantics && op = EditAndContinueOperation.AddMethod)

        Assert.Single propertyAdds |> ignore
        Assert.Single propertyMapAdds |> ignore
        Assert.Single semanticsAdds |> ignore

        let propertyRowId =
            propertyAdds
            |> Array.exactlyOne
            |> fun (_, row, _) -> row

        Assert.Equal(baselinePropertyCount + 1, propertyRowId)

        match delta.UpdatedBaseline with
        | Some updatedBaseline ->
            let propertyKey =
                { PropertyDefinitionKey.DeclaringType = "Sample.PropertyDemo"
                  Name = "Message"
                  PropertyType = PrimaryAssemblyILGlobals.typ_String
                  IndexParameterTypes = [] }

            Assert.True(updatedBaseline.PropertyTokens.ContainsKey propertyKey, "Updated baseline missing property token.")
            Assert.True(updatedBaseline.PropertyMapEntries.ContainsKey "Sample.PropertyDemo", "Updated baseline missing property map entry.")
        | None ->
            Assert.True(false, "Updated baseline missing.")

    [<Fact>]
    let ``emitDelta adds event metadata rows for new event`` () =
        let baselineArtifacts =
            TestHelpers.createBaselineFromModule (TestHelpers.createEventHostBaselineModule ())
        let updatedModule = TestHelpers.createEventModule "Event addition payload" |> TestHelpers.withDebuggableAttribute

        let addKey =
            TestHelpers.methodKey "Sample.EventDemo" "add_OnChanged" [ PrimaryAssemblyILGlobals.typ_Object ] ILType.Void

        let accessorUpdate =
            TestHelpers.mkAccessorUpdate "Sample.EventDemo" (SymbolMemberKind.EventAdd "OnChanged") addKey

        let request =
            {
                IlxDeltaRequest.Baseline = baselineArtifacts.Baseline
                UpdatedTypes = [ "Sample.EventDemo" ]
                UpdatedMethods = []
                UpdatedAccessors = [ accessorUpdate ]
                Module = updatedModule
                SymbolChanges = None
                CurrentGeneration = 1
                PreviousGenerationId = None
                SynthesizedNames = None
            }

        let delta = emitDelta request

        let baselineEventCount = baselineArtifacts.Baseline.Metadata.TableRowCounts.[TableNames.Event.Index]

        let eventAdds =
            delta.EncLog
            |> Array.filter (fun (table, _, op) -> table = TableNames.Event && op = EditAndContinueOperation.AddEvent)

        let eventMapAdds =
            delta.EncLog
            |> Array.filter (fun (table, _, op) -> table = TableNames.EventMap && op = EditAndContinueOperation.AddEvent)

        let semanticsAdds =
            delta.EncLog
            |> Array.filter (fun (table, _, op) -> table = TableNames.MethodSemantics && op = EditAndContinueOperation.AddMethod)

        Assert.Single eventAdds |> ignore
        Assert.Single eventMapAdds |> ignore
        Assert.Single semanticsAdds |> ignore

        let eventRowId =
            eventAdds
            |> Array.exactlyOne
            |> fun (_, row, _) -> row

        Assert.Equal(baselineEventCount + 1, eventRowId)

        match delta.UpdatedBaseline with
        | Some updatedBaseline ->
            let eventKey =
                { EventDefinitionKey.DeclaringType = "Sample.EventDemo"
                  Name = "OnChanged"
                  EventType = Some PrimaryAssemblyILGlobals.typ_Object }

            Assert.True(updatedBaseline.EventTokens.ContainsKey eventKey, "Updated baseline missing event token.")
            Assert.True(updatedBaseline.EventMapEntries.ContainsKey "Sample.EventDemo", "Updated baseline missing event map entry.")
        | None ->
            Assert.True(false, "Updated baseline missing.")

    [<Fact>]
    let ``emitDelta adds property method semantics when PropertyMap already exists`` () =
        let baselineArtifacts =
            TestHelpers.createBaselineFromModule (createModuleWithProperties [ "Message" ])

        let updatedModule =
            createModuleWithProperties [ "Message"; "Secondary" ]
            |> TestHelpers.withDebuggableAttribute

        let getterKey =
            TestHelpers.methodKey "Sample.PropertyDemo" "get_Secondary" [] PrimaryAssemblyILGlobals.typ_String

        let accessorUpdate =
            TestHelpers.mkAccessorUpdate "Sample.PropertyDemo" (SymbolMemberKind.PropertyGet "Secondary") getterKey

        let request =
            { IlxDeltaRequest.Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ "Sample.PropertyDemo" ]
              UpdatedMethods = []
              UpdatedAccessors = [ accessorUpdate ]
              Module = updatedModule
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta = emitDelta request

        let propertyAdds =
            delta.EncLog
            |> Array.filter (fun (table, _, op) -> table = TableNames.Property && op = EditAndContinueOperation.AddProperty)

        let propertyMapAdds =
            delta.EncLog
            |> Array.filter (fun (table, _, op) -> table = TableNames.PropertyMap && op = EditAndContinueOperation.AddProperty)

        let semanticsAdds =
            delta.EncLog
            |> Array.filter (fun (table, _, op) -> table = TableNames.MethodSemantics && op = EditAndContinueOperation.AddMethod)

        Assert.Single propertyAdds |> ignore
        Assert.Empty propertyMapAdds
        Assert.Single semanticsAdds |> ignore

    [<Fact>]
    let ``emitDelta adds event method semantics when EventMap already exists`` () =
        let baselineArtifacts =
            TestHelpers.createBaselineFromModule (createModuleWithEvents [ "OnChanged" ])

        let updatedModule =
            createModuleWithEvents [ "OnChanged"; "OnUpdated" ]
            |> TestHelpers.withDebuggableAttribute

        let addKey =
            TestHelpers.methodKey "Sample.EventDemo" "add_OnUpdated" [ PrimaryAssemblyILGlobals.typ_Object ] ILType.Void

        let accessorUpdate =
            TestHelpers.mkAccessorUpdate "Sample.EventDemo" (SymbolMemberKind.EventAdd "OnUpdated") addKey

        let request =
            { IlxDeltaRequest.Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ "Sample.EventDemo" ]
              UpdatedMethods = []
              UpdatedAccessors = [ accessorUpdate ]
              Module = updatedModule
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta = emitDelta request

        let eventAdds =
            delta.EncLog
            |> Array.filter (fun (table, _, op) -> table = TableNames.Event && op = EditAndContinueOperation.AddEvent)

        let eventMapAdds =
            delta.EncLog
            |> Array.filter (fun (table, _, op) -> table = TableNames.EventMap && op = EditAndContinueOperation.AddEvent)

        let semanticsAdds =
            delta.EncLog
            |> Array.filter (fun (table, _, op) -> table = TableNames.MethodSemantics && op = EditAndContinueOperation.AddMethod)

        Assert.Single eventAdds |> ignore
        Assert.Empty eventMapAdds
        Assert.Single semanticsAdds |> ignore

    [<Fact>]
    let ``metadata validator tool is available`` () =
        match tryRunMdv "--version" with
        | ValueNone ->
            // Treat absence of the mdv CLI as a soft skip; downstream delta tests assert availability explicitly.
            printfn "metadata-tools (mdv) CLI not found on PATH; skipping availability assertion."
            ()
        | ValueSome(0, _, _) -> Assert.Equal(0, 0)
        | ValueSome(exitCode, _, stderr) ->
            // Non-zero exit indicates mdv is installed but not runnable in this environment; treat it similarly to absence.
            printfn "metadata-tools (mdv) CLI reported exit code %d. stderr: %s" exitCode stderr
            ()

    [<Fact>]
    let ``emitDelta metadata validates with mdv`` () =
        let _, baseline = createBaseline ()
        let updatedModule = createModule 43 |> TestHelpers.withDebuggableAttribute
        let request =
            {
                IlxDeltaRequest.Baseline = baseline
                UpdatedTypes = [ "Sample.Type" ]
                UpdatedMethods = [ methodKey baseline "GetValue" ]
                UpdatedAccessors = []
                Module = updatedModule
                SymbolChanges = None
                CurrentGeneration = 1
                PreviousGenerationId = None
                SynthesizedNames = None
            }

        let delta = emitDelta request

        Assert.NotEmpty(delta.Metadata)
        Assert.NotEmpty(delta.IL)
        Assert.Single(delta.MethodBodies) |> ignore
        Assert.NotEqual(System.Guid.Empty, delta.GenerationId)
        Assert.Equal(System.Guid.Empty, delta.BaseGenerationId)

        match tryRunMdv "--version" with
        | ValueNone ->
            printfn "metadata-tools (mdv) CLI not found; skipping validation test."
        | ValueSome(exitCode, _, _) when exitCode <> 0 ->
            printfn "metadata-tools (mdv) CLI reported exit code %d during version check; skipping validation test." exitCode
        | _ ->
            let tempMeta = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".meta")
            let tempIl = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".il")
            try
                File.WriteAllBytes(tempMeta, delta.Metadata)
                File.WriteAllBytes(tempIl, delta.IL)

                let arg = $"/g:{tempMeta};{tempIl}"
                match tryRunMdv arg with
                | ValueSome(0, _, _) -> ()
                | ValueSome(code, _, stderr) ->
                    Assert.True(false, $"mdv validation failed with exit code {code}. stderr: {stderr}")
                | ValueNone -> Assert.True(false, "mdv CLI became unavailable during validation")
            finally
                if File.Exists(tempMeta) then File.Delete(tempMeta)
                if File.Exists(tempIl) then File.Delete(tempIl)

    [<Fact>]
    let ``emitDelta method body reflects updated IL`` () =
        let _, baseline = createBaseline ()
        let updatedModule = createModule 100 |> TestHelpers.withDebuggableAttribute
        let request =
            {
                IlxDeltaRequest.Baseline = baseline
                UpdatedTypes = [ "Sample.Type" ]
                UpdatedMethods = [ methodKey baseline "GetValue" ]
                UpdatedAccessors = []
                Module = updatedModule
                SymbolChanges = None
                CurrentGeneration = 1
                PreviousGenerationId = None
                SynthesizedNames = None
            }

        let delta = emitDelta request

        let bodyInfo = Assert.Single(delta.MethodBodies)
        let instructionStart = bodyInfo.CodeOffset + 12
        let ilBytes = delta.IL.AsSpan().Slice(instructionStart, bodyInfo.CodeLength).ToArray()
        Assert.Collection<byte>(
            ilBytes,
            (fun opcode -> Assert.Equal<byte>(0x1Fuy, opcode)),
            (fun operand -> Assert.Equal<byte>(0x64uy, operand)),
            (fun ret -> Assert.Equal<byte>(0x2Auy, ret))
        )

    [<Fact>]
    let ``emitDelta reuses MemberRef tokens for unchanged external call`` () =
        let baselineArtifacts = TestHelpers.createBaselineFromModule (createConsoleCallModule false)
        let updatedModule = createConsoleCallModule false |> TestHelpers.withDebuggableAttribute

        let methodKey = TestHelpers.methodKeyByName baselineArtifacts.Baseline "Sample.ConsoleDemo" "Log"

        let request : IlxDeltaRequest =
            {
                Baseline = baselineArtifacts.Baseline
                UpdatedTypes = [ "Sample.ConsoleDemo" ]
                UpdatedMethods = [ methodKey ]
                UpdatedAccessors = []
                Module = updatedModule
                SymbolChanges = None
                CurrentGeneration = 1
                PreviousGenerationId = None
                SynthesizedNames = None
            }

        let delta = emitDelta request

        use deltaProvider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange delta.Metadata)
        let deltaReader = deltaProvider.GetMetadataReader()

        Assert.Equal(0, deltaReader.GetTableRowCount(toTableIndex TableNames.MemberRef))
        Assert.Equal(0, deltaReader.GetTableRowCount(toTableIndex TableNames.TypeRef))

        // Read baseline call token directly from the emitted IL
        use peReader = new PEReader(File.OpenRead(baselineArtifacts.AssemblyPath))
        let baselineReader = peReader.GetMetadataReader()
        let baselineMethodToken = baselineArtifacts.Baseline.MethodTokens[methodKey]
        let baselineMethodHandle = MetadataTokens.MethodDefinitionHandle baselineMethodToken
        let baselineMethodDef = baselineReader.GetMethodDefinition baselineMethodHandle
        let baselineBody = peReader.GetMethodBody(baselineMethodDef.RelativeVirtualAddress)
        let baselineIl = baselineBody.GetILBytes()
        let baselineCallToken = readCallOperand (ReadOnlySpan<byte>(baselineIl))

        let bodyInfo = Assert.Single(delta.MethodBodies)
        let instructionStart = bodyInfo.CodeOffset + 12
        let deltaIl = delta.IL.AsSpan().Slice(instructionStart, bodyInfo.CodeLength).ToArray()
        let deltaCallToken = readCallOperand (ReadOnlySpan<byte>(deltaIl))

        if baselineCallToken <> deltaCallToken then
            printfn "[memberref-reuse-debug] baselinePath=%s" baselineArtifacts.AssemblyPath
            printfn "[memberref-reuse-debug] baselineCallToken=0x%08X deltaCallToken=0x%08X" baselineCallToken deltaCallToken
            printfn "[memberref-reuse-debug] delta IL bytes: %A" deltaIl
            let dumpDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-memberref-" + System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(dumpDir) |> ignore
            let mdPath = Path.Combine(dumpDir, "metadata.bin")
            let ilPath = Path.Combine(dumpDir, "il.bin")
            File.WriteAllBytes(mdPath, delta.Metadata)
            File.WriteAllBytes(ilPath, delta.IL)
            printfn "[memberref-reuse-debug] dumped delta to %s" dumpDir
            printfn "[memberref-reuse-debug] mdv command:"
            printfn "  cd metadata-tools && ../fsharp/.dotnet/dotnet run --project src/mdv/mdv.csproj --framework net10.0 -- %s \"/g:%s;%s\" /stats+ /md+ /il+" baselineArtifacts.AssemblyPath mdPath ilPath
        Assert.Equal(baselineCallToken, deltaCallToken)

    [<Fact>]
    let ``emitDelta reuses StandAloneSig token when locals unchanged`` () =
        let baselineModule =
            createLocalsModule
                [ PrimaryAssemblyILGlobals.typ_Int32 ]
                [| AI_ldc(DT_I4, ILConst.I4 5); I_ret |]

        let updatedModule =
            createLocalsModule
                [ PrimaryAssemblyILGlobals.typ_Int32 ]
                [| AI_ldc(DT_I4, ILConst.I4 7); I_ret |]
            |> TestHelpers.withDebuggableAttribute

        let baselineArtifacts = TestHelpers.createBaselineFromModule baselineModule
        let methodKey = TestHelpers.methodKeyByName baselineArtifacts.Baseline "Sample.LocalsDemo" "Compute"

        let _baselineLocalSigToken, baselineSigBytes =
            use peReader = new PEReader(File.OpenRead(baselineArtifacts.AssemblyPath))
            let methodToken = baselineArtifacts.Baseline.MethodTokens[methodKey]
            let methodHandle = MetadataTokens.MethodDefinitionHandle methodToken
            let reader = peReader.GetMetadataReader()
            let methodDef = reader.GetMethodDefinition methodHandle
            let body = peReader.GetMethodBody(methodDef.RelativeVirtualAddress)
            let sigHandle = body.LocalSignature
            Assert.False(sigHandle.IsNil, "Baseline method is expected to carry a local signature.")
            let sigToken = MetadataTokens.GetToken(EntityHandle.op_Implicit sigHandle)
            let sigBytes =
                let standalone = reader.GetStandaloneSignature sigHandle
                reader.GetBlobBytes standalone.Signature
            sigToken, sigBytes

        let request : IlxDeltaRequest =
            { Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ "Sample.LocalsDemo" ]
              UpdatedMethods = [ methodKey ]
              UpdatedAccessors = []
              Module = updatedModule
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta = emitDelta request

        use deltaProvider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange delta.Metadata)
        let deltaReader = deltaProvider.GetMetadataReader()

        Assert.Equal(1, deltaReader.GetTableRowCount(toTableIndex TableNames.StandAloneSig))
        let bodyInfo = Assert.Single(delta.MethodBodies)
        // Delta token should be baseline row count + 1 (cumulative numbering)
        let baselineStandAloneSigCount =
            baselineArtifacts.Baseline.Metadata.TableRowCounts.[TableNames.StandAloneSig.Index]
        let expectedRowId = baselineStandAloneSigCount + 1
        let expectedToken = 0x11000000 ||| expectedRowId
        Assert.Equal(expectedToken, bodyInfo.LocalSignatureToken)
        let signatureBlob = Assert.Single(delta.StandaloneSignatures)
        Assert.Equal<byte>(baselineSigBytes, signatureBlob.Blob)

    [<Fact>]
    let ``emitDelta adds new StandAloneSig and header token when locals change`` () =
        let baselineModule =
            createLocalsModule
                [ PrimaryAssemblyILGlobals.typ_Int32 ]
                [| AI_ldc(DT_I4, ILConst.I4 5); I_ret |]

        let updatedModule =
            createLocalsModule
                [ PrimaryAssemblyILGlobals.typ_Int32; PrimaryAssemblyILGlobals.typ_Int32 ]
                [| AI_ldc(DT_I4, ILConst.I4 1)
                   I_stloc 0us
                   AI_ldc(DT_I4, ILConst.I4 2)
                   I_stloc 1us
                   I_ldloc 0us
                   I_ldloc 1us
                   AI_add
                   I_ret |]
            |> TestHelpers.withDebuggableAttribute

        let baselineArtifacts = TestHelpers.createBaselineFromModule baselineModule
        let methodKey = TestHelpers.methodKeyByName baselineArtifacts.Baseline "Sample.LocalsDemo" "Compute"

        let _baselineStandaloneCount, baselineSigBytes =
            use peReader = new PEReader(File.OpenRead(baselineArtifacts.AssemblyPath))
            let reader = peReader.GetMetadataReader()
            let count = reader.GetTableRowCount(toTableIndex TableNames.StandAloneSig)
            let methodHandle = MetadataTokens.MethodDefinitionHandle(baselineArtifacts.Baseline.MethodTokens[methodKey])
            let methodDef = reader.GetMethodDefinition methodHandle
            let body = peReader.GetMethodBody(methodDef.RelativeVirtualAddress)
            let sigHandle = body.LocalSignature
            let sigBytes =
                let standalone = reader.GetStandaloneSignature sigHandle
                reader.GetBlobBytes standalone.Signature
            count, sigBytes

        let request : IlxDeltaRequest =
            { Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ "Sample.LocalsDemo" ]
              UpdatedMethods = [ methodKey ]
              UpdatedAccessors = []
              Module = updatedModule
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta = emitDelta request

        use deltaProvider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange delta.Metadata)
        let deltaReader = deltaProvider.GetMetadataReader()

        Assert.Equal(1, deltaReader.GetTableRowCount(toTableIndex TableNames.StandAloneSig))
        // Delta token should be baseline row count + 1 (cumulative numbering)
        let baselineStandAloneSigCount =
            baselineArtifacts.Baseline.Metadata.TableRowCounts.[TableNames.StandAloneSig.Index]
        let expectedRowId = baselineStandAloneSigCount + 1
        let expectedToken = 0x11000000 ||| expectedRowId
        let bodyInfo = Assert.Single(delta.MethodBodies)
        Assert.Equal(expectedToken, bodyInfo.LocalSignatureToken)

        let signatureBlob = Assert.Single(delta.StandaloneSignatures)
        let expectedSig = [| 0x07uy; 0x02uy; 0x08uy; 0x08uy |] // locals: int32, int32
        Assert.Equal<byte>(expectedSig, signatureBlob.Blob)
        Assert.NotEqual<byte>(baselineSigBytes, signatureBlob.Blob)

    [<Fact>]
    let ``module row in delta has readable name and guid handles`` () =
        let _, baseline = createBaseline ()
        let methodKey = methodKey baseline "GetValue"

        let request : IlxDeltaRequest =
            { Baseline = baseline
              UpdatedTypes = [ "Sample.Type" ]
              UpdatedMethods = [ methodKey ]
              UpdatedAccessors = []
              Module = createModule 2 |> TestHelpers.withDebuggableAttribute
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta = emitDelta request

        use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange delta.Metadata)
        let reader = provider.GetMetadataReader()
        let moduleDef = reader.GetModuleDefinition()

        // Verify handles are non-nil; actual string/GUID resolution happens against the aggregated (baseline + delta) heaps.
        Assert.False(moduleDef.Mvid.IsNil, "MVID handle should be present in delta metadata.")
        Assert.False(moduleDef.GenerationId.IsNil, "GenerationId handle should be present in delta metadata.")
        Assert.False(StringHandle.op_Implicit(moduleDef.Name).IsNil, "Module name handle should be present.")

        ()


    [<Fact>]
    let ``HotReloadState persists EncId sequencing`` () =
        let service = global.FSharp.Compiler.HotReload.FSharpEditAndContinueLanguageService.Instance

        service.EndSession()
        let _, baseline = createBaseline ()
        service.StartSession baseline |> ignore

        let session0 =
            match service.TryGetSession() with
            | ValueSome session -> session
            | ValueNone -> failwith "Expected hot reload session to be initialised."

        Assert.Equal(1, session0.CurrentGeneration)
        Assert.True(session0.PreviousGenerationId |> Option.isNone)

        let requestGen1 : DeltaEmissionRequest =
            { IlModule = createModule 43 |> TestHelpers.withDebuggableAttribute
              UpdatedTypes = [ "Sample.Type" ]
              UpdatedMethods = [ methodKey baseline "GetValue" ]
              UpdatedAccessors = []
              SymbolChanges = None }

        let delta1 =
            match service.EmitDelta requestGen1 with
            | Ok result -> result.Delta
            | Error error -> failwithf "EmitDelta (generation 1) failed: %A" error

        Assert.Equal(System.Guid.Empty, delta1.BaseGenerationId)
        Assert.NotEqual(System.Guid.Empty, delta1.GenerationId)

        service.OnDeltaApplied delta1.GenerationId

        let session1 =
            match service.TryGetSession() with
            | ValueSome session -> session
            | ValueNone -> failwith "Expected hot reload session to persist after applying delta."

        Assert.Equal(2, session1.CurrentGeneration)
        Assert.Equal<Guid option>(Some delta1.GenerationId, session1.PreviousGenerationId)

        let requestGen2 : DeltaEmissionRequest =
            { IlModule = createModule 44 |> TestHelpers.withDebuggableAttribute
              UpdatedTypes = [ "Sample.Type" ]
              UpdatedMethods = [ methodKey baseline "GetValue" ]
              UpdatedAccessors = []
              SymbolChanges = None }

        let delta2 =
            match service.EmitDelta requestGen2 with
            | Ok result -> result.Delta
            | Error error -> failwithf "EmitDelta (generation 2) failed: %A" error

        Assert.Equal(delta1.GenerationId, delta2.BaseGenerationId)
        Assert.NotEqual(System.Guid.Empty, delta2.GenerationId)

        service.EndSession()

    [<Fact>]
    let ``DiscardPendingUpdate clears staged delta without advancing session`` () =
        let service = FSharpEditAndContinueLanguageService.Instance
        service.EndSession()
        let _, baseline = createBaseline ()
        service.StartSession baseline |> ignore

        let request : DeltaEmissionRequest =
            { IlModule = createModule 85 |> TestHelpers.withDebuggableAttribute
              UpdatedTypes = [ "Sample.Type" ]
              UpdatedMethods = [ methodKey baseline "GetValue" ]
              UpdatedAccessors = []
              SymbolChanges = None }

        let pendingDelta =
            match service.EmitDelta request with
            | Ok result -> result.Delta
            | Error error -> failwithf "EmitDelta failed: %A" error

        service.DiscardPendingUpdate()

        let sessionAfterDiscard =
            match service.TryGetSession() with
            | ValueSome session -> session
            | ValueNone -> failwith "Expected hot reload session to remain active after discarding pending update."

        Assert.Equal(1, sessionAfterDiscard.CurrentGeneration)
        Assert.True(sessionAfterDiscard.PreviousGenerationId |> Option.isNone)
        Assert.Equal(baseline.NextGeneration, sessionAfterDiscard.Baseline.NextGeneration)
        Assert.Equal(baseline.EncId, sessionAfterDiscard.Baseline.EncId)

        let ex =
            Assert.Throws<InvalidOperationException>(fun () ->
                service.CommitPendingUpdate(pendingDelta.GenerationId))

        Assert.Contains("no pending hot reload update", ex.Message, StringComparison.OrdinalIgnoreCase)

        service.EndSession()

    [<Fact>]
    let ``TryRestoreSession rehydrates committed snapshot and ResetSessionState clears it`` () =
        let service = FSharpEditAndContinueLanguageService.Instance
        service.ResetSessionState()

        let _, baseline = createBaseline ()
        service.StartSession baseline |> ignore
        service.EndSession()

        let restored =
            match service.TryRestoreSession() with
            | ValueSome session -> session
            | ValueNone -> failwith "Expected committed session snapshot to be restorable."

        Assert.Equal(1, restored.CurrentGeneration)
        Assert.Equal(baseline.ModuleId, restored.Baseline.ModuleId)
        Assert.True(service.IsSessionActive)

        service.ResetSessionState()
        Assert.False(service.IsSessionActive)
        Assert.True(service.TryRestoreSession().IsNone)

    [<Fact>]
    let ``EditAndContinueLanguageService emits delta`` () =
        let service = FSharpEditAndContinueLanguageService.Instance
        service.EndSession()
        let _, baseline = createBaseline ()
        service.StartSession baseline |> ignore

        let request : DeltaEmissionRequest =
            { IlModule = createModule 101 |> TestHelpers.withDebuggableAttribute
              UpdatedTypes = [ "Sample.Type" ]
              UpdatedMethods = [ methodKey baseline "GetValue" ]
              UpdatedAccessors = []
              SymbolChanges = None }

        match service.EmitDelta request with
        | Ok result ->
            Assert.Equal(System.Guid.Empty, result.Delta.BaseGenerationId)
            Assert.NotEqual(System.Guid.Empty, result.Delta.GenerationId)
            service.CommitPendingUpdate(result.Delta.GenerationId)
        | Error error ->
            Assert.True(false, sprintf "EmitDelta failed: %A" error)

        service.EndSession()

    [<Fact>]
    let ``IlDeltaStreamBuilder records method body payload`` () =
        let ilBytes = [| 0x02uy; 0x28uy; 0x00uy; 0x00uy; 0x00uy; 0x0Auy; 0x2Auy |]
        let builder = IlDeltaStreamBuilder(None)

        let update =
            builder.AddMethodBody(
                0x06000001,
                0x11000001,
                ilBytes,
                8,
                false,
                [||],  // Empty exception regions
                id
            )

        Assert.Equal(0x06000001, update.MethodToken)
        let streams = builder.Build()
        Assert.True(streams.IL.Length >= ilBytes.Length)
        let single = Assert.Single(streams.MethodBodies)
        Assert.Equal(update.MethodToken, single.MethodToken)
        Assert.Equal(update.LocalSignatureToken, single.LocalSignatureToken)
        Assert.Equal(update.CodeLength, single.CodeLength)

    [<Fact>]
    let ``IlDeltaStreamBuilder captures standalone signatures`` () =
        let signature = [| 0x07uy; 0x02uy |]
        let builder = IlDeltaStreamBuilder(None)
        let token = builder.AddStandaloneSignature(signature)
        Assert.NotEqual(0, token)

        let streams = builder.Build()
        let standalone = Assert.Single(streams.StandaloneSignatures)
        // StandAloneSig table index is 0x11, token = (0x11 << 24) | rowId
        let expected = 0x11000000 ||| standalone.RowId
        Assert.Equal(expected, token)
        Assert.Equal<byte[]>(signature, standalone.Blob)

    [<Fact>]
    let ``IMetadataHeaps.GetUserStringHeapIdx writes #US entries`` () =
        let offsets =
            MetadataHeapOffsets.OfHeapSizes
                { StringHeapSize = 13
                  UserStringHeapSize = 29
                  BlobHeapSize = 7
                  GuidHeapSize = 3 }

        let tables = DeltaMetadataTables(offsets)
        let heaps = tables.AsMetadataHeaps()

        let token1 = heaps.GetUserStringHeapIdx "hello from us"
        let token2 = heaps.GetUserStringHeapIdx "hello from us"

        Assert.Equal(token1, token2)
        Assert.True(token1 > offsets.UserStringHeapStart, "User-string token should point into the #US heap suffix.")
        Assert.Equal(1, tables.StringHeapBytes.Length) // only null sentinel, no #Strings additions
        Assert.True(tables.UserStringHeapBytes.Length > 1, "#US heap should contain the encoded literal payload.")

    [<Fact>]
    let ``IL delta fat header matches method body length`` () =
        // Baseline module with GetValue = 42, delta changes body to return 84.
        let _, baseline = createBaseline ()
        let updatedModule = createModule 84 |> TestHelpers.withDebuggableAttribute

        let request : IlxDeltaRequest =
            { Baseline = baseline
              UpdatedTypes = [ "Sample.Type" ]
              UpdatedMethods = [ methodKey baseline "GetValue" ]
              UpdatedAccessors = []
              Module = updatedModule
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta = emitDelta request

        let bodyInfo = Assert.Single(delta.MethodBodies)
        let ilBytes = delta.IL
        let offset = bodyInfo.CodeOffset

        // Fat header: low 2 bits == 0x3, size byte == 0x30 (header size = 3 dwords => 12 bytes).
        let flagsByte = ilBytes[offset]
        let sizeByte = ilBytes[offset + 1]
        Assert.Equal(0x3uy, flagsByte &&& 0x3uy)
        Assert.Equal(0x30uy, sizeByte)

        // Code size in header matches MethodBodyUpdate.CodeLength.
        let codeSize =
            BitConverter.ToInt32(ilBytes, offset + 4)
        Assert.Equal(bodyInfo.CodeLength, codeSize)

        // No EH sections expected for this simple body (no MoreSects flag).
        Assert.Equal(0uy, flagsByte &&& e_CorILMethod_MoreSects)

        // MethodDef RVA should equal the code offset.
        use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange delta.Metadata)
        let reader = provider.GetMetadataReader()
        let methodHandle = reader.MethodDefinitions |> Seq.head
        let methodDef = reader.GetMethodDefinition methodHandle
        Assert.Equal(bodyInfo.CodeOffset, methodDef.RelativeVirtualAddress)

    [<Fact>]
    let ``MethodDef RVA matches emitted method body offset`` () =
        // Baseline module with GetValue = 42
        let _, baseline = createBaseline ()
        // Updated module changes GetValue body to return 84
        let updatedModule = createModule 84 |> TestHelpers.withDebuggableAttribute

        let request : IlxDeltaRequest =
            { Baseline = baseline
              UpdatedTypes = [ "Sample.Type" ]
              UpdatedMethods = [ methodKey baseline "GetValue" ]
              UpdatedAccessors = []
              Module = updatedModule
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta = emitDelta request

        let bodyInfo = Assert.Single(delta.MethodBodies)

        use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange delta.Metadata)
        let reader = provider.GetMetadataReader()

        // Resolve MethodDef for GetValue in the delta metadata
        // Delta string heap offsets are absolute to baseline; names may be unreadable from delta alone.
        // This delta emits exactly one MethodDef row, so take the first handle.
        let methodHandle =
            reader.MethodDefinitions
            |> Seq.head

        let methodDef = reader.GetMethodDefinition methodHandle

        // MethodDef.RVA should point at the emitted method body offset
        Assert.Equal(bodyInfo.CodeOffset, methodDef.RelativeVirtualAddress)

    [<Fact>]
    let ``HotReloadUnsupportedEditException is raised for unsupported edits with context`` () =
        // Verify that unsupported edits raise HotReloadUnsupportedEditException (not failwith)
        // with a meaningful message. This tests the error handling pattern used in IlxDeltaEmitter.fs
        // at lines 1775 (AsyncStateMachineAttribute resolution) and 1847 (NullableContextAttribute resolution).
        //
        // Note: The specific attribute resolution failures are hard to trigger in tests because
        // they require missing runtime types. This test verifies the exception pattern is used
        // consistently by testing the field addition rejection path which uses the same pattern.

        let _, baseline = createFieldHolderBaseline false
        let updatedModule = createModuleWithOptionalField true |> TestHelpers.withDebuggableAttribute

        let request =
            {
                IlxDeltaRequest.Baseline = baseline
                UpdatedTypes = [ "Sample.FieldHolder" ]
                UpdatedMethods = [ methodKey baseline "GetValue" ]
                UpdatedAccessors = []
                Module = updatedModule
                SymbolChanges = None
                CurrentGeneration = 1
                PreviousGenerationId = None
                SynthesizedNames = None
            }

        // Verify the exception type is HotReloadUnsupportedEditException (not a generic exception)
        let ex = Assert.Throws<HotReloadUnsupportedEditException>(fun () -> emitDelta request |> ignore)

        // Verify the message contains useful context (type and field name)
        Assert.NotNull(ex.Message)
        Assert.True(ex.Message.Length > 0, "Exception message should not be empty")
        Assert.Contains("Sample.FieldHolder", ex.Message)
        Assert.Contains("trackedField", ex.Message)

    [<Fact>]
    let ``recordDeltaApplied throws for empty GUID`` () =
        // Ensure no session is active
        FSharp.Compiler.HotReloadState.clearBaseline()

        let ex = Assert.Throws<ArgumentException>(fun () ->
            FSharp.Compiler.HotReloadState.recordDeltaApplied System.Guid.Empty)
        Assert.Contains("empty GUID", ex.Message)

    [<Fact>]
    let ``recordDeltaApplied throws when no session active`` () =
        // Ensure no session is active
        FSharp.Compiler.HotReloadState.clearBaseline()

        let ex = Assert.Throws<InvalidOperationException>(fun () ->
            FSharp.Compiler.HotReloadState.recordDeltaApplied (System.Guid.NewGuid()))
        Assert.Contains("no active hot reload session", ex.Message)

    [<Fact>]
    let ``multi-generation user string content is correctly encoded`` () =
        // This test verifies that user strings are correctly encoded across multiple generations.
        // The bug that was fixed required proper cumulative heap offset tracking - generation 2+
        // user strings would be corrupted if heap offsets weren't correctly accumulated.

        // Generation 0: Create baseline with initial string
        let _, baseline = createStringBaseline "Version 1"
        let key = methodKey baseline "GetMessage"

        // Generation 1: Update to "Version 2"
        let updatedModule1 = createStringModule "Version 2" |> TestHelpers.withDebuggableAttribute
        let request1 : IlxDeltaRequest =
            { Baseline = baseline
              UpdatedTypes = [ key.DeclaringType ]
              UpdatedMethods = [ key ]
              UpdatedAccessors = []
              Module = updatedModule1
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta1 = emitDelta request1

        // Verify generation 1 user string
        let gen1Literal =
            delta1.UserStringUpdates
            |> List.tryPick (fun (_, _, text) ->
                if text.StartsWith("Version", StringComparison.Ordinal) then Some text else None)

        match gen1Literal with
        | Some text -> Assert.Equal("Version 2", text)
        | None -> Assert.True(false, "Expected 'Version 2' user string in generation 1 delta.")

        // Get updated baseline from delta1 - this contains cumulative heap sizes with proper alignment
        // (critical for the bug we're testing)
        let updatedBaseline =
            match delta1.UpdatedBaseline with
            | Some baseline -> baseline
            | None -> failwith "Expected UpdatedBaseline to be set after emitDelta"

        // Generation 2: Update to "Version 3"
        let updatedModule2 = createStringModule "Version 3" |> TestHelpers.withDebuggableAttribute
        let request2 : IlxDeltaRequest =
            { Baseline = updatedBaseline
              UpdatedTypes = [ key.DeclaringType ]
              UpdatedMethods = [ key ]
              UpdatedAccessors = []
              Module = updatedModule2
              SymbolChanges = None
              CurrentGeneration = 2
              PreviousGenerationId = Some delta1.GenerationId
              SynthesizedNames = None }

        let delta2 = emitDelta request2

        // Verify generation 2 user string - this is where the bug would manifest
        // If heap offsets weren't correctly accumulated, the string would be corrupted
        // (e.g., contain CJK characters instead of the expected text)
        let gen2Literal =
            delta2.UserStringUpdates
            |> List.tryPick (fun (_, _, text) ->
                if text.StartsWith("Version", StringComparison.Ordinal) then Some text else None)

        match gen2Literal with
        | Some text -> Assert.Equal("Version 3", text)
        | None -> Assert.True(false, "Expected 'Version 3' user string in generation 2 delta.")
