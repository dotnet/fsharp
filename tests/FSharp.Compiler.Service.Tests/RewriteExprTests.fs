module FSharp.Compiler.Service.Tests.RewriteExprTests

open Xunit

open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

/// The non-linear tail of a linear expression got PostTransform twice, the second time on its own output.
[<Fact>]
let ``PostTransform runs once per node on the tail of a linear expression`` () =
    let m = Range.range0
    let ty = mkTyparTy (Construct.NewRigidTypar "a" m)
    let x, xExpr = mkLocal m "x" ty
    let _, fExpr = mkLocal m "f" ty

    // let x = 1 in f x
    let expr =
        mkLet DebugPointAtBinding.NoneAtLet m x (Expr.Const(Const.Int32 1, m, ty)) (Expr.App(fExpr, ty, [], [ xExpr ], m))

    let mutable calls = 0
    let mutable appCalls = 0

    let env =
        {
            PreIntercept = None
            PreInterceptBinding = None
            PostTransform =
                fun e ->
                    calls <- calls + 1

                    match e with
                    | Expr.App _ -> appCalls <- appCalls + 1
                    | _ -> ()

                    None
            RewriteQuotations = false
            StackGuard = StackGuard(nameof RewriteExpr)
        }

    RewriteExpr env expr |> ignore

    // Let, its bound constant, the App, f and x.
    Assert.Equal(1, appCalls)
    Assert.Equal(5, calls)
