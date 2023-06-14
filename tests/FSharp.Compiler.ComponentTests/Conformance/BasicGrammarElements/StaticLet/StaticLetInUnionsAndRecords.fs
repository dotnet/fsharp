module Conformance.BasicGrammarElements.StaticLet

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System.IO

let testCasesForFSharp7ErrorMessage = 
    Directory.EnumerateFiles(__SOURCE_DIRECTORY__) 
    |> Seq.toArray 
    |> Array.map Path.GetFileName
    |> Array.except [__SOURCE_FILE__;"PlainEnum.fs";"StaticLetExtensionToBuiltinType.fs"] // ALl files in the folder except this one with the tests
    |> Array.map (fun f -> [|f :> obj|])


[<Theory>]
[<MemberData(nameof(testCasesForFSharp7ErrorMessage))>]
let ``Should fail in F# 7 and lower`` (implFileName:string) =    
    let fileContents = File.ReadAllText (Path.Combine(__SOURCE_DIRECTORY__,implFileName))

    Fs fileContents     
    |> withLangVersion70
    |> typecheck
    |> shouldFail
    |> withErrorCode 902
    |> withDiagnosticMessageMatches "static value definitions may only be used in types with a primary constructor"

[<Theory>]
[<InlineData("7.0")>]
[<InlineData("preview")>]
let ``Member val regression - not allowed without primary constructor``  (langVersion:string) = 
    Fs """module Test
type Bad3 = 
    member val X = 1 + 1   """
    |> withLangVersion langVersion
    |> typecheck
    |> shouldFail
    |> withDiagnostics []


[<Theory>]
[<InlineData("7.0")>]
[<InlineData("preview")>]
let ``Type augmentation with abstract slot not allowed`` (langVersion:string) =
    Fs """module Test
type System.Random with
       abstract M : int -> int
       static member Factory() = 1    """
    |> withLangVersion langVersion
    |> typecheck
    |> shouldFail
    |> withDiagnostics [Error 912, Line 3, Col 8, Line 3, Col 31, "This declaration element is not permitted in an augmentation"]

let verifyCompileAndRun compilation =
    compilation
    |> withLangVersionPreview
    |> asExe
    |> compileAndRun


[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UnionShowCase.fs"|])>]
let ``Static let - union showcase`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """init U 1
init U 2
init end
Case2 1
Case2 2"""


[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecordShowCase.fs"|])>]
let ``Static let - record showcase`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """init R 1
init R 2
1
2"""

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SimpleEmptyType.fs"|])>]
let ``Static let in empty type`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """init
5
3"""

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SimpleEmptyGenericType.fs"|])>]
let ``Static let in empty generic type`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """Accessing name for Int32
Accessing name for String
Accessing name for Byte"""

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SimpleUnion.fs"|])>]
let ``Static let in simple union`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "A 42"

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlainEnum.fs"|])>]
let ``Support in plain enums - typecheck should fail`` compilation =
    compilation
    |> withLangVersionPreview
    |> typecheck    
    |> shouldFail
    |> withDiagnosticMessage "Enumerations cannot have members"

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ActivePatternForUnion.fs"|])>]
let ``Static active pattern in  union`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """B 999"""

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StructUnion.fs"|])>]
let ``Static let in struct union`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "A 42"

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SimpleRecord.fs"|])>]
let ``Static let in simple record`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "7"


[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StructRecord.fs"|])>]
let ``Static let in struct record`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "7"
    
[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CreateUnionCases.fs"|])>]
let ``Static let creating DU cases`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "..."

    
[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UnionOrderOfExecution.fs"|])>]
let ``Static let union - order of execution`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """init type U
side effect in let binding case2cachedVal
Before accessing type
side effect in member Singleton
calling print Case2 42
side effect in member Singleton
calling print Case2 42"""
    
[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecordOrderOfExecution.fs"|])>]
let ``Static let record - order of execution`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """init R 1
side effect in let binding R1
Before accessing type
side effect in member R1
calling print 1
side effect in member R1
calling print 1"""

    
[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecursiveDUs.fs"|])>]
let ``Static let - recursive DU definitions calling each other`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """DuA init
DuA allVals access
DuB init
DuB allVals access
DuC init
DuC allVals access
total = 999
uniq = 2"""

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecursiveRecords.fs"|])>]
let ``Static let - recursive record definitions calling each other`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """Chicken init
creating firstEggEver
Egg init
Omelette init
1"""

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"QuotationsForStaticLetUnions.fs"|])>]
let ``Static let - quotations support for unions`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """Let (s, Value ("A"), Call (None, ofString, [s]))"""


[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StaticLetExtensionToBuiltinType.fs"|])>]
let ``Static let extension to builtin type`` compilation =
    compilation
    |> typecheck
    |> shouldFail
    |> withDiagnostics [Error 3567, Line 4, Col 5, Line 4, Col 51, "Static bindings cannot be added to extrinsic augmentations. Consider using a 'static member' instead."]
    
[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"QuotationsForStaticLetRecords.fs"|])>]
let ``Static let - quotations support for records`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "Let (v1, Value (42), Lambda (r, Call (None, createRecord, [v1, r])))"


[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StaticLetInGenericUnion.fs"|])>]
let ``Static let union - executes per generic struct typar`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """Creating cached val for Int32 * Int32
sizeof MyUnion<int,int> = 12
Creating cached val for Int32 * String
sizeof MyUnion<int,string> = 16
Creating cached val for String * String
sizeof MyUnion<string,string> = 24"""


[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StaticLetInGenericRecords.fs"|])>]
let ``Static let record - executes per generic typar`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """Creating cached val for Int32
2x sizeof<int> = 8
Creating cached val for String
2x sizeof<string> = 16
Creating cached val for DateTime
2x sizeof<System.DateTime> = 16
Creating cached val for Uri
2x sizeof<System.Uri> = 16"""

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StaticLetInGenericRecordsILtest.fs"|])>]
let ``Static let record - generics - IL test`` compilation =
    compilation
    |> withLangVersionPreview
    |> compile
    |> verifyIL ["""        .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldtoken    !T
      IL_0005:  call       class [netstandard]System.Type [netstandard]System.Type::GetTypeFromHandle(valuetype [netstandard]System.RuntimeTypeHandle)
      IL_000a:  callvirt   instance string [runtime]System.Reflection.MemberInfo::get_Name()
      IL_000f:  call       void [runtime]System.Console::WriteLine(string)
      IL_0014:  ldtoken    !T
      IL_0019:  call       class [netstandard]System.Type [netstandard]System.Type::GetTypeFromHandle(valuetype [netstandard]System.RuntimeTypeHandle)
      IL_001e:  callvirt   instance string [runtime]System.Reflection.MemberInfo::get_Name()
      IL_0023:  stsfld     string class Test/MyRecord`1<!T>::cachedVal
      IL_0028:  ldc.i4.0
      IL_0029:  volatile.
      IL_002b:  stsfld     int32 class Test/MyRecord`1<!T>::init@7
      IL_0030:  ret
    } 

    .method public static string  GetMyName() cil managed
    {
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 class Test/MyRecord`1<!T>::init@7
      IL_0007:  ldc.i4.0
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     string class Test/MyRecord`1<!T>::cachedVal
      IL_0016:  ret
    } """]
   