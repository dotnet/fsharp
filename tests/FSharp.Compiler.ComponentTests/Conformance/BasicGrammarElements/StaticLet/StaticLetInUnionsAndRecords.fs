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
    |> withDiagnosticMessageMatches "For F#7 and lower, static 'let','do' and 'member val' definitions may only be used in types with a primary constructor.*"

[<Theory>]
[<InlineData("7.0")>]
[<InlineData("8.0")>]
[<InlineData("preview")>]
let ``Regression in Member val  - not allowed without primary constructor``  (langVersion:string) = 
    Fs """module Test
type Bad3 = 
    member val X = 1 + 1   """
    |> withLangVersion langVersion
    |> typecheck
    |> shouldFail
    |> withDiagnostics [Error 3133, Line 3, Col 5, Line 3, Col 25, "'member val' definitions are only permitted in types with a primary constructor. Consider adding arguments to your type definition, e.g. 'type X(args) = ...'."]


[<Theory>]
[<InlineData("7.0")>]
[<InlineData("8.0")>]
[<InlineData("preview")>]
let ``Regression - Type augmentation with abstract slot not allowed`` (langVersion:string) =
    Fs """module Test
type System.Random with
       abstract M : int -> int
       static member Factory() = 1    """
    |> withLangVersion langVersion
    |> typecheck
    |> shouldFail
    |> withDiagnostics [Error 912, Line 3, Col 8, Line 3, Col 31, "This declaration element is not permitted in an augmentation"]

[<Theory>]
[<InlineData("7.0")>]
[<InlineData("8.0")>]
[<InlineData("preview")>]
let ``Regression - record with abstract slot not allowed`` (langVersion:string) =
    Fs """module Test
type myRecord2 = { field1: int; field2: string }
  with abstract member AbstractMemberNotAllowedInAugmentation : string -> string end    """
    |> withLangVersion langVersion
    |> typecheck
    |> shouldFail
    |> withDiagnostics [Error 912, Line 3, Col 8, Line 3, Col 81, "This declaration element is not permitted in an augmentation"]

let verifyCompileAndRun compilation =
    compilation
    |> withLangVersion80
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

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecordOptimizerRegression.fs"|])>]
let ``Static let - record optimizer regression`` compilation =
    compilation
    |> withOptimize
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains """{ xyz = "" }"""



[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"LowercaseDuTest.fs"|])>]
let ``Static let - lowercase DU`` compilation =
    compilation
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed    

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

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StaticMemberValInEmptyType.fs"|])>]
let ``Static member val in empty type`` compilation =
    compilation
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StaticMemberValInEmptyType.fs"|])>]
let ``Static member val in empty type Fsharp 7`` compilation =
    compilation
    |> withLangVersion70
    |> typecheck
    |> shouldFail
    |> withDiagnostics [Error 902, Line 4, Col 5, Line 4, Col 41, "For F#7 and lower, static 'let','do' and 'member val' definitions may only be used in types with a primary constructor ('type X(args) = ...'). To enable them in all other types, use language version '8' or higher."]

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SimpleUnion.fs"|])>]
let ``Static let in simple union`` compilation =
    compilation
    |> verifyCompileAndRun
    |> shouldSucceed
    |> withStdOutContains "A 42"

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlainEnum.fs"|])>]
let ``Support in plain enums - typecheck should fail`` compilation =
    compilation
    |> withLangVersion80
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
    |> withDiagnostics [Error 3573, Line 4, Col 5, Line 4, Col 51, "Static bindings cannot be added to extrinsic augmentations. Consider using a 'static member' instead."]
    
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
    |> withLangVersion80
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


[<Fact>]
let ``Static let in DU in penultimate file`` () =
    // This file is included in the compilation, but its types are not access => statics are not executed
    let firstFile = """
module TypesWhichAreNotAccessed

do printfn "TypesWhichAreNotAccessed module 'do'"

type NotAccessedType =
    | Case19
    | Case40 of int

    static do printfn "NotAccessedType 'do'"


"""

    let types =
        """
module MyTypes

do printfn "MyTypes module 'do'"

type U =
    | Case1
    | Case2 of int

    static do printfn "MyTypes.U 'do'"
    static let u1 = 
        do printfn "creating MyTypes.U.u1 now"
        Case2 1
    static member U1 = u1


do printfn "MyTypes module 'do' no. 2"

module InnerModuleNotAccess = 
    do printfn "InnerModuleNotAccess 'do'"
    let someVal = ""
        """

    let program =
        """
module Test

[<EntryPoint>]
let main _ = 
    do printfn "Before static access"
    let u1 = MyTypes.U.U1
    printfn "%A" u1
    0
        """

    FSharp firstFile
    |> withAdditionalSourceFiles [SourceCodeFileKind.Create("types.fs", types);SourceCodeFileKind.Create("program.fs", program)]
    |> verifyCompileAndRun
    |> withStdOutContains """Before static access
MyTypes module 'do'
MyTypes.U 'do'
creating MyTypes.U.u1 now
MyTypes module 'do' no. 2
InnerModuleNotAccess 'do'
Case2 1"""
   

[<Fact>]
let ``Static let IL init single file test withRealInternalSignatureOff`` () = 
    FSharp """
module Test
open System

do Console.WriteLine("module before type")
[<NoEquality;NoComparison>]
type X =
    static do Console.WriteLine("from type")
do Console.WriteLine("module after type")
"""
    |> withLangVersion80
    |> withRealInternalSignatureOff
    |> compile
    |> shouldSucceed
    |> verifyIL ["""
.class public abstract auto ansi sealed Test
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public X
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Test::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Test::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldstr      "module before type"
    IL_0005:  call       void [runtime]System.Console::WriteLine(string)
    IL_000a:  ldstr      "from type"
    IL_000f:  call       void [runtime]System.Console::WriteLine(string)
    IL_0014:  ldstr      "module after type"
    IL_0019:  call       void [runtime]System.Console::WriteLine(string)
    IL_001e:  ret
  } 

}"""]

[<Fact>]
let ``Static let IL init single file test withRealInternalSignatureOn`` () = 
    FSharp """
module Test
open System

do Console.WriteLine("module before type")
[<NoEquality;NoComparison>]
type X =
    static do Console.WriteLine("from type")
do Console.WriteLine("module after type")
"""
    |> withLangVersion80
    |> withRealInternalSignatureOn
    |> compile
    |> shouldSucceed
    |> verifyIL ["""
.class public abstract auto ansi sealed Test
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public X
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Test::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Test::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

    .method assembly specialname static void staticInitialization@() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldstr      "from type"
      IL_0005:  call       void [runtime]System.Console::WriteLine(string)
      IL_000a:  ret
    } 

  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Test::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Test::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldstr      "module before type"
    IL_0005:  call       void [runtime]System.Console::WriteLine(string)
    IL_000a:  call       void Test/X::staticInitialization@()
    IL_000f:  ldstr      "module after type"
    IL_0014:  call       void [runtime]System.Console::WriteLine(string)
    IL_0019:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void Test::staticInitialization@()
    IL_0005:  ret
  } 

} 
"""]

[<Fact>]
let ``Static let in penultimate file IL test withRealInternalSignatureOff`` () =
    let types = """
namespace MyTypes
open System

[<NoEquality;NoComparison>]
type X =
    static do Console.WriteLine("from type")
    static let mutable x_value = 42
    static member GetX = x_value

"""

    let program = """
module ProgramMain
open System
Console.Write(MyTypes.X.GetX)
"""

    FSharp types
    |> withAdditionalSourceFiles [SourceCodeFileKind.Create("program.fs", program)]
    |> withLangVersion80
    |> withRealInternalSignatureOff
    |> compile
    |> shouldSucceed
    |> verifyIL ["""
.class public auto ansi serializable MyTypes.X
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
  .field static assembly int32 x_value
  .field static assembly int32 init@6
  .method public specialname static int32 
          get_GetX() cil managed
  {
    
    .maxstack  8
    IL_0000:  volatile.
    IL_0002:  ldsfld     int32 MyTypes.X::init@6
    IL_0007:  ldc.i4.0
    IL_0008:  bge.s      IL_0011

    IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
    IL_000f:  br.s       IL_0011

    IL_0011:  ldsfld     int32 MyTypes.X::x_value
    IL_0016:  ret
  } 

  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Test::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Test::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .property int32 GetX()
  {
    .get int32 MyTypes.X::get_GetX()
  } 
} 

.class public abstract auto ansi sealed ProgramMain
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$ProgramMain
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       int32 MyTypes.X::get_GetX()
    IL_0005:  call       void [runtime]System.Console::Write(int32)
    IL_000a:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldstr      "from type"
    IL_0005:  call       void [runtime]System.Console::WriteLine(string)
    IL_000a:  ldc.i4.s   42
    IL_000c:  stsfld     int32 MyTypes.X::x_value
    IL_0011:  ldc.i4.0
    IL_0012:  volatile.
    IL_0014:  stsfld     int32 MyTypes.X::init@6
    IL_0019:  ret
  } 

}"""]

[<Fact>]
let ``Static let in penultimate file IL test withRealInternalSignatureOn`` () =
    let types = """
namespace MyTypes
open System

[<NoEquality;NoComparison>]
type X =
    static do Console.WriteLine("from type")
    static let mutable x_value = 42
    static member GetX = x_value

"""

    let program = """
module ProgramMain
open System
Console.Write(MyTypes.X.GetX)
"""

    FSharp types
    |> withAdditionalSourceFiles [SourceCodeFileKind.Create("program.fs", program)]
    |> withLangVersion80
    |> withRealInternalSignatureOn
    |> compile
    |> shouldSucceed
    |> verifyIL ["""
.class public auto ansi serializable MyTypes.X
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
  .field static assembly int32 x_value
  .field static assembly int32 init@6
  .method public specialname static int32 get_GetX() cil managed
  {
    
    .maxstack  8
    IL_0000:  volatile.
    IL_0002:  ldsfld     int32 MyTypes.X::init@6
    IL_0007:  ldc.i4.0
    IL_0008:  bge.s      IL_0011

    IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
    IL_000f:  br.s       IL_0011

    IL_0011:  ldsfld     int32 MyTypes.X::x_value
    IL_0016:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Test::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Test::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldstr      "from type"
    IL_0005:  call       void [runtime]System.Console::WriteLine(string)
    IL_000a:  ldc.i4.s   42
    IL_000c:  stsfld     int32 MyTypes.X::x_value
    IL_0011:  ldc.i4.0
    IL_0012:  volatile.
    IL_0014:  stsfld     int32 MyTypes.X::init@6
    IL_0019:  ret
  } 

  .property int32 GetX()
  {
    .get int32 MyTypes.X::get_GetX()
  } 
} 

.class public abstract auto ansi sealed ProgramMain
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$ProgramMain::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$ProgramMain::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       int32 MyTypes.X::get_GetX()
    IL_0005:  call       void [runtime]System.Console::Write(int32)
    IL_000a:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$ProgramMain
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void ProgramMain::staticInitialization@()
    IL_0005:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void MyTypes.X::staticInitialization@()
    IL_0005:  ret
  }
"""]


[<Theory>]
[<InlineData("preview")>]
let ``Regression 16009 - module rec does not initialize let bindings`` langVersion =
    let moduleWithBinding = """
module rec Module

open System

let binding = 
    do Console.WriteLine("Asked for Module.binding")
    let b = Foo.StaticMember
    do Console.Write("isNull b in Module.binding after the call to Foo.StaticMember? ")
    do Console.WriteLine(isNull b)
    b

module NestedModule =
    let Binding =         
        do Console.WriteLine("Asked for NestedModule.Binding, before creating obj()")
        let b = new obj()
        do Console.Write("isNull b in NestedModule.Binding after 'new obj()'? ")
        do Console.WriteLine(isNull b)
        b

type Foo =
    static member StaticMember = 
        do Console.WriteLine("Asked for Foo.StaticMember")
        let b = NestedModule.Binding
        do Console.Write("isNull b in Foo.StaticMember after access to NestedModule.Binding? ")
        do Console.WriteLine(isNull b)
        b
"""

    let program = """
open Module
open System

do Console.WriteLine("Right before calling binding.ToString() in program.fs")
let b = binding
b.ToString() |> ignore
"""

    FSharp moduleWithBinding
    |> withAdditionalSourceFiles [SourceCodeFileKind.Create("program.fs", program)]
    |> withLangVersion langVersion
    |> asExe
    |> ignoreWarnings
    |> compileAndRun
    |> shouldSucceed 


[<Theory>]
[<InlineData("preview")>]
let ``Regression 16009 - as a single file program`` langVersion =
    let code = """
namespace MyProgram

module rec Module = 

    open System

    let binding = Foo.StaticMember

    module NestedModule =
        let Binding = new obj()

    type Foo =
        static member StaticMember = NestedModule.Binding
            
module ActualProgram = 
    open Module
    open System
    
    binding.ToString() |> Console.WriteLine
"""

    FSharp code
    |> withLangVersion langVersion
    |> asExe
    |> ignoreWarnings
    |> compileAndRun
    |> shouldSucceed  