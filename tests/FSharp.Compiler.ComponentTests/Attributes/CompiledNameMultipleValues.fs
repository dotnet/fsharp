namespace FSharp.Compiler.ComponentTests.Attributes

open Xunit
open FSharp.Test.Compiler

module CompiledNameMultipleValues =

    [<Theory>]
    [<InlineData("module M\n[<CompiledName(\"X\")>]\nlet a, b = 1, 2")>]
    [<InlineData("module M\n[<CompiledName(\"X\")>]\nlet (a, b) = 1, 2")>]
    [<InlineData("module M\n[<CompiledName(\"X\")>]\nlet ((a, b), c) = (1, 2), 3")>]
    [<InlineData("module M\ntype R = { F: int; G: int }\n[<CompiledName(\"X\")>]\nlet { F = a; G = b } = { F = 1; G = 2 }")>]
    [<InlineData("module M\ntype T = Pair of int * int\n[<CompiledName(\"X\")>]\nlet (Pair(a, b)) = Pair(1, 2)")>]
    [<InlineData("module M\n[<CompiledName(\"X\")>]\nlet (Some (a, b)) = Some (1, 2)")>]
    [<InlineData("module M\n[<CompiledName(\"X\")>]\nlet [| a; b |] = [| 1; 2 |]")>]
    [<InlineData("module M\n[<CompiledName(\"X\")>]\nlet a :: b = [1; 2]")>]
    [<InlineData("module M\n[<CompiledName(\"X\")>]\nlet (x as y) = 1")>]
    let ``CompiledName on multi-value let-binding produces FS0755`` (source: string) =
        FSharp source
        |> typecheck
        |> shouldFail
        |> withErrorCode 755
        |> ignore

    [<Fact>]
    let ``CompiledName on tuple binding fires FS0755 exactly once`` () =
        let source = "module M\n[<CompiledName(\"X\")>]\nlet a, b, c = 1, 2, 3"
        let result =
            FSharp source
            |> typecheck
            |> shouldFail

        let diag755Count =
            result.Output.Diagnostics
            |> List.filter (fun d -> d.Error = Error 755)
            |> List.length
        Assert.Equal(1, diag755Count)

    [<Fact>]
    let ``CompiledName on single-value let-binding still compiles`` () =
        FSharp "module M\n[<CompiledName(\"X\")>]\nlet a = 1"
        |> compile
        |> shouldSucceed
        |> withWarnings []
        |> ignore

    [<Theory>]
    [<InlineData("module M\nlet a, b = 1, 2")>]
    [<InlineData("module M\nlet (a, b) = 1, 2")>]
    [<InlineData("module M\ntype R = { F: int; G: int }\nlet { F = a; G = b } = { F = 1; G = 2 }")>]
    let ``Multi-value let-binding without CompiledName still compiles`` (source: string) =
        FSharp source
        |> compile
        |> shouldSucceed
        |> withWarnings []
        |> ignore

    [<Theory>]
    [<InlineData("module M\n[<CompiledName(\"X\")>]\nlet (Some x) = Some 1")>]
    [<InlineData("module M\ntype Box = Box of int\n[<CompiledName(\"X\")>]\nlet (Box x) = Box 1")>]
    [<InlineData("module M\n[<CompiledName(\"X\")>]\nlet a, _ = 1, 2")>]
    [<InlineData("module M\ntype R = { F: int; G: int }\n[<CompiledName(\"X\")>]\nlet { F = a } = { F = 1; G = 2 }")>]
    [<InlineData("module M\n[<CompiledName(\"X\")>]\nlet [| a |] = [| 1 |]")>]
    [<InlineData("module M\n[<CompiledName(\"X\")>]\nlet a :: _ = [1; 2]")>]
    [<InlineData("module M\n[<CompiledName(\"X\")>]\nlet (|Even|Odd|) n = if n % 2 = 0 then Even else Odd")>]
    let ``CompiledName on single-value destructure does not fire FS0755`` (source: string) =
        let result = FSharp source |> typecheck
        let diags =
            match result with
            | CompilationResult.Success r -> r.Diagnostics
            | CompilationResult.Failure r -> r.Diagnostics
        let fs755 = diags |> List.filter (fun d -> d.Error = Error 755)
        Assert.Empty(fs755)
