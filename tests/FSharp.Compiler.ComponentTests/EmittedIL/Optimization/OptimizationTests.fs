module FSharp.Compiler.ComponentTests.EmittedIL.Optimization.OptimizationTests
open Xunit
open FSharp.Test
open FSharp.Test.Compiler
let [<Literal>] folder = __SOURCE_DIRECTORY__ + "/samples"

[<Theory;
    Directory(
        folder
        , Includes=[|
            "if_then_else_bool_expression.fsx"
            "du_for_same_expression.fsx"
            "lambda_inlining.fsx"
            "for_loop_custom_step.fsx"
            "for_loop_non_int.fsx"
            "option_elision_non_constant_result.fsx"
            "tailcall_last_expression_parens.fsx"
            "inplace_array_update.fsx"
            "static_init_field.fsx"
            "spurious_tail.fsx"
            "use_binding_on_struct_enumerator.fsx"
            "typedefof.fsx"
            "array_of_function_no_alloc.fsx"
        |]
)>]
let ``baselines`` compilation =
    compilation
    |> asFsx
    |> verifyBaselines
    |> compileAndRun
    |> shouldSucceed  
