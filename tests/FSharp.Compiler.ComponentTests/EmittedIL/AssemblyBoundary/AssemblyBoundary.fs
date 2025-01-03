namespace EmittedIL

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module AssemblyBoundary =

    let verifyCompileAndExecution compilation =
        compilation
        |> asFs
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed


    //NoMT SOURCE=test01.fs SCFLAGS="--optimize+ -r:lib01.dll" PRECMD="\$FSC_PIPE -a --optimize+ lib01.fs" # test01.fs
    [<Theory; FileInlineData("test01.fs")>]
    let ``test01_fs`` compilation =
        let lib01 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "lib01.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> getCompilation
        |> withReferences [lib01]
        |> verifyCompileAndExecution

    //NoMT SOURCE=test01.fs SCFLAGS="--optimize+ -r:lib01.dll" PRECMD="\$FSC_PIPE -a --optimize+ lib01.fs" # test01.fs
    [<Theory; FileInlineData("test02.fs")>]
    let ``test02_fs`` compilation =
        let lib02 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "lib02.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> getCompilation
        |> withReferences [lib02]
        |> verifyCompileAndExecution

    //NoMT SOURCE=test03.fs SCFLAGS="--optimize+ -r:lib03.dll" PRECMD="\$FSC_PIPE -a --optimize+ lib03.fs" # test03.fs
    [<Theory; FileInlineData("test03.fs")>]
    let ``test03_fs`` compilation =
        let lib03 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "lib03.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> getCompilation
        |> withReferences [lib03]
        |> verifyCompileAndExecution

    //NoMT SOURCE=test04.fs SCFLAGS="--optimize+ -r:lib04.dll" PRECMD="\$FSC_PIPE -a --optimize+ lib04.fs" # test04.fs
    [<Theory; FileInlineData("test04.fs")>]
    let ``test04_fs`` compilation =
        let lib04 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "lib04.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> getCompilation
        |> withReferences [lib04]
        |> verifyCompileAndExecution


    // SOURCE=InlineWithPrivateValues01.fs SCFLAGS="-r:TypeLib01.dll" PRECMD="\$FSC_PIPE -a --optimize+ TypeLib01.fs" # InlineWithPrivateValuesStruct
    [<Theory; FileInlineData("InlineWithPrivateValues01.fs")>]
    let ``InlineWithPrivateValues01_fs_TypeLib01_fs`` compilation =
        let typeLib01 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "TypeLib01.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> getCompilation
        |> withReferences [typeLib01]
        |> verifyCompileAndExecution

    // SOURCE=InlineWithPrivateValues01.fs SCFLAGS="-r:TypeLib02.dll" PRECMD="\$FSC_PIPE -a --optimize+ TypeLib02.fs" # InlineWithPrivateValuesRef
    [<Theory; FileInlineData("InlineWithPrivateValues01.fs")>]
    let ``InlineWithPrivateValues01_fs_TypeLib02_fs`` compilation =
        let typeLib02 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "TypeLib02.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> getCompilation
        |> withReferences [typeLib02]
        |> verifyCompileAndExecution

    [<Fact>]
    let ``copyOfStruct doesn't reallocate local in case of cross-assembly inlining`` () =
        let library =
            FSharp """
namespace Library

#nowarn "346"

[<Struct>]
type ID (value : string) =
    member _.Value = value
    member inline this.Hello(other: ID) = System.Console.WriteLine(this.Value + " " + other.Value)

type ID2 = { Value : ID } with
    member inline this.Hello(other: ID2) = this.Value.Hello other.Value
            """
            |> asLibrary
            |> withName "Library"
            |> withOptimize

        let program =
            FSharp """
open Library

[<EntryPoint>]
let main _ =

    let aBar = { Value = ID "first" }
    let bBar = { Value = ID "second" }

    aBar.Hello(bBar)

    0
            """
            |> withReferences [library]
            |> withOptimize

        let compilation =
            program
            |> asExe
            |> compile

        compilation
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> verifyOutputContains [| "first second" |]
        |> verifyIL
            [ """
.method public static int32  main(string[] _arg1) cil managed
{
    .entrypoint
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class [Library]Library.ID2 V_0,
            class [Library]Library.ID2 V_1,
            valuetype [Library]Library.ID& V_2,
            valuetype [Library]Library.ID V_3,
            valuetype [Library]Library.ID V_4)
    IL_0000:  ldstr      "first"
    IL_0005:  newobj     instance void [Library]Library.ID::.ctor(string)
    IL_000a:  newobj     instance void [Library]Library.ID2::.ctor(valuetype [Library]Library.ID)
    IL_000f:  stloc.0
    IL_0010:  ldstr      "second"
    IL_0015:  newobj     instance void [Library]Library.ID::.ctor(string)
    IL_001a:  newobj     instance void [Library]Library.ID2::.ctor(valuetype [Library]Library.ID)
    IL_001f:  stloc.1
    IL_0020:  ldloc.0
    IL_0021:  call       instance valuetype [Library]Library.ID [Library]Library.ID2::get_Value()
    IL_0026:  stloc.3
    IL_0027:  ldloca.s   V_3
    IL_0029:  stloc.2
    IL_002a:  ldloc.1
    IL_002b:  call       instance valuetype [Library]Library.ID [Library]Library.ID2::get_Value()
    IL_0030:  stloc.s    V_4
    IL_0032:  ldloc.2
    IL_0033:  call       instance string [Library]Library.ID::get_Value()
    IL_0038:  ldstr      " "
    IL_003d:  ldloca.s   V_4
    IL_003f:  call       instance string [Library]Library.ID::get_Value()
    IL_0044:  call       string [runtime]System.String::Concat(string,
                                                                    string,
                                                                    string)
    IL_0049:  call       void [runtime]System.Console::WriteLine(string)
    IL_004e:  ldc.i4.0
    IL_004f:  ret
}
              """]