// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ByrefSafetyAnalysis =
    let withPrelude =
        withReferences [
            FsFromPath (__SOURCE_DIRECTORY__ ++ "Prelude.fs")
            |> withName "Prelude"
        ]
    
    let verifyCompile compilation =
        compilation
        |> asExe
        |> withNoWarn 3370
        |> withOptions ["--test:ErrorRanges"]
        |> withPrelude
        |> compile
    
    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withNoWarn 3370
        |> withOptions ["--test:ErrorRanges"]
        |> withPrelude
        |> compileAndRun
    
    // SOURCE=Migrated02.fs SCFLAGS="--test:ErrorRanges"                                  # Migrated02.fs
    [<Theory; FileInlineData("MigratedTest02.fs")>]
    let``MigratedTest02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun |> shouldSucceed

    // SOURCE=E_Migrated02.fs SCFLAGS="--test:ErrorRanges"                                # E_Migrated02.fs
    [<Theory; FileInlineData("E_MigratedTest02.fs")>]
    let``E_Migrated02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
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
            (Error 3301, Line 125, Col 9, Line 125, Col 14, "The function or method has an invalid return type 'byref<int> * int'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 125, Col 34, Line 125, Col 39, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 418, Line 125, Col 35, Line 125, Col 36, "The byref typed value 'x' cannot be used at this point")
            (Error 3301, Line 127, Col 9, Line 127, Col 14, "The function or method has an invalid return type 'byref<int> -> unit'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 129, Col 14, Line 129, Col 15, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 131, Col 17, Line 131, Col 18, "The parameter 'x' has an invalid type '(byref<int> -> unit) * int'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 133, Col 17, Line 133, Col 18, "The parameter 'x' has an invalid type 'byref<int> -> unit'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 133, Col 41, Line 133, Col 42, "The parameter 'y' has an invalid type 'byref<int> * int'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 139, Col 36, Line 139, Col 39, "The parameter 'tup' has an invalid type 'inref<int> * int'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 140, Col 13, Line 140, Col 33, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 142, Col 37, Line 142, Col 38, "The parameter 'x' has an invalid type 'byref<int> -> unit'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 144, Col 37, Line 144, Col 38, "The parameter 'x' has an invalid type 'byref<int> option'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 146, Col 17, Line 146, Col 18, "The parameter 'x' has an invalid type 'byref<int> option'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 151, Col 13, Line 151, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 151, Col 17, Line 151, Col 30, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3301, Line 154, Col 9, Line 154, Col 15, "The function or method has an invalid return type 'byref<int> -> unit'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 155, Col 9, Line 155, Col 22, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 158, Col 13, Line 158, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 160, Col 13, Line 160, Col 26, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 165, Col 9, Line 165, Col 22, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 169, Col 13, Line 169, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 169, Col 17, Line 169, Col 28, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3301, Line 172, Col 9, Line 172, Col 15, "The function or method has an invalid return type 'int -> byref<int> -> unit'. This is not permitted by the rules of Common IL.")
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
            (Error 3301, Line 201, Col 9, Line 201, Col 15, "The function or method has an invalid return type 'byref<int> * int'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 203, Col 10, Line 203, Col 15, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 421, Line 203, Col 11, Line 203, Col 12, "The address of the variable 'x' cannot be used at this point")
            (Error 412, Line 206, Col 9, Line 206, Col 18, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 210, Col 13, Line 210, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 210, Col 17, Line 210, Col 26, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]
        
    // SOURCE=Migrated03.fs SCFLAGS="--test:ErrorRanges"                                  # Migrated03.fs
    [<Theory; FileInlineData("MigratedTest03.fs")>]
    let``MigratedTest03_fs`` compilation =
        let csharpLib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "MigratedTest03CSharpLib.cs")
            |> withName "CSharpLib3"
        
        compilation
        |> getCompilation
        |> withReferences [ csharpLib ]
        |> withOptions ["--nowarn:3370"]
        |> compileExeAndRun
        |> shouldSucceed
        
    // SOURCE=E_Migrated03.fs SCFLAGS="--test:ErrorRanges"                                # E_Migrated03.fs
    [<Theory; FileInlineData("E_MigratedTest03.fs")>]
    let``E_Migrated03_fs`` compilation =
        let csharpLib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "MigratedTest03CSharpLib.cs")
            |> withName "CSharpLib3"
        
        compilation
        |> getCompilation
        |> asExe
        |> withReferences [ csharpLib ]
        |> withNoWarn 52
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3237, Line 23, Col 18, Line 23, Col 28, "Cannot call the byref extension method 'Test2. 'this' parameter requires the value to be mutable or a non-readonly byref type.")
            (Error 1, Line 24, Col 9, Line 24, Col 11, "Type mismatch. Expecting a
    'byref<DateTime>'    
but given a
    'inref<DateTime>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
            (Error 3237, Line 28, Col 9, Line 28, Col 20, "Cannot call the byref extension method 'Change. 'this' parameter requires the value to be mutable or a non-readonly byref type.")
            (Error 3237, Line 33, Col 19, Line 33, Col 30, "Cannot call the byref extension method 'Test2. 'this' parameter requires the value to be mutable or a non-readonly byref type.")
            (Error 3237, Line 39, Col 9, Line 39, Col 21, "Cannot call the byref extension method 'Change. 'this' parameter requires the value to be mutable or a non-readonly byref type.")
            (Error 3239, Line 43, Col 17, Line 43, Col 29, "Cannot partially apply the extension method 'NotChange' because the first parameter is a byref type.")
            (Error 3239, Line 44, Col 17, Line 44, Col 24, "Cannot partially apply the extension method 'Test' because the first parameter is a byref type.")
            (Error 3239, Line 45, Col 17, Line 45, Col 26, "Cannot partially apply the extension method 'Change' because the first parameter is a byref type.")
            (Error 3239, Line 46, Col 17, Line 46, Col 25, "Cannot partially apply the extension method 'Test2' because the first parameter is a byref type.")
        ]
    
    [<Theory; FileInlineData("TryGetValue.fs")>]
    let``TryGetValue_fs`` compilation =
        compilation
        |> getCompilation
        |> withPrelude |> compileExeAndRun |> shouldSucceed
    
    [<Theory; FileInlineData("CompareExchange.fs")>]
    let``CompareExchange_fs`` compilation =
        compilation
        |> getCompilation
        |> withPrelude
        |> withOptions ["--nowarn:3370"]
        |> compileExeAndRun
        |> shouldSucceed
        
    [<Theory; FileInlineData("ByRefParam.fs")>]
    let``ByRefParam_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun |> shouldSucceed

    [<Theory; FileInlineData("ByRefParam_ExplicitOutAttribute.fs")>]
    let``ByRefParam_ExplicitOutAttribute_fs`` compilation =
        compilation
        |> getCompilation
        |> withPrelude
        |> withOptions ["--nowarn:3370"] |> compileExeAndRun |> shouldSucceed

    [<Theory; FileInlineData("ByRefParam_ExplicitInAttribute.fs")>]
    let``ByRefParam_ExplicitInAttribute_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun |> shouldSucceed

    [<Theory; FileInlineData("ByRefReturn.fs")>]
    let``ByRefReturn_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("Slot_ByRefReturn.fs")>]
    let``Slot_ByRefReturn_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefReturn.fs")>]
    let``InRefReturn_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("Slot_InRefReturn.fs")>]
    let``Slot_InRefReturn_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("OutRefParam.fs")>]
    let``OutRefParam_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("OutRefParam_ExplicitOutAttribute.fs")>]
    let``OutRefParam_ExplicitOutAttribute_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("Slot_OutRefParam.fs")>]
    let``Slot_OutRefParam_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("ByRefParam_OverloadedTest_ExplicitOutAttribute.fs")>]
    let``ByRefParam_OverloadedTest_ExplicitOutAttribute_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("OutRefParam_Overloaded_ExplicitOutAttribute.fs")>]
    let``OutRefParam_Overloaded_ExplicitOutAttribute_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("OutRefParam_Overloaded.fs")>]
    let``OutRefParam_Overloaded_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParam_ExplicitInAttribute.fs")>]
    let``InRefParam_ExplicitInAttribute_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParam_ExplicitInAttributeDateTime.fs")>]
    let``InRefParam_ExplicitInAttributeDateTime_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParam.fs")>]
    let``InRefParam`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParamOverload_ExplicitAddressOfAtCallSite.fs")>]
    let``InRefParamOverload_ExplicitAddressOfAtCallSite`` compilation =
        compilation
        |> getCompilation
        |> withNoWarn 52
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParamOverload_ImplicitAddressOfAtCallSite.fs")>]
    let``InRefParamOverload_ImplicitAddressOfAtCallSite`` compilation =
        compilation
        |> getCompilation
        |> withNoWarn 52
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParamOverload_ImplicitAddressOfAtCallSite2.fs")>]
    let``InRefParamOverload_ImplicitAddressOfAtCallSite2`` compilation =
        compilation
        |> getCompilation
        |> withNoWarn 52
        |> verifyCompileAndRun
        |> shouldSucceed

    // TODO: Delete this, move into feature branch, or finish this. See: https://github.com/dotnet/fsharp/pull/7989#discussion_r369841104
#if IMPLICIT_ADDRESS_OF
    module FeatureImplicitAddressOf =
        [<Theory; Directory(__SOURCE_DIRECTORY__ + "/ImplicitAddressOf", Includes=[|"InRefParam_DateTime.fs"|])>]
        let``InRefParam_DateTime`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
            
        [<Theory; Directory(__SOURCE_DIRECTORY__ + "/ImplicitAddressOf", Includes=[|"InRefParam_DateTime_ImplicitAddressOfAtCallSite.fs"|])>]
        let``InRefParam_DateTime_ImplicitAddressOfAtCallSite`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
            
        [<Theory; Directory(__SOURCE_DIRECTORY__ + "/ImplicitAddressOf", Includes=[|"InRefParam_DateTime_ImplicitAddressOfAtCallSite2.fs"|])>]
        let``InRefParam_DateTime_ImplicitAddressOfAtCallSite2`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
            
            
        [<Theory; Directory(__SOURCE_DIRECTORY__ + "/ImplicitAddressOf", Includes=[|"InRefParam_DateTime_ImplicitAddressOfAtCallSite3.fs"|])>]
        let``InRefParam_DateTime_ImplicitAddressOfAtCallSite3`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
            
        [<Theory; Directory(__SOURCE_DIRECTORY__ + "/ImplicitAddressOf", Includes=[|"InRefParam_DateTime_ImplicitAddressOfAtCallSite4.fs"|])>]
        let``InRefParam_DateTime_ImplicitAddressOfAtCallSite4`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
#endif
    
    [<Theory; FileInlineData("InRefParam_Generic_ExplicitAddressOfAttCallSite1.fs")>]
    let``InRefParam_Generic_ExplicitAddressOfAttCallSite1`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParam_Generic_ExplicitAddressOfAttCallSite2.fs")>]
    let``InRefParam_Generic_ExplicitAddressOfAttCallSite2`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    module ByrefReturn =
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestImmediateReturn.fs"|])>]
        let``TestImmediateReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
            
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestMatchReturn.fs"|])>]
        let``TestMatchReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestConditionalReturn.fs"|])>]
        let``TestConditionalReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestTryWithReturn.fs"|])>]
        let``TestTryWithReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestTryFinallyReturn.fs"|])>]
        let``TestTryFinallyReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestOneArgument.fs"|])>]
        let``TestOneArgument`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestTwoArguments.fs"|])>]
        let``TestTwoArguments`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestRecordParam.fs"|])>]
        let``TestRecordParam`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
            
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestRecordParam2.fs"|])>]
        let``TestRecordParam2`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestClassParamMutableField.fs"|])>]
        let``TestClassParamMutableField`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestArrayParam.fs"|])>]
        let``TestArrayParam`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestStructParam.fs"|])>]
        let``TestStructParam`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestInterfaceMethod.fs"|])>]
        let``TestInterfaceMethod`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestInterfaceProperty.fs"|])>]
        let``TestInterfaceProperty`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestDelegateMethod.fs"|])>]
        let``TestDelegateMethod`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestBaseCall.fs"|])>]
        let``TestBaseCall`` compilation =
            compilation |> withNoWarn 988 |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestDelegateMethod2.fs"|])>]
        let``TestDelegateMethod2`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
    module ByrefReturnMember =
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestImmediateReturn.fs"|])>]
        let``TestImmediateReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestMatchReturn.fs"|])>]
        let``TestMatchReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestConditionalReturn.fs"|])>]
        let``TestConditionalReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestTryWithReturn.fs"|])>]
        let``TestTryWithReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestOneArgument.fs"|])>]
        let``TestOneArgument`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestOneArgumentInRefReturned.fs"|])>]
        let``TestOneArgumentInRefReturned`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestOneArgumentOutRef.fs"|])>]
        let``TestOneArgumentOutRef`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestTwoArguments.fs"|])>]
        let``TestTwoArguments`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestRecordParam.fs"|])>]
        let``TestRecordParam`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestRecordParam2.fs"|])>]
        let``TestRecordParam2`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestClassParamMutableField.fs"|])>]
        let``TestClassParamMutableField`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestArrayParam.fs"|])>]
        let``TestArrayParam`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestArrayParam.fs"|])>]
        let``TestStructParam`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestInterfaceMethod.fs"|])>]
        let``TestInterfaceMethod`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestInterfaceProperty.fs"|])>]
        let``TestInterfaceProperty`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestDelegateMethod.fs"|])>]
        let``TestDelegateMethod`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestBaseCall.fs"|])>]
        let``TestBaseCall`` compilation =
            compilation |> withNoWarn 988 |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestDelegateMethod2.fs"|])>]
        let``TestDelegateMethod2`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"ByRefExtensionMethods1.fs"|])>]
        let``ByRefExtensionMethods1`` compilation =
            compilation |> withNoWarn 52 |> verifyCompileAndRun |> shouldSucceed
        
        // [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"ByRefExtensionMethodsOverloading.fs"|])>]
        // let``ByRefExtensionMethodsOverloading`` compilation =
        //     compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestReadOnlyAddressOfStaticField.fs"|])>]
        let``TestReadOnlyAddressOfStaticField`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestAssignToReturnByref.fs"|])>]
        let``TestAssignToReturnByref`` compilation =
            compilation |> withNoWarn 52 |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestAssignToReturnByref2.fs"|])>]
        let``TestAssignToReturnByref2`` compilation =
            compilation |> withNoWarn 52 |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"BaseCallByref.fs"|])>]
        let``BaseCallByref`` compilation =
            compilation |> withNoWarn 988 |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"Bug820.fs"|])>]
        let``Bug820`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"Bug820b.fs"|])>]
        let``Bug820b`` compilation =
            compilation |> withNoWarn 988 |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestNameModuleGeneric.fs"|])>]
        let``TestNameModuleGeneric`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestNameModuleNonGeneric.fs"|])>]
        let``TestNameModuleNonGeneric`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestNameModuleNonGenericSubsume.fs"|])>]
        let``TestNameModuleNonGenericSubsume`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"GenericTestNameRecursive.fs"|])>]
        let``GenericTestNameRecursive`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"NonGenericTestNameRecursiveInClass.fs"|])>]
        let``NonGenericTestNameRecursiveInClass`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"NonGenericTestNameRecursiveInClassSubsume.fs"|])>]
        let``NonGenericTestNameRecursiveInClassSubsume`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"StaticGenericTestNameRecursiveInClass.fs"|])>]
        let``StaticGenericTestNameRecursiveInClass`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"StaticNonGenericTestNameRecursiveInClass.fs"|])>]
        let``StaticNonGenericTestNameRecursiveInClass`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestInRefMutation.fs"|])>]
        let``TestInRefMutation`` compilation =
            compilation |> withNoWarn 52 |> withNoWarn 20 |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"MutateInRef3.fs"|])>]
        let``MutateInRef3`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"MatrixOfTests.fs"|])>]
        let``MatrixOfTests`` compilation =
            compilation |> withNoWarn 988 |> verifyCompileAndRun |> shouldSucceed
        
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestStructRecord.fs"|])>]
        let``TestStructRecord`` compilation =
            compilation |> withNoWarn 988 |> withNoWarn 3560 |> verifyCompileAndRun |> shouldSucceed
    
    [<Theory; FileInlineData("NoTailcallToByrefsWithModReq.fs")>]
    let``NoTailcallToByrefsWithModReq`` compilation =
        compilation
        |> getCompilation
        |> withNoWarn 20
        |> verifyCompileAndRun
        |> shouldSucceed
    
    [<Theory; FileInlineData("E_CantTakeAddressOfExpressionReturningReferenceType.fs")>]
    let``E_CantTakeAddressOfExpressionReturningReferenceType`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3236, Line 15, Col 17, Line 15, Col 32, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.")
            (Error 1, Line 15, Col 17, Line 15, Col 32, "Type mismatch. Expecting a
    'byref<float array>'    
but given a
    'inref<float array>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
            (Error 3236, Line 20, Col 17, Line 20, Col 25, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.")
        ]
    
    
    
    [<Fact>]
    let ``E_WriteToInRef`` () =
        FSharp """let f1 (x: inref<int>) = x <- 1"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3224, Line 1, Col 26, Line 1, Col 32, "The byref pointer is readonly, so this write is not permitted.")
        ]
    
    [<Fact>]
    let ``E_WriteToInRefStructInner`` () =
        FSharp """let f1 (x: inref<S>) = x.X <- 1"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 1, Col 18, Line 1, Col 19, "The type 'S' is not defined.")
            (Error 72, Line 1, Col 24, Line 1, Col 27, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")
        ]
    
    [<Fact>]
    let ``E_InRefToByRef`` () =
        FSharp """
let f1 (x: byref<'T>) = 1
let f2 (x: inref<'T>) = f1 &x    // not allowed 
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 3, Col 28, Line 3, Col 30, "Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
        ]
    
    [<Fact>]
    let ``E_InRefToOutRef`` () =
        FSharp """
let f1 (x: outref<'T>) = 1
let f2 (x: inref<'T>) = f1 &x     // not allowed
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 3, Col 28, Line 3, Col 30, "Type mismatch. Expecting a
    'outref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'")
        ]
    
    [<Fact>]
    let ``E_InRefToByRefClassMethod`` () =
        FSharp """
type C() = 
    static member f1 (x: byref<'T>) = 1
let f2 (x: inref<'T>) = C.f1 &x // not allowed
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 4, Col 30, Line 4, Col 32, "Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
        ]
    
    [<Fact>]
    let ``E_InRefToOutRefClassMethod`` () =
        FSharp """
type C() = 
    static member f1 (x: byref<'T>) = 1
let f2 (x: inref<'T>) = C.f1 &x // not allowed
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 4, Col 30, Line 4, Col 32, "Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
        ]
    
    [<Fact>]
    let ``E_InRefToByRefClassMethod2`` () =
        FSharp """
type C() = 
    static member f1 (x: byref<'T>) = 1
let f2 (x: inref<'T>) = C.f1(&x) // not allowed
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 4, Col 30, Line 4, Col 32, "Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
        ]
    
    [<Fact>]
    let ``E_InRefToOutRefClassMethod2`` () =
        FSharp """
type C() = 
    static member f1 (x: outref<'T>) = 1 // not allowed (not yet)
let f2 (x: inref<'T>) = C.f1(&x) // not allowed
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 4, Col 30, Line 4, Col 32, "Type mismatch. Expecting a
    'outref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'")
        ]
    
    [<Fact>]
    let ``E_UseOfLibraryOnly`` () =
        FSharp """
type C() = 
    static member f1 (x: byref<'T, 'U>) = 1
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1204, Line 3, Col 26, Line 3, Col 39, "This construct is for use in the FSharp.Core library and should not be used directly")
        ]
    
    [<Fact>]
    let ``E_CantTakeAddress`` () =
        FSharp """
let test1 () =
    let x = &1 // not allowed
    let y = &2 // not allowed
    x + y

let test2_helper (x: byref<int>) = x
let test2 () =
    let mutable x = 1
    let y = &test2_helper &x // not allowed
    ()
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3236, Line 3, Col 13, Line 3, Col 15, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.");
            (Error 3236, Line 4, Col 13, Line 4, Col 15, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.");
            (Error 3236, Line 10, Col 13, Line 10, Col 29, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.")
        ]
    
    [<Fact>]
    let ``E_InRefParam_DateTime`` () =
        FSharp """
type C() = 
    static member M(x: inref<System.DateTime>) = x
let w = System.DateTime.Now
let v =  C.M(w) // not allowed
check "cweweoiwe51btw" v w
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 5, Col 14, Line 5, Col 15, "This expression was expected to have type
    'inref<System.DateTime>'    
but here has type
    'System.DateTime'    ");
            (Error 39, Line 6, Col 1, Line 6, Col 6, "The value or constructor 'check' is not defined. Maybe you want one of the following:
   Checked
   Unchecked")
        ]
    
    [<Fact>]
    let ``E_ExtensionMethodOnByrefType`` () =
        FSharp """
type byref<'T> with
    member this.Test() = 1

type inref<'T> with
    member this.Test() = 1

type outref<'T> with
    member this.Test() = 1
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3238, Line 2, Col 6, Line 2, Col 11, "Byref types are not allowed to have optional type extensions.")
            (Error 3238, Line 5, Col 6, Line 5, Col 11, "Byref types are not allowed to have optional type extensions.")
            (Error 3238, Line 8, Col 6, Line 8, Col 12, "Byref types are not allowed to have optional type extensions.")
        ]
    
    
    // SOURCE=E_ByrefAsArrayElement.fs SCFLAGS="--test:ErrorRanges"                       # E_ByrefAsArrayElement.fs
    [<Theory; FileInlineData("E_ByrefAsArrayElement.fs")>]
    let``E_ByrefAsArrayElement_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3300, Line 5, Col 18, Line 5, Col 19, "The parameter 'f' has an invalid type 'byref<int> -> 'a'. This is not permitted by the rules of Common IL.")
            (Error 424, Line 7, Col 6, Line 7, Col 13, "The address of an array element cannot be used at this point")
            (Error 412, Line 9, Col 19, Line 9, Col 20, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 11, Col 19, Line 11, Col 20, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_ByrefAsGenericArgument01.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefAsGenericArgument01.fs
    [<Theory; FileInlineData("E_ByrefAsGenericArgument01.fs")>]
    let``E_ByrefAsGenericArgument01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 431, Line 9, Col 5, Line 9, Col 9, "A byref typed value would be stored here. Top-level let-bound byref values are not permitted.")
            (Error 412, Line 9, Col 5, Line 9, Col 9, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 9, Col 30, Line 9, Col 32, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // NoMT SOURCE=ByrefInFSI1.fsx FSIMODE=PIPE COMPILE_ONLY=1                            # ByrefInFSI1.fsx
    [<Theory; FileInlineData("ByrefInFSI1.fsx")>]
    let``ByrefInFSI1_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=E_ByrefUsedInInnerLambda01.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefUsedInInnerLambda01.fs
    [<Theory; FileInlineData("E_ByrefUsedInInnerLambda01.fs")>]
    let``E_ByrefUsedInInnerLambda01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 12, Col 34, Line 12, Col 48, "The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]

    // SOURCE=E_ByrefUsedInInnerLambda02.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefUsedInInnerLambda02.fs
    [<Theory; FileInlineData("E_ByrefUsedInInnerLambda02.fs")>]
    let``E_ByrefUsedInInnerLambda02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 11, Col 24, Line 11, Col 55, "The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]

    // SOURCE=E_ByrefUsedInInnerLambda03.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefUsedInInnerLambda03.fs
    [<Theory; FileInlineData("E_ByrefUsedInInnerLambda03.fs")>]
    let``E_ByrefUsedInInnerLambda03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 11, Col 24, Line 11, Col 60, "The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]

    // SOURCE=E_SetFieldToByref01.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref01.fs
    [<Theory; FileInlineData("E_SetFieldToByref01.fs")>]
    let``E_SetFieldToByref01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 412, Line 11, Col 17, Line 11, Col 27, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 437, Line 10, Col 6, Line 10, Col 9, "A type would store a byref typed value. This is not permitted by Common IL.")
            (Error 412, Line 11, Col 50, Line 11, Col 54, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_SetFieldToByref02.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref02.fs
    [<Theory; FileInlineData("E_SetFieldToByref02.fs")>]
    let``E_SetFieldToByref02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 431, Line 8, Col 9, Line 8, Col 17, "A byref typed value would be stored here. Top-level let-bound byref values are not permitted.")
        ]
    // SOURCE=E_SetFieldToByref03.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref03.fs
    [<Theory; FileInlineData("E_SetFieldToByref03.fs")>]
    let``E_SetFieldToByref03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1178, Line 8, Col 6, Line 8, Col 21, "The struct, record or union type 'RecordWithByref' is not structurally comparable because the type 'byref<int>' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'RecordWithByref' to clarify that the type is not comparable")
            (Error 412, Line 8, Col 25, Line 8, Col 26, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 437, Line 8, Col 6, Line 8, Col 21, "A type would store a byref typed value. This is not permitted by Common IL.")
        ]

    // SOURCE=E_SetFieldToByref04.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref04.fs
    [<Theory; FileInlineData("E_SetFieldToByref04.fs")>]
    let``E_SetFieldToByref04_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 412, Line 14, Col 28, Line 14, Col 37, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 421, Line 14, Col 29, Line 14, Col 30, "The address of the variable 'x' cannot be used at this point")
            (Error 412, Line 19, Col 20, Line 19, Col 53, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_SetFieldToByref05.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref05.fs
    [<Theory; FileInlineData("E_SetFieldToByref05.fs")>]
    let``E_SetFieldToByref05_fs`` compilation =
        compilation
        |> getCompilation
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
    [<Theory; FileInlineData("E_FirstClassFuncTakesByref.fs")>]
    let``E_FirstClassFuncTakesByref_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 8, Col 12, Line 8, Col 16, "This expression was expected to have type\n    'byref<'a>'    \nbut here has type\n    'int ref'    ")
        ]

    // SOURCE=E_StaticallyResolvedByRef01.fs SCFLAGS="--test:ErrorRanges"                 # E_StaticallyResolvedByRef01.fs
    [<Theory; FileInlineData("E_StaticallyResolvedByRef01.fs")>]
    let``E_StaticallyResolvedByRef01_fs`` compilation =
        compilation
        |> getCompilation
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
    [<Theory; FileInlineData("UseByrefInLambda01.fs")>]
    let``UseByrefInLambda01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed
        
#if NET7_0_OR_GREATER
// This constructor added in .NET 7: https://learn.microsoft.com/en-us/dotnet/api/system.span-1.-ctor?view=net-7.0#system-span-1-ctor(-0@)
    [<Theory; FileInlineData("ReturnFieldSetBySpan.fs")>]
    let``ReturnFieldSetBySpan_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed
        
    [<Theory; FileInlineData("ReturnSpan01.fs")>]
    let``ReturnSpan01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // --- Improved byref-like escape analysis (ref fields) ---

    [<Theory; FileInlineData("E_ReturnSpanFromLocalByref.fs")>]
    let``E_ReturnSpanFromLocalByref_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3234, Line 8, Col 5, Line 8, Col 9, "The Span or IsByRefLike variable 'span' cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
        ]

    [<Theory; FileInlineData("E_ReturnSpanFromLocalByref02.fs")>]
    let``E_ReturnSpanFromLocalByref02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3235, Line 9, Col 5, Line 9, Col 22, "A Span or IsByRefLike value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
        ]

    [<Theory; FileInlineData("ReturnSpanFromParamByref.fs")>]
    let``ReturnSpanFromParamByref_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ReturnSpanFromArray.fs")>]
    let``ReturnSpanFromArray_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("UseSpanFromLocalByref.fs")>]
    let``UseSpanFromLocalByref_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed

    // Old behavior preserved: error cases must compile successfully without preview lang version
    [<Theory; FileInlineData("E_ReturnSpanFromLocalByref.fs")>]
    let``E_ReturnSpanFromLocalByref_fs_without_preview`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("E_ReturnSpanFromLocalByref02.fs")>]
    let``E_ReturnSpanFromLocalByref02_fs_without_preview`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compile
        |> shouldSucceed

    // --- ScopedRefAttribute consumption ---

    [<Fact>]
    let ``Scoped ref param does not trigger escape error`` () =
        let csharpLib =
            CSharp """
using System;

public static class ScopedHelper
{
    public static Span<int> NotCapturing(scoped ref int x, int[] arr)
    {
        return new Span<int>(arr);
    }

    public static Span<int> NotCapturingIn(scoped in int x, int[] arr)
    {
        return new Span<int>(arr);
    }
}
"""         |> withName "ScopedLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        let fsharpSource = """
module Test

open System

let test () : Span<int> =
    let mutable local = 42
    ScopedHelper.NotCapturing(&local, [| 1; 2; 3 |])

let test2 () : Span<int> =
    let mutable local = 42
    ScopedHelper.NotCapturingIn(&local, [| 1; 2; 3 |])
"""
        FSharp fsharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    let private nonScopedRefParamCSharpLib =
        CSharp """
using System;

public static class UnscopedHelper
{
    public static Span<int> MayCapture(ref int x)
    {
        return default;
    }
}
"""     |> withName "UnscopedLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private nonScopedRefParamFSharpSource = """
module Test

open System

let test () : Span<int> =
    let mutable local = 42
    UnscopedHelper.MayCapture(&local)
"""

    [<Fact>]
    let ``Non-scoped ref param still triggers escape error`` () =
        FSharp nonScopedRefParamFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [nonScopedRefParamCSharpLib]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3235, Line 8, Col 5, Line 8, Col 38, "A Span or IsByRefLike value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
        ]

    [<Fact>]
    let ``Non-scoped ref param still triggers escape error - backward compat`` () =
        FSharp nonScopedRefParamFSharpSource
        |> asLibrary
        |> withReferences [nonScopedRefParamCSharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Scoped ref on instance method does not trigger escape error`` () =
        let csharpLib =
            CSharp """
using System;

public class SpanFactory
{
    private int[] _data;
    public SpanFactory(int[] arr) { _data = arr; }
    public Span<int> GetData(scoped ref int x) { return new Span<int>(_data); }
}
"""         |> withName "ScopedInstanceLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        let fsharpSource = """
module Test

open System

let test (factory: SpanFactory) : Span<int> =
    let mutable local = 42
    factory.GetData(&local)
"""
        FSharp fsharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    let private mixedScopedCSharpLib =
        CSharp """
using System;

public static class MixedHelper
{
    public static Span<int> Mixed(scoped ref int safe, ref int unsafeRef)
    {
        return default;
    }
}
"""     |> withName "MixedScopedLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private mixedScopedFSharpSource = """
module Test

open System

let test () : Span<int> =
    let mutable safe = 1
    let mutable unsafe' = 2
    MixedHelper.Mixed(&safe, &unsafe')
"""

    [<Fact>]
    let ``Mixed scoped and non-scoped params - non-scoped still errors`` () =
        FSharp mixedScopedFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [mixedScopedCSharpLib]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3235, Line 9, Col 5, Line 9, Col 39, "A Span or IsByRefLike value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
        ]

    [<Fact>]
    let ``Mixed scoped and non-scoped params - backward compat`` () =
        FSharp mixedScopedFSharpSource
        |> asLibrary
        |> withReferences [mixedScopedCSharpLib]
        |> compile
        |> shouldSucceed

    let private returnReadOnlySpanFromLocalByrefSource = """
module Test
open System

let f () =
    let mutable x = 1
    ReadOnlySpan<int>(&x)
"""

    let private nestedScopePropagationSource = """
module Test
open System

let passThrough (s: Span<int>) : Span<int> = s

let f () =
    let mutable x = 1
    passThrough(Span<int>(&x))
"""

    let private returnReadOnlySpanFromLocalInrefSource = """
module Test
open System

let f () =
    let mutable x = 1
    let y = &x
    ReadOnlySpan<int>(&y)
"""

    let private memoryMarshalCreateSpanSource = """
module Test
open System
open System.Runtime.InteropServices

let f () =
    let mutable x = 1
    MemoryMarshal.CreateSpan(&x, 1)
"""

    let private memoryMarshalCreateReadOnlySpanSource = """
module Test
open System
open System.Runtime.InteropServices

let f () =
    let mutable x = 1
    MemoryMarshal.CreateReadOnlySpan(&x, 1)
"""

    [<Fact>]
    let ``E_ReturnReadOnlySpanFromLocalByref`` () =
        FSharp returnReadOnlySpanFromLocalByrefSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3235, Line 7, Col 5, Line 7, Col 26, "A Span or IsByRefLike value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
        ]

    [<Fact>]
    let ``ReturnReadOnlySpanFromParamByref`` () =
        let fsharpSource = """
module Test
open System

let f (x: inref<int>) =
    ReadOnlySpan<int>(&x)
"""
        FSharp fsharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``E_NestedScopePropagation`` () =
        FSharp nestedScopePropagationSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3235, Line 9, Col 5, Line 9, Col 31, "A Span or IsByRefLike value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
        ]

    [<Fact>]
    let ``E_ReturnReadOnlySpanFromLocalInref`` () =
        FSharp returnReadOnlySpanFromLocalInrefSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3235, Line 8, Col 5, Line 8, Col 26, "A Span or IsByRefLike value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
        ]

    [<Fact>]
    let ``MemoryMarshalCreateSpan_ScopedParam_Succeeds`` () =
        FSharp memoryMarshalCreateSpanSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``MemoryMarshalCreateReadOnlySpan_ScopedParam_Succeeds`` () =
        FSharp memoryMarshalCreateReadOnlySpanSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    // Backward compatibility: same error-case code must compile WITHOUT preview

    [<Fact>]
    let ``E_ReturnReadOnlySpanFromLocalByref - backward compat`` () =
        FSharp returnReadOnlySpanFromLocalByrefSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``E_NestedScopePropagation - backward compat`` () =
        FSharp nestedScopePropagationSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``E_ReturnReadOnlySpanFromLocalInref - backward compat`` () =
        FSharp returnReadOnlySpanFromLocalInrefSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``MemoryMarshalCreateSpan_ScopedParam_Succeeds - backward compat`` () =
        FSharp memoryMarshalCreateSpanSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``MemoryMarshalCreateReadOnlySpan_ScopedParam_Succeeds - backward compat`` () =
        FSharp memoryMarshalCreateReadOnlySpanSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Constructor_NotAsReceiver_DefaultLangVersion`` () =
        FSharp """
module Test
open System

let f (x: byref<int>) =
    Span<int>(&x)
"""
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``GenericSpanReturn_CorrectTypeResolution_DefaultLangVersion`` () =
        FSharp """
module Test
open System
open System.Runtime.InteropServices

let f (x: byref<int>) =
    MemoryMarshal.CreateSpan(&x, 1)
"""
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``TaskCE_ByrefInClosure_FiresFS0412_BeforeEscapeAnalysis`` () =
        FSharp """
module Test
open System
open System.Threading.Tasks

let f () = task {
    let mutable x = 1
    return Span<int>(&x)
}
"""
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [412]

    // --- C# Interop: Custom IsByRefLike and Struct Receiver tests ---

    let private customIsByRefLikeCSharpLib =
        CSharp """
using System;

public ref struct MySpan<T>
{
    private ref T _ref;
    private int _length;

    public MySpan(ref T reference)
    {
        _ref = ref reference;
        _length = 1;
    }

    public ref T this[int index] => ref _ref;
}
"""     |> withName "CustomSpanLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private customIsByRefLikeFSharpSource = """
module Test

let f () =
    let mutable local = 42
    MySpan<int>(&local)
"""

    let private unscopedRefEscapesCSharpLib =
        CSharp """
using System;
using System.Diagnostics.CodeAnalysis;

public struct EvilStruct
{
    public int Field;

    [UnscopedRef]
    public Span<int> AsSpan()
    {
        return new Span<int>(ref Field);
    }
}
"""     |> withName "UnscopedRefLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private unscopedRefEscapesFSharpSource = """
module Test
open System

let f () : Span<int> =
    let mutable s = EvilStruct(Field = 42)
    s.AsSpan()
"""

    let private safeStructConservativeCSharpLib =
        CSharp """
using System;

public struct SafeStruct
{
    public Span<int> GetSpan(int[] arr)
    {
        return new Span<int>(arr);
    }
}
"""     |> withName "SafeStructLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private safeStructConservativeFSharpSource = """
module Test
open System

let f () : Span<int> =
    let mutable s = SafeStruct()
    s.GetSpan([| 1; 2; 3 |])
"""

    [<Fact>]
    let ``E_CustomIsByRefLikeStructEscapesLocal`` () =
        FSharp customIsByRefLikeFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [customIsByRefLikeCSharpLib]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3235, Line 6, Col 5, Line 6, Col 24, "A Span or IsByRefLike value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
        ]

    [<Fact>]
    let ``E_CustomIsByRefLikeStructEscapesLocal - backward compat`` () =
        FSharp customIsByRefLikeFSharpSource
        |> asLibrary
        |> withReferences [customIsByRefLikeCSharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``E_StructReceiverUnscopedRefEscapes`` () =
        FSharp unscopedRefEscapesFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [unscopedRefEscapesCSharpLib]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3235, Line 7, Col 5, Line 7, Col 15, "A Span or IsByRefLike value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
        ]

    [<Fact>]
    let ``E_StructReceiverUnscopedRefEscapes - backward compat`` () =
        FSharp unscopedRefEscapesFSharpSource
        |> asLibrary
        |> withReferences [unscopedRefEscapesCSharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``StructReceiverSafeCaseNoFalsePositive`` () =
        // Safe struct method without [UnscopedRef]: receiver is implicitly scoped, excluded from limit.
        FSharp safeStructConservativeFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [safeStructConservativeCSharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``E_StructReceiverSafeCaseConservativeError - backward compat`` () =
        FSharp safeStructConservativeFSharpSource
        |> asLibrary
        |> withReferences [safeStructConservativeCSharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``StructReceiverParamByrefNoError`` () =
        // Struct method called on a parameter (not local) — should succeed regardless of [UnscopedRef].
        let fsharpSource = """
module Test
open System

let f (s: byref<EvilStruct>) : Span<int> =
    s.AsSpan()
"""
        FSharp fsharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [unscopedRefEscapesCSharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``ScopedRef on constructor does not trigger escape error`` () =
        let csharpLib =
            CSharp """
using System;

public class ScopedCtorWrapper
{
    private int[] _data;

    public ScopedCtorWrapper(scoped ref int x, int[] arr)
    {
        _data = arr;
    }

    public Span<int> AsSpan() => new Span<int>(_data);
}
"""         |> withName "ScopedCtorLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        let fsharpSource = """
module Test

let test () =
    let mutable local = 42
    ScopedCtorWrapper(&local, [| 1; 2; 3 |])
"""
        FSharp fsharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``ScopedRef on IsByRefLike constructor does not trigger escape error`` () =
        let csharpLib =
            CSharp """
using System;

public ref struct ScopedRefStruct
{
    private Span<int> _span;

    public ScopedRefStruct(scoped ref int x, int[] arr)
    {
        _span = new Span<int>(arr);
    }
}
"""         |> withName "ScopedRefStructLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        let fsharpSource = """
module Test

let test () =
    let mutable local = 42
    ScopedRefStruct(&local, [| 1; 2; 3 |])
"""
        FSharp fsharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    let private unscopedRefStructCSharpLib =
        CSharp """
using System;

public ref struct UnscopedRefStruct
{
    private ref int _ref;

    public UnscopedRefStruct(ref int x)
    {
        _ref = ref x;
    }
}
"""     |> withName "UnscopedRefStructLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private unscopedRefStructFSharpSource = """
module Test

let test () =
    let mutable local = 42
    UnscopedRefStruct(&local)
"""

    [<Fact>]
    let ``E_NonScopedIsByRefLikeConstructorEscapes`` () =
        FSharp unscopedRefStructFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [unscopedRefStructCSharpLib]
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``E_NonScopedIsByRefLikeConstructorEscapes - backward compat`` () =
        FSharp unscopedRefStructFSharpSource
        |> asLibrary
        |> withReferences [unscopedRefStructCSharpLib]
        |> compile
        |> shouldSucceed

    // Simplified from `allows ref struct` generic (C# 13 required) to a direct Span<int>
    // return. The `allows ref struct` constraint is not available in the current test
    // infrastructure, so this tests the equivalent ref int → Span<int> escape path directly.
    let private genericFactoryCSharpLib =
        CSharp """
using System;

public static class GenericFactory
{
    public static Span<int> Create(ref int x) => new Span<int>(ref x);
}
"""     |> withName "AllowsRefStructLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.Preview

    let private genericFactoryFSharpSource = """
module Test
open System

let test () : Span<int> =
    let mutable local = 42
    GenericFactory.Create(&local)
"""

    [<Fact>]
    let ``E_AllowsRefStructGenericEscapes`` () =
        FSharp genericFactoryFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [genericFactoryCSharpLib]
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``E_AllowsRefStructGenericEscapes - backward compat`` () =
        FSharp genericFactoryFSharpSource
        |> asLibrary
        |> withReferences [genericFactoryCSharpLib]
        |> compile
        |> shouldSucceed

    // --- Control flow and chaining tests ---

    let private ifElseSpanFromLocalByrefSource = """
module Test
open System

let f flag (arr: int[]) =
    let mutable x = 1
    if flag then Span<int>(&x) else Span<int>(arr)
"""

    let private matchSpanFromLocalByrefSource = """
module Test
open System

let f (choice: bool) (arr: int[]) =
    let mutable x = 1
    match choice with
    | true -> Span<int>(&x)
    | false -> Span<int>(arr)
"""

    let private tryWithSpanFromLocalByrefSource = """
module Test
open System

let f (arr: int[]) =
    let mutable x = 1
    try Span<int>(&x)
    with _ -> Span<int>(arr)
"""

    let private sliceOnEscapedSpanSource = """
module Test
open System

let f () =
    let mutable x = 1
    let s = Span<int>(&x)
    s.Slice(0)
"""

    let private implicitConversionEscapedSpanSource = """
module Test
#nowarn "3391"
open System

let f () : ReadOnlySpan<int> =
    let mutable x = 1
    Span<int>(&x)
"""

    [<Fact>]
    let ``E_IfElseSpanFromLocalByref`` () =
        FSharp ifElseSpanFromLocalByrefSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3235, Line 7, Col 18, Line 7, Col 31, "A Span or IsByRefLike value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
        ]

    [<Fact>]
    let ``E_IfElseSpanFromLocalByref - backward compat`` () =
        FSharp ifElseSpanFromLocalByrefSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``E_MatchSpanFromLocalByref`` () =
        FSharp matchSpanFromLocalByrefSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3235, Line 8, Col 15, Line 8, Col 28, "A Span or IsByRefLike value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
        ]

    [<Fact>]
    let ``E_MatchSpanFromLocalByref - backward compat`` () =
        FSharp matchSpanFromLocalByrefSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``E_TryWithSpanFromLocalByref`` () =
        FSharp tryWithSpanFromLocalByrefSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3235, Line 7, Col 9, Line 7, Col 22, "A Span or IsByRefLike value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
        ]

    [<Fact>]
    let ``E_TryWithSpanFromLocalByref - backward compat`` () =
        FSharp tryWithSpanFromLocalByrefSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``E_SliceOnEscapedSpan`` () =
        FSharp sliceOnEscapedSpanSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``E_SliceOnEscapedSpan - backward compat`` () =
        FSharp sliceOnEscapedSpanSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``E_ImplicitConversionEscapedSpan`` () =
        FSharp implicitConversionEscapedSpanSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``E_ImplicitConversionEscapedSpan - backward compat`` () =
        FSharp implicitConversionEscapedSpanSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    // --- C# Interop edge case tests ---

    [<Fact>]
    let ``AllScopedParamsNoError`` () =
        let csharpLib =
            CSharp """
using System;

public static class AllScopedHelper
{
    public static Span<int> AllScoped(scoped ref int a, scoped ref int b, int[] arr)
    {
        return new Span<int>(arr);
    }
}
"""         |> withName "AllScopedLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        FSharp """
module Test
open System

let f () =
    let mutable a = 1
    let mutable b = 2
    AllScopedHelper.AllScoped(&a, &b, [| 1; 2; 3 |])
"""
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``ScopedWithOptionalParamNoError`` () =
        let csharpLib =
            CSharp """
using System;

public static class OptionalScopedHelper
{
    public static Span<int> WithOptional(scoped ref int x, int[] arr, int y = 0)
    {
        return new Span<int>(arr);
    }
}
"""         |> withName "OptionalScopedLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        FSharp """
module Test
open System

let f () =
    let mutable x = 1
    OptionalScopedHelper.WithOptional(&x, [| 1; 2; 3 |])
"""
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``ScopedValueParamNoError`` () =
        let csharpLib =
            CSharp """
using System;

public static class ScopedValueHelper
{
    public static Span<int> FromScopedValue(scoped Span<int> input, int[] arr)
    {
        return new Span<int>(arr);
    }
}
"""         |> withName "ScopedValueLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        FSharp """
module Test
open System

let f () =
    let mutable x = 1
    ScopedValueHelper.FromScopedValue(Span<int>(&x), [| 1; 2; 3 |])
"""
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    let private refReturnChainingCSharpLib =
        CSharp """
using System;

public static class RefReturnHelper
{
    public static ref int RefIdentity(ref int x) => ref x;
}
"""     |> withName "RefReturnLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private refReturnChainingFSharpSource = """
module Test
open System

let f () =
    let mutable local = 1
    Span<int>(&RefReturnHelper.RefIdentity(&local))
"""

    [<Fact>]
    let ``E_RefReturnChainingIntoSpan`` () =
        FSharp refReturnChainingFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [refReturnChainingCSharpLib]
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``E_RefReturnChainingIntoSpan - backward compat`` () =
        FSharp refReturnChainingFSharpSource
        |> asLibrary
        |> withReferences [refReturnChainingCSharpLib]
        |> compile
        |> shouldSucceed

    // --- Miscellaneous escape analysis tests ---

    let private spanFromLocalByrefInFsxSource = """
open System

let f () =
    let mutable x = 1
    Span<int>(&x)
"""

    let private spanFromLocalByrefInObjExprSource = """
module Test
open System

type ISpanProvider =
    abstract GetSpan: unit -> Span<int>

let makeProvider () =
    { new ISpanProvider with
        member _.GetSpan() =
            let mutable x = 1
            Span<int>(&x) }
"""

    let private spanFromLocalByrefInNestedScopeSource = """
module Test
open System

let outer () =
    let result =
        let mutable x = 1
        Span<int>(&x)
    result
"""

    [<Fact>]
    let ``E_SpanFromLocalByrefInFsx`` () =
        Fsx spanFromLocalByrefInFsxSource
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``E_SpanFromLocalByrefInFsx - backward compat`` () =
        Fsx spanFromLocalByrefInFsxSource
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``E_SpanFromLocalByrefInObjExpr`` () =
        FSharp spanFromLocalByrefInObjExprSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``E_SpanFromLocalByrefInObjExpr - backward compat`` () =
        FSharp spanFromLocalByrefInObjExprSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``E_SpanFromLocalByrefInNestedScope`` () =
        FSharp spanFromLocalByrefInNestedScopeSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3234]

    [<Fact>]
    let ``E_SpanFromLocalByrefInNestedScope - backward compat`` () =
        FSharp spanFromLocalByrefInNestedScopeSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("E_IndexerSpanCapturingByref.fs")>]
    let``E_IndexerSpanCapturingByref_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Theory; FileInlineData("E_IndexerSpanCapturingByref.fs")>]
    let``E_IndexerSpanCapturingByref_fs_without_preview`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("E_OperatorOverloadSpanCapturingByref.fs")>]
    let``E_OperatorOverloadSpanCapturingByref_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Theory; FileInlineData("E_OperatorOverloadSpanCapturingByref.fs")>]
    let``E_OperatorOverloadSpanCapturingByref_fs_without_preview`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("E_MatchExpressionSpanCapturingByref.fs")>]
    let``E_MatchExpressionSpanCapturingByref_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Theory; FileInlineData("E_MatchExpressionSpanCapturingByref.fs")>]
    let``E_MatchExpressionSpanCapturingByref_fs_without_preview`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("TestMemoryMarshal.fs")>]
    let``TestMemoryMarshal_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3234]

    [<Theory; FileInlineData("TestMemoryMarshal.fs")>]
    let``TestMemoryMarshal_fs_without_preview`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("TestGaps2.fs")>]
    let``TestGaps2_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [412]

    [<Theory; FileInlineData("TestGaps2.fs")>]
    let``TestGaps2_fs_without_preview`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> compile
        |> shouldFail
        |> withErrorCodes [412]

    // ---- T11a: Non-scoped 'in T' (RFC row 17) ----
    let private nonScopedInParamCSharpLib =
        CSharp """
using System;
public static class InHelper
{
    public static Span<int> FromIn(in int x, int[] arr)
    {
        return new Span<int>(arr);
    }
}
"""     |> withName "InLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private nonScopedInParamFSharpSource = """
module Test
open System
let f () =
    let mutable local = 42
    InHelper.FromIn(&local, [| 1; 2; 3 |])
"""

    [<Fact>]
    let ``NonScopedInParam triggers escape error`` () =
        FSharp nonScopedInParamFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [nonScopedInParamCSharpLib]
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``NonScopedInParam backward compat`` () =
        FSharp nonScopedInParamFSharpSource
        |> asLibrary
        |> withReferences [nonScopedInParamCSharpLib]
        |> compile
        |> shouldSucceed

    // ---- T11b: [UnscopedRef] out T (RFC row 22) ----
    let private unscopedRefOutParamCSharpLib =
        CSharp """
using System;
using System.Diagnostics.CodeAnalysis;
public static class UnscopedOutHelper
{
    public static Span<int> ViaOut([UnscopedRef] out int x, int[] arr)
    {
        x = 42;
        return new Span<int>(arr);
    }
}
"""     |> withName "UnscopedOutLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private unscopedRefOutParamFSharpSource = """
module Test
open System
let f () =
    let mutable local = 0
    UnscopedOutHelper.ViaOut(&local, [| 1; 2; 3 |])
"""

    [<Fact>]
    let ``UnscopedRefOutParam triggers escape error`` () =
        FSharp unscopedRefOutParamFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [unscopedRefOutParamCSharpLib]
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``UnscopedRefOutParam backward compat`` () =
        FSharp unscopedRefOutParamFSharpSource
        |> asLibrary
        |> withReferences [unscopedRefOutParamCSharpLib]
        |> compile
        |> shouldSucceed

    // ---- T11c: F#-to-F# [<ScopedRef>] positive ----
    [<Fact>]
    let ``FSharpScopedRefParam does not trigger escape error`` () =
        FSharp """
module Test
open System
open System.Runtime.CompilerServices

let safeFactory ([<ScopedRef>] x: byref<int>) (arr: int[]) : Span<int> =
    x <- 42
    Span<int>(arr)

let test () : Span<int> =
    let mutable local = 1
    safeFactory &local [| 1; 2; 3 |]
"""
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    // ---- T11d: F#-to-F# non-scoped byref negative ----
    let private fsharpNonScopedByrefParamSource = """
module Test
open System

let unsafeFactory (x: byref<int>) : Span<int> =
    Span<int>(&x)

let test () : Span<int> =
    let mutable local = 1
    unsafeFactory &local
"""

    [<Fact>]
    let ``FSharpNonScopedByrefParam triggers escape error`` () =
        FSharp fsharpNonScopedByrefParamSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``FSharpNonScopedByrefParam backward compat`` () =
        FSharp fsharpNonScopedByrefParamSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    // ---- T11e: scoped in int x positive (RFC row 16) ----
    [<Fact>]
    let ``ScopedInParam does not trigger escape error`` () =
        let csharpLib =
            CSharp """
using System;
public static class ScopedInHelper
{
    public static Span<int> FromScopedIn(scoped in int x, int[] arr)
    {
        return new Span<int>(arr);
    }
}
"""         |> withName "ScopedInLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        FSharp """
module Test
open System
let f () =
    let mutable local = 42
    ScopedInHelper.FromScopedIn(&local, [| 1; 2; 3 |])
"""
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    // ---- T11f: Plain out T — implicitly scoped when RefSafetyRulesVersion >= 11 ----
    let private outParamCSharpLib =
        CSharp """
using System;
public static class OutHelper
{
    public static Span<int> ViaPlainOut(out int x, int[] arr)
    {
        x = 42;
        return new Span<int>(arr);
    }
}
"""     |> withName "OutLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private outParamFSharpSource = """
module Test
open System
let f () =
    let mutable local = 0
    OutHelper.ViaPlainOut(&local, [| 1; 2; 3 |])
"""

    [<Fact>]
    let ``OutParam without explicit scoped is implicitly scoped`` () =
        FSharp outParamFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [outParamCSharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``OutParam without explicit scoped backward compat`` () =
        FSharp outParamFSharpSource
        |> asLibrary
        |> withReferences [outParamCSharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``FSharpScopedRefParam cross-assembly does not trigger escape error`` () =
        let fsharpLib =
            FSharp
                """
module Lib
open System
open System.Runtime.CompilerServices

let safeFactory ([<ScopedRef>] x: byref<int>) (arr: int[]) : Span<int> =
    x <- 42
    Span<int>(arr)
"""
            |> withName "ScopedRefLib"
            |> asLibrary
            |> withLangVersionPreview

        FSharp
            """
module Test
open System
let f () : Span<int> =
    let mutable local = 1
    Lib.safeFactory &local [| 1; 2; 3 |]
"""
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [ fsharpLib ]
        |> compile
        |> shouldSucceed

    let private inlineFunctionEscapeSource = """
module Test
open System
let inline createSpan (x: byref<int>) : Span<int> = Span<int>(&x)
let test () : Span<int> =
    let mutable local = 1
    createSpan &local
"""

    [<Fact>]
    let ``Inline function escape caught at call site`` () =
        FSharp inlineFunctionEscapeSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [ 3235 ]

    [<Fact>]
    let ``Inline function escape caught at call site - backward compat`` () =
        FSharp inlineFunctionEscapeSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    // --- UnscopedRef on out param mixed params test data ---

    let private unscopedRefOutParamMixedCSharpLib =
        CSharp
            """
using System;
using System.Diagnostics.CodeAnalysis;

public static class Helper
{
    // scoped ref int => byref is scoped (safe)
    // [UnscopedRef] out int => out is UN-scoped (can escape)
    public static Span<int> MixedCapture(scoped ref int safe, [UnscopedRef] out int escapable)
    {
        escapable = 0;
        return default;
    }
}
"""
        |> withName "UnscopedRefParamLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private unscopedRefOutParamMixedFSharpSource = """
module Test
open System
let f () : Span<int> =
    let mutable safe = 1
    let mutable escapable = 2
    Helper.MixedCapture(&safe, &escapable)
"""

    [<Fact>]
    let ``UnscopedRef on out param negates scoping in mixed params`` () =
        FSharp unscopedRefOutParamMixedFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [ unscopedRefOutParamMixedCSharpLib ]
        |> compile
        |> shouldFail
        |> withErrorCodes [ 3235 ]

    [<Fact>]
    let ``UnscopedRef on out param negates scoping in mixed params - backward compat`` () =
        FSharp unscopedRefOutParamMixedFSharpSource
        |> asLibrary
        |> withReferences [ unscopedRefOutParamMixedCSharpLib ]
        |> compile
        |> shouldSucceed

    // --- RefSafetyRulesAttribute test data ---

    let private outParamCSharp8Lib =
        CSharp """
using System;
public static class OutHelperOld
{
    public static Span<int> ViaPlainOut(out int x, int[] arr)
    {
        x = 42;
        return new Span<int>(arr);
    }
}
"""     |> withName "OutLibOld"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp8

    let private outParamCSharp8FSharpSource = """
module Test
open System
let f () =
    let mutable local = 0
    OutHelperOld.ViaPlainOut(&local, [| 1; 2; 3 |])
"""

    let private genericCSharp8Lib =
        CSharp """
using System;
public static class GenericHelperOld
{
    public static Span<T> Create<T>(ref T value, T[] arr)
    {
        return new Span<T>(arr);
    }
}
"""     |> withName "GenericLib8"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp8

    let private genericCSharp8FSharpSource = """
module Test
open System
let f () =
    let mutable local = 1
    GenericHelperOld.Create(&local, [| 1; 2; 3 |])
"""

    let private genericCSharp11Lib =
        CSharp """
using System;
using System.Runtime.CompilerServices;
public static class GenericHelper
{
    public static Span<T> Create<T>(scoped ref T value, T[] arr)
    {
        return new Span<T>(arr);
    }
}
"""     |> withName "GenericLib11"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private genericCSharp11FSharpSource = """
module Test
open System
let f () =
    let mutable local = 1
    GenericHelper.Create(&local, [| 1; 2; 3 |])
"""

    let private refSpanCSharp11Lib =
        CSharp """
using System;
public static class RefSpanHelper
{
    public static Span<int> ViaRefSpan(ref Span<int> input, int[] arr)
    {
        return new Span<int>(arr);
    }
}
"""     |> withName "RefSpanLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private refSpanCSharp11FSharpSource = """
module Test
open System
let f () =
    let mutable local = Span<int>.Empty
    RefSpanHelper.ViaRefSpan(&local, [| 1; 2; 3 |])
"""

    let private fsharpRoundtripLibSource = """
module Lib
open System
let makeSpan (x: outref<int>) (arr: int[]) : Span<int> =
    x <- 42
    Span<int>(arr)
"""

    let private fsharpRoundtripFSharpSource = """
module Test
open System
let f () =
    let mutable local = 0
    Lib.makeSpan &local [| 1; 2; 3 |]
"""

    // --- RefSafetyRulesAttribute tests ---

    [<Fact>]
    let ``OutParam CSharp8 no RefSafetyRules is implicitly scoped`` () =
        // Note: Even with CSharp8 language version, modern Roslyn (.NET 10+) emits
        // RefSafetyRulesAttribute(11) when the type is available in the target framework.
        // So the out param IS implicitly scoped, and this correctly succeeds.
        FSharp outParamCSharp8FSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [outParamCSharp8Lib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``OutParam CSharp8 no RefSafetyRules backward compat`` () =
        FSharp outParamCSharp8FSharpSource
        |> asLibrary
        |> withReferences [outParamCSharp8Lib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Generic method CSharp11 with ScopedRef succeeds`` () =
        FSharp genericCSharp11FSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [genericCSharp11Lib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Generic method CSharp11 with ScopedRef backward compat`` () =
        FSharp genericCSharp11FSharpSource
        |> asLibrary
        |> withReferences [genericCSharp11Lib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Generic method CSharp8 without RefSafetyRules triggers escape error`` () =
        FSharp genericCSharp8FSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [genericCSharp8Lib]
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``Generic method CSharp8 without RefSafetyRules backward compat`` () =
        FSharp genericCSharp8FSharpSource
        |> asLibrary
        |> withReferences [genericCSharp8Lib]
        |> compile
        |> shouldSucceed

    let private refSafetyRulesSource = """
module TestLib
open System
let makeSpan (arr: int[]) : Span<int> = Span<int>(arr)
"""

    [<Fact>]
    let ``FSharp assembly emits RefSafetyRulesAttribute`` () =
        FSharp refSafetyRulesSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL
            [
                """
.custom instance void [runtime]System.Runtime.CompilerServices.RefSafetyRulesAttribute::.ctor(int32) = ( 01 00 0B 00 00 00 00 00 )"""
            ]

    [<Fact>]
    let ``Ref to refstruct CSharp11 is implicitly scoped`` () =
        FSharp refSpanCSharp11FSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [refSpanCSharp11Lib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Ref to refstruct CSharp11 backward compat`` () =
        FSharp refSpanCSharp11FSharpSource
        |> asLibrary
        |> withReferences [refSpanCSharp11Lib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``FSharp roundtrip cross-assembly implicit scoping`` () =
        let fsharpLib =
            FSharp fsharpRoundtripLibSource
            |> asLibrary
            |> withName "RoundtripLib"
            |> withLangVersionPreview
            |> withOptions ["--nointerfacedata"]

        // Without sigdata, the consumer sees IL metadata (tupled args), so use tupled call syntax.
        FSharp """
module Test
open System
let f () =
    let mutable local = 0
    Lib.makeSpan(&local, [| 1; 2; 3 |])
"""
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [fsharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``FSharp roundtrip cross-assembly implicit scoping backward compat`` () =
        let fsharpLib =
            FSharp fsharpRoundtripLibSource
            |> asLibrary
            |> withName "RoundtripLib"

        FSharp fsharpRoundtripFSharpSource
        |> asLibrary
        |> withReferences [fsharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``FSharp assembly does not emit RefSafetyRulesAttribute without preview`` () =
        FSharp refSafetyRulesSource
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyILNotPresent
            [
                "RefSafetyRulesAttribute"
            ]

    let private curriedScopedRefSource = """
module Test
open System
open System.Runtime.CompilerServices

let safeFactory ([<ScopedRef>] x: byref<int>) (arr: int[]) : Span<int> =
    x <- 42
    Span<int>(arr)

let f () : Span<int> =
    let mutable local = 1
    safeFactory &local [| 1; 2; 3 |]
"""

    [<Fact>]
    let ``Curried F# ScopedRef function does not crash`` () =
        // Same-assembly curried call: argsl in the typed tree is flat [&local; arr],
        // and ArgInfos [[x]; [arr]] flattens to length 2. ScopedRef on x should be honored.
        FSharp curriedScopedRefSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Curried F# ScopedRef function does not crash - backward compat`` () =
        FSharp curriedScopedRefSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    // ---- GAP-1: ScopedRef body enforcement ----

    // --- ScopedRef body enforcement test data ---

    let private scopedRefSpanCtorEscapeSource = """
module Test
open System
open System.Runtime.CompilerServices
let leak ([<ScopedRef>] x: byref<int>) : Span<int> = Span<int>(&x)
"""

    let private scopedRefByrefReturnSource = """
module Test
open System.Runtime.CompilerServices
let leak ([<ScopedRef>] x: byref<int>) : byref<int> = &x
"""

    let private scopedRefSpanEscapeSource = """
module Test
open System
open System.Runtime.CompilerServices
let leak ([<ScopedRef>] s: Span<int>) : Span<int> = s
"""

    [<Fact>]
    let ``ScopedRef param cannot escape via Span ctor in body`` () =
        FSharp scopedRefSpanCtorEscapeSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``ScopedRef param cannot escape via Span ctor in body - backward compat`` () =
        FSharp scopedRefSpanCtorEscapeSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``ScopedRef param cannot be returned as byref`` () =
        FSharp scopedRefByrefReturnSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3209]

    [<Fact>]
    let ``ScopedRef param cannot be returned as byref - backward compat`` () =
        FSharp scopedRefByrefReturnSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``ScopedRef param not returned is OK`` () =
        FSharp """
module Test
open System
open System.Runtime.CompilerServices
let safe ([<ScopedRef>] x: byref<int>) (arr: int[]) : Span<int> = Span<int>(arr)
"""
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``ScopedRef span param cannot escape in body`` () =
        FSharp scopedRefSpanEscapeSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3234]

    [<Fact>]
    let ``ScopedRef span param cannot escape in body - backward compat`` () =
        FSharp scopedRefSpanEscapeSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    // ---- GAP-2: ScopedRef IL emission verification ----

    [<Fact>]
    let ``ScopedRef attribute is emitted to IL on parameter`` () =
        FSharp """
module Test
open System
open System.Runtime.CompilerServices
let safeFactory ([<ScopedRef>] x: byref<int>) (arr: int[]) : Span<int> = Span<int>(arr)
"""
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL
            [
                """
.custom instance void [runtime]System.Runtime.CompilerServices.ScopedRefAttribute::.ctor() = ( 01 00 00 00 )"""
            ]

    // ---- GAP-3: Override scope variance ----

    let private scopeVarianceCSharpBaseLib =
        CSharp """
using System;
public abstract class Base {
    public abstract Span<int> M(scoped ref int x, int[] arr);
}
"""     |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11
        |> withName "ScopeVarianceBase"

    let private scopeVarianceWideningFSharpSource = """
module Test
open System

type Derived() =
    inherit Base()
    override _.M(x: byref<int>, arr: int[]) = Span<int>(arr)
"""

    [<Fact>]
    let ``Override cannot widen scoped parameter from CSharp base`` () =
        FSharp scopeVarianceWideningFSharpSource
        |> withReferences [scopeVarianceCSharpBaseLib]
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withWarningCode 3882

    [<Fact>]
    let ``Override keeps scoped parameter is OK`` () =
        FSharp """
module Test
open System
open System.Runtime.CompilerServices

type Derived() =
    inherit Base()
    override _.M([<ScopedRef>] x: byref<int>, arr: int[]) = Span<int>(arr)
"""
        |> withReferences [scopeVarianceCSharpBaseLib]
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Override widens scoped parameter backward compat`` () =
        FSharp scopeVarianceWideningFSharpSource
        |> withReferences [scopeVarianceCSharpBaseLib]
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``F# abstract override widens scoped parameter`` () =
        FSharp """
module Test
open System
open System.Runtime.CompilerServices

[<AbstractClass>]
type Base() =
    abstract M : x: byref<int> * arr: int[] -> Span<int>

type Derived() =
    inherit Base()
    override _.M(x: byref<int>, arr: int[]) = Span<int>(arr)
"""
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    // ---- GAP-4: UnscopedRef on F# struct methods same-assembly ----

    let private unscopedRefSameAssemblySource = """
module Test
open System
open System.Runtime.CompilerServices
open System.Diagnostics.CodeAnalysis

[<Struct; IsByRefLike>]
type S =
    val mutable X: int
    [<UnscopedRef>]
    member this.AsSpan() : Span<int> = Span<int>(&this.X)

let test (s: byref<S>) : Span<int> =
    s.AsSpan()
"""

    [<Fact>]
    let ``UnscopedRef on F# struct method allows this to escape same-assembly`` () =
        FSharp unscopedRefSameAssemblySource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``UnscopedRef on F# struct method allows this to escape same-assembly - backward compat`` () =
        FSharp unscopedRefSameAssemblySource
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Struct method without UnscopedRef has scoped receiver same-assembly`` () =
        FSharp """
module Test
open System
open System.Runtime.CompilerServices

[<Struct; IsByRefLike>]
type S =
    val mutable X: int
    member this.GetSpan(arr: int[]) : Span<int> = Span<int>(arr)

let test () : Span<int> =
    let mutable s = S()
    s.X <- 42
    s.GetSpan([| 1; 2; 3 |])
"""
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Local struct receiver without UnscopedRef is still scoped`` () =
        FSharp """
module Test
open System
open System.Runtime.CompilerServices

[<Struct; IsByRefLike>]
type S =
    val mutable X: int
    member this.AsSpan() : Span<int> = Span<int>(&this.X)

let test () : Span<int> =
    let mutable s = S()
    s.X <- 42
    s.AsSpan()
"""
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    let private unscopedRefLocalReceiverSource = """
module Test
open System
open System.Runtime.CompilerServices
open System.Diagnostics.CodeAnalysis

[<Struct; IsByRefLike>]
type S =
    val mutable X: int
    [<UnscopedRef>]
    member this.AsSpan() : Span<int> = Span<int>(&this.X)

let test () : Span<int> =
    let mutable s = S()
    s.AsSpan()
"""

    [<Fact>]
    let ``E_UnscopedRef local struct receiver escapes same-assembly`` () =
        FSharp unscopedRefLocalReceiverSource
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``E_UnscopedRef local struct receiver escapes same-assembly - backward compat`` () =
        FSharp unscopedRefLocalReceiverSource
        |> asLibrary
        |> compile
        |> shouldSucceed

    // ---- T4: Explicit interface implementation override variance ----

    let private scopeVarianceCSharpInterfaceLib =
        CSharp """
using System;
public interface IFoo
{
    Span<int> M(scoped ref int x, int[] arr);
}
"""     |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11
        |> withName "ScopeVarianceInterface"

    let private interfaceImplWideningFSharpSource = """
module Test
open System

type Impl() =
    interface IFoo with
        member _.M(x: byref<int>, arr: int[]) = Span<int>(arr)
"""

    [<Fact>]
    let ``Explicit interface impl widens scoped parameter from CSharp base`` () =
        FSharp interfaceImplWideningFSharpSource
        |> withReferences [scopeVarianceCSharpInterfaceLib]
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withWarningCode 3882

    [<Fact>]
    let ``Explicit interface impl keeps scoped parameter from CSharp base`` () =
        FSharp """
module Test
open System
open System.Runtime.CompilerServices

type Impl() =
    interface IFoo with
        member _.M([<ScopedRef>] x: byref<int>, arr: int[]) = Span<int>(arr)
"""
        |> withReferences [scopeVarianceCSharpInterfaceLib]
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Explicit interface impl widens scoped parameter backward compat`` () =
        FSharp interfaceImplWideningFSharpSource
        |> withReferences [scopeVarianceCSharpInterfaceLib]
        |> asLibrary
        |> compile
        |> shouldSucceed

    // ---- T3: [UnscopedRef] negating implicit scoping on ref Span<T> ----

    let private unscopedRefRefSpanCSharpLib =
        CSharp """
using System;
using System.Diagnostics.CodeAnalysis;
public static class UnscopedRefSpanHelper
{
    // ref Span<int> is implicitly scoped (RefSafetyRules=11, ref to byref-like).
    // [UnscopedRef] negates the implicit scoping, so the span CAN escape.
    public static Span<int> ViaUnscopedRefSpan([UnscopedRef] ref Span<int> s)
    {
        return s;
    }

    // For comparison: without [UnscopedRef], ref Span<int> IS scoped.
    public static Span<int> ViaScopedRefSpan(ref Span<int> s)
    {
        return default;
    }
}
"""     |> withName "UnscopedRefSpanLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private unscopedRefRefSpanFSharpSource = """
module Test
open System
let f () =
    let mutable s = Span<int>.Empty
    UnscopedRefSpanHelper.ViaUnscopedRefSpan(&s)
"""

    [<Fact>]
    let ``UnscopedRef on ref Span param negates implicit scoping - triggers escape error`` () =
        FSharp unscopedRefRefSpanFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [unscopedRefRefSpanCSharpLib]
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``UnscopedRef on ref Span param backward compat`` () =
        FSharp unscopedRefRefSpanFSharpSource
        |> asLibrary
        |> withReferences [unscopedRefRefSpanCSharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Implicitly scoped ref Span param without UnscopedRef is safe`` () =
        FSharp """
module Test
open System
let f () =
    let mutable s = Span<int>.Empty
    UnscopedRefSpanHelper.ViaScopedRefSpan(&s)
"""
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [unscopedRefRefSpanCSharpLib]
        |> compile
        |> shouldSucceed

    // ---- [UnscopedRef] negating implicit scoping on in Span<T> ----

    let private unscopedRefInSpanCSharpLib =
        CSharp """
using System;
using System.Diagnostics.CodeAnalysis;
public static class UnscopedInSpanHelper
{
    // in Span<int> is implicitly scoped (RefSafetyRules=11, in to byref-like).
    // [UnscopedRef] negates implicit scoping.
    public static Span<int> ViaUnscopedInSpan([UnscopedRef] in Span<int> s)
    {
        return default;
    }
}
"""     |> withName "UnscopedInSpanLib"
        |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

    let private unscopedRefInSpanFSharpSource = """
module Test
open System
let f () =
    let mutable s = Span<int>.Empty
    UnscopedInSpanHelper.ViaUnscopedInSpan(&s)
"""

    [<Fact>]
    let ``UnscopedRef on in Span param negates implicit scoping - triggers escape error`` () =
        FSharp unscopedRefInSpanFSharpSource
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [unscopedRefInSpanCSharpLib]
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``UnscopedRef on in Span param backward compat`` () =
        FSharp unscopedRefInSpanFSharpSource
        |> asLibrary
        |> withReferences [unscopedRefInSpanCSharpLib]
        |> compile
        |> shouldSucceed

    // ---- Generic method without RefSafetyRules: ScopedRef attribute ignored (methInst guard) ----

    [<Fact>]
    let ``Generic method ScopedRef ignored without RefSafetyRules`` () =
        // Simulate a C# library WITHOUT [module: RefSafetyRules(11)] but WITH ScopedRefAttribute on a generic method.
        // The F# compiler should NOT trust ScopedRefAttribute on generic methods from such assemblies,
        // so &local should still trigger FS3235 (conservative behavior).
        let csharpLib =
            CSharp """
using System;
public class Lib {
    // Without RefSafetyRulesAttribute, this is an older assembly.
    // scoped ref is compiled to ScopedRefAttribute but cannot be trusted on generics.
    public static Span<T> Create<T>(ref T x) => default;
}
"""         |> withName "GenericNoRefSafetyLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        FSharp """
module Test
open System

let f () =
    let mutable local = 42
    Lib.Create<int>(&local)
"""
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``Generic method ScopedRef ignored without RefSafetyRules - backward compat`` () =
        let csharpLib =
            CSharp """
using System;
public class Lib {
    public static Span<T> Create<T>(ref T x) => default;
}
"""         |> withName "GenericNoRefSafetyLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        FSharp """
module Test
open System

let f () =
    let mutable local = 42
    Lib.Create<int>(&local)
"""
        |> asLibrary
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    // ---- Override variance with overloaded methods differing in generic arity ----

    [<Fact>]
    let ``Override variance correctly distinguishes overloads by generic arity`` () =
        let csharpLib =
            CSharp """
using System;
using System.Runtime.CompilerServices;
public abstract class Base {
    // Non-generic overload: scoped ref
    public abstract Span<int> M(scoped ref int x, int[] arr);
    // Generic overload with same param count: ref (not scoped)
    public abstract Span<T> M<T>(ref T x, T[] arr);
}
"""         |> withName "OverloadGenericArityLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        // Override the non-generic M without [<ScopedRef>] — should warn (widens scoped)
        // Override the generic M<T> without [<ScopedRef>] — should NOT warn (was never scoped)
        FSharp """
module Test
open System

type Derived() =
    inherit Base()
    override _.M(x: byref<int>, arr: int[]) = Span<int>(arr)
    override _.M<'T>(x: byref<'T>, arr: 'T[]) = Span<'T>(arr)
"""
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3882, Line 7, Col 16, Line 7, Col 17, "The override of 'M' widens the 'scoped' constraint on parameter 'x'. The base method declares this parameter as 'scoped' (it cannot escape), but the override allows it to escape. This may cause unsoundness when callers use the base type.")
        ]

    [<Fact>]
    let ``Override variance with generic overloads backward compat`` () =
        let csharpLib =
            CSharp """
using System;
using System.Runtime.CompilerServices;
public abstract class Base {
    public abstract Span<int> M(scoped ref int x, int[] arr);
    public abstract Span<T> M<T>(ref T x, T[] arr);
}
"""         |> withName "OverloadGenericArityLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        FSharp """
module Test
open System

type Derived() =
    inherit Base()
    override _.M(x: byref<int>, arr: int[]) = Span<int>(arr)
    override _.M<'T>(x: byref<'T>, arr: 'T[]) = Span<'T>(arr)
"""
        |> asLibrary
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    // ---- ScopedRef on value-type Span parameter in F# authoring ----

    [<Fact>]
    let ``ScopedRef on Span value param prevents escape in body`` () =
        FSharp """
module Test
open System
open System.Runtime.CompilerServices

let leak ([<ScopedRef>] s: Span<int>) : Span<int> = s
"""
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [3234]

    [<Fact>]
    let ``ScopedRef on Span value param prevents escape in body - backward compat`` () =
        FSharp """
module Test
open System
open System.Runtime.CompilerServices

let leak ([<ScopedRef>] s: Span<int>) : Span<int> = s
"""
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``ScopedRef on Span value param excluded at call site`` () =
        FSharp """
module Test
open System
open System.Runtime.CompilerServices

let safe ([<ScopedRef>] s: Span<int>) (arr: int[]) : Span<int> = Span<int>(arr)

let f () =
    let mutable x = 1
    safe (Span<int>(&x)) [| 1; 2; 3 |]
"""
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

#endif

    // ---- Finding 2: ref T (type variable) conservative fallback with RefSafetyRules(11) ----

    [<Fact>]
    let ``ref T type variable is not implicitly scoped even with RefSafetyRules`` () =
        // C# 11 automatically emits [module: RefSafetyRules(11)].
        let csharpLib =
            CSharp """
using System;
public class Lib {
    public static Span<T> M<T>(ref T x) => default;
}
"""         |> withName "RefTTypeVarLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        FSharp """
module Test
open System

let f () =
    let mutable local = 42
    Lib.M<int>(&local)
"""
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withErrorCodes [3235]

    [<Fact>]
    let ``ref T type variable is not implicitly scoped even with RefSafetyRules - backward compat`` () =
        let csharpLib =
            CSharp """
using System;
public class Lib {
    public static Span<T> M<T>(ref T x) => default;
}
"""         |> withName "RefTTypeVarLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        FSharp """
module Test
open System

let f () =
    let mutable local = 42
    Lib.M<int>(&local)
"""
        |> asLibrary
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    // ---- Finding 3: async { } CE interaction ----

    [<Fact>]
    let ``AsyncCE_ByrefInClosure_FiresFS0412_BeforeEscapeAnalysis`` () =
        FSharp """
module Test
open System

let f () = async {
    let mutable x = 1
    return Span<int>(&x)
}
"""
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCodes [412]

    // ---- Finding 5: Property getter returning span-like ----

#if NET7_0_OR_GREATER
    [<Fact>]
    let ``Property getter returning span-like from UnscopedRef struct`` () =
        // Known limitation: property getters with [UnscopedRef] on a struct are not yet caught
        // by escape analysis (unlike method calls such as E_StructReceiverUnscopedRefEscapes).
        // Ideally this should fail with FS3235, but currently succeeds.
        let csharpLib =
            CSharp """
using System;
using System.Diagnostics.CodeAnalysis;
public struct Container {
    public int Field;
    [UnscopedRef]
    public Span<int> Data => new Span<int>(ref Field);
}
"""         |> withName "PropertyGetterSpanLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        FSharp """
module Test
open System

let f () : Span<int> =
    let mutable c = Container(Field = 42)
    c.Data
"""
        |> asLibrary
        |> withLangVersionPreview
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Property getter returning span-like from UnscopedRef struct - backward compat`` () =
        let csharpLib =
            CSharp """
using System;
using System.Diagnostics.CodeAnalysis;
public struct Container {
    public int Field;
    [UnscopedRef]
    public Span<int> Data => new Span<int>(ref Field);
}
"""         |> withName "PropertyGetterSpanLib"
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11

        FSharp """
module Test
open System

let f () : Span<int> =
    let mutable c = Container(Field = 42)
    c.Data
"""
        |> asLibrary
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed
#endif

#if NETSTANDARD2_1_OR_GREATER
    [<Theory; FileInlineData("E_TopLevelByref.fs")>]
    let``E_TopLevelByref_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 431, Line 6, Col 5, Line 6, Col 13, "A byref typed value would be stored here. Top-level let-bound byref values are not permitted.")
        ]
        
    [<Theory; FileInlineData("E_SpanUsedInInnerLambda01.fs")>]
    let``E_SpanUsedInInnerLambda01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 8, Col 34, Line 8, Col 45, "The byref-typed variable 'span' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]
        
    [<Theory; FileInlineData("E_SpanUsedInInnerLambda02.fs")>]
    let``E_SpanUsedInInnerLambda02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 8, Col 34, Line 8, Col 45, "The byref-typed variable 'span' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]

#endif

