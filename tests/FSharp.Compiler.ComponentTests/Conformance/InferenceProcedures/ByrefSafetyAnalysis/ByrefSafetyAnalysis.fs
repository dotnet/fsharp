// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ByrefSafetyAnalysis =

    // SOURCE=MigratedTest01.fs SCFLAGS="--test:ErrorRanges"                              # MigratedTest01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MigratedTest01.fs"|])>]
    let``MigratedTest01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> compileExeAndRun
        |> withDiagnostics [
            (Warning 52, Line 1219, Col 13, Line 1219, Col 25, "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed")
            (Warning 20, Line 1227, Col 9, Line 1227, Col 15, "The result of this expression has type 'TestMut' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 1423, Col 5, Line 1423, Col 10, "The result of this expression has type 'System.Collections.IEnumerator' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Information 3370, Line 10, Col 26, Line 10, Col 28, "The use of ':=' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change 'cell := expr' to 'cell.Value <- expr'.")
            (Information 3370, Line 10, Col 29, Line 10, Col 30, "The use of '!' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change '!cell' to 'cell.Value'.")
        ]
        |> shouldSucceed
    
    // SOURCE=E_Migrated01.fs SCFLAGS="--test:ErrorRanges"                                # E_Migrated01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_MigratedTest01.fs"|])>]
    let``E_Migrated01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3224, Line 9, Col 34, Line 9, Col 40, "The byref pointer is readonly, so this write is not permitted.")
            (Error 39, Line 12, Col 26, Line 12, Col 27, "The type 'S' is not defined.")
            (Error 72, Line 12, Col 32, Line 12, Col 35, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")
            (Error 1, Line 16, Col 36, Line 16, Col 38, "Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
            (Error 1, Line 20, Col 36, Line 20, Col 38, "Type mismatch. Expecting a
    'outref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'")
            (Error 1, Line 25, Col 38, Line 25, Col 40, "Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
            (Error 1, Line 30, Col 38, Line 30, Col 40, "Type mismatch. Expecting a
    'outref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'")
            (Error 1, Line 35, Col 38, Line 35, Col 40, "Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
            (Error 1, Line 40, Col 38, Line 40, Col 40, "Type mismatch. Expecting a
    'outref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'")
            (Error 1204, Line 44, Col 34, Line 44, Col 47, "This construct is for use in the FSharp.Core library and should not be used directly")
            (Error 3236, Line 49, Col 21, Line 49, Col 23, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.")
            (Error 3236, Line 50, Col 21, Line 50, Col 23, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.")
            (Error 3236, Line 56, Col 21, Line 56, Col 37, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.")
            (Error 1, Line 63, Col 22, Line 63, Col 23, "This expression was expected to have type
    'inref<System.DateTime>'    
but here has type
    'System.DateTime'    ")
            (Error 39, Line 64, Col 9, Line 64, Col 14, "The value or constructor 'check' is not defined. Maybe you want one of the following:
   Checked
   Unchecked")
            (Error 3238, Line 66, Col 10, Line 66, Col 15, "Byref types are not allowed to have optional type extensions.")
            (Error 3238, Line 70, Col 10, Line 70, Col 15, "Byref types are not allowed to have optional type extensions.")
            (Error 3238, Line 74, Col 10, Line 74, Col 16, "Byref types are not allowed to have optional type extensions.")
            (Error 3236, Line 92, Col 21, Line 92, Col 36, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.")
            (Error 1, Line 92, Col 21, Line 92, Col 36, "Type mismatch. Expecting a
    'byref<float array>'    
but given a
    'inref<float array>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
            (Error 3236, Line 97, Col 21, Line 97, Col 29, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.")
        ]
    
    // SOURCE=Migrated02.fs SCFLAGS="--test:ErrorRanges"                                  # Migrated02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MigratedTest02.fs"|])>]
    let``MigratedTest02_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> compileExeAndRun
        |> shouldSucceed
        |> withDiagnostics [
            (Information 3370, Line 10, Col 26, Line 10, Col 28, "The use of ':=' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change 'cell := expr' to 'cell.Value <- expr'.")
            (Information 3370, Line 10, Col 29, Line 10, Col 30, "The use of '!' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change '!cell' to 'cell.Value'.")
        ]
        
    // SOURCE=E_Migrated02.fs SCFLAGS="--test:ErrorRanges"                                # E_Migrated02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_MigratedTest02.fs"|])>]
    let``E_Migrated02_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 193, Line 165, Col 9, Line 165, Col 22, "This expression is a function value, i.e. is missing arguments. Its type is byref<int> -> unit.")
            (Warning 193, Line 183, Col 9, Line 183, Col 20, "This expression is a function value, i.e. is missing arguments. Its type is int -> byref<int> -> unit.")
            (Warning 193, Line 198, Col 9, Line 198, Col 24, "This expression is a function value, i.e. is missing arguments. Its type is inref<int> * int -> unit.")
            (Warning 20, Line 206, Col 9, Line 206, Col 18, "The result of this expression has type 'byref<int> * int' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Error 3209, Line 13, Col 18, Line 13, Col 19, "The address of the variable 'y' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 20, Col 18, Line 20, Col 19, "The address of the variable 'z' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 29, Col 14, Line 29, Col 15, "The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 33, Col 14, Line 33, Col 15, "The address of the variable 'y' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 40, Col 14, Line 40, Col 15, "The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 43, Col 14, Line 43, Col 15, "The address of the variable 'y' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 52, Col 18, Line 52, Col 19, "The address of the variable 'z' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 53, Col 10, Line 53, Col 11, "The address of the variable 'y' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3228, Line 63, Col 14, Line 63, Col 29, "The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3228, Line 71, Col 14, Line 71, Col 29, "The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 412, Line 77, Col 13, Line 77, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 77, Col 17, Line 77, Col 29, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 421, Line 77, Col 28, Line 77, Col 29, "The address of the variable 'x' cannot be used at this point")
            (Error 425, Line 77, Col 17, Line 77, Col 29, "The type of a first-class function cannot contain byrefs")
            (Error 3209, Line 96, Col 53, Line 96, Col 54, "The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 108, Col 41, Line 108, Col 42, "The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 438, Line 117, Col 23, Line 117, Col 33, "Duplicate method. The method 'TestMethod' has the same name and signature as another method in type 'NegativeTests.TestNegativeOverloading'.")
            (Error 438, Line 115, Col 23, Line 115, Col 33, "Duplicate method. The method 'TestMethod' has the same name and signature as another method in type 'NegativeTests.TestNegativeOverloading'.")
            (Error 438, Line 113, Col 23, Line 113, Col 33, "Duplicate method. The method 'TestMethod' has the same name and signature as another method in type 'NegativeTests.TestNegativeOverloading'.")
            (Error 412, Line 121, Col 18, Line 121, Col 22, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 123, Col 18, Line 123, Col 23, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3301, Line 125, Col 9, Line 125, Col 14, "The function or method has an invalid return type '(byref<int> * int)'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 125, Col 34, Line 125, Col 39, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 418, Line 125, Col 35, Line 125, Col 36, "The byref typed value 'x' cannot be used at this point")
            (Error 3301, Line 127, Col 9, Line 127, Col 14, "The function or method has an invalid return type '(byref<int> -> unit)'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 129, Col 14, Line 129, Col 15, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 131, Col 17, Line 131, Col 18, "The parameter 'x' has an invalid type '((byref<int> -> unit) * int)'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 133, Col 17, Line 133, Col 18, "The parameter 'x' has an invalid type '(byref<int> -> unit)'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 133, Col 41, Line 133, Col 42, "The parameter 'y' has an invalid type '(byref<int> * int)'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 139, Col 36, Line 139, Col 39, "The parameter 'tup' has an invalid type '(inref<int> * int)'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 140, Col 13, Line 140, Col 33, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 142, Col 37, Line 142, Col 38, "The parameter 'x' has an invalid type '(byref<int> -> unit)'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 144, Col 37, Line 144, Col 38, "The parameter 'x' has an invalid type 'byref<int> option'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 146, Col 17, Line 146, Col 18, "The parameter 'x' has an invalid type 'byref<int> option'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 151, Col 13, Line 151, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 151, Col 17, Line 151, Col 30, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3301, Line 154, Col 9, Line 154, Col 15, "The function or method has an invalid return type '(byref<int> -> unit)'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 155, Col 9, Line 155, Col 22, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 158, Col 13, Line 158, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 160, Col 13, Line 160, Col 26, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 165, Col 9, Line 165, Col 22, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 169, Col 13, Line 169, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 169, Col 17, Line 169, Col 28, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3301, Line 172, Col 9, Line 172, Col 15, "The function or method has an invalid return type '(int -> byref<int> -> unit)'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 173, Col 9, Line 173, Col 20, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 176, Col 13, Line 176, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 178, Col 13, Line 178, Col 24, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 183, Col 9, Line 183, Col 20, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 187, Col 13, Line 187, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 187, Col 17, Line 187, Col 32, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 425, Line 187, Col 17, Line 187, Col 32, "The type of a first-class function cannot contain byrefs")
            (Error 412, Line 191, Col 13, Line 191, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 193, Col 13, Line 193, Col 28, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 198, Col 9, Line 198, Col 24, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3301, Line 201, Col 9, Line 201, Col 15, "The function or method has an invalid return type '(byref<int> * int)'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 203, Col 10, Line 203, Col 15, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 421, Line 203, Col 11, Line 203, Col 12, "The address of the variable 'x' cannot be used at this point")
            (Error 412, Line 206, Col 9, Line 206, Col 18, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 210, Col 13, Line 210, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 210, Col 17, Line 210, Col 26, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]
        
    // SOURCE=Migrated03.fs SCFLAGS="--test:ErrorRanges"                                  # Migrated03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MigratedTest03.fs"|])>]
    let``MigratedTest03_fs`` compilation =
        let csharpLib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "MigratedTest03CSharpLib.cs")
            |> withName "CSharpLib3"
        
        compilation
        |> withReferences [ csharpLib ]
        |> ignoreWarnings
        |> compileExeAndRun
        |> shouldSucceed
        |> withDiagnostics [
            (Information 3370, Line 13, Col 26, Line 13, Col 28, "The use of ':=' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change 'cell := expr' to 'cell.Value <- expr'.")
            (Information 3370, Line 13, Col 29, Line 13, Col 30, "The use of '!' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change '!cell' to 'cell.Value'.")]
        
    // SOURCE=E_Migrated03.fs SCFLAGS="--test:ErrorRanges"                                # E_Migrated03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_MigratedTest03.fs"|])>]
    let``E_Migrated03_fs`` compilation =
        let csharpLib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "MigratedTest03CSharpLib.cs")
            |> withName "CSharpLib3"
        
        compilation
        |> asExe
        |> withReferences [ csharpLib ]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3237, Line 23, Col 18, Line 23, Col 28, "Cannot call the byref extension method 'Test2. The first parameter requires the value to be mutable or a non-readonly byref type.")
            (Error 1, Line 24, Col 9, Line 24, Col 11, "Type mismatch. Expecting a
    'byref<DateTime>'    
but given a
    'inref<DateTime>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
            (Error 3237, Line 28, Col 9, Line 28, Col 20, "Cannot call the byref extension method 'Change. The first parameter requires the value to be mutable or a non-readonly byref type.")
            (Error 3237, Line 33, Col 19, Line 33, Col 30, "Cannot call the byref extension method 'Test2. The first parameter requires the value to be mutable or a non-readonly byref type.")
            (Error 3237, Line 39, Col 9, Line 39, Col 21, "Cannot call the byref extension method 'Change. The first parameter requires the value to be mutable or a non-readonly byref type.")
            (Error 3239, Line 43, Col 17, Line 43, Col 29, "Cannot partially apply the extension method 'NotChange' because the first parameter is a byref type.")
            (Error 3239, Line 44, Col 17, Line 44, Col 24, "Cannot partially apply the extension method 'Test' because the first parameter is a byref type.")
            (Error 3239, Line 45, Col 17, Line 45, Col 26, "Cannot partially apply the extension method 'Change' because the first parameter is a byref type.")
            (Error 3239, Line 46, Col 17, Line 46, Col 25, "Cannot partially apply the extension method 'Test2' because the first parameter is a byref type.")
        ]
    
    // SOURCE=E_ByrefAsArrayElement.fs SCFLAGS="--test:ErrorRanges"                       # E_ByrefAsArrayElement.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ByrefAsArrayElement.fs"|])>]
    let``E_ByrefAsArrayElement_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3300, Line 5, Col 18, Line 5, Col 19, "The parameter 'f' has an invalid type '(byref<int> -> 'a)'. This is not permitted by the rules of Common IL.")
            (Error 424, Line 7, Col 6, Line 7, Col 13, "The address of an array element cannot be used at this point")
            (Error 412, Line 9, Col 19, Line 9, Col 20, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 11, Col 19, Line 11, Col 20, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_ByrefAsGenericArgument01.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefAsGenericArgument01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ByrefAsGenericArgument01.fs"|])>]
    let``E_ByrefAsGenericArgument01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 431, Line 9, Col 5, Line 9, Col 9, "A byref typed value would be stored here. Top-level let-bound byref values are not permitted.")
            (Error 412, Line 9, Col 5, Line 9, Col 9, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 9, Col 30, Line 9, Col 32, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // NoMT SOURCE=ByrefInFSI1.fsx FSIMODE=PIPE COMPILE_ONLY=1                            # ByrefInFSI1.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ByrefInFSI1.fsx"|])>]
    let``ByrefInFSI1_fsx`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=E_ByrefUsedInInnerLambda01.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefUsedInInnerLambda01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ByrefUsedInInnerLambda01.fs"|])>]
    let``E_ByrefUsedInInnerLambda01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 12, Col 34, Line 12, Col 48, "The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]

    // SOURCE=E_ByrefUsedInInnerLambda02.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefUsedInInnerLambda02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ByrefUsedInInnerLambda02.fs"|])>]
    let``E_ByrefUsedInInnerLambda02_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 11, Col 24, Line 11, Col 55, "The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]

    // SOURCE=E_ByrefUsedInInnerLambda03.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefUsedInInnerLambda03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ByrefUsedInInnerLambda03.fs"|])>]
    let``E_ByrefUsedInInnerLambda03_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 11, Col 24, Line 11, Col 60, "The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]

    // SOURCE=E_SetFieldToByref01.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SetFieldToByref01.fs"|])>]
    let``E_SetFieldToByref01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 412, Line 11, Col 17, Line 11, Col 27, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 437, Line 10, Col 6, Line 10, Col 9, "A type would store a byref typed value. This is not permitted by Common IL.")
            (Error 412, Line 11, Col 50, Line 11, Col 54, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_SetFieldToByref02.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SetFieldToByref02.fs"|])>]
    let``E_SetFieldToByref02_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 431, Line 8, Col 9, Line 8, Col 17, "A byref typed value would be stored here. Top-level let-bound byref values are not permitted.")
        ]
    // SOURCE=E_SetFieldToByref03.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SetFieldToByref03.fs"|])>]
    let``E_SetFieldToByref03_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1178, Line 8, Col 6, Line 8, Col 21, "The struct, record or union type 'RecordWithByref' is not structurally comparable because the type 'byref<int>' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'RecordWithByref' to clarify that the type is not comparable")
            (Error 412, Line 8, Col 25, Line 8, Col 26, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 437, Line 8, Col 6, Line 8, Col 21, "A type would store a byref typed value. This is not permitted by Common IL.")
        ]

    // SOURCE=E_SetFieldToByref04.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SetFieldToByref04.fs"|])>]
    let``E_SetFieldToByref04_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 412, Line 14, Col 28, Line 14, Col 37, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 421, Line 14, Col 29, Line 14, Col 30, "The address of the variable 'x' cannot be used at this point")
            (Error 412, Line 19, Col 20, Line 19, Col 53, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_SetFieldToByref05.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SetFieldToByref05.fs"|])>]
    let``E_SetFieldToByref05_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1178, Line 8, Col 6, Line 8, Col 17, "The struct, record or union type 'DUWithByref' is not structurally comparable because the type 'byref<int>' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'DUWithByref' to clarify that the type is not comparable")
            (Error 412, Line 9, Col 18, Line 9, Col 28, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 437, Line 8, Col 6, Line 8, Col 17, "A type would store a byref typed value. This is not permitted by Common IL.")
            (Error 412, Line 9, Col 7, Line 9, Col 8, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_FirstClassFuncTakesByref.fs SCFLAGS="--test:ErrorRanges --flaterrors"     # E_FirstClassFuncTakesByref.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_FirstClassFuncTakesByref.fs"|])>]
    let``E_FirstClassFuncTakesByref_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 8, Col 12, Line 8, Col 16, "This expression was expected to have type\n    'byref<'a>'    \nbut here has type\n    'int ref'    ")
        ]

    // SOURCE=E_StaticallyResolvedByRef01.fs SCFLAGS="--test:ErrorRanges"                 # E_StaticallyResolvedByRef01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_StaticallyResolvedByRef01.fs"|])>]
    let``E_StaticallyResolvedByRef01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
#if NETCOREAPP
            (Error 43, Line 11, Col 11, Line 11, Col 12, "The member or object constructor 'TryParse' does not take 1 argument(s). An overload was found taking 4 arguments.")
#else
            (Error 43, Line 11, Col 11, Line 11, Col 12, "The member or object constructor 'TryParse' does not take 1 argument(s). An overload was found taking 2 arguments.")
#endif
        ]

    // SOURCE=UseByrefInLambda01.fs                                                       # UseByrefInLambda01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseByrefInLambda01.fs"|])>]
    let``UseByrefInLambda01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed
        
#if NET7_0_OR_GREATER
    // SOURCE=ReturnFieldSetBySpan.fs                                                     # ReturnFieldSetBySpan.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ReturnFieldSetBySpan.fs"|])>]
    let``ReturnFieldSetBySpan_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed
        
    // SOURCE=ReturnSpan02.fs                                                             # ReturnSpan02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ReturnSpan01.fs"|])>]
    let``ReturnSpan01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed
#endif

    // // SOURCE=E_ByrefFieldEscapingLocalScope01.fs SCFLAGS="--test:ErrorRanges"                 # E_ByrefFieldEscapingLocalScope01.fs
    // [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ByrefFieldEscapingLocalScope01.fs"|])>]
    // let``E_ByrefEscapingLocalScope01_fs`` compilation =
    //     compilation
    //     |> asExe
    //     |> compile
    //     |> shouldFail
    //     |> withDiagnostics [
    //         (Error 3234, Line 7, Col 5, Line 7, Col 9, "The Span or IsByRefLike variable 'span' cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
    //     ]
    //      
    // // SOURCE=E_ByrefFieldEscapingLocalScope02.fs SCFLAGS="--test:ErrorRanges"                 # E_ByrefFieldEscapingLocalScope02.f
    // [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ByrefFieldEscapingLocalScope02.fs"|])>]
    // let``E_ByrefEscapingLocalScope02_fs`` compilation =
    //     compilation
    //     |> asExe
    //     |> compile
    //     |> shouldFail
    //     |> withDiagnostics [
    //         (Error 3234, Line 8, Col 9, Line 8, Col 13, "The Span or IsByRefLike variable 'span' cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
    //     ]
        
    // SOURCE=E_TopLevelByref.fs SCFLAGS="--test:ErrorRanges"                             # E_TopLevelByref.f
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_TopLevelByref.fs"|])>]
    let``E_TopLevelByref_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 431, Line 6, Col 5, Line 6, Col 13, "A byref typed value would be stored here. Top-level let-bound byref values are not permitted.")
        ]
        
    // SOURCE=E_SpanUsedInInnerLambda01.fs SCFLAGS="--test:ErrorRanges"                   # E_SpanUsedInInnerLambda01.f
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SpanUsedInInnerLambda01.fs"|])>]
    let``E_SpanUsedInInnerLambda01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 8, Col 34, Line 8, Col 45, "The byref-typed variable 'span' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]
        
    // SOURCE=E_SpanUsedInInnerLambda02.fs SCFLAGS="--test:ErrorRanges"                   # E_SpanUsedInInnerLambda02.f
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SpanUsedInInnerLambda02.fs"|])>]
    let``E_SpanUsedInInnerLambda02_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 8, Col 34, Line 8, Col 45, "The byref-typed variable 'span' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]
