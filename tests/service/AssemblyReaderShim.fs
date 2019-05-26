#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module FSharp.Compiler.Service.Tests.AssemblyReaderShim
#endif

open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.Internal.Library
open FsUnit
open NUnit.Framework

[<Test>]
let ``Assembly reader shim gets requests`` () =
    let defaultReader = AssemblyReader
    let mutable gotRequest = false
    let reader =
        { new IAssemblyReader with
            member x.GetILModuleReader(path, opts) =
                gotRequest <- true
                defaultReader.GetILModuleReader(path, opts)

            member x.GetLastWriteTime(path, bypassFileSystemShim) =
                FileSystem.GetLastWriteTime(path, bypassFileSystemShim)

            member x.Exists(path, bypassFileSystemShim) =
                FileSystem.Exists(path, bypassFileSystemShim) }

    AssemblyReader <- reader
    let source = """
module M
let x = 123
"""

    let fileName, options = Common.mkTestFileAndOptions source [| |]
    Common.checker.ParseAndCheckFileInProject(fileName, 0, FSharp.Compiler.Text.SourceText.ofString source, options) |> Async.RunSynchronously |> ignore
    gotRequest |> should be True
