// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module NullnessMetadata =
    type Optimize = Optimize | DoNotOptimize

    let addOptions (o:Optimize) compilation =
        compilation
        |> withOptions ["--checknulls"]
        |> (match o with | Optimize -> withOptimize | DoNotOptimize -> withNoOptimize)
        |> withNoDebug
        |> withNoInterfaceData
        |> withNoOptimizationData
        |> asLibrary

    let verifyCompilation (o:Optimize) compilation =
        compilation
        |> addOptions o     
        |> verifyILBaseline

    [<Theory; FileInlineData("ModuleLevelBindings.fs")>]
    let ``Nullable attr for module bindings`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("ModuleLevelFunctions.fs")>]
    let ``Nullable attr for module functions`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("ModuleLevelFunctionsOpt.fs")>]
    let ``Nullable attr for module functions optimize`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation Optimize

    [<Theory; FileInlineData("CurriedFunctions.fs")>]
    let ``Nullable attr for curriedFunc optimize`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation Optimize

    [<Theory; FileInlineData("AnonRecords.fs")>]
    let ``Nullable attr for anon records`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("Records.fs")>]
    let ``Nullable attr for records with generics`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("PlainRecord.fs")>]
    let ``Nullable attr for plain records`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("ExceptionType.fs")>]
    let ``Nullable attr for exception types`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("ReferenceDU.fs")>]
    let ``Nullable attr for ref DUs`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("StructDU.fs")>]
    let ``Nullable attr for struct DUs`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("CustomType.fs")>]
    let ``Nullable attr for custom type`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("NullAsTrueValue.fs")>]
    let ``Nullable attr for Option clones`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("GenericStructDu.fs")>]
    let ``Generic struct DU`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("NullableInheritance.fs")>]
    let ``Nullable inheritance`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("CustomPipe.fs")>]
    let ``Custom pipe`` compilation =  
        compilation
        |> getCompilation
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("SupportsNull.fs")>]
    let ``SupportsNull`` compilation =  
        compilation
        |> getCompilation
        |> withNoWarn 52
        |> verifyCompilation DoNotOptimize

    [<Theory; FileInlineData("GenericCode.fs")>]
    let ``GenericCode`` compilation =  
        compilation
        |> getCompilation
        |> withNoWarn 52
        |> verifyCompilation DoNotOptimize

    [<Fact>]
    let ``Override missing in signature`` () =  
        FsFromPath (__SOURCE_DIRECTORY__ ++ "HasSignatureWithMissingOverride.fsi")
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "HasSignatureWithMissingOverride.fs"))
        |> withNoWarn 52
        |> addOptions DoNotOptimize
        |> compile
        |> withILContains 
            [".method public hidebysig virtual instance string ToString() cil managed"
             ".method public hidebysig virtual instance int32 GetHashCode() cil managed"
             "hidebysig virtual instance bool Equals(object obj) cil managed"
             ]
        |> shouldSucceed

    [<Theory; FileInlineData("NullableDowncasting.fs")>]
    let ``Downcasting and typetests`` compilation =  
        compilation
        |> getCompilation
        |> withNoWarn 52
        |> verifyCompilation DoNotOptimize

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NullableDowncasting.fs"|], BaselineSuffix = ".opt")>]
    let ``Downcasting and typetests optimized`` compilation =  
        compilation
        |> withNoWarn 52
        |> verifyCompilation Optimize


    module Interop  =
        open System.IO

        let fsharpLibCreator = 
            FSharp 
            >> asLibrary 
            >> withName "MyFSharpLib" 
            >> withOptions ["--checknulls"]

        let csharpLibCompile fsLibReference = 
            CSharp 
            >> withReferences [fsLibReference] 
            >> withCSharpLanguageVersion CSharpLanguageVersion.Preview
            >> asLibrary
            >> withName "CsharpAppConsumingNullness"
            >> compile

        let FsharpFromFile filename = 
            Path.Combine(__SOURCE_DIRECTORY__, filename)
            |> File.ReadAllText
            |> fsharpLibCreator

        [<Fact>]
        let ``Csharp understands option like type using UseNullAsTrueValue`` () = 
            let csharpCode = """
using System;
using static TestModule;
using static Microsoft.FSharp.Core.FuncConvert;
#nullable enable
public class C {
    // MyNullableOption has [<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>] applied on it
    public void M(MyNullableOption<string> customOption) {

        Console.WriteLine(customOption.ToString());  // should not warn

        var thisIsNone = MyNullableOption<string>.MyNone; 
        Console.WriteLine(thisIsNone.ToString());  //   !! should warn !!

        var mapped = mapPossiblyNullable<string,string>(ToFSharpFunc<string,string>(x => x.ToString()), customOption); // should not warn, because null will not be passed
        var mapped2 = mapPossiblyNullable<string,string>(ToFSharpFunc<string,string>(x => x + ".."), thisIsNone); // should NOT warn for passing in none, this is allowed

        if(thisIsNone != null)
            Console.WriteLine(thisIsNone.ToString());  // should NOT warn

        if(customOption != null)
            Console.WriteLine(customOption.ToString());  // should NOT warn

        Console.WriteLine(MyOptionWhichCannotHaveNullInTheInside<string>.NewMyNotNullSome("").ToString());  // should NOT warn

    }
    }"""
            csharpCode
            |> csharpLibCompile (FsharpFromFile "NullAsTrueValue.fs")       
            |> withDiagnostics [ Warning 8602, Line 12, Col 27, Line 12, Col 37, "Dereference of a possibly null reference."]

        [<Fact>]
        let ``Csharp understands Fsharp-produced struct unions via IsXXX flow analysis`` () = 
            let csharpCode = """
#nullable enable
public class C {
    public void M() {
        var data = MyTestModule.MyStructDU.A;
        if(data.IsA){
            string thisMustWarn = data.nonNullableString;
            string? thisIsOk = data.nonNullableString;
            string? thisIsAlsoOk = data.nullableString;
            System.Console.Write(thisMustWarn + thisIsOk + thisIsAlsoOk);
        } else if(data.IsB){
            string thisMustBeOK = data.nonNullableString;
            System.Console.Write(thisMustBeOK.Length);
        } else if (data.IsC){
            string evenThoughItIsC_ThisMustStillFailBecauseFieldIsNullable = data.nullableString;
            System.Console.Write(evenThoughItIsC_ThisMustStillFailBecauseFieldIsNullable.Length);
        }

        var thisCreationIsBad = MyTestModule.MyStructDU.NewB(null);
        var thisCreationIsFine = MyTestModule.MyStructDU.NewC(null);

        System.Console.Write(thisCreationIsBad.ToString() + thisCreationIsFine.ToString() );
    }
    }"""
            csharpCode
            |> csharpLibCompile (FsharpFromFile "StructDU.fs")       
            |> withDiagnostics [
                        Warning 8600, Line 6, Col 35, Line 6, Col 57, "Converting null literal or possible null value to non-nullable type."
                        Warning 8600, Line 14, Col 78, Line 14, Col 97, "Converting null literal or possible null value to non-nullable type."
                        Warning 8602, Line 15, Col 34, Line 15, Col 89, "Dereference of a possibly null reference."
                        Warning 8625, Line 18, Col 62, Line 18, Col 66, "Cannot convert null literal to non-nullable reference type."]

        [<Fact>]
        let ``Csharp code understands Fsharp-produced generics`` () = 
            let fsharpcode = """
module MyFSharpLib
let stringTupleInOut(x:struct(string * string|null)) = x"""
            let fsLib = fsharpLibCreator fsharpcode
            let csharpCode = """
#nullable enable
public class C {
    public void M() {
        string? nullString = null;
        MyFSharpLib.stringTupleInOut(("a good string here",nullString));
    }
}"""
            csharpCode
            |> csharpLibCompile fsLib       
            |> withDiagnostics []

        [<Fact>]
        let ``Csharp code can work with annotated FSharp module`` () =
            Path.Combine(__SOURCE_DIRECTORY__,"CsharpConsumer.cs")
            |> File.ReadAllText
            |> csharpLibCompile (FsharpFromFile "ModuleLevelFunctions.fs")
            |> shouldFail
            |> withDiagnostics [
                        Error 29, Line 28, Col 20, Line 28, Col 61, "Cannot implicitly convert type 'int' to 'string'"
                        Warning 8625, Line 12, Col 74, Line 12, Col 78, "Cannot convert null literal to non-nullable reference type."
                        Warning 8604, Line 14, Col 88, Line 14, Col 113, "Possible null reference argument for parameter 'x' in 'string MyTestModule.nonNullableInputOutputFunc(string x)'."
                        Warning 8620, Line 19, Col 88, Line 19, Col 101, "Argument of type '(string?, string?, int, int, int, int)' cannot be used for parameter 'x' of type '(string, string?, int, int, int, int)' in '(string, string?, int, int, int, int) MyTestModule.genericValueTypeTest((string, string?, int, int, int, int) x)' due to differences in the nullability of reference types."
                        Warning 8604, Line 21, Col 84, Line 21, Col 114, "Possible null reference argument for parameter 'x' in 'string MyTestModule.nonNullableInputOutputFunc(string x)'."
                        Warning 8604, Line 24, Col 60, Line 24, Col 70, "Possible null reference argument for parameter 'x_0' in 'Tuple<string, string?, int, int, int, int> MyTestModule.genericRefTypeTest(string x_0, string? x_1, int x_2, int x_3, int x_4, int x_5)'."
                        Warning 8604, Line 25, Col 45, Line 25, Col 59, "Possible null reference argument for parameter 'x_0' in 'Tuple<string, string?, int, int, int, int> MyTestModule.genericRefTypeTest(string x_0, string? x_1, int x_2, int x_3, int x_4, int x_5)'."
                        Warning 8625, Line 28, Col 51, Line 28, Col 55, "Cannot convert null literal to non-nullable reference type."]