module internal FSharp.Compiler.ReuseTcResults.TcResultsImport

open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.IO
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.TypedTreePickle
open FSharp.Compiler.ReuseTcResults.TcResultsPickle

let GetTypecheckingData (file, ilScopeRef, ilModule, byteReaderA, byteReaderB) =

    let memA = byteReaderA ()

    let memB =
        match byteReaderB with
        | None -> ByteMemory.Empty.AsReadOnly()
        | Some br -> br ()

    unpickleObjWithDanglingCcus file ilScopeRef ilModule unpickleTcInfo memA memB

let WriteTypecheckingData (tcConfig: TcConfig, tcGlobals, fileName, inMem, ccu, tcInfo) =

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
        pickleTcInfo
        tcInfo

let EncodeTypecheckingData (tcConfig: TcConfig, tcGlobals, generatedCcu, outfile, isIncrementalBuild, tcInfo) =
    let r1, r2 =
        WriteTypecheckingData(
            tcConfig, 
            tcGlobals,
            outfile, 
            isIncrementalBuild, 
            generatedCcu,
            tcInfo)

    let resources =
        [
            r1
            match r2 with
            | None -> ()
            | Some r -> r
        ]

    resources
