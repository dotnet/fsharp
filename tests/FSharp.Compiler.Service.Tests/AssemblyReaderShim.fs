module FSharp.Compiler.Service.Tests.AssemblyReaderShim

open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.ILBinaryReader
open Xunit

[<Fact(Skip = "Flaky: seems to run fine locally but often fails in CI")>]
let ``Assembly reader shim gets requests`` () =
    let defaultReader = AssemblyReader
    let mutable gotRequest = false
    let reader =
        { new IAssemblyReader with
            member x.GetILModuleReader(path, opts) =
                gotRequest <- true
                defaultReader.GetILModuleReader(path, opts)
        }
    AssemblyReader <- reader
    let source = """
module M
let x = 123
"""

    let fileName, options = mkTestFileAndOptions source [| |]
    checker.ParseAndCheckFileInProject(fileName, 0, SourceText.ofString source, options) |> Async.RunImmediate |> ignore
    gotRequest |> Assert.True
