module Language.NullableRegressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

let withVersionAndCheckNulls (version,checknulls) cu =
    cu
    |> withLangVersion version
    |> withWarnOn 3261
    |> withOptions ["--warnaserror+"]
    |> if checknulls then withCheckNulls else id

    
[<Theory>]
[<InlineData("preview",true)>]
[<InlineData("preview",false)>]
[<InlineData("8.0",false)>]
let ``Micro compilation`` langVersion checknulls =

    FsFromPath (__SOURCE_DIRECTORY__ ++ "micro.fsi")
    |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "micro.fs"))
    |> withLangVersion langVersion
    |> fun x -> 
        if checknulls then 
            x |> withCheckNulls |> withDefines ["CHECKNULLS"]
        else x
    |> compile
    |> shouldSucceed

[<Theory>]
[<InlineData("preview",true)>]
let ``Signature conformance`` langVersion checknulls =

    FsFromPath (__SOURCE_DIRECTORY__ ++ "signatures.fsi")
    |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "signatures.fs"))
    |> withLangVersion langVersion
    |> fun x -> 
        if checknulls then 
            x |> withCheckNulls |> withDefines ["CHECKNULLS"]
        else x
    |> compile
    |> shouldFail
    |> withDiagnostics
        [Warning 3262, Line 18, Col 48, Line 18, Col 60, "Value known to be without null passed to a function meant for nullables: You can create 'Some value' directly instead of 'ofObj', or consider not using an option for this value."
         (Warning 3261, Line 4, Col 5, Line 4, Col 10, "Nullness warning: Module 'M' contains
            val test2: x: string | null -> unit    
        but its signature specifies
            val test2: string -> unit    
        The types differ in their nullness annotations");
        (Warning 3261, Line 3, Col 5, Line 3, Col 10, "Nullness warning: Module 'M' contains
            val test1: x: string -> unit    
        but its signature specifies
            val test1: string | null -> unit    
        The types differ in their nullness annotations");
        (Warning 3261, Line 6, Col 5, Line 6, Col 17, "Nullness warning: Module 'M' contains
            val iRejectNulls: x: string | null -> string    
        but its signature specifies
            val iRejectNulls: string -> string    
        The types differ in their nullness annotations");
        (Warning 3261, Line 14, Col 14, Line 14, Col 21, "Nullness warning: Module 'M' contains
            member GenericContainer.GetNull: unit -> 'T    
        but its signature specifies
            member GenericContainer.GetNull: unit -> 'T | null    
        The types differ in their nullness annotations");
        (Warning 3261, Line 15, Col 14, Line 15, Col 24, "Nullness warning: Module 'M' contains
            member GenericContainer.GetNotNull: unit -> 'T | null    
        but its signature specifies
            member GenericContainer.GetNotNull: unit -> 'T    
        The types differ in their nullness annotations")]

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"existing-positive.fs"|])>]
let ``Existing positive v8 disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("8.0",false)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"existing-positive.fs"|])>]
let ``Existing positive vPreview disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".checknulls_on", Includes=[|"existing-positive.fs"|])>]
let ``Existing positive vPreview enabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",true)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"existing-negative.fs"|])>]
let ``Existing negative v8 disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("8.0",false)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"existing-negative.fs"|])>]
let ``Existing negative vPreview disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".checknulls_on", Includes=[|"existing-negative.fs"|])>]
let ``Existing negative vPreview enabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",true)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"library-functions.fs"|])>]
let ``Library functions nullness disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".checknulls_on", Includes=[|"library-functions.fs"|])>]
let ``Library functions nullness enabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",true)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"using-nullness-syntax-positive.fs"|])>]
let ``With new nullness syntax nullness disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"positive-defaultValue-bug.fs"|])>]
let ``DefaultValueBug when checknulls is disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".checknulls_on", Includes=[|"using-nullness-syntax-positive.fs"|])>]
let ``With new nullness syntax nullness enabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",true)   
    |> verifyBaseline



[<Theory>]
[<InlineData("preview",true,true)>]
[<InlineData("preview",true,false)>]
[<InlineData("preview",false,true)>]
[<InlineData("preview",false,false)>]
[<InlineData("8.0",false,false)>]
[<InlineData("8.0",false,true)>]
let ``DefaultValue regression`` (version,checknulls,fullCompile) = 
    FSharp $"""
module MyLib

[<Struct;NoComparison;NoEquality>]
type C7 =
    [<DefaultValue>]
    val mutable Whoops : (int -> int) {if version="preview" then " | null" else ""} // no warnings in checknulls+
    """
    |> asLibrary
    |> withVersionAndCheckNulls (version,checknulls)
    |> (if fullCompile then compile else typecheck)
    |> fun x -> 
        if checknulls then 
            x |> shouldSucceed
        else 
            x
            |> shouldFail
            |> withDiagnostics 
                [(Error 444, Line 7, Col 17, Line 7, Col 23, "The type of a field using the 'DefaultValue' attribute must admit default initialization, i.e. have 'null' as a proper value or be a struct type whose fields all admit default initialization. You can use 'DefaultValue(false)' to disable this check")]
