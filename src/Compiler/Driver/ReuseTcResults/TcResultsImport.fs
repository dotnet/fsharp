module internal FSharp.Compiler.ReuseTcResults.TcResultsImport

open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.IO
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.TypedTreePickle
open FSharp.Compiler.ReuseTcResults.TcResultsPickle

let GetSharedData (file, ilScopeRef, ilModule, byteReaderA, byteReaderB) =

    let memA = byteReaderA ()

    let memB =
        match byteReaderB with
        | None -> ByteMemory.Empty.AsReadOnly()
        | Some br -> br ()

    unpickleObjWithDanglingCcus file ilScopeRef ilModule unpickleSharedData memA memB

let GetCheckedImplFile (file, ilScopeRef, ilModule, byteReaderA, byteReaderB) =

    let memA = byteReaderA ()

    let memB =
        match byteReaderB with
        | None -> ByteMemory.Empty.AsReadOnly()
        | Some br -> br ()

    unpickleObjWithDanglingCcus file ilScopeRef ilModule unpickleCheckedImplFile memA memB

let GetTypecheckingDataTcState (file, ilScopeRef, ilModule, byteReaderA, byteReaderB) =

    let memA = byteReaderA ()

    let memB =
        match byteReaderB with
        | None -> ByteMemory.Empty.AsReadOnly()
        | Some br -> br ()

    unpickleObjWithDanglingCcus file ilScopeRef ilModule unpickleTcState memA memB

let WriteSharedData (tcConfig: TcConfig, tcGlobals, fileName, inMem, ccu, sharedData) =

    // need to understand the naming and if we even want two resources here...
    let rName = "FSharpTypecheckingData"
    let rNameB = "FSharpTypecheckingDataB"

    PickleToResource
        inMem
        fileName
        tcGlobals
        tcConfig.compressMetadata
        ccu
        (rName + ccu.AssemblyName)
        (rNameB + ccu.AssemblyName)
        pickleSharedData
        sharedData

let WriteCheckedImplFile (tcConfig: TcConfig, tcGlobals, fileName, inMem, ccu, checkedImplFile) =

    // need to understand the naming and if we even want two resources here...
    let rName = "FSharpTypecheckingData"
    let rNameB = "FSharpTypecheckingDataB"

    PickleToResource
        inMem
        fileName
        tcGlobals
        tcConfig.compressMetadata
        ccu
        (rName + ccu.AssemblyName)
        (rNameB + ccu.AssemblyName)
        pickleCheckedImplFile
        checkedImplFile

let WriteTypecheckingDataTcState (tcConfig: TcConfig, tcGlobals, fileName, inMem, ccu, tcState) =

    // need to understand the naming and if we even want two resources here...
    let rName = "FSharpTypecheckingData"
    let rNameB = "FSharpTypecheckingDataB"

    PickleToResource
        inMem
        fileName
        tcGlobals
        tcConfig.compressMetadata
        ccu
        (rName + ccu.AssemblyName)
        (rNameB + ccu.AssemblyName)
        pickleTcState
        tcState

let EncodeSharedData (tcConfig: TcConfig, tcGlobals, generatedCcu, outfile, isIncrementalBuild, sharedData) =
    let r1, r2 =
        WriteSharedData(tcConfig, tcGlobals, outfile, isIncrementalBuild, generatedCcu, sharedData)

    let resources =
        [
            r1
            match r2 with
            | None -> ()
            | Some r -> r
        ]

    resources

let EncodeCheckedImplFile (tcConfig: TcConfig, tcGlobals, generatedCcu, outfile, isIncrementalBuild, checkedImplFile) =
    let r1, r2 =
        WriteCheckedImplFile(tcConfig, tcGlobals, outfile, isIncrementalBuild, generatedCcu, checkedImplFile)

    let resources =
        [
            r1
            match r2 with
            | None -> ()
            | Some r -> r
        ]

    resources

let EncodeTypecheckingDataTcState (tcConfig: TcConfig, tcGlobals, generatedCcu, outfile, isIncrementalBuild, tcState) =
    let r1, r2 =
        WriteTypecheckingDataTcState(tcConfig, tcGlobals, outfile, isIncrementalBuild, generatedCcu, tcState)

    let resources =
        [
            r1
            match r2 with
            | None -> ()
            | Some r -> r
        ]

    resources
