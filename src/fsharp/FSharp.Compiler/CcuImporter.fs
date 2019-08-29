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

//let getILModuleReader ilg (peStream: Stream, pdbStreamOpt: Stream option) =
//    let opts: ILReaderOptions = 
//        { ilGlobals = ilg
//          metadataOnly = MetadataOnlyFlag.Yes
//          reduceMemoryUsage = ReduceMemoryFlag.No
//          pdbDirPath = None
//          tryGetMetadataSnapshot = fun _ -> None } 
                    
//    AssemblyReader.GetILModuleReader(location, opts)