// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Driver for F# compiler. 
// 
// Roughly divides into:
//    - Parsing
//    - Flags 
//    - Importing IL assemblies
//    - Compiling (including optimizing)
//    - Linking (including ILX-IL transformation)


module internal FSharp.Compiler.Driver 

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Diagnostics
open System.Globalization
open System.IO
open System.Reflection
open System.Runtime.CompilerServices
open System.Text
open System.Threading

open Internal.Utilities
open Internal.Utilities.Collections
open Internal.Utilities.Filename

open FSharp.Compiler 
open FSharp.Compiler.AbstractIL 
open FSharp.Compiler.AbstractIL.IL 
open FSharp.Compiler.AbstractIL.ILBinaryReader 
open FSharp.Compiler.AbstractIL.Internal 
open FSharp.Compiler.AbstractIL.Internal.Library 
open FSharp.Compiler.AbstractIL.Diagnostics
open FSharp.Compiler.IlxGen

open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.AttributeChecking
open FSharp.Compiler.Ast
open FSharp.Compiler.CompileOps
open FSharp.Compiler.CompileOptions
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Lib
open FSharp.Compiler.Range
open FSharp.Compiler.Tast
open FSharp.Compiler.Tastops
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypeChecker

#if !NO_EXTENSIONTYPING
open FSharp.Compiler.ExtensionTyping
#endif

#nowarn "45" // This method will be made public in the underlying IL because it may implement an interface or override a method

//----------------------------------------------------------------------------
// Reporting - warnings, errors
//----------------------------------------------------------------------------

/// An error logger that reports errors up to some maximum, notifying the exiter when that maximum is reached
[<AbstractClass>]
type ErrorLoggerUpToMaxErrors(tcConfigB: TcConfigBuilder, exiter: Exiter, nameForDebugging) = 
    inherit ErrorLogger(nameForDebugging)

    let mutable errors = 0

    /// Called when an error or warning occurs
    abstract HandleIssue: tcConfigB: TcConfigBuilder * error: PhasedDiagnostic * isError: bool -> unit

    /// Called when 'too many errors' has occurred
    abstract HandleTooManyErrors: text: string -> unit

    override x.ErrorCount = errors

    override x.DiagnosticSink(err, isError) = 
      if isError || ReportWarningAsError tcConfigB.errorSeverityOptions err then 
        if errors >= tcConfigB.maxErrors then 
            x.HandleTooManyErrors(FSComp.SR.fscTooManyErrors())
            exiter.Exit 1

        x.HandleIssue(tcConfigB, err, true)

        errors <- errors + 1

        match err.Exception, tcConfigB.simulateException with 
        | InternalError (msg, _), None 
        | Failure msg, None -> Debug.Assert(false, sprintf "Bug in compiler: %s\n%s" msg (err.Exception.ToString()))
        | :? KeyNotFoundException, None -> Debug.Assert(false, sprintf "Lookup exception in compiler: %s" (err.Exception.ToString()))
        | _ ->  ()

      elif ReportWarning tcConfigB.errorSeverityOptions err then
          x.HandleIssue(tcConfigB, err, isError)
    

/// Create an error logger that counts and prints errors 
let ConsoleErrorLoggerUpToMaxErrors (tcConfigB: TcConfigBuilder, exiter : Exiter) = 
    { new ErrorLoggerUpToMaxErrors(tcConfigB, exiter, "ConsoleErrorLoggerUpToMaxErrors") with
            
            member __.HandleTooManyErrors(text : string) = 
                DoWithErrorColor false (fun () -> Printf.eprintfn "%s" text)

            member __.HandleIssue(tcConfigB, err, isError) =
                DoWithErrorColor isError (fun () -> 
                    let diag = OutputDiagnostic (tcConfigB.implicitIncludeDir, tcConfigB.showFullPaths, tcConfigB.flatErrors, tcConfigB.errorStyle, isError)
                    writeViaBufferWithEnvironmentNewLines stderr diag err
                    stderr.WriteLine())
    } :> ErrorLogger

/// This error logger delays the messages it receives. At the end, call ForwardDelayedDiagnostics
/// to send the held messages.     
type DelayAndForwardErrorLogger(exiter: Exiter, errorLoggerProvider: ErrorLoggerProvider) =
    inherit CapturingErrorLogger("DelayAndForwardErrorLogger")

    member x.ForwardDelayedDiagnostics(tcConfigB: TcConfigBuilder) = 
        let errorLogger =  errorLoggerProvider.CreateErrorLoggerUpToMaxErrors(tcConfigB, exiter)
        x.CommitDelayedDiagnostics errorLogger

and [<AbstractClass>]
    ErrorLoggerProvider() =

    member this.CreateDelayAndForwardLogger exiter = DelayAndForwardErrorLogger(exiter, this)

    abstract CreateErrorLoggerUpToMaxErrors : tcConfigBuilder : TcConfigBuilder * exiter : Exiter -> ErrorLogger

    
/// Part of LegacyHostedCompilerForTesting
///
/// Yet another ErrorLogger implementation, capturing the messages but only up to the maxerrors maximum
type InProcErrorLoggerProvider() = 
    let errors = ResizeArray()
    let warnings = ResizeArray()

    member __.Provider = 
        { new ErrorLoggerProvider() with

            member log.CreateErrorLoggerUpToMaxErrors(tcConfigBuilder, exiter) =

                { new ErrorLoggerUpToMaxErrors(tcConfigBuilder, exiter, "InProcCompilerErrorLoggerUpToMaxErrors") with

                    member this.HandleTooManyErrors text = warnings.Add(Diagnostic.Short(false, text))

                    member this.HandleIssue(tcConfigBuilder, err, isError) =
                        // 'true' is passed for "suggestNames", since we want to suggest names with fsc.exe runs and this doesn't affect IDE perf
                        let errs =
                            CollectDiagnostic
                                (tcConfigBuilder.implicitIncludeDir, tcConfigBuilder.showFullPaths,
                                 tcConfigBuilder.flatErrors, tcConfigBuilder.errorStyle, isError, err, true)
                        let container = if isError then errors else warnings 
                        container.AddRange errs } 
                :> ErrorLogger }

    member __.CapturedErrors = errors.ToArray()

    member __.CapturedWarnings = warnings.ToArray()

/// The default ErrorLogger implementation, reporting messages to the Console up to the maxerrors maximum
type ConsoleLoggerProvider() = 

    inherit ErrorLoggerProvider()

    override this.CreateErrorLoggerUpToMaxErrors(tcConfigBuilder, exiter) = ConsoleErrorLoggerUpToMaxErrors(tcConfigBuilder, exiter)

/// Notify the exiter if any error has occurred 
let AbortOnError (errorLogger: ErrorLogger, exiter : Exiter) = 
    if errorLogger.ErrorCount > 0 then
        exiter.Exit 1

//----------------------------------------------------------------------------
// DisposablesTracker
//----------------------------------------------------------------------------

/// Track a set of resources to cleanup
type DisposablesTracker() = 

    let items = Stack<IDisposable>()

    member this.Register i = items.Push i

    interface IDisposable with

        member this.Dispose() = 
            let l = List.ofSeq items
            items.Clear()
            for i in l do 
                try i.Dispose() with _ -> ()


let TypeCheck (ctok, tcConfig, tcImports, tcGlobals, errorLogger: ErrorLogger, assemblyName, niceNameGen, tcEnv0, inputs, exiter: Exiter) =
    try 
        if isNil inputs then error(Error(FSComp.SR.fscNoImplementationFiles(), Range.rangeStartup))
        let ccuName = assemblyName
        let tcInitialState = GetInitialTcState (rangeStartup, ccuName, tcConfig, tcGlobals, tcImports, niceNameGen, tcEnv0)
        TypeCheckClosedInputSet (ctok, (fun () -> errorLogger.ErrorCount > 0), tcConfig, tcImports, tcGlobals, None, tcInitialState, inputs)
    with e -> 
        errorRecovery e rangeStartup
        exiter.Exit 1

/// Check for .fsx and, if present, compute the load closure for of #loaded files.
let AdjustForScriptCompile(ctok, tcConfigB: TcConfigBuilder, commandLineSourceFiles, lexResourceManager) =

    let combineFilePath file =
        try
            if FileSystem.IsPathRootedShim file then file
            else Path.Combine(tcConfigB.implicitIncludeDir, file)
        with _ ->
            error (Error(FSComp.SR.pathIsInvalid file, rangeStartup)) 
            
    let commandLineSourceFiles = 
        commandLineSourceFiles 
        |> List.map combineFilePath
        
    let allSources = ref []       
    
    let tcConfig = TcConfig.Create(tcConfigB, validate=false) 
    
    let AddIfNotPresent(filename: string) =
        if not(!allSources |> List.contains filename) then
            allSources := filename :: !allSources
    
    let AppendClosureInformation filename =
        if IsScript filename then 
            let closure = 
                LoadClosure.ComputeClosureOfScriptFiles
                   (ctok, tcConfig, [filename, rangeStartup], CodeContext.Compilation, lexResourceManager=lexResourceManager)

            // Record the references from the analysis of the script. The full resolutions are recorded as the corresponding #I paths used to resolve them
            // are local to the scripts and not added to the tcConfigB (they are added to localized clones of the tcConfigB).
            let references =
                closure.References
                |> List.collect snd
                |> List.filter (fun r -> not (Range.equals r.originalReference.Range range0) && not (Range.equals r.originalReference.Range rangeStartup))

            references |> List.iter (fun r -> tcConfigB.AddReferencedAssemblyByPath(r.originalReference.Range, r.resolvedPath))
            closure.NoWarns |> List.collect (fun (n, ms) -> ms|>List.map(fun m->m, n)) |> List.iter (fun (x,m) -> tcConfigB.TurnWarningOff(x, m))
            closure.SourceFiles |> List.map fst |> List.iter AddIfNotPresent
            closure.AllRootFileDiagnostics |> List.iter diagnosticSink
            
        else AddIfNotPresent filename
         
    // Find closure of .fsx files.
    commandLineSourceFiles |> List.iter AppendClosureInformation

    List.rev !allSources

let ProcessCommandLineFlags (tcConfigB: TcConfigBuilder, setProcessThreadLocals, lcidFromCodePage, argv) =
    let inputFilesRef   = ref ([] : string list)
    let collect name = 
        let lower = String.lowercase name
        if List.exists (Filename.checkSuffix lower) [".resx"]  then
            error(Error(FSComp.SR.fscResxSourceFileDeprecated name, rangeStartup))
        else
            inputFilesRef := name :: !inputFilesRef
    let abbrevArgs = GetAbbrevFlagSet tcConfigB true

    // This is where flags are interpreted by the command line fsc.exe.
    ParseCompilerOptions (collect, GetCoreFscCompilerOptions tcConfigB, List.tail (PostProcessCompilerArgs abbrevArgs argv))

    if not (tcConfigB.portablePDB || tcConfigB.embeddedPDB) then
        if tcConfigB.embedAllSource || (tcConfigB.embedSourceList |> isNil |> not) then
            error(Error(FSComp.SR.optsEmbeddedSourceRequirePortablePDBs(), rangeCmdArgs))
        if not (String.IsNullOrEmpty(tcConfigB.sourceLink)) then
            error(Error(FSComp.SR.optsSourceLinkRequirePortablePDBs(), rangeCmdArgs))

    if tcConfigB.debuginfo && not tcConfigB.portablePDB then
        if tcConfigB.deterministic then
            error(Error(FSComp.SR.fscDeterministicDebugRequiresPortablePdb(), rangeCmdArgs))

        if tcConfigB.pathMap <> PathMap.empty then
            error(Error(FSComp.SR.fscPathMapDebugRequiresPortablePdb(), rangeCmdArgs))

    let inputFiles = List.rev !inputFilesRef

    // Check if we have a codepage from the console
    match tcConfigB.lcid with
    | Some _ -> ()
    | None -> tcConfigB.lcid <- lcidFromCodePage

    setProcessThreadLocals tcConfigB

    (* step - get dll references *)
    let dllFiles, sourceFiles = inputFiles |> List.map(fun p -> trimQuotes p) |> List.partition Filename.isDll
    match dllFiles with
    | [] -> ()
    | h :: _ -> errorR (Error(FSComp.SR.fscReferenceOnCommandLine h, rangeStartup))

    dllFiles |> List.iter (fun f->tcConfigB.AddReferencedAssemblyByPath(rangeStartup, f))
    sourceFiles

module InterfaceFileWriter =

    let BuildInitialDisplayEnvForSigFileGeneration tcGlobals = 
        let denv = DisplayEnv.Empty tcGlobals
        let denv = 
            { denv with 
               showImperativeTyparAnnotations=true
               showHiddenMembers=true
               showObsoleteMembers=true
               showAttributes=true }
        denv.SetOpenPaths 
            [ FSharpLib.RootPath 
              FSharpLib.CorePath 
              FSharpLib.CollectionsPath 
              FSharpLib.ControlPath 
              (IL.splitNamespace FSharpLib.ExtraTopLevelOperatorsName) ] 

    let WriteInterfaceFile (tcGlobals, tcConfig: TcConfig, infoReader, declaredImpls) =

        /// Use a UTF-8 Encoding with no Byte Order Mark
        let os = 
            if tcConfig.printSignatureFile="" then Console.Out
            else (File.CreateText tcConfig.printSignatureFile :> TextWriter)

        if tcConfig.printSignatureFile <> "" && not (List.exists (Filename.checkSuffix tcConfig.printSignatureFile) FSharpLightSyntaxFileSuffixes) then
            fprintfn os "#light" 
            fprintfn os "" 

        for (TImplFile (_, _, mexpr, _, _, _)) in declaredImpls do
            let denv = BuildInitialDisplayEnvForSigFileGeneration tcGlobals
            writeViaBufferWithEnvironmentNewLines os (fun os s -> Printf.bprintf os "%s\n\n" s)
              (NicePrint.layoutInferredSigOfModuleExpr true denv infoReader AccessibleFromSomewhere range0 mexpr |> Layout.squashTo 80 |> Layout.showL)
       
        if tcConfig.printSignatureFile <> "" then os.Dispose()

module XmlDocWriter =

    let getDoc xmlDoc = 
        match XmlDoc.Process xmlDoc with
        | XmlDoc [| |] -> ""
        | XmlDoc strs  -> strs |> Array.toList |> String.concat Environment.NewLine

    let hasDoc xmlDoc =
        // No need to process the xml doc - just need to know if there's anything there
        match xmlDoc with
        | XmlDoc [| |] -> false
        | _ -> true
        
    let computeXmlDocSigs (tcGlobals, generatedCcu: CcuThunk) =
        (* the xmlDocSigOf* functions encode type into string to be used in "id" *)
        let g = tcGlobals
        let doValSig ptext (v: Val)  = if (hasDoc v.XmlDoc) then v.XmlDocSig <- XmlDocSigOfVal g ptext v
        let doTyconSig ptext (tc: Tycon) = 
            if (hasDoc tc.XmlDoc) then tc.XmlDocSig <- XmlDocSigOfTycon [ptext; tc.CompiledName]
            for vref in tc.MembersOfFSharpTyconSorted do 
                doValSig ptext vref.Deref
            for uc in tc.UnionCasesArray do
                if (hasDoc uc.XmlDoc) then uc.XmlDocSig <- XmlDocSigOfUnionCase [ptext; tc.CompiledName; uc.Id.idText]
            for rf in tc.AllFieldsArray do
                if (hasDoc rf.XmlDoc) then
                    rf.XmlDocSig <-
                        if tc.IsRecordTycon && (not rf.IsStatic) then 
                            // represents a record field, which is exposed as a property
                            XmlDocSigOfProperty [ptext; tc.CompiledName; rf.Id.idText]
                        else
                            XmlDocSigOfField [ptext; tc.CompiledName; rf.Id.idText]

        let doModuleMemberSig path (m: ModuleOrNamespace) = m.XmlDocSig <- XmlDocSigOfSubModul [path]
        (* moduleSpec - recurses *)
        let rec doModuleSig path (mspec: ModuleOrNamespace) = 
            let mtype = mspec.ModuleOrNamespaceType
            let path = 
                (* skip the first item in the path which is the assembly name *)
                match path with 
                | None -> Some ""
                | Some "" -> Some mspec.LogicalName
                | Some p -> Some (p+"."+mspec.LogicalName)
            let ptext = match path with None -> "" | Some t -> t
            if mspec.IsModule then doModuleMemberSig ptext mspec
            let vals = 
                mtype.AllValsAndMembers
                |> Seq.toList
                |> List.filter (fun x  -> not x.IsCompilerGenerated) 
                |> List.filter (fun x -> x.MemberInfo.IsNone || x.IsExtensionMember)
            List.iter (doModuleSig  path)  mtype.ModuleAndNamespaceDefinitions
            List.iter (doTyconSig  ptext) mtype.ExceptionDefinitions
            List.iter (doValSig    ptext) vals
            List.iter (doTyconSig  ptext) mtype.TypeDefinitions
       
        doModuleSig None generatedCcu.Contents          

    let writeXmlDoc (assemblyName, generatedCcu: CcuThunk, xmlfile) =
        if not (Filename.hasSuffixCaseInsensitive "xml" xmlfile ) then 
            error(Error(FSComp.SR.docfileNoXmlSuffix(), Range.rangeStartup))
        (* the xmlDocSigOf* functions encode type into string to be used in "id" *)
        let members = ref []
        let addMember id xmlDoc = 
            if hasDoc xmlDoc then
                let doc = getDoc xmlDoc
                members := (id, doc) :: !members
        let doVal (v: Val) = addMember v.XmlDocSig v.XmlDoc
        let doUnionCase (uc: UnionCase) = addMember uc.XmlDocSig uc.XmlDoc
        let doField (rf: RecdField) = addMember rf.XmlDocSig rf.XmlDoc
        let doTycon (tc: Tycon) = 
            addMember tc.XmlDocSig tc.XmlDoc
            for vref in tc.MembersOfFSharpTyconSorted do 
                doVal vref.Deref 
            for uc in tc.UnionCasesArray do
                doUnionCase uc
            for rf in tc.AllFieldsArray do
                doField rf

        let modulMember (m: ModuleOrNamespace) = addMember m.XmlDocSig m.XmlDoc
        
        (* moduleSpec - recurses *)
        let rec doModule (mspec: ModuleOrNamespace) = 
            let mtype = mspec.ModuleOrNamespaceType
            if mspec.IsModule then modulMember mspec
            let vals = 
                mtype.AllValsAndMembers
                |> Seq.toList
                |> List.filter (fun x  -> not x.IsCompilerGenerated) 
                |> List.filter (fun x -> x.MemberInfo.IsNone || x.IsExtensionMember)
            List.iter doModule mtype.ModuleAndNamespaceDefinitions
            List.iter doTycon mtype.ExceptionDefinitions
            List.iter doVal vals
            List.iter doTycon mtype.TypeDefinitions
       
        doModule generatedCcu.Contents

        use os = File.CreateText xmlfile

        fprintfn os ("<?xml version=\"1.0\" encoding=\"utf-8\"?>")
        fprintfn os ("<doc>")
        fprintfn os ("<assembly><name>%s</name></assembly>") assemblyName
        fprintfn os ("<members>")
        !members |> List.iter (fun (id, doc) -> 
            fprintfn os  "<member name=\"%s\">" id
            fprintfn os  "%s" doc
            fprintfn os  "</member>")
        fprintfn os "</members>" 
        fprintfn os "</doc>"   


let DefaultFSharpBinariesDir = FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(FSharpEnvironment.tryCurrentDomain()).Value

let GenerateInterfaceData(tcConfig: TcConfig) = 
    not tcConfig.standalone && not tcConfig.noSignatureData 

let EncodeInterfaceData(tcConfig: TcConfig, tcGlobals, exportRemapping, generatedCcu, outfile, isIncrementalBuild) = 
    if GenerateInterfaceData tcConfig then 
        let resource = WriteSignatureData (tcConfig, tcGlobals, exportRemapping, generatedCcu, outfile, isIncrementalBuild)
        // The resource gets written to a file for FSharp.Core
        let useDataFiles = (tcConfig.useOptimizationDataFile || tcGlobals.compilingFslib) && not isIncrementalBuild
        if useDataFiles then 
            let sigDataFileName = (Filename.chopExtension outfile)+".sigdata"
            File.WriteAllBytes(sigDataFileName, resource.GetBytes())
        let resources = 
            [ resource ]
        let sigAttr = mkSignatureDataVersionAttr tcGlobals (IL.parseILVersion Internal.Utilities.FSharpEnvironment.FSharpBinaryMetadataFormatRevision) 
        [sigAttr], resources
      else 
        [], []

let GenerateOptimizationData tcConfig = 
    GenerateInterfaceData tcConfig 

let EncodeOptimizationData(tcGlobals, tcConfig: TcConfig, outfile, exportRemapping, data, isIncrementalBuild) = 
    if GenerateOptimizationData tcConfig then 
        let data = map2Of2 (Optimizer.RemapOptimizationInfo tcGlobals exportRemapping) data
        // As with the sigdata file, the optdata gets written to a file for FSharp.Core
        let useDataFiles = (tcConfig.useOptimizationDataFile || tcGlobals.compilingFslib) && not isIncrementalBuild
        if useDataFiles then 
            let ccu, modulInfo = data
            let bytes = TastPickle.pickleObjWithDanglingCcus isIncrementalBuild outfile tcGlobals ccu Optimizer.p_CcuOptimizationInfo modulInfo
            let optDataFileName = (Filename.chopExtension outfile)+".optdata"
            File.WriteAllBytes(optDataFileName, bytes)
        let (ccu, optData) = 
            if tcConfig.onlyEssentialOptimizationData then 
                map2Of2 Optimizer.AbstractOptimizationInfoToEssentials data 
            else 
                data
        [ WriteOptimizationData (tcGlobals, outfile, isIncrementalBuild, ccu, optData) ]
    else
        [ ]

//----------------------------------------------------------------------------
// .res file format, for encoding the assembly version attribute. 
//--------------------------------------------------------------------------

// Helpers for generating binary blobs
module BinaryGenerationUtilities = 
    // Little-endian encoding of int32 
    let b0 n =  byte (n &&& 0xFF)
    let b1 n =  byte ((n >>> 8) &&& 0xFF)
    let b2 n =  byte ((n >>> 16) &&& 0xFF)
    let b3 n =  byte ((n >>> 24) &&& 0xFF)

    let i16 (i: int32) = [| b0 i; b1 i |]
    let i32 (i: int32) = [| b0 i; b1 i; b2 i; b3 i |]

    // Emit the bytes and pad to a 32-bit alignment
    let Padded initialAlignment (v: byte[]) = 
        [| yield! v
           for _ in 1..(4 - (initialAlignment + v.Length) % 4) % 4 do
               yield 0x0uy |]

// Generate nodes in a .res file format. These are then linked by Abstract IL using the 
// linkNativeResources function, which invokes the cvtres.exe utility
module ResFileFormat = 
    open BinaryGenerationUtilities
    
    let ResFileNode(dwTypeID, dwNameID, wMemFlags, wLangID, data: byte[]) =
        [| yield! i32 data.Length  // DWORD ResHdr.dwDataSize
           yield! i32 0x00000020  // dwHeaderSize
           yield! i32 ((dwTypeID <<< 16) ||| 0x0000FFFF)  // dwTypeID, sizeof(DWORD)
           yield! i32 ((dwNameID <<< 16) ||| 0x0000FFFF)   // dwNameID, sizeof(DWORD)
           yield! i32 0x00000000 // DWORD       dwDataVersion
           yield! i16 wMemFlags // WORD        wMemFlags
           yield! i16 wLangID   // WORD        wLangID
           yield! i32 0x00000000 // DWORD       dwVersion
           yield! i32 0x00000000 // DWORD       dwCharacteristics
           yield! Padded 0 data |]

    let ResFileHeader() = ResFileNode(0x0, 0x0, 0x0, 0x0, [| |]) 

// Generate the VS_VERSION_INFO structure held in a Win32 Version Resource in a PE file
//
// Web reference: http://www.piclist.com/tecHREF/os/win/api/win32/struc/src/str24_5.htm
module VersionResourceFormat = 
    open BinaryGenerationUtilities

    let VersionInfoNode(data: byte[]) =
        [| yield! i16 (data.Length + 2) // wLength : int16 // Specifies the length, in bytes, of the VS_VERSION_INFO structure. 
           yield! data |]

    let VersionInfoElement(wType, szKey, valueOpt: byte[] option, children: byte[][], isString) =
        // for String structs, wValueLength represents the word count, not the byte count
        let wValueLength = (match valueOpt with None -> 0 | Some value -> (if isString then value.Length / 2 else value.Length))
        VersionInfoNode
            [| yield! i16 wValueLength // wValueLength: int16. Specifies the length, in words, of the Value member. 
               yield! i16 wType        // wType : int16 Specifies the type of data in the version resource. 
               yield! Padded 2 szKey 
               match valueOpt with 
               | None -> yield! []
               | Some value -> yield! Padded 0 value 
               for child in children do 
                   yield! child  |]

    let Version(version: ILVersionInfo) = 
        [| // DWORD dwFileVersionMS
           // Specifies the most significant 32 bits of the file's binary 
           // version number. This member is used with dwFileVersionLS to form a 64-bit value used 
           // for numeric comparisons. 
           yield! i32 (int32 version.Major <<< 16 ||| int32 version.Minor) 
           
           // DWORD dwFileVersionLS 
           // Specifies the least significant 32 bits of the file's binary 
           // version number. This member is used with dwFileVersionMS to form a 64-bit value used 
           // for numeric comparisons. 
           yield! i32 (int32 version.Build <<< 16 ||| int32 version.Revision) 
        |]

    let String(string, value) = 
        let wType = 0x1 // Specifies the type of data in the version resource. 
        let szKey = Bytes.stringAsUnicodeNullTerminated string
        VersionInfoElement(wType, szKey, Some (Bytes.stringAsUnicodeNullTerminated value), [| |], true)

    let StringTable(language, strings) = 
        let wType = 0x1 // Specifies the type of data in the version resource. 
        let szKey = Bytes.stringAsUnicodeNullTerminated language
             // Specifies an 8-digit hexadecimal number stored as a Unicode string. 
                       
        let children =  
            [| for string in strings do
                   yield String string |] 
        VersionInfoElement(wType, szKey, None, children, false)

    let StringFileInfo(stringTables: #seq<string * #seq<string * string> >) = 
        let wType = 0x1 // Specifies the type of data in the version resource. 
        let szKey = Bytes.stringAsUnicodeNullTerminated "StringFileInfo" // Contains the Unicode string StringFileInfo
        // Contains an array of one or more StringTable structures. 
        let children =  
            [| for stringTable in stringTables do
                   yield StringTable stringTable |] 
        VersionInfoElement(wType, szKey, None, children, false)

    let VarFileInfo(vars: #seq<int32 * int32>) = 
        let wType = 0x1 // Specifies the type of data in the version resource. 
        let szKey = Bytes.stringAsUnicodeNullTerminated "VarFileInfo" // Contains the Unicode string StringFileInfo
        // Contains an array of one or more StringTable structures. 
        let children =  
            [| for (lang, codePage) in vars do
                   let szKey = Bytes.stringAsUnicodeNullTerminated "Translation"
                   yield VersionInfoElement(0x0, szKey, Some([| yield! i16 lang
                                                                yield! i16 codePage |]), [| |], false) |] 
        VersionInfoElement(wType, szKey, None, children, false)

    let VS_FIXEDFILEINFO(fileVersion: ILVersionInfo, 
                         productVersion: ILVersionInfo, 
                         dwFileFlagsMask, 
                         dwFileFlags, dwFileOS, 
                         dwFileType, dwFileSubtype, 
                         lwFileDate: int64) = 
        let dwStrucVersion = 0x00010000
        [| // DWORD dwSignature // Contains the value 0xFEEFO4BD. 
           yield! i32  0xFEEF04BD 
           
           // DWORD dwStrucVersion // Specifies the binary version number of this structure. 
           yield! i32 dwStrucVersion 
           
           // DWORD dwFileVersionMS, dwFileVersionLS // Specifies the most/least significant 32 bits of the file's binary version number. 
           yield! Version fileVersion 
           
           // DWORD dwProductVersionMS, dwProductVersionLS // Specifies the most/least significant 32 bits of the file's binary version number. 
           yield! Version productVersion 
           
           // DWORD dwFileFlagsMask // Contains a bitmask that specifies the valid bits in dwFileFlags. 
           yield! i32 dwFileFlagsMask 
           
           // DWORD dwFileFlags // Contains a bitmask that specifies the Boolean attributes of the file. 
           yield! i32 dwFileFlags 
                  // VS_FF_DEBUG 0x1L     The file contains debugging information or is compiled with debugging features enabled. 
                  // VS_FF_INFOINFERRED   The file's version structure was created dynamically; therefore, some of the members 
                  //                      in this structure may be empty or incorrect. This flag should never be set in a file's 
                  //                      VS_VERSION_INFO data. 
                  // VS_FF_PATCHED        The file has been modified and is not identical to the original shipping file of 
                  //                      the same version number. 
                  // VS_FF_PRERELEASE     The file is a development version, not a commercially released product. 
                  // VS_FF_PRIVATEBUILD   The file was not built using standard release procedures. If this flag is 
                  //                      set, the StringFileInfo structure should contain a PrivateBuild entry. 
                  // VS_FF_SPECIALBUILD   The file was built by the original company using standard release procedures 
                  //                      but is a variation of the normal file of the same version number. If this 
                  //                      flag is set, the StringFileInfo structure should contain a SpecialBuild entry. 
           
           //Specifies the operating system for which this file was designed. This member can be one of the following values: Flag 
           yield! i32 dwFileOS 
                  //VOS_DOS 0x0001L  The file was designed for MS-DOS. 
                  //VOS_NT  0x0004L  The file was designed for Windows NT. 
                  //VOS__WINDOWS16  The file was designed for 16-bit Windows. 
                  //VOS__WINDOWS32  The file was designed for the Win32 API. 
                  //VOS_OS216 0x00020000L  The file was designed for 16-bit OS/2. 
                  //VOS_OS232  0x00030000L  The file was designed for 32-bit OS/2. 
                  //VOS__PM16  The file was designed for 16-bit Presentation Manager. 
                  //VOS__PM32  The file was designed for 32-bit Presentation Manager. 
                  //VOS_UNKNOWN  The operating system for which the file was designed is unknown to Windows. 
           
           // Specifies the general type of file. This member can be one of the following values: 
           yield! i32 dwFileType 
           
                //VFT_UNKNOWN The file type is unknown to Windows. 
                //VFT_APP  The file contains an application. 
                //VFT_DLL  The file contains a dynamic-link library (DLL). 
                //VFT_DRV  The file contains a device driver. If dwFileType is VFT_DRV, dwFileSubtype contains a more specific description of the driver. 
                //VFT_FONT  The file contains a font. If dwFileType is VFT_FONT, dwFileSubtype contains a more specific description of the font file. 
                //VFT_VXD  The file contains a virtual device. 
                //VFT_STATIC_LIB  The file contains a static-link library. 

           // Specifies the function of the file. The possible values depend on the value of 
           // dwFileType. For all values of dwFileType not described in the following list, 
           // dwFileSubtype is zero. If dwFileType is VFT_DRV, dwFileSubtype can be one of the following values: 
           yield! i32 dwFileSubtype 
                //VFT2_UNKNOWN  The driver type is unknown by Windows. 
                //VFT2_DRV_COMM  The file contains a communications driver. 
                //VFT2_DRV_PRINTER  The file contains a printer driver. 
                //VFT2_DRV_KEYBOARD  The file contains a keyboard driver. 
                //VFT2_DRV_LANGUAGE  The file contains a language driver. 
                //VFT2_DRV_DISPLAY  The file contains a display driver. 
                //VFT2_DRV_MOUSE  The file contains a mouse driver. 
                //VFT2_DRV_NETWORK  The file contains a network driver. 
                //VFT2_DRV_SYSTEM  The file contains a system driver. 
                //VFT2_DRV_INSTALLABLE  The file contains an installable driver. 
                //VFT2_DRV_SOUND  The file contains a sound driver. 
                //
                //If dwFileType is VFT_FONT, dwFileSubtype can be one of the following values: 
                // 
                //VFT2_UNKNOWN  The font type is unknown by Windows. 
                //VFT2_FONT_RASTER  The file contains a raster font. 
                //VFT2_FONT_VECTOR  The file contains a vector font. 
                //VFT2_FONT_TRUETYPE  The file contains a TrueType font. 
                //
                //If dwFileType is VFT_VXD, dwFileSubtype contains the virtual device identifier included in the virtual device control block. 
           
           // Specifies the most significant 32 bits of the file's 64-bit binary creation date and time stamp. 
           yield! i32 (int32 (lwFileDate >>> 32)) 
           
           //Specifies the least significant 32 bits of the file's 64-bit binary creation date and time stamp. 
           yield! i32 (int32 lwFileDate) 
         |] 


    let VS_VERSION_INFO(fixedFileInfo, stringFileInfo, varFileInfo)  =
        let wType = 0x0 
        let szKey = Bytes.stringAsUnicodeNullTerminated "VS_VERSION_INFO" // Contains the Unicode string VS_VERSION_INFO
        let value = VS_FIXEDFILEINFO (fixedFileInfo)
        let children =  
            [| yield StringFileInfo stringFileInfo 
               yield VarFileInfo varFileInfo 
            |] 
        VersionInfoElement(wType, szKey, Some value, children, false)
       
    let VS_VERSION_INFO_RESOURCE data = 
        let dwTypeID = 0x0010
        let dwNameID = 0x0001
        let wMemFlags = 0x0030 // REVIEW: HARDWIRED TO ENGLISH
        let wLangID = 0x0
        ResFileFormat.ResFileNode(dwTypeID, dwNameID, wMemFlags, wLangID, VS_VERSION_INFO data)
        
module ManifestResourceFormat =
    
    let VS_MANIFEST_RESOURCE(data, isLibrary) =
        let dwTypeID = 0x0018
        let dwNameID = if isLibrary then 0x2 else 0x1
        let wMemFlags = 0x0
        let wLangID = 0x0
        ResFileFormat.ResFileNode(dwTypeID, dwNameID, wMemFlags, wLangID, data)

//----------------------------------------------------------------------------
// Helpers for finding attributes
//----------------------------------------------------------------------------

module AttributeHelpers = 

    /// Try to find an attribute that takes a string argument
    let TryFindStringAttribute (g: TcGlobals) attrib attribs =
      match g.TryFindSysAttrib attrib with 
      | None -> None
      | Some attribRef -> 
        match TryFindFSharpAttribute g attribRef attribs with
        | Some (Attrib(_, _, [ AttribStringArg s ], _, _, _, _))  -> Some (s)
        | _ -> None
        
    let TryFindIntAttribute (g: TcGlobals) attrib attribs =
      match g.TryFindSysAttrib attrib with 
      | None -> None
      | Some attribRef -> 
        match TryFindFSharpAttribute g attribRef attribs with
        | Some (Attrib(_, _, [ AttribInt32Arg i ], _, _, _, _)) -> Some (i)
        | _ -> None
        
    let TryFindBoolAttribute (g: TcGlobals) attrib attribs =
      match g.TryFindSysAttrib attrib with 
      | None -> None
      | Some attribRef -> 
        match TryFindFSharpAttribute g attribRef attribs with
        | Some (Attrib(_, _, [ AttribBoolArg p ], _, _, _, _)) -> Some (p)
        | _ -> None

    let (|ILVersion|_|) (versionString: string) =
        try Some (IL.parseILVersion versionString)
        with e -> 
            None

    // Try to find an AssemblyVersion attribute 
    let TryFindVersionAttribute g attrib attribName attribs deterministic =
        match TryFindStringAttribute g attrib attribs with
        | Some versionString ->
             if deterministic && versionString.Contains("*") then
                 errorR(Error(FSComp.SR.fscAssemblyWildcardAndDeterminism(attribName, versionString), Range.rangeStartup))
             try Some (IL.parseILVersion versionString)
             with e ->
                 // Warning will be reported by TypeChecker.fs
                 None
        | _ -> None

//----------------------------------------------------------------------------
// Building the contents of the finalized IL module
//----------------------------------------------------------------------------

module MainModuleBuilder = 

    let injectedCompatTypes = 
      set [ "System.Tuple`1"
            "System.Tuple`2" 
            "System.Tuple`3" 
            "System.Tuple`4"
            "System.Tuple`5"
            "System.Tuple`6"
            "System.Tuple`7"
            "System.Tuple`8"
            "System.ITuple"
            "System.Tuple"
            "System.Collections.IStructuralComparable"
            "System.Collections.IStructuralEquatable" ]

    let typesForwardedToMscorlib = 
      set [ "System.AggregateException"
            "System.Threading.CancellationTokenRegistration"
            "System.Threading.CancellationToken"
            "System.Threading.CancellationTokenSource"
            "System.Lazy`1"
            "System.IObservable`1"
            "System.IObserver`1" ]

    let typesForwardedToSystemNumerics =
      set [ "System.Numerics.BigInteger" ]

    let createMscorlibExportList (tcGlobals: TcGlobals) =
      // We want to write forwarders out for all injected types except for System.ITuple, which is internal
      // Forwarding System.ITuple will cause FxCop failures on 4.0
      Set.union (Set.filter (fun t -> t <> "System.ITuple") injectedCompatTypes) typesForwardedToMscorlib |>
          Seq.map (fun t -> mkTypeForwarder (tcGlobals.ilg.primaryAssemblyScopeRef) t (mkILNestedExportedTypes List.empty<ILNestedExportedType>) (mkILCustomAttrs List.empty<ILAttribute>) ILTypeDefAccess.Public ) 
          |> Seq.toList

    let createSystemNumericsExportList (tcConfig: TcConfig) (tcImports: TcImports) =
        let refNumericsDllName =
            if (tcConfig.primaryAssembly.Name = "mscorlib") then "System.Numerics"
            else "System.Runtime.Numerics"
        let numericsAssemblyRef =
            match tcImports.GetImportedAssemblies() |> List.tryFind<ImportedAssembly>(fun a -> a.FSharpViewOfMetadata.AssemblyName = refNumericsDllName) with
            | Some asm ->
                match asm.ILScopeRef with 
                | ILScopeRef.Assembly aref -> Some aref
                | _ -> None
            | None -> None
        match numericsAssemblyRef with
        | Some aref ->
            let systemNumericsAssemblyRef = ILAssemblyRef.Create(refNumericsDllName, aref.Hash, aref.PublicKey, aref.Retargetable, aref.Version, aref.Locale)
            typesForwardedToSystemNumerics |>
                Seq.map (fun t ->
                            { ScopeRef = ILScopeRef.Assembly systemNumericsAssemblyRef
                              Name = t
                              Attributes = enum<TypeAttributes>(0x00200000) ||| TypeAttributes.Public
                              Nested = mkILNestedExportedTypes []
                              CustomAttrsStored = storeILCustomAttrs emptyILCustomAttrs
                              MetadataIndex = NoMetadataIdx }) |>
                Seq.toList
        | None -> []

    let fileVersion findStringAttr (assemblyVersion: ILVersionInfo) =
        let attrName = "System.Reflection.AssemblyFileVersionAttribute"
        match findStringAttr attrName with
        | None -> assemblyVersion
        | Some (AttributeHelpers.ILVersion v) -> v
        | Some _ ->
            // Warning will be reported by TypeChecker.fs
            assemblyVersion

    let productVersion findStringAttr (fileVersion: ILVersionInfo) =
        let attrName = "System.Reflection.AssemblyInformationalVersionAttribute"
        let toDotted (version: ILVersionInfo) = sprintf "%d.%d.%d.%d" version.Major version.Minor version.Build version.Revision
        match findStringAttr attrName with
        | None | Some "" -> fileVersion |> toDotted
        | Some (AttributeHelpers.ILVersion v) -> v |> toDotted
        | Some v -> 
            // Warning will be reported by TypeChecker.fs
            v

    let productVersionToILVersionInfo (version: string) : ILVersionInfo =
        let parseOrZero v = match System.UInt16.TryParse v with (true, i) -> i | (false, _) -> 0us
        let validParts =
            version.Split('.')
            |> Seq.map parseOrZero
            |> Seq.takeWhile ((<>) 0us) 
            |> Seq.toList
        match validParts @ [0us; 0us; 0us; 0us] with
        | major :: minor :: build :: rev :: _ -> ILVersionInfo(major, minor, build, rev)
        | x -> failwithf "error converting product version '%s' to binary, tried '%A' " version x


    let CreateMainModule  
            (ctok, tcConfig: TcConfig, tcGlobals, tcImports: TcImports, 
             pdbfile, assemblyName, outfile, topAttrs, 
             (iattrs, intfDataResources), optDataResources, 
             codegenResults, assemVerFromAttrib, metadataVersion, secDecls) =

        RequireCompilationThread ctok
        let ilTypeDefs = 
            //let topTypeDef = mkILTypeDefForGlobalFunctions tcGlobals.ilg (mkILMethods [], emptyILFields)
            mkILTypeDefs codegenResults.ilTypeDefs

        let mainModule = 
            let hashAlg = AttributeHelpers.TryFindIntAttribute tcGlobals "System.Reflection.AssemblyAlgorithmIdAttribute" topAttrs.assemblyAttrs
            let locale = AttributeHelpers.TryFindStringAttribute tcGlobals "System.Reflection.AssemblyCultureAttribute" topAttrs.assemblyAttrs
            let flags =  match AttributeHelpers.TryFindIntAttribute tcGlobals "System.Reflection.AssemblyFlagsAttribute" topAttrs.assemblyAttrs with | Some f -> f | _ -> 0x0

            // You're only allowed to set a locale if the assembly is a library
            if (locale <> None && locale.Value <> "") && tcConfig.target <> CompilerTarget.Dll then
              error(Error(FSComp.SR.fscAssemblyCultureAttributeError(), rangeCmdArgs))

            // Add the type forwarders to any .NET DLL post-.NET-2.0, to give binary compatibility
            let exportedTypesList = 
                if (tcConfig.compilingFslib && tcConfig.compilingFslib40) then 
                   (List.append (createMscorlibExportList tcGlobals)
                                (if tcConfig.compilingFslibNoBigInt then [] else (createSystemNumericsExportList tcConfig tcImports))
                   )
                else
                    []

            let ilModuleName = GetGeneratedILModuleName tcConfig.target assemblyName
            let isDLL = (tcConfig.target = CompilerTarget.Dll || tcConfig.target = CompilerTarget.Module)
            mkILSimpleModule assemblyName ilModuleName isDLL tcConfig.subsystemVersion tcConfig.useHighEntropyVA ilTypeDefs hashAlg locale flags (mkILExportedTypes exportedTypesList) metadataVersion

        let disableJitOptimizations = not (tcConfig.optSettings.jitOpt())

        let tcVersion = tcConfig.version.GetVersionInfo(tcConfig.implicitIncludeDir)

        let reflectedDefinitionAttrs, reflectedDefinitionResources = 
            codegenResults.quotationResourceInfo 
            |> List.map (fun (referencedTypeDefs, reflectedDefinitionBytes) -> 
                let reflectedDefinitionResourceName = QuotationPickler.SerializedReflectedDefinitionsResourceNameBase+"-"+assemblyName+"-"+string(newUnique())+"-"+string(hash reflectedDefinitionBytes)
                let reflectedDefinitionAttrs = 
                    match QuotationTranslator.QuotationGenerationScope.ComputeQuotationFormat tcGlobals with
                    | QuotationTranslator.QuotationSerializationFormat.FSharp_40_Plus ->
                        [ mkCompilationMappingAttrForQuotationResource tcGlobals (reflectedDefinitionResourceName, referencedTypeDefs) ]
                    | QuotationTranslator.QuotationSerializationFormat.FSharp_20_Plus ->
                        [  ]
                let reflectedDefinitionResource = 
                  { Name=reflectedDefinitionResourceName
                    Location = ILResourceLocation.LocalOut reflectedDefinitionBytes
                    Access= ILResourceAccess.Public
                    CustomAttrsStored = storeILCustomAttrs emptyILCustomAttrs
                    MetadataIndex = NoMetadataIdx }
                reflectedDefinitionAttrs, reflectedDefinitionResource) 
            |> List.unzip
            |> (fun (attrs, resource) -> List.concat attrs, resource)

        let manifestAttrs = 
            mkILCustomAttrs
                 [ if not tcConfig.internConstantStrings then 
                       yield mkILCustomAttribute tcGlobals.ilg
                                 (tcGlobals.FindSysILTypeRef "System.Runtime.CompilerServices.CompilationRelaxationsAttribute", 
                                  [tcGlobals.ilg.typ_Int32], [ILAttribElem.Int32( 8)], []) 
                   yield! iattrs
                   yield! codegenResults.ilAssemAttrs
                   if Option.isSome pdbfile then
                       yield (tcGlobals.mkDebuggableAttributeV2 (tcConfig.jitTracking, tcConfig.ignoreSymbolStoreSequencePoints, disableJitOptimizations, false (* enableEnC *) )) 
                   yield! reflectedDefinitionAttrs ]

        // Make the manifest of the assembly
        let manifest = 
             if tcConfig.target = CompilerTarget.Module then None else
             let man = mainModule.ManifestOfAssembly
             let ver = 
                 match assemVerFromAttrib with 
                 | None -> tcVersion
                 | Some v -> v
             Some { man with Version= Some ver
                             CustomAttrsStored = storeILCustomAttrs manifestAttrs
                             DisableJitOptimizations=disableJitOptimizations
                             JitTracking= tcConfig.jitTracking
                             IgnoreSymbolStoreSequencePoints = tcConfig.ignoreSymbolStoreSequencePoints
                             SecurityDeclsStored=storeILSecurityDecls secDecls } 

        let resources = 
          mkILResources 
            [ for file in tcConfig.embedResources do
                 let name, bytes, pub = 
                         let file, name, pub = TcConfigBuilder.SplitCommandLineResourceInfo file
                         let file = tcConfig.ResolveSourceFile(rangeStartup, file, tcConfig.implicitIncludeDir)
                         let bytes = FileSystem.ReadAllBytesShim file
                         name, bytes, pub
                 yield { Name=name 
                         Location=ILResourceLocation.LocalOut bytes
                         Access=pub 
                         CustomAttrsStored=storeILCustomAttrs emptyILCustomAttrs 
                         MetadataIndex = NoMetadataIdx }
               
              yield! reflectedDefinitionResources
              yield! intfDataResources
              yield! optDataResources
              for ri in tcConfig.linkResources do 
                 let file, name, pub = TcConfigBuilder.SplitCommandLineResourceInfo ri
                 yield { Name=name 
                         Location=ILResourceLocation.File(ILModuleRef.Create(name=file, hasMetadata=false, hash=Some (sha1HashBytes (FileSystem.ReadAllBytesShim file))), 0)
                         Access=pub 
                         CustomAttrsStored=storeILCustomAttrs emptyILCustomAttrs
                         MetadataIndex = NoMetadataIdx } ]

        let assemblyVersion = 
            match tcConfig.version with
            | VersionNone -> assemVerFromAttrib
            | _ -> Some tcVersion

        let findAttribute name =
            AttributeHelpers.TryFindStringAttribute tcGlobals name topAttrs.assemblyAttrs 


        //NOTE: the culture string can be turned into a number using this:
        //    sprintf "%04x" (CultureInfo.GetCultureInfo("en").KeyboardLayoutId )
        let assemblyVersionResources findAttr assemblyVersion =
            match assemblyVersion with 
            | None -> []
            | Some assemblyVersion ->
                let FindAttribute key attrib = 
                    match findAttr attrib with
                    | Some text  -> [(key, text)]
                    | _ -> []

                let fileVersionInfo = fileVersion findAttr assemblyVersion

                let productVersionString = productVersion findAttr fileVersionInfo

                let stringFileInfo = 
                     // 000004b0:
                     // Specifies an 8-digit hexadecimal number stored as a Unicode string. The 
                     // four most significant digits represent the language identifier. The four least 
                     // significant digits represent the code page for which the data is formatted. 
                     // Each Microsoft Standard Language identifier contains two parts: the low-order 10 bits 
                     // specify the major language, and the high-order 6 bits specify the sublanguage. 
                     // For a table of valid identifiers see Language Identifiers.                                           //
                     // see e.g. http://msdn.microsoft.com/en-us/library/aa912040.aspx 0000 is neutral and 04b0(hex)=1252(dec) is the code page.
                      [ ("000004b0", [ yield ("Assembly Version", (sprintf "%d.%d.%d.%d" assemblyVersion.Major assemblyVersion.Minor assemblyVersion.Build assemblyVersion.Revision))
                                       yield ("FileVersion", (sprintf "%d.%d.%d.%d" fileVersionInfo.Major fileVersionInfo.Minor fileVersionInfo.Build fileVersionInfo.Revision))
                                       yield ("ProductVersion", productVersionString)
                                       match tcConfig.outputFile with
                                       | Some f -> yield ("OriginalFilename", Path.GetFileName f)
                                       | None -> ()
                                       yield! FindAttribute "Comments" "System.Reflection.AssemblyDescriptionAttribute" 
                                       yield! FindAttribute "FileDescription" "System.Reflection.AssemblyTitleAttribute" 
                                       yield! FindAttribute "ProductName" "System.Reflection.AssemblyProductAttribute" 
                                       yield! FindAttribute "CompanyName" "System.Reflection.AssemblyCompanyAttribute" 
                                       yield! FindAttribute "LegalCopyright" "System.Reflection.AssemblyCopyrightAttribute" 
                                       yield! FindAttribute "LegalTrademarks" "System.Reflection.AssemblyTrademarkAttribute" ]) ]

                // These entries listed in the MSDN documentation as "standard" string entries are not yet settable

                // InternalName: 
                //     The Value member identifies the file's internal name, if one exists. For example, this 
                //     string could contain the module name for Windows dynamic-link libraries (DLLs), a virtual 
                //     device name for Windows virtual devices, or a device name for MS-DOS device drivers. 
                // OriginalFilename: 
                //     The Value member identifies the original name of the file, not including a path. This 
                //     enables an application to determine whether a file has been renamed by a user. This name 
                //     may not be MS-DOS 8.3-format if the file is specific to a non-FAT file system. 
                // PrivateBuild: 
                //     The Value member describes by whom, where, and why this private version of the 
                //     file was built. This string should only be present if the VS_FF_PRIVATEBUILD flag 
                //     is set in the dwFileFlags member of the VS_FIXEDFILEINFO structure. For example, 
                //     Value could be 'Built by OSCAR on \OSCAR2'. 
                // SpecialBuild: 
                //     The Value member describes how this version of the file differs from the normal version. 
                //     This entry should only be present if the VS_FF_SPECIALBUILD flag is set in the dwFileFlags 
                //     member of the VS_FIXEDFILEINFO structure. For example, Value could be 'Private build 
                //     for Olivetti solving mouse problems on M250 and M250E computers'. 

                // "If you use the Var structure to list the languages your application 
                // or DLL supports instead of using multiple version resources, 
                // use the Value member to contain an array of DWORD values indicating the 
                // language and code page combinations supported by this file. The 
                // low-order word of each DWORD must contain a Microsoft language identifier, 
                // and the high-order word must contain the IBM code page number. 
                // Either high-order or low-order word can be zero, indicating that 
                // the file is language or code page independent. If the Var structure is 
                // omitted, the file will be interpreted as both language and code page independent. "
                let varFileInfo = [ (0x0, 0x04b0)  ]

                let fixedFileInfo = 
                    let dwFileFlagsMask = 0x3f // REVIEW: HARDWIRED
                    let dwFileFlags = 0x00 // REVIEW: HARDWIRED
                    let dwFileOS = 0x04 // REVIEW: HARDWIRED
                    let dwFileType = 0x01 // REVIEW: HARDWIRED
                    let dwFileSubtype = 0x00 // REVIEW: HARDWIRED
                    let lwFileDate = 0x00L // REVIEW: HARDWIRED
                    (fileVersionInfo, productVersionString |> productVersionToILVersionInfo, dwFileFlagsMask, dwFileFlags, dwFileOS, dwFileType, dwFileSubtype, lwFileDate)

                let vsVersionInfoResource = 
                    VersionResourceFormat.VS_VERSION_INFO_RESOURCE(fixedFileInfo, stringFileInfo, varFileInfo)

                let resource = 
                    [| yield! ResFileFormat.ResFileHeader()
                       yield! vsVersionInfoResource |]

                [ resource ]

        // a user cannot specify both win32res and win32manifest
        if not(tcConfig.win32manifest = "") && not(tcConfig.win32res = "") then
            error(Error(FSComp.SR.fscTwoResourceManifests(), rangeCmdArgs))

        let win32Manifest =
            // use custom manifest if provided
            if not(tcConfig.win32manifest = "") then tcConfig.win32manifest

            // don't embed a manifest if target is not an exe, if manifest is specifically excluded, if another native resource is being included, or if running on mono
            elif not(tcConfig.target.IsExe) || not(tcConfig.includewin32manifest) || not(tcConfig.win32res = "") || runningOnMono then ""
            // otherwise, include the default manifest
            else
#if FX_NO_RUNTIMEENVIRONMENT
                // On coreclr default manifest is alongside the compiler
                Path.Combine(System.AppContext.BaseDirectory, @"default.win32manifest")
#else
                // On the desktop default manifest is alongside the clr
                Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), @"default.win32manifest")
#endif
        let nativeResources = 
            [ for av in assemblyVersionResources findAttribute assemblyVersion do
                  yield ILNativeResource.Out av
              if not(tcConfig.win32res = "") then
                  yield ILNativeResource.Out (FileSystem.ReadAllBytesShim tcConfig.win32res) 
              if tcConfig.includewin32manifest && not(win32Manifest = "") && not runningOnMono then
                  yield  ILNativeResource.Out [| yield! ResFileFormat.ResFileHeader() 
                                                 yield! (ManifestResourceFormat.VS_MANIFEST_RESOURCE((FileSystem.ReadAllBytesShim win32Manifest), tcConfig.target = CompilerTarget.Dll)) |]]

        // Add attributes, version number, resources etc. 
        {mainModule with 
              StackReserveSize = tcConfig.stackReserveSize
              Name = (if tcConfig.target = CompilerTarget.Module then Filename.fileNameOfPath outfile else mainModule.Name)
              SubSystemFlags = (if tcConfig.target = CompilerTarget.WinExe then 2 else 3) 
              Resources= resources
              ImageBase = (match tcConfig.baseAddress with None -> 0x00400000l | Some b -> b)
              IsDLL=(tcConfig.target = CompilerTarget.Dll || tcConfig.target=CompilerTarget.Module)
              Platform = tcConfig.platform 
              Is32Bit=(match tcConfig.platform with Some X86 -> true | _ -> false)
              Is64Bit=(match tcConfig.platform with Some AMD64 | Some IA64 -> true | _ -> false)          
              Is32BitPreferred = if tcConfig.prefer32Bit && not tcConfig.target.IsExe then (error(Error(FSComp.SR.invalidPlatformTarget(), rangeCmdArgs))) else tcConfig.prefer32Bit
              CustomAttrsStored= 
                  storeILCustomAttrs
                    (mkILCustomAttrs 
                      [ if tcConfig.target = CompilerTarget.Module then 
                           yield! iattrs 
                        yield! codegenResults.ilNetModuleAttrs ])
              NativeResources=nativeResources
              Manifest = manifest }


//----------------------------------------------------------------------------
// Static linking
//----------------------------------------------------------------------------

/// Optional static linking of all DLLs that depend on the F# Library, plus other specified DLLs
module StaticLinker = 
    let debugStaticLinking = condition "FSHARP_DEBUG_STATIC_LINKING"

    let StaticLinkILModules (tcConfig, ilGlobals, ilxMainModule, dependentILModules: (CcuThunk option * ILModuleDef) list) = 
        if isNil dependentILModules then 
            ilxMainModule, (fun x -> x) 
        else

            // Check no dependent assemblies use quotations   
            let dependentCcuUsingQuotations = dependentILModules |> List.tryPick (function (Some ccu, _) when ccu.UsesFSharp20PlusQuotations -> Some ccu | _ -> None)   
            match dependentCcuUsingQuotations with   
            | Some ccu -> error(Error(FSComp.SR.fscQuotationLiteralsStaticLinking(ccu.AssemblyName), rangeStartup))   
            | None -> ()  
                
            // Check we're not static linking a .EXE
            if dependentILModules |> List.exists (fun (_, x) -> not x.IsDLL)  then 
                error(Error(FSComp.SR.fscStaticLinkingNoEXE(), rangeStartup))

            // Check we're not static linking something that is not pure IL
            if dependentILModules |> List.exists (fun (_, x) -> not x.IsILOnly)  then 
                error(Error(FSComp.SR.fscStaticLinkingNoMixedDLL(), rangeStartup))

            // The set of short names for the all dependent assemblies
            let assems = 
                set [ for (_, m) in dependentILModules  do
                         match m.Manifest with 
                         | Some m -> yield m.Name 
                         | _ -> () ]
            
            // A rewriter which rewrites scope references to things in dependent assemblies to be local references 
            let rewriteExternalRefsToLocalRefs x = 
                if assems.Contains (getNameOfScopeRef x) then ILScopeRef.Local else x

            let savedManifestAttrs = 
                [ for (_, depILModule) in dependentILModules do 
                    match depILModule.Manifest with 
                    | Some m -> 
                        for ca in m.CustomAttrs.AsArray do
                           if ca.Method.MethodRef.DeclaringTypeRef.FullName = typeof<CompilationMappingAttribute>.FullName then 
                               yield ca
                    | _ -> () ]

            let savedResources = 
                let allResources = [ for (ccu, m) in dependentILModules do for r in m.Resources.AsList do yield (ccu, r) ]
                // Don't save interface, optimization or resource definitions for provider-generated assemblies.
                // These are "fake".
                let isProvided (ccu: CcuThunk option) = 
#if !NO_EXTENSIONTYPING
                    match ccu with 
                    | Some c -> c.IsProviderGenerated 
                    | None -> false
#else
                    ignore ccu
                    false
#endif

                // Save only the interface/optimization attributes of generated data 
                let intfDataResources, others = allResources |> List.partition (snd >> IsSignatureDataResource)
                let intfDataResources = 
                    [ for (ccu, r) in intfDataResources do 
                          if GenerateInterfaceData tcConfig && not (isProvided ccu) then 
                              yield r ]

                let optDataResources, others = others |> List.partition (snd >> IsOptimizationDataResource)
                let optDataResources = 
                    [ for (ccu, r) in optDataResources do 
                          if GenerateOptimizationData tcConfig && not (isProvided ccu) then 
                              yield r ]

                let otherResources = others |> List.map snd 

                let result = intfDataResources@optDataResources@otherResources
                result

            let moduls = ilxMainModule :: (List.map snd dependentILModules)

            let savedNativeResources = 
                [ //yield! ilxMainModule.NativeResources 
                  for m in moduls do 
                      yield! m.NativeResources ]

            let topTypeDefs, normalTypeDefs = 
                moduls 
                |> List.map (fun m -> m.TypeDefs.AsList |> List.partition (fun td -> isTypeNameForGlobalFunctions td.Name)) 
                |> List.unzip

            let topTypeDef = 
                let topTypeDefs = List.concat topTypeDefs
                mkILTypeDefForGlobalFunctions ilGlobals
                   (mkILMethods (topTypeDefs |> List.collect (fun td -> td.Methods.AsList)), 
                    mkILFields (topTypeDefs |> List.collect (fun td -> td.Fields.AsList)))

            let ilxMainModule = 
                { ilxMainModule with 
                    Manifest = (let m = ilxMainModule.ManifestOfAssembly in Some {m with CustomAttrsStored = storeILCustomAttrs (mkILCustomAttrs (m.CustomAttrs.AsList @ savedManifestAttrs)) })
                    CustomAttrsStored = storeILCustomAttrs (mkILCustomAttrs [ for m in moduls do yield! m.CustomAttrs.AsArray ])
                    TypeDefs = mkILTypeDefs (topTypeDef :: List.concat normalTypeDefs)
                    Resources = mkILResources (savedResources @ ilxMainModule.Resources.AsList)
                    NativeResources = savedNativeResources }

            ilxMainModule, rewriteExternalRefsToLocalRefs


    // LEGACY: This is only used when compiling an FSharp.Core for .NET 2.0 (FSharp.Core 2.3.0.0). We no longer
    // build new FSharp.Core for that configuration.
    //
    // Find all IL modules that are to be statically linked given the static linking roots.
    let LegacyFindAndAddMscorlibTypesForStaticLinkingIntoFSharpCoreLibraryForNet20 (tcConfig: TcConfig, ilGlobals: ILGlobals, ilxMainModule) = 
        let mscorlib40 = tcConfig.compilingFslib20.Value 
              
        let ilBinaryReader = 
            let ilGlobals = mkILGlobals ILScopeRef.Local
            let opts : ILReaderOptions = 
                { ilGlobals = ilGlobals
                  reduceMemoryUsage = tcConfig.reduceMemoryUsage
                  metadataOnly = MetadataOnlyFlag.No
                  tryGetMetadataSnapshot = (fun _ -> None)
                  pdbDirPath = None  } 
            ILBinaryReader.OpenILModuleReader mscorlib40 opts
              
        let tdefs1 = ilxMainModule.TypeDefs.AsList  |> List.filter (fun td -> not (MainModuleBuilder.injectedCompatTypes.Contains(td.Name)))
        let tdefs2 = ilBinaryReader.ILModuleDef.TypeDefs.AsList |> List.filter (fun td -> MainModuleBuilder.injectedCompatTypes.Contains(td.Name))
        //printfn "tdefs2 = %A" (tdefs2 |> List.map (fun tdef -> tdef.Name))

        // rewrite the mscorlib references 
        let tdefs2 = 
            let fakeModule = mkILSimpleModule "" "" true (4, 0) false (mkILTypeDefs tdefs2) None None 0 (mkILExportedTypes []) ""
            let fakeModule = 
                  fakeModule |> Morphs.morphILTypeRefsInILModuleMemoized ilGlobals (fun tref -> 
                      if MainModuleBuilder.injectedCompatTypes.Contains(tref.Name)  || (tref.Enclosing  |> List.exists (fun x -> MainModuleBuilder.injectedCompatTypes.Contains x)) then 
                          tref
                          //|> Morphs.morphILScopeRefsInILTypeRef (function ILScopeRef.Local -> ilGlobals.mscorlibScopeRef | x -> x) 
                      // The implementations of Tuple use two private methods from System.Environment to get a resource string. Remap it
                      elif tref.Name = "System.Environment" then 
                          ILTypeRef.Create(ILScopeRef.Local, [], "Microsoft.FSharp.Core.PrivateEnvironment")  //|> Morphs.morphILScopeRefsInILTypeRef (function ILScopeRef.Local -> ilGlobals.mscorlibScopeRef | x -> x) 
                      else 
                          tref |> Morphs.morphILScopeRefsInILTypeRef (fun _ -> ilGlobals.primaryAssemblyScopeRef) )
                  
            // strip out System.Runtime.TargetedPatchingOptOutAttribute, which doesn't exist for 2.0
            let fakeModule = 
              {fakeModule with 
                TypeDefs = 
                  mkILTypeDefs 
                      ([ for td in fakeModule.TypeDefs do 
                             let meths = td.Methods.AsList
                                            |> List.map (fun md ->
                                                md.With(customAttrs = 
                                                              mkILCustomAttrs (td.CustomAttrs.AsList |> List.filter (fun ilattr ->
                                                                ilattr.Method.DeclaringType.TypeRef.FullName <> "System.Runtime.TargetedPatchingOptOutAttribute")))) 
                                            |> mkILMethods
                             let td = td.With(methods=meths)
                             yield td.With(methods=meths) ])}
            //ILAsciiWriter.output_module stdout fakeModule
            fakeModule.TypeDefs.AsList

        let ilxMainModule = 
            { ilxMainModule with 
                TypeDefs = mkILTypeDefs (tdefs1 @ tdefs2) }
        ilxMainModule

    [<NoEquality; NoComparison>]
    type Node = 
        { name: string
          data: ILModuleDef 
          ccu: option<CcuThunk>
          refs: ILReferences
          mutable edges: list<Node> 
          mutable visited: bool }

    // Find all IL modules that are to be statically linked given the static linking roots.
    let FindDependentILModulesForStaticLinking (ctok, tcConfig: TcConfig, tcImports: TcImports, ilGlobals, ilxMainModule) = 
        if not tcConfig.standalone && tcConfig.extraStaticLinkRoots.IsEmpty then 
            []
        else
            // Recursively find all referenced modules and add them to a module graph 
            let depModuleTable = HashMultiMap(0, HashIdentity.Structural)
            let dummyEntry nm =
                { refs = IL.emptyILRefs 
                  name=nm
                  ccu=None
                  data=ilxMainModule // any old module
                  edges = [] 
                  visited = true }
            let assumedIndependentSet = set [ "mscorlib";  "System"; "System.Core"; "System.Xml"; "Microsoft.Build.Framework"; "Microsoft.Build.Utilities" ]      

            begin 
                let remaining = ref (computeILRefs ilxMainModule).AssemblyReferences
                while not (isNil !remaining) do
                    let ilAssemRef = List.head !remaining
                    remaining := List.tail !remaining
                    if assumedIndependentSet.Contains ilAssemRef.Name || (ilAssemRef.PublicKey = Some ecmaPublicKey) then 
                        depModuleTable.[ilAssemRef.Name] <- dummyEntry ilAssemRef.Name
                    else
                        if not (depModuleTable.ContainsKey ilAssemRef.Name) then 
                            match tcImports.TryFindDllInfo(ctok, Range.rangeStartup, ilAssemRef.Name, lookupOnly=false) with 
                            | Some dllInfo ->
                                let ccu = 
                                    match tcImports.FindCcuFromAssemblyRef (ctok, Range.rangeStartup, ilAssemRef) with 
                                    | ResolvedCcu ccu -> Some ccu
                                    | UnresolvedCcu(_ccuName) -> None

                                let fileName = dllInfo.FileName
                                let modul = 
                                    let pdbDirPathOption = 
                                        // We open the pdb file if one exists parallel to the binary we 
                                        // are reading, so that --standalone will preserve debug information. 
                                        if tcConfig.openDebugInformationForLaterStaticLinking then 
                                            let pdbDir = (try Filename.directoryName fileName with _ -> ".") 
                                            let pdbFile = (try Filename.chopExtension fileName with _ -> fileName)+".pdb" 
                                            if FileSystem.SafeExists pdbFile then 
                                                if verbose then dprintf "reading PDB file %s from directory %s during static linking\n" pdbFile pdbDir
                                                Some pdbDir
                                            else 
                                                None 
                                        else   
                                            None

                                    let opts : ILReaderOptions = 
                                        { ilGlobals = ilGlobals
                                          metadataOnly = MetadataOnlyFlag.No // turn this off here as we need the actual IL code
                                          reduceMemoryUsage = tcConfig.reduceMemoryUsage
                                          pdbDirPath = pdbDirPathOption
                                          tryGetMetadataSnapshot = (fun _ -> None) } 

                                    let reader = ILBinaryReader.OpenILModuleReader dllInfo.FileName opts
                                    reader.ILModuleDef

                                let refs = 
                                    if ilAssemRef.Name = GetFSharpCoreLibraryName() then 
                                        IL.emptyILRefs 
                                    elif not modul.IsILOnly then 
                                        warning(Error(FSComp.SR.fscIgnoringMixedWhenLinking ilAssemRef.Name, rangeStartup))
                                        IL.emptyILRefs 
                                    else
                                        { AssemblyReferences = dllInfo.ILAssemblyRefs 
                                          ModuleReferences = [] }

                                depModuleTable.[ilAssemRef.Name] <- 
                                    { refs=refs
                                      name=ilAssemRef.Name
                                      ccu=ccu
                                      data=modul 
                                      edges = [] 
                                      visited = false }

                                // Push the new work items
                                remaining := refs.AssemblyReferences @ !remaining

                            | None -> 
                                warning(Error(FSComp.SR.fscAssumeStaticLinkContainsNoDependencies(ilAssemRef.Name), rangeStartup)) 
                                depModuleTable.[ilAssemRef.Name] <- dummyEntry ilAssemRef.Name
                done
            end

            ReportTime tcConfig "Find dependencies"

            // Add edges from modules to the modules that depend on them 
            for (KeyValue(_, n)) in depModuleTable do 
                for aref in n.refs.AssemblyReferences do
                    let n2 = depModuleTable.[aref.Name] 
                    n2.edges <- n :: n2.edges
                    
            // Find everything that depends on FSharp.Core
            let roots = 
                [ if tcConfig.standalone && depModuleTable.ContainsKey (GetFSharpCoreLibraryName()) then 
                      yield depModuleTable.[GetFSharpCoreLibraryName()]
                  for n in tcConfig.extraStaticLinkRoots  do
                      match depModuleTable.TryFind n with 
                      | Some x -> yield x
                      | None -> error(Error(FSComp.SR.fscAssemblyNotFoundInDependencySet n, rangeStartup)) 
                ]
                              
            let remaining = ref roots
            [ while not (isNil !remaining) do
                let n = List.head !remaining
                remaining := List.tail !remaining
                if not n.visited then 
                    if verbose then dprintn ("Module "+n.name+" depends on "+GetFSharpCoreLibraryName())
                    n.visited <- true
                    remaining := n.edges @ !remaining
                    yield (n.ccu, n.data)  ]

    // Add all provider-generated assemblies into the static linking set
    let FindProviderGeneratedILModules (ctok, tcImports: TcImports, providerGeneratedAssemblies: (ImportedBinary * _) list) = 
        [ for (importedBinary, provAssemStaticLinkInfo) in providerGeneratedAssemblies do 
              let ilAssemRef  = importedBinary.ILScopeRef.AssemblyRef
              if debugStaticLinking then printfn "adding provider-generated assembly '%s' into static linking set" ilAssemRef.Name
              match tcImports.TryFindDllInfo(ctok, Range.rangeStartup, ilAssemRef.Name, lookupOnly=false) with 
              | Some dllInfo ->
                  let ccu = 
                      match tcImports.FindCcuFromAssemblyRef (ctok, Range.rangeStartup, ilAssemRef) with 
                      | ResolvedCcu ccu -> Some ccu
                      | UnresolvedCcu(_ccuName) -> None

                  let modul = dllInfo.RawMetadata.TryGetILModuleDef().Value
                  yield (ccu, dllInfo.ILScopeRef, modul), (ilAssemRef.Name, provAssemStaticLinkInfo)
              | None -> () ]

    // Compute a static linker. This only captures tcImports (a large data structure) if
    // static linking is enabled. Normally this is not the case, which lets us collect tcImports
    // prior to this point.
    let StaticLink (ctok, tcConfig: TcConfig, tcImports: TcImports, ilGlobals: ILGlobals) = 

#if !NO_EXTENSIONTYPING
        let providerGeneratedAssemblies = 

            [ // Add all EST-generated assemblies into the static linking set
                for KeyValue(_, importedBinary: ImportedBinary) in tcImports.DllTable do
                    if importedBinary.IsProviderGenerated then 
                        match importedBinary.ProviderGeneratedStaticLinkMap with 
                        | None -> ()
                        | Some provAssemStaticLinkInfo -> yield (importedBinary, provAssemStaticLinkInfo) ]
#endif
        if tcConfig.compilingFslib && tcConfig.compilingFslib20.IsSome then 
            (fun ilxMainModule -> LegacyFindAndAddMscorlibTypesForStaticLinkingIntoFSharpCoreLibraryForNet20 (tcConfig, ilGlobals, ilxMainModule))
          
        elif not tcConfig.standalone && tcConfig.extraStaticLinkRoots.IsEmpty 
#if !NO_EXTENSIONTYPING
             && providerGeneratedAssemblies.IsEmpty 
#endif
             then 
            (fun ilxMainModule -> ilxMainModule)
        else 
            (fun ilxMainModule  ->
              ReportTime tcConfig "Find assembly references"

              let dependentILModules = FindDependentILModulesForStaticLinking (ctok, tcConfig, tcImports, ilGlobals, ilxMainModule)

              ReportTime tcConfig "Static link"

#if !NO_EXTENSIONTYPING
              Morphs.enableMorphCustomAttributeData()
              let providerGeneratedILModules =  FindProviderGeneratedILModules (ctok, tcImports, providerGeneratedAssemblies) 

              // Transform the ILTypeRefs references in the IL of all provider-generated assemblies so that the references
              // are now local.
              let providerGeneratedILModules = 
               
                  providerGeneratedILModules |> List.map (fun ((ccu, ilOrigScopeRef, ilModule), (_, localProvAssemStaticLinkInfo)) -> 
                      let ilAssemStaticLinkMap = 
                          dict [ for (_, (_, provAssemStaticLinkInfo)) in providerGeneratedILModules do 
                                     for KeyValue(k, v) in provAssemStaticLinkInfo.ILTypeMap do 
                                         yield (k, v)
                                 for KeyValue(k, v) in localProvAssemStaticLinkInfo.ILTypeMap do
                                     yield (ILTypeRef.Create(ILScopeRef.Local, k.Enclosing, k.Name), v) ]

                      let ilModule = 
                          ilModule |> Morphs.morphILTypeRefsInILModuleMemoized ilGlobals (fun tref -> 
                                  if debugStaticLinking then printfn "deciding whether to rewrite type ref %A" tref.QualifiedName 
                                  let ok, v = ilAssemStaticLinkMap.TryGetValue tref
                                  if ok then 
                                      if debugStaticLinking then printfn "rewriting type ref %A to %A" tref.QualifiedName v.QualifiedName
                                      v
                                  else 
                                      tref)
                      (ccu, ilOrigScopeRef, ilModule))

              // Relocate provider generated type definitions into the expected shape for the [<Generate>] declarations in an assembly
              let providerGeneratedILModules, ilxMainModule = 
                  // Build a dictionary of all remapped IL type defs 
                  let ilOrigTyRefsForProviderGeneratedTypesToRelocate = 
                      let rec walk acc (ProviderGeneratedType(ilOrigTyRef, _, xs) as node) = List.fold walk ((ilOrigTyRef, node) :: acc) xs 
                      dict (Seq.fold walk [] tcImports.ProviderGeneratedTypeRoots)

                  // Build a dictionary of all IL type defs, mapping ilOrigTyRef --> ilTypeDef
                  let allTypeDefsInProviderGeneratedAssemblies = 
                      let rec loop ilOrigTyRef (ilTypeDef: ILTypeDef) = 
                          seq { yield (ilOrigTyRef, ilTypeDef) 
                                for ntdef in ilTypeDef.NestedTypes do 
                                    yield! loop (mkILTyRefInTyRef (ilOrigTyRef, ntdef.Name)) ntdef }
                      dict [ 
                          for (_ccu, ilOrigScopeRef, ilModule) in providerGeneratedILModules do 
                              for td in ilModule.TypeDefs do 
                                  yield! loop (mkILTyRef (ilOrigScopeRef, td.Name)) td ]


                  // Debugging output
                  if debugStaticLinking then 
                      for (ProviderGeneratedType(ilOrigTyRef, _, _)) in tcImports.ProviderGeneratedTypeRoots do
                          printfn "Have [<Generate>] root '%s'" ilOrigTyRef.QualifiedName

                  // Build the ILTypeDefs for generated types, starting with the roots 
                  let generatedILTypeDefs = 
                      let rec buildRelocatedGeneratedType (ProviderGeneratedType(ilOrigTyRef, ilTgtTyRef, ch)) = 
                          let isNested = not (isNil ilTgtTyRef.Enclosing)
                          match allTypeDefsInProviderGeneratedAssemblies.TryGetValue ilOrigTyRef with
                          | true, ilOrigTypeDef ->
                              if debugStaticLinking then printfn "Relocating %s to %s " ilOrigTyRef.QualifiedName ilTgtTyRef.QualifiedName
                              let ilOrigTypeDef = 
                                if isNested then
                                    ilOrigTypeDef
                                        .WithAccess(match ilOrigTypeDef.Access with 
                                                    | ILTypeDefAccess.Public -> ILTypeDefAccess.Nested ILMemberAccess.Public
                                                    | ILTypeDefAccess.Private -> ILTypeDefAccess.Nested ILMemberAccess.Private
                                                    | _ -> ilOrigTypeDef.Access)
                                else ilOrigTypeDef
                              ilOrigTypeDef.With(name = ilTgtTyRef.Name,
                                                 nestedTypes = mkILTypeDefs (List.map buildRelocatedGeneratedType ch))
                          | _ ->
                              // If there is no matching IL type definition, then make a simple container class
                              if debugStaticLinking then 
                                  printfn "Generating simple class '%s' because we didn't find an original type '%s' in a provider generated assembly" 
                                      ilTgtTyRef.QualifiedName ilOrigTyRef.QualifiedName

                              let access = (if isNested  then ILTypeDefAccess.Nested ILMemberAccess.Public else ILTypeDefAccess.Public)
                              let tdefs = mkILTypeDefs (List.map buildRelocatedGeneratedType ch)
                              mkILSimpleClass ilGlobals (ilTgtTyRef.Name, access, emptyILMethods, emptyILFields, tdefs, emptyILProperties, emptyILEvents, emptyILCustomAttrs, ILTypeInit.OnAny) 

                      [ for (ProviderGeneratedType(_, ilTgtTyRef, _) as node) in tcImports.ProviderGeneratedTypeRoots  do
                           yield (ilTgtTyRef, buildRelocatedGeneratedType node) ]
                  
                  // Implant all the generated type definitions into the ilxMainModule (generating a new ilxMainModule)
                  let ilxMainModule = 

                      /// Split the list into left, middle and right parts at the first element satisfying 'p'. If no element matches return
                      /// 'None' for the middle part.
                      let trySplitFind p xs = 
                          let rec loop xs acc = 
                              match xs with 
                              | [] -> List.rev acc, None, [] 
                              | h :: t -> if p h then List.rev acc, Some h, t else loop t (h :: acc)
                          loop xs []

                      /// Implant the (nested) type definition 'td' at path 'enc' in 'tdefs'. 
                      let rec implantTypeDef isNested (tdefs: ILTypeDefs) (enc: string list) (td: ILTypeDef) = 
                          match enc with 
                          | [] -> addILTypeDef td tdefs
                          | h :: t -> 
                               let tdefs = tdefs.AsList
                               let (ltdefs, htd, rtdefs) = 
                                   match tdefs |> trySplitFind (fun td -> td.Name = h) with 
                                   | (ltdefs, None, rtdefs) -> 
                                       let access = if isNested  then ILTypeDefAccess.Nested ILMemberAccess.Public else ILTypeDefAccess.Public
                                       let fresh = mkILSimpleClass ilGlobals (h, access, emptyILMethods, emptyILFields, emptyILTypeDefs, emptyILProperties, emptyILEvents, emptyILCustomAttrs, ILTypeInit.OnAny)
                                       (ltdefs, fresh, rtdefs)
                                   | (ltdefs, Some htd, rtdefs) -> 
                                       (ltdefs, htd, rtdefs)
                               let htd = htd.With(nestedTypes = implantTypeDef true htd.NestedTypes t td)
                               mkILTypeDefs (ltdefs @ [htd] @ rtdefs)

                      let newTypeDefs = 
                          (ilxMainModule.TypeDefs, generatedILTypeDefs) ||> List.fold (fun acc (ilTgtTyRef, td) -> 
                              if debugStaticLinking then printfn "implanting '%s' at '%s'" td.Name ilTgtTyRef.QualifiedName 
                              implantTypeDef false acc ilTgtTyRef.Enclosing td) 
                      { ilxMainModule with TypeDefs = newTypeDefs } 
                  
                  // Remove any ILTypeDefs from the provider generated modules if they have been relocated because of a [<Generate>] declaration.
                  let providerGeneratedILModules = 
                      providerGeneratedILModules |> List.map (fun (ccu, ilOrigScopeRef, ilModule) -> 
                          let ilTypeDefsAfterRemovingRelocatedTypes = 
                              let rec rw enc (tdefs: ILTypeDefs) = 
                                  mkILTypeDefs
                                   [ for tdef in tdefs do 
                                        let ilOrigTyRef = mkILNestedTyRef (ilOrigScopeRef, enc, tdef.Name)
                                        if  not (ilOrigTyRefsForProviderGeneratedTypesToRelocate.ContainsKey ilOrigTyRef) then
                                          if debugStaticLinking then printfn "Keep provided type %s in place because it wasn't relocated" ilOrigTyRef.QualifiedName
                                          yield tdef.With(nestedTypes = rw (enc@[tdef.Name]) tdef.NestedTypes) ]
                              rw [] ilModule.TypeDefs
                          (ccu, { ilModule with TypeDefs = ilTypeDefsAfterRemovingRelocatedTypes }))

                  providerGeneratedILModules, ilxMainModule
             
              Morphs.disableMorphCustomAttributeData()
#else
              let providerGeneratedILModules = []
#endif

              // Glue all this stuff into ilxMainModule 
              let ilxMainModule, rewriteExternalRefsToLocalRefs = 
                  StaticLinkILModules (tcConfig, ilGlobals, ilxMainModule, dependentILModules @ providerGeneratedILModules)
              
              // Rewrite type and assembly references
              let ilxMainModule =
                  let isMscorlib = ilGlobals.primaryAssemblyName = PrimaryAssembly.Mscorlib.Name
                  let validateTargetPlatform (scopeRef : ILScopeRef) = 
                      let name = getNameOfScopeRef scopeRef
                      if (not isMscorlib && name = PrimaryAssembly.Mscorlib.Name) then
                          error (Error(FSComp.SR.fscStaticLinkingNoProfileMismatches(), rangeCmdArgs))
                      scopeRef
                  let rewriteAssemblyRefsToMatchLibraries = NormalizeAssemblyRefs (ctok, tcImports)
                  Morphs.morphILTypeRefsInILModuleMemoized ilGlobals (Morphs.morphILScopeRefsInILTypeRef (validateTargetPlatform >> rewriteExternalRefsToLocalRefs >> rewriteAssemblyRefsToMatchLibraries)) ilxMainModule

              ilxMainModule)
  
//----------------------------------------------------------------------------
// ValidateKeySigningAttributes, GetStrongNameSigner
//----------------------------------------------------------------------------

type StrongNameSigningInfo = StrongNameSigningInfo of (* delaysign:*) bool * (* publicsign:*) bool * (*signer:*)  string option * (*container:*) string option

let ValidateKeySigningAttributes (tcConfig : TcConfig, tcGlobals, topAttrs) =
    let delaySignAttrib = AttributeHelpers.TryFindBoolAttribute tcGlobals "System.Reflection.AssemblyDelaySignAttribute" topAttrs.assemblyAttrs
    let signerAttrib = AttributeHelpers.TryFindStringAttribute tcGlobals "System.Reflection.AssemblyKeyFileAttribute" topAttrs.assemblyAttrs
    let containerAttrib = AttributeHelpers.TryFindStringAttribute tcGlobals "System.Reflection.AssemblyKeyNameAttribute" topAttrs.assemblyAttrs
    
    // if delaySign is set via an attribute, validate that it wasn't set via an option
    let delaysign = 
        match delaySignAttrib with 
        | Some delaysign -> 
          if tcConfig.delaysign then
            warning(Error(FSComp.SR.fscDelaySignWarning(), rangeCmdArgs)) 
            tcConfig.delaysign
          else
            delaysign
        | _ -> tcConfig.delaysign
        
    // if signer is set via an attribute, validate that it wasn't set via an option
    let signer = 
        match signerAttrib with
        | Some signer -> 
            if tcConfig.signer.IsSome && tcConfig.signer <> Some signer then
                warning(Error(FSComp.SR.fscKeyFileWarning(), rangeCmdArgs)) 
                tcConfig.signer
            else
                Some signer
        | None -> tcConfig.signer
    
    // if container is set via an attribute, validate that it wasn't set via an option, and that they keyfile wasn't set
    // if keyfile was set, use that instead (silently)
    // REVIEW: This is C# behavior, but it seems kind of sketchy that we fail silently
    let container = 
        match containerAttrib with 
        | Some container -> 
            if tcConfig.container.IsSome && tcConfig.container <> Some container then
              warning(Error(FSComp.SR.fscKeyNameWarning(), rangeCmdArgs)) 
              tcConfig.container
            else
              Some container
        | None -> tcConfig.container
    
    StrongNameSigningInfo (delaysign, tcConfig.publicsign, signer, container)

/// Checks if specified file name is absolute path. If yes - returns the name as is, otherwise makes full path using tcConfig.implicitIncludeDir as base.
let expandFileNameIfNeeded (tcConfig : TcConfig) name = 
    if FileSystem.IsPathRootedShim name then 
        name 
    else 
        Path.Combine(tcConfig.implicitIncludeDir, name)

let GetStrongNameSigner signingInfo = 
        let (StrongNameSigningInfo(delaysign, publicsign, signer, container)) = signingInfo
        // REVIEW: favor the container over the key file - C# appears to do this
        if Option.isSome container then
          Some (ILBinaryWriter.ILStrongNameSigner.OpenKeyContainer container.Value)
        else
            match signer with 
            | None -> None
            | Some s ->
                try 
                if publicsign || delaysign then
                    Some (ILBinaryWriter.ILStrongNameSigner.OpenPublicKeyOptions s publicsign)
                else
                    Some (ILBinaryWriter.ILStrongNameSigner.OpenKeyPairFile s) 
                with e -> 
                    // Note :: don't use errorR here since we really want to fail and not produce a binary
                    error(Error(FSComp.SR.fscKeyFileCouldNotBeOpened s, rangeCmdArgs))

//----------------------------------------------------------------------------
// CopyFSharpCore
//----------------------------------------------------------------------------

// If the --nocopyfsharpcore switch is not specified, this will:
// 1) Look into the referenced assemblies, if FSharp.Core.dll is specified, it will copy it to output directory.
// 2) If not, but FSharp.Core.dll exists beside the compiler binaries, it will copy it to output directory.
// 3) If not, it will produce an error.
let CopyFSharpCore(outFile: string, referencedDlls: AssemblyReference list) =
    let outDir = Path.GetDirectoryName outFile
    let fsharpCoreAssemblyName = GetFSharpCoreLibraryName() + ".dll"
    let fsharpCoreDestinationPath = Path.Combine(outDir, fsharpCoreAssemblyName)
    let copyFileIfDifferent src dest =
        if not (File.Exists dest) || (File.GetCreationTimeUtc src <> File.GetCreationTimeUtc dest) then
            File.Copy(src, dest, true)

    match referencedDlls |> Seq.tryFind (fun dll -> String.Equals(Path.GetFileName(dll.Text), fsharpCoreAssemblyName, StringComparison.CurrentCultureIgnoreCase)) with
    | Some referencedFsharpCoreDll -> copyFileIfDifferent referencedFsharpCoreDll.Text fsharpCoreDestinationPath
    | None ->
        let executionLocation =
            Assembly.GetExecutingAssembly().Location
        let compilerLocation = Path.GetDirectoryName executionLocation
        let compilerFsharpCoreDllPath = Path.Combine(compilerLocation, fsharpCoreAssemblyName)
        if File.Exists compilerFsharpCoreDllPath then
            copyFileIfDifferent compilerFsharpCoreDllPath fsharpCoreDestinationPath
        else
            errorR(Error(FSComp.SR.fsharpCoreNotFoundToBeCopied(), rangeCmdArgs))

//----------------------------------------------------------------------------
// Main phases of compilation
//-----------------------------------------------------------------------------

[<NoEquality; NoComparison>]
type Args<'T> = Args  of 'T

let main0(ctok, argv, legacyReferenceResolver, bannerAlreadyPrinted, 
          reduceMemoryUsage: ReduceMemoryFlag, defaultCopyFSharpCore: CopyFSharpCoreFlag, 
          exiter: Exiter, errorLoggerProvider : ErrorLoggerProvider, disposables : DisposablesTracker) = 

    // See Bug 735819 
    let lcidFromCodePage = 
#if FX_LCIDFROMCODEPAGE
        if (Console.OutputEncoding.CodePage <> 65001) &&
           (Console.OutputEncoding.CodePage <> Thread.CurrentThread.CurrentUICulture.TextInfo.OEMCodePage) &&
           (Console.OutputEncoding.CodePage <> Thread.CurrentThread.CurrentUICulture.TextInfo.ANSICodePage) then
                Thread.CurrentThread.CurrentUICulture <- new CultureInfo("en-US")
                Some 1033
        else
#endif
            None

    let directoryBuildingFrom = Directory.GetCurrentDirectory()
    let setProcessThreadLocals tcConfigB =
                    match tcConfigB.preferredUiLang with
#if FX_RESHAPED_GLOBALIZATION
                    | Some s -> CultureInfo.CurrentUICulture <- new CultureInfo(s)
#else
                    | Some s -> Thread.CurrentThread.CurrentUICulture <- new CultureInfo(s)
#endif
                    | None -> ()
                    if tcConfigB.utf8output then 
                        Console.OutputEncoding <- Encoding.UTF8

    let displayBannerIfNeeded tcConfigB =
                    // display the banner text, if necessary
                    if not bannerAlreadyPrinted then 
                        DisplayBannerText tcConfigB

    let tryGetMetadataSnapshot = (fun _ -> None)

    let tcConfigB = 
       TcConfigBuilder.CreateNew(legacyReferenceResolver, DefaultFSharpBinariesDir, 
          reduceMemoryUsage=reduceMemoryUsage, implicitIncludeDir=directoryBuildingFrom, 
          isInteractive=false, isInvalidationSupported=false, 
          defaultCopyFSharpCore=defaultCopyFSharpCore, 
          tryGetMetadataSnapshot=tryGetMetadataSnapshot)

    // Preset: --optimize+ -g --tailcalls+ (see 4505)
    SetOptimizeSwitch tcConfigB OptionSwitch.On
    SetDebugSwitch    tcConfigB None OptionSwitch.Off
    SetTailcallSwitch tcConfigB OptionSwitch.On    

    // Now install a delayed logger to hold all errors from flags until after all flags have been parsed (for example, --vserrors)
    let delayForFlagsLogger =  errorLoggerProvider.CreateDelayAndForwardLogger exiter
    let _unwindEL_1 = PushErrorLoggerPhaseUntilUnwind (fun _ -> delayForFlagsLogger)          
    
    // Share intern'd strings across all lexing/parsing
    let lexResourceManager = new Lexhelp.LexResourceManager()

    // process command line, flags and collect filenames 
    let sourceFiles = 

        // The ParseCompilerOptions function calls imperative function to process "real" args
        // Rather than start processing, just collect names, then process them. 
        try 
            let sourceFiles = 
                let files = ProcessCommandLineFlags (tcConfigB, setProcessThreadLocals, lcidFromCodePage, argv)
                AdjustForScriptCompile(ctok, tcConfigB, files, lexResourceManager)
            sourceFiles

        with e -> 
            errorRecovery e rangeStartup
            delayForFlagsLogger.ForwardDelayedDiagnostics tcConfigB
            exiter.Exit 1 
    
    tcConfigB.conditionalCompilationDefines <- "COMPILED" :: tcConfigB.conditionalCompilationDefines 
    displayBannerIfNeeded tcConfigB

    // Create tcGlobals and frameworkTcImports
    let outfile, pdbfile, assemblyName = 
        try 
            tcConfigB.DecideNames sourceFiles
        with e ->
            errorRecovery e rangeStartup
            delayForFlagsLogger.ForwardDelayedDiagnostics tcConfigB
            exiter.Exit 1 
                    
    // DecideNames may give "no inputs" error. Abort on error at this point. bug://3911
    if not tcConfigB.continueAfterParseFailure && delayForFlagsLogger.ErrorCount > 0 then
        delayForFlagsLogger.ForwardDelayedDiagnostics tcConfigB
        exiter.Exit 1
    
    // If there's a problem building TcConfig, abort    
    let tcConfig = 
        try
            TcConfig.Create(tcConfigB, validate=false)
        with e ->
            delayForFlagsLogger.ForwardDelayedDiagnostics tcConfigB
            exiter.Exit 1
    
    let errorLogger =  errorLoggerProvider.CreateErrorLoggerUpToMaxErrors(tcConfigB, exiter)

    // Install the global error logger and never remove it. This logger does have all command-line flags considered.
    let _unwindEL_2 = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger)
    
    // Forward all errors from flags
    delayForFlagsLogger.CommitDelayedDiagnostics errorLogger

    if not tcConfigB.continueAfterParseFailure then 
        AbortOnError(errorLogger, exiter)

    // Resolve assemblies
    ReportTime tcConfig "Import mscorlib and FSharp.Core.dll"
    let foundationalTcConfigP = TcConfigProvider.Constant tcConfig
    let sysRes, otherRes, knownUnresolved = TcAssemblyResolutions.SplitNonFoundationalResolutions(ctok, tcConfig)
    
    // Import basic assemblies
    let tcGlobals, frameworkTcImports = TcImports.BuildFrameworkTcImports (ctok, foundationalTcConfigP, sysRes, otherRes) |> Cancellable.runWithoutCancellation

    // Register framework tcImports to be disposed in future
    disposables.Register frameworkTcImports

    // Parse sourceFiles 
    ReportTime tcConfig "Parse inputs"
    use unwindParsePhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse
    let inputs =
        try
            let isLastCompiland, isExe = sourceFiles |> tcConfig.ComputeCanContainEntryPoint 
            isLastCompiland |> List.zip sourceFiles
            // PERF: consider making this parallel, once uses of global state relevant to parsing are cleaned up 
            |> List.choose (fun (filename: string, isLastCompiland) -> 
                let pathOfMetaCommandSource = Path.GetDirectoryName filename
                match ParseOneInputFile(tcConfig, lexResourceManager, ["COMPILED"], filename, (isLastCompiland, isExe), errorLogger, (*retryLocked*)false) with
                | Some input -> Some (input, pathOfMetaCommandSource)
                | None -> None
                ) 
        with e -> 
            errorRecoveryNoRange e
            exiter.Exit 1
    
    let inputs, _ =
        (Map.empty, inputs)
        ||> List.mapFold (fun state (input,x) -> let inputT, stateT = DeduplicateParsedInputModuleName state input in (inputT,x), stateT)

    if tcConfig.parseOnly then exiter.Exit 0 
    if not tcConfig.continueAfterParseFailure then 
        AbortOnError(errorLogger, exiter)

    if tcConfig.printAst then                
        inputs |> List.iter (fun (input, _filename) -> printf "AST:\n"; printfn "%+A" input; printf "\n") 

    let tcConfig = (tcConfig, inputs) ||> List.fold (fun z (x, m) -> ApplyMetaCommandsFromInputToTcConfig(z, x, m))
    let tcConfigP = TcConfigProvider.Constant tcConfig

    // Import other assemblies
    ReportTime tcConfig "Import non-system references"
    let tcImports = TcImports.BuildNonFrameworkTcImports(ctok, tcConfigP, tcGlobals, frameworkTcImports, otherRes, knownUnresolved)  |> Cancellable.runWithoutCancellation

    // register tcImports to be disposed in future
    disposables.Register tcImports

    if not tcConfig.continueAfterParseFailure then 
        AbortOnError(errorLogger, exiter)

    if tcConfig.importAllReferencesOnly then exiter.Exit 0 

    // Build the initial type checking environment
    ReportTime tcConfig "Typecheck"
    use unwindParsePhase = PushThreadBuildPhaseUntilUnwind BuildPhase.TypeCheck
    let tcEnv0 = GetInitialTcEnv (assemblyName, rangeStartup, tcConfig, tcImports, tcGlobals)

    // Type check the inputs
    let inputs = inputs |> List.map fst
    let tcState, topAttrs, typedAssembly, _tcEnvAtEnd = 
        TypeCheck(ctok, tcConfig, tcImports, tcGlobals, errorLogger, assemblyName, NiceNameGenerator(), tcEnv0, inputs, exiter)

    AbortOnError(errorLogger, exiter)
    ReportTime tcConfig "Typechecked"

    Args (ctok, tcGlobals, tcImports, frameworkTcImports, tcState.Ccu, typedAssembly, topAttrs, tcConfig, outfile, pdbfile, assemblyName, errorLogger, exiter)

let main1(Args (ctok, tcGlobals, tcImports: TcImports, frameworkTcImports, generatedCcu: CcuThunk, typedImplFiles, topAttrs, tcConfig: TcConfig, outfile, pdbfile, assemblyName, errorLogger, exiter: Exiter)) =

    if tcConfig.typeCheckOnly then exiter.Exit 0
    
    generatedCcu.Contents.SetAttribs(generatedCcu.Contents.Attribs @ topAttrs.assemblyAttrs)

    use unwindPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.CodeGen
    let signingInfo = ValidateKeySigningAttributes (tcConfig, tcGlobals, topAttrs)
    
    AbortOnError(errorLogger, exiter)

    // Build an updated errorLogger that filters according to the scopedPragmas. Then install
    // it as the updated global error logger and never remove it
    let oldLogger = errorLogger
    let errorLogger = 
        let scopedPragmas = [ for (TImplFile (_, pragmas, _, _, _, _)) in typedImplFiles do yield! pragmas ]
        GetErrorLoggerFilteringByScopedPragmas(true, scopedPragmas, oldLogger)

    let _unwindEL_3 = PushErrorLoggerPhaseUntilUnwind(fun _ -> errorLogger)

    // Try to find an AssemblyVersion attribute 
    let assemVerFromAttrib = 
        match AttributeHelpers.TryFindVersionAttribute tcGlobals "System.Reflection.AssemblyVersionAttribute" "AssemblyVersionAttribute" topAttrs.assemblyAttrs tcConfig.deterministic with
        | Some v -> 
           match tcConfig.version with 
           | VersionNone -> Some v
           | _ -> warning(Error(FSComp.SR.fscAssemblyVersionAttributeIgnored(), Range.rangeStartup)); None
        | _ -> None

    // write interface, xmldoc
    begin
      ReportTime tcConfig ("Write Interface File")
      use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Output
      if tcConfig.printSignature   then InterfaceFileWriter.WriteInterfaceFile (tcGlobals, tcConfig, InfoReader(tcGlobals, tcImports.GetImportMap()), typedImplFiles)

      ReportTime tcConfig ("Write XML document signatures")
      if tcConfig.xmlDocOutputFile.IsSome then 
          XmlDocWriter.computeXmlDocSigs (tcGlobals, generatedCcu) 

      ReportTime tcConfig ("Write XML docs")
      tcConfig.xmlDocOutputFile |> Option.iter ( fun xmlFile -> 
          let xmlFile = tcConfig.MakePathAbsolute xmlFile
          XmlDocWriter.writeXmlDoc (assemblyName, generatedCcu, xmlFile)
        )
      ReportTime tcConfig ("Write HTML docs")
    end

    // Pass on only the minimum information required for the next phase
    Args (ctok, tcConfig, tcImports, frameworkTcImports, tcGlobals, errorLogger, generatedCcu, outfile, typedImplFiles, topAttrs, pdbfile, assemblyName, assemVerFromAttrib, signingInfo, exiter)


// This is for the compile-from-AST feature of FCS.
// TODO: consider removing this feature from FCS, which as far as I know is not used by anyone.
let main1OfAst (ctok, legacyReferenceResolver, reduceMemoryUsage, assemblyName, target, outfile, pdbFile, dllReferences, noframework, exiter, errorLoggerProvider: ErrorLoggerProvider, inputs : ParsedInput list) =

    let tryGetMetadataSnapshot = (fun _ -> None)

    let tcConfigB = 
        TcConfigBuilder.CreateNew(legacyReferenceResolver, DefaultFSharpBinariesDir, 
            reduceMemoryUsage=reduceMemoryUsage, implicitIncludeDir=Directory.GetCurrentDirectory(), 
            isInteractive=false, isInvalidationSupported=false, 
            defaultCopyFSharpCore=CopyFSharpCoreFlag.No, 
            tryGetMetadataSnapshot=tryGetMetadataSnapshot)

    tcConfigB.framework <- not noframework 
    // Preset: --optimize+ -g --tailcalls+ (see 4505)
    SetOptimizeSwitch tcConfigB OptionSwitch.On
    SetDebugSwitch    tcConfigB None (
        match pdbFile with
        | Some _ -> OptionSwitch.On
        | None -> OptionSwitch.Off)
    SetTailcallSwitch tcConfigB OptionSwitch.On
    tcConfigB.target <- target
        
    let errorLogger = errorLoggerProvider.CreateErrorLoggerUpToMaxErrors (tcConfigB, exiter)

    tcConfigB.conditionalCompilationDefines <- "COMPILED" :: tcConfigB.conditionalCompilationDefines

    // append assembly dependencies
    dllReferences |> List.iter (fun ref -> tcConfigB.AddReferencedAssemblyByPath(rangeStartup,ref))

    // If there's a problem building TcConfig, abort    
    let tcConfig = 
        try
            TcConfig.Create(tcConfigB,validate=false)
        with e ->
            exiter.Exit 1
    
    let foundationalTcConfigP = TcConfigProvider.Constant tcConfig
    let sysRes,otherRes,knownUnresolved = TcAssemblyResolutions.SplitNonFoundationalResolutions(ctok, tcConfig)
    let tcGlobals,frameworkTcImports = TcImports.BuildFrameworkTcImports (ctok, foundationalTcConfigP, sysRes, otherRes) |> Cancellable.runWithoutCancellation

    use unwindParsePhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parse) 

    let meta = Directory.GetCurrentDirectory()
    let tcConfig = (tcConfig,inputs) ||> List.fold (fun tcc inp -> ApplyMetaCommandsFromInputToTcConfig (tcc, inp,meta))
    let tcConfigP = TcConfigProvider.Constant tcConfig

    let tcGlobals,tcImports =  
        let tcImports = TcImports.BuildNonFrameworkTcImports(ctok, tcConfigP, tcGlobals, frameworkTcImports, otherRes,knownUnresolved) |> Cancellable.runWithoutCancellation
        tcGlobals,tcImports

    use unwindParsePhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.TypeCheck)            
    let tcEnv0 = GetInitialTcEnv (assemblyName, rangeStartup, tcConfig, tcImports, tcGlobals)

    let tcState,topAttrs,typedAssembly,_tcEnvAtEnd = 
        TypeCheck(ctok, tcConfig, tcImports, tcGlobals, errorLogger, assemblyName, NiceNameGenerator(), tcEnv0, inputs,exiter)

    let generatedCcu = tcState.Ccu
    generatedCcu.Contents.SetAttribs(generatedCcu.Contents.Attribs @ topAttrs.assemblyAttrs)

    use unwindPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.CodeGen)
    let signingInfo = ValidateKeySigningAttributes (tcConfig, tcGlobals, topAttrs)

    // Try to find an AssemblyVersion attribute 
    let assemVerFromAttrib = 
        match AttributeHelpers.TryFindVersionAttribute tcGlobals "System.Reflection.AssemblyVersionAttribute" "AssemblyVersionAttribute" topAttrs.assemblyAttrs tcConfig.deterministic with
        | Some v -> 
            match tcConfig.version with 
            | VersionNone -> Some v
            | _ -> warning(Error(FSComp.SR.fscAssemblyVersionAttributeIgnored(),Range.range0)); None
        | _ -> None

    // Pass on only the minimum information required for the next phase to ensure GC kicks in.
    // In principle the JIT should be able to do good liveness analysis to clean things up, but the
    // data structures involved here are so large we can't take the risk.
    Args(ctok, tcConfig, tcImports, frameworkTcImports, tcGlobals, errorLogger, 
         generatedCcu, outfile, typedAssembly, topAttrs, pdbFile, assemblyName, 
         assemVerFromAttrib, signingInfo,exiter)

  
/// Phase 2a: encode signature data, optimize, encode optimization data
let main2a(Args (ctok, tcConfig, tcImports, frameworkTcImports: TcImports, tcGlobals, 
                 errorLogger: ErrorLogger, generatedCcu: CcuThunk, outfile, typedImplFiles, 
                 topAttrs, pdbfile, assemblyName, assemVerFromAttrib, signingInfo, exiter: Exiter)) = 
      
    // Encode the signature data
    ReportTime tcConfig ("Encode Interface Data")
    let exportRemapping = MakeExportRemapping generatedCcu generatedCcu.Contents
    
    let sigDataAttributes, sigDataResources = 
      try
        EncodeInterfaceData(tcConfig, tcGlobals, exportRemapping, generatedCcu, outfile, false)
      with e -> 
        errorRecoveryNoRange e
        exiter.Exit 1
        
    // Perform optimization
    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Optimize
    
    let optEnv0 = GetInitialOptimizationEnv (tcImports, tcGlobals)
   
    let importMap = tcImports.GetImportMap()
    let metadataVersion = 
        match tcConfig.metadataVersion with
        | Some v -> v
        | _ -> 
            match frameworkTcImports.DllTable.TryFind tcConfig.primaryAssembly.Name with 
             | Some ib -> ib.RawMetadata.TryGetILModuleDef().Value.MetadataVersion 
             | _ -> ""

    let optimizedImpls, optimizationData, _ = 
        ApplyAllOptimizations 
            (tcConfig, tcGlobals, (LightweightTcValForUsingInBuildMethodCall tcGlobals), outfile, 
             importMap, false, optEnv0, generatedCcu, typedImplFiles)

    AbortOnError(errorLogger, exiter)
        
    // Encode the optimization data
    ReportTime tcConfig ("Encoding OptData")
    let optDataResources = EncodeOptimizationData(tcGlobals, tcConfig, outfile, exportRemapping, (generatedCcu, optimizationData), false)

    // Pass on only the minimum information required for the next phase
    Args (ctok, tcConfig, tcImports, tcGlobals, errorLogger, 
          generatedCcu, outfile, optimizedImpls, topAttrs, pdbfile, assemblyName, 
          (sigDataAttributes, sigDataResources), optDataResources, assemVerFromAttrib, signingInfo, metadataVersion, exiter)

/// Phase 2b: IL code generation
let main2b 
      (tcImportsCapture,dynamicAssemblyCreator) 
      (Args (ctok, tcConfig: TcConfig, tcImports, tcGlobals: TcGlobals, errorLogger, 
             generatedCcu: CcuThunk, outfile, optimizedImpls, topAttrs, pdbfile, assemblyName, 
             idata, optDataResources, assemVerFromAttrib, signingInfo, metadataVersion, exiter: Exiter)) = 

    match tcImportsCapture with 
    | None -> ()
    | Some f -> f tcImports

    // Compute a static linker. 
    let ilGlobals = tcGlobals.ilg
    if tcConfig.standalone && generatedCcu.UsesFSharp20PlusQuotations then    
        error(Error(FSComp.SR.fscQuotationLiteralsStaticLinking0(), rangeStartup))  
    let staticLinker = StaticLinker.StaticLink (ctok, tcConfig, tcImports, ilGlobals)

    // Generate IL code
    ReportTime tcConfig "TAST -> IL"
    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.IlxGen
    let ilxGenerator = CreateIlxAssemblyGenerator (tcConfig, tcImports, tcGlobals, (LightweightTcValForUsingInBuildMethodCall tcGlobals), generatedCcu)

    let codegenBackend = (if Option.isSome dynamicAssemblyCreator then IlReflectBackend else IlWriteBackend)
    let codegenResults = GenerateIlxCode (codegenBackend, Option.isSome dynamicAssemblyCreator, false, tcConfig, topAttrs, optimizedImpls, generatedCcu.AssemblyName, ilxGenerator)
    let topAssemblyAttrs = codegenResults.topAssemblyAttrs
    let topAttrs = {topAttrs with assemblyAttrs=topAssemblyAttrs}
    let permissionSets = codegenResults.permissionSets
    let secDecls = mkILSecurityDecls permissionSets 

    let ilxMainModule = MainModuleBuilder.CreateMainModule (ctok, tcConfig, tcGlobals, tcImports, pdbfile, assemblyName, outfile, topAttrs, idata, optDataResources, codegenResults, assemVerFromAttrib, metadataVersion, secDecls)

    AbortOnError(errorLogger, exiter)

    // Pass on only the minimum information required for the next phase
    Args (ctok, tcConfig, tcImports, tcGlobals, errorLogger, staticLinker, outfile, pdbfile, ilxMainModule, signingInfo, exiter)

/// Phase 3: static linking
let main3(Args (ctok, tcConfig, tcImports, tcGlobals, errorLogger: ErrorLogger, staticLinker, outfile, pdbfile, ilxMainModule, signingInfo, exiter: Exiter)) = 
        
    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Output

    // Static linking, if any
    let ilxMainModule =  
        try  staticLinker ilxMainModule
        with e -> 
            errorRecoveryNoRange e
            exiter.Exit 1

    AbortOnError(errorLogger, exiter)
        
    // Pass on only the minimum information required for the next phase
    Args (ctok, tcConfig, tcImports, tcGlobals, errorLogger, ilxMainModule, outfile, pdbfile, signingInfo, exiter)

/// Phase 4: write the binaries
let main4 dynamicAssemblyCreator (Args (ctok, tcConfig,  tcImports: TcImports, tcGlobals: TcGlobals, 
                                        errorLogger: ErrorLogger, ilxMainModule, outfile, pdbfile, 
                                        signingInfo, exiter: Exiter)) = 

    ReportTime tcConfig "Write .NET Binary"

    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Output
    let outfile = tcConfig.MakePathAbsolute outfile

    DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok

    let pdbfile = pdbfile |> Option.map (tcConfig.MakePathAbsolute >> FileSystem.GetFullPathShim)

    let normalizeAssemblyRefs (aref: ILAssemblyRef) = 
        match tcImports.TryFindDllInfo (ctok, Range.rangeStartup, aref.Name, lookupOnly=false) with 
        | Some dllInfo ->
            match dllInfo.ILScopeRef with 
            | ILScopeRef.Assembly ref -> ref
            | _ -> aref
        | None -> aref

    match dynamicAssemblyCreator with 
    | None -> 
        try
            try 
                ILBinaryWriter.WriteILBinary 
                 (outfile, 
                  { ilg = tcGlobals.ilg
                    pdbfile=pdbfile
                    emitTailcalls = tcConfig.emitTailcalls
                    deterministic = tcConfig.deterministic
                    showTimes = tcConfig.showTimes
                    portablePDB = tcConfig.portablePDB
                    embeddedPDB = tcConfig.embeddedPDB
                    embedAllSource = tcConfig.embedAllSource
                    embedSourceList = tcConfig.embedSourceList
                    sourceLink = tcConfig.sourceLink
                    signer = GetStrongNameSigner signingInfo
                    dumpDebugInfo = tcConfig.dumpDebugInfo
                    pathMap = tcConfig.pathMap },
                  ilxMainModule,
                  normalizeAssemblyRefs
                  )
            with Failure msg -> 
                error(Error(FSComp.SR.fscProblemWritingBinary(outfile, msg), rangeCmdArgs))
        with e -> 
            errorRecoveryNoRange e
            exiter.Exit 1 
    | Some da -> da (tcGlobals,outfile,ilxMainModule)

    AbortOnError(errorLogger, exiter)

    // Don't copy referenced FSharp.core.dll if we are building FSharp.Core.dll
    if (tcConfig.copyFSharpCore = CopyFSharpCoreFlag.Yes) && not tcConfig.compilingFslib && not tcConfig.standalone then
        CopyFSharpCore(outfile, tcConfig.referencedDLLs)

    ReportTime tcConfig "Exiting"

//----------------------------------------------------------------------------
// Entry points to compilation
//-----------------------------------------------------------------------------

/// Entry point typecheckAndCompile
let typecheckAndCompile 
       (ctok, argv, legacyReferenceResolver, bannerAlreadyPrinted, reduceMemoryUsage, 
        defaultCopyFSharpCore, exiter: Exiter, errorLoggerProvider, tcImportsCapture, dynamicAssemblyCreator) =

    use d = new DisposablesTracker()
    use e = new SaveAndRestoreConsoleEncoding()

    main0(ctok, argv, legacyReferenceResolver, bannerAlreadyPrinted, reduceMemoryUsage, defaultCopyFSharpCore, exiter, errorLoggerProvider, d)
    |> main1
    |> main2a
    |> main2b (tcImportsCapture,dynamicAssemblyCreator)
    |> main3 
    |> main4 dynamicAssemblyCreator


let compileOfAst 
       (ctok, legacyReferenceResolver, reduceMemoryUsage, assemblyName, target, 
        outFile, pdbFile, dllReferences, noframework, exiter, errorLoggerProvider, inputs, tcImportsCapture, dynamicAssemblyCreator) = 

    main1OfAst (ctok, legacyReferenceResolver, reduceMemoryUsage, assemblyName, target, outFile, pdbFile, 
                dllReferences, noframework, exiter, errorLoggerProvider, inputs)
    |> main2a
    |> main2b (tcImportsCapture, dynamicAssemblyCreator)
    |> main3
    |> main4 dynamicAssemblyCreator

let mainCompile 
        (ctok, argv, legacyReferenceResolver, bannerAlreadyPrinted, reduceMemoryUsage, 
         defaultCopyFSharpCore, exiter, errorLoggerProvider, tcImportsCapture, dynamicAssemblyCreator) = 

    typecheckAndCompile
       (ctok, argv, legacyReferenceResolver, bannerAlreadyPrinted, reduceMemoryUsage, 
        defaultCopyFSharpCore, exiter, errorLoggerProvider, tcImportsCapture, dynamicAssemblyCreator)

