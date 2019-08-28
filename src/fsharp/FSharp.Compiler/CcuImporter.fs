module FSharp.Compiler.Compilation.CcuImporter

open System
open System.IO
open System.Threading
open System.Diagnostics
open System.Collections.Immutable
open System.Collections.Generic
open FSharp.NativeInterop

open FSharp.Compiler
open FSharp.Compiler.CompileOps
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Driver
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.ErrorLogger
open Internal.Utilities

//let openILBinary (filename, openDebugInformationForLaterStaticLinking, ) =
//    let pdbDirPath =
//        // We open the pdb file if one exists parallel to the binary we 
//        // are reading, so that --standalone will preserve debug information. 
//        if openDebugInformationForLaterStaticLinking then 
//            let pdbDir = try Filename.directoryName filename with _ -> "."
//            let pdbFile = (try Filename.chopExtension filename with _ -> filename) + ".pdb" 

//            if FileSystem.SafeExists pdbFile then
//                Some pdbDir
//            else
//                None
//        else
//            None

//    let ilGlobals =
//        // ILScopeRef.Local can be used only for primary assembly (mscorlib or System.Runtime) itself
//        // Remaining assemblies should be opened using existing ilGlobals (so they can properly locate fundamental types)
//        match ilGlobalsOpt with
//        | None -> mkILGlobals ILScopeRef.Local
//        | Some g -> g

//    let ilILBinaryReader =
//        OpenILBinary (filename, tcConfig.reduceMemoryUsage, ilGlobals, pdbDirPath, tcConfig.shadowCopyReferences, tcConfig.tryGetMetadataSnapshot)

//    ilILBinaryReader.ILModuleDef, ilILBinaryReader.ILAssemblyRefs