// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
module Miscellaneous.FsharpSuiteMigrated_OverloadResolution

open Xunit
open FSharp.Test
open Miscellaneous.FsharpSuiteMigrated.TestFrameworkAdapter

module ``fsharpqa migrated tests`` =
    let [<FactForDESKTOP>] ``Conformance\Expressions\SyntacticSugar (E_Slices01_fs)`` () = singleNegTest ( "conformance/expressions/syntacticsugar") "E_Slices01"
    let [<FactForDESKTOP>] ``Conformance\Expressions\Type-relatedExpressions (E_RigidTypeAnnotation03_fsx)`` () = singleNegTest ( "conformance/expressions/type-relatedexpressions") "E_RigidTypeAnnotation03"
    let [<FactForDESKTOP>] ``Conformance\Inference (E_OneTypeVariable03_fs)`` () = singleNegTest ( "conformance/inference") "E_OneTypeVariable03"
    let [<FactForDESKTOP>] ``Conformance\Inference (E_OneTypeVariable03rec_fs)`` () = singleNegTest ( "conformance/inference") "E_OneTypeVariable03rec"
    let [<FactForDESKTOP>] ``Conformance\Inference (E_TwoDifferentTypeVariablesGen00_fs)`` () = singleNegTest ( "conformance/inference") "E_TwoDifferentTypeVariablesGen00"
    let [<FactForDESKTOP>] ``Conformance\Inference (E_TwoDifferentTypeVariables01_fs)`` () = singleNegTest ( "conformance/inference") "E_TwoDifferentTypeVariables01"
    let [<FactForDESKTOP>] ``Conformance\Inference (E_TwoDifferentTypeVariables01rec_fs)`` () = singleNegTest ( "conformance/inference") "E_TwoDifferentTypeVariables01rec"
    let [<FactForDESKTOP>] ``Conformance\Inference (E_TwoDifferentTypeVariablesGen00rec_fs)`` () = singleNegTest ( "conformance/inference") "E_TwoDifferentTypeVariablesGen00rec"
    let [<FactForDESKTOP>] ``Conformance\Inference (E_TwoEqualTypeVariables02_fs)`` () = singleNegTest ( "conformance/inference") "E_TwoEqualTypeVariables02"
    let [<FactForDESKTOP>] ``Conformance\Inference (E_TwoEqualTypeVariables02rec_fs)`` () = singleNegTest ( "conformance/inference") "E_TwoEqualTypeVariables02rec"
    let [<FactForDESKTOP>] ``Conformance\Inference (E_LeftToRightOverloadResolution01_fs)`` () = singleNegTest ( "conformance/inference") "E_LeftToRightOverloadResolution01"
    let [<FactForDESKTOP>] ``Conformance\WellFormedness (E_Clashing_Values_in_AbstractClass01_fs)`` () = singleNegTest ( "conformance/wellformedness") "E_Clashing_Values_in_AbstractClass01"
    let [<FactForDESKTOP>] ``Conformance\WellFormedness (E_Clashing_Values_in_AbstractClass03_fs)`` () = singleNegTest ( "conformance/wellformedness") "E_Clashing_Values_in_AbstractClass03"
    let [<FactForDESKTOP>] ``Conformance\WellFormedness (E_Clashing_Values_in_AbstractClass04_fs)`` () = singleNegTest ( "conformance/wellformedness") "E_Clashing_Values_in_AbstractClass04"
    // note: this test still exist in fsharpqa to assert the compiler doesn't crash
    // the part of the code generating a flaky error due to https://github.com/dotnet/fsharp/issues/6725
    // is elided here to focus on overload resolution error messages
    let [<FactForDESKTOP>] ``Conformance\LexicalAnalysis\SymbolicOperators (E_LessThanDotOpenParen001_fs)`` () = singleNegTest ( "conformance/lexicalanalysis") "E_LessThanDotOpenParen001"

module ``error messages using BCL``=
    let [<FactForDESKTOP>] ``neg_System_Convert_ToString_OverloadList``() = singleNegTest ( "typecheck/overloads") "neg_System.Convert.ToString.OverloadList"
    let [<FactForDESKTOP>] ``neg_System_Threading_Tasks_Task_Run_OverloadList``() = singleNegTest ( "typecheck/overloads") "neg_System.Threading.Tasks.Task.Run.OverloadList"
    let [<FactForDESKTOP>] ``neg_System_Drawing_Graphics_DrawRectangleOverloadList_fsx``() = singleNegTest ( "typecheck/overloads") "neg_System.Drawing.Graphics.DrawRectangleOverloadList"

module ``ad hoc code overload error messages``=
    let [<FactForDESKTOP>] ``neg_many_many_overloads`` () = singleNegTest ( "typecheck/overloads") "neg_many_many_overloads"
    let [<FactForDESKTOP>] ``neg_interface_generics`` () = singleNegTest ( "typecheck/overloads") "neg_interface_generics"
    let [<FactForDESKTOP>] ``neg_known_return_type_and_known_type_arguments`` () = singleNegTest ( "typecheck/overloads") "neg_known_return_type_and_known_type_arguments"
    let [<FactForDESKTOP>] ``neg_generic_known_argument_types`` () = singleNegTest ( "typecheck/overloads") "neg_generic_known_argument_types"
    let [<FactForDESKTOP>] ``neg_tupled_arguments`` () = singleNegTest ( "typecheck/overloads") "neg_tupled_arguments"