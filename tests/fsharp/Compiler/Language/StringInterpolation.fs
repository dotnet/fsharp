// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.UnitTests.Utilities

[<TestFixture>]
module StringInterpolationTests =

    [<Test>]
    let ``Basic string interpolation`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

check "vcewweh1" $"this is 2" "this is 2"

check "vcewweh2" $"this is {1} + 1 = 2" "this is 1 + 1 = 2"

check "vcewweh3" $"this is {1} + {1+1} = 3"  "this is 1 + 2 = 3"

check "vcewweh4" $"this is {1} + {1+1}"  "this is 1 + 2"

check "vcewweh5" $"this is {1}"  "this is 1"

check "vcewweh6" $"123{456}789{012}345"  "12345678912345"

check "vcewweh7" $"123{456}789{"012"}345"  "123456789012345"

            """

    [<Test>]
    let ``Basic string interpolation verbatim strings`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

check "xvcewweh1" @$"this is 2" "this is 2"

check "xvcewweh2" @$"this is {1} + 1 = 2" "this is 1 + 1 = 2"

check "xvcewweh3" @$"this is {1} + {1+1} = 3"  "this is 1 + 2 = 3"

check "xvcewweh4" @$"this is {1} + {1+1}"  "this is 1 + 2"

check "xvcewweh5" @$"this is {1}"  "this is 1"

check "xvcewweh6" @$"this i\s {1}"  "this i\s 1"

check "xvcewweh1b" $@"this is 2" "this is 2"

check "xvcewweh2b" $@"this is {1} + 1 = 2" "this is 1 + 1 = 2"

check "xvcewweh3b" $@"this is {1} + {1+1} = 3"  "this is 1 + 2 = 3"

check "xvcewweh4b" $@"this is {1} + {1+1}"  "this is 1 + 2"

check "xvcewweh5b" $@"this is {1}"  "this is 1"

check "xvcewweh6b" $@"this i\s {1}"  "this i\s 1"

            """

    [<Test>]
    let ``Basic string interpolation triple quote strings`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            "
let check msg a b = 
    if a = b then printfn \"%s succeeded\" msg else failwithf \"%s failed, expected %A, got %A\" msg b a

check \"xvcewweh1\" $\"\"\"this is 2\"\"\" \"this is 2\"

check \"xvcewweh2\" $\"\"\"this is {1} + 1 = 2\"\"\" \"this is 1 + 1 = 2\"

check \"xvcewweh3\" $\"\"\"this is {1} + {1+1} = 3\"\"\"  \"this is 1 + 2 = 3\"

check \"xvcewweh4\" $\"\"\"this is {1} + {1+1}\"\"\"  \"this is 1 + 2\"

check \"xvcewweh5\" $\"\"\"this is {1}\"\"\"  \"this is 1\"

check \"xvcewweh6\" $\"\"\"this i\s {1}\"\"\"  \"this i\s 1\"

// multiline
check \"xvcewweh6\"
    $\"\"\"this
is {1+1}\"\"\"
    \"\"\"this
is 2\"\"\"
            "

    [<Test>]
    let ``String interpolation using atomic expression forms`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
let x = 12
let s = "sixsix"
let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

check "vcewweh4" $"this is %d{1} + {1+1+3} = 6"  "this is 1 + 5 = 6"

check "vcewweh5" $"this is 0x%08x{x} + {1+1} = 14" "this is 0x0000000c + 2 = 14"

// Check dot notation
check "vcewweh6" $"this is {s.Length} + {1+1} = 8" "this is 6 + 2 = 8"

// Check null expression
check "vcewweh8" $"abc{null}def" "abcdef"

            """


    [<Test>]
    let ``String interpolation using nested control flow expressions`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

let x = 12
let s = "sixsix"

// Check let expression
check "vcewweh7" $"abc {let x = 3 in x + x} def" "abc 6 def"

// Check if expression (parenthesized)
check "vcewweh9" $"abc{(if true then 3 else 4)}def" "abc3def"

// Check if-then-else expression (un-parenthesized)
check "vcewweh10" $"abc{if true then 3 else 4}def" "abc3def"

// Check two if-then-else expression (un-parenthesized)
check "vcewweh11" $"abc{if true then 3 else 4}def{if false then 3 else 4}xyz" "abc3def4xyz"

// Check two if-then-else expression (un-parenthesized, first split)
check "vcewweh12" $"abc{if true then 3
                        else 4}def{if false then 3 else 4}xyz" "abc3def4xyz"

// Check two if-then-else expression (un-parenthesized, second split)
check "vcewweh13" $"abc{if true then 3 else 4}def{if false then 3
                                                  else 4}xyz" "abc3def4xyz"

// Check two if-then-else expression (un-parenthesized, both split)
check "vcewweh14" $"abc{if true then 3
                        else 4}def{if false then 3
                                   else 4}xyz" "abc3def4xyz"

// Check if-then expression (un-parenthesized)
check "vcewweh15" $"abc{if true then ()}def" "abcdef"

// Check two if-then expression (un-parenthesized)
check "vcewweh16" $"abc{if true then ()}def{if true then ()}xyz" "abcdefxyz"

// Check multi-line let with parentheses
check "fahweelvlo"
    $"abc {(let x = 3
            x + x)} def"
    "abc 6 def"

// Check multi-line let without parentheses
check "fahweelvlo3"
    $"abc {let x = 3
           x + x} def"
    "abc 6 def"

// Check multi-line let without parentheses (two)
check "fahweelvlo4"
    $"abc {let x = 3
           x + x} def {let x = 3
                       x + x} xyz"
    "abc 6 def 6 xyz"

// Check while expression (un-parenthesized)
check "vcewweh17" $"abc{while false do ()}def" "abcdef"

            """



    [<Test>]
    let ``String interpolation using nested string`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

// check nested string
check "vcewweh22m1" $"x = {"1"} " "x = 1 "

check "vcewweh22m2" $"x = {$""} " "x =  "

check "vcewweh22m3" $"x = {$"1 {"2" + $"{3}" + "2"} 1"} " "x = 1 232 1 "

check "vcewweh22m4" $"x = {$"1 {"2" + $"{3}" + "2"} 11 {2} 1"} " "x = 1 232 11 2 1 "

check "vcewweh22m5" $"x = {$"1 {"2" + $"{3}" + "2"} 11 {    $"{{%A{seq{yield 2; yield 2;}  }}}"  } 1"} " "x = 1 232 11 {seq [2; 2]} 1 "

do
    let genreSpecified = true
    let getGenre() = "comedy"
    check "vcewweh22m6" $"/api/movie/{if not genreSpecified then "" else $"q?genre={getGenre()}"}" "/api/movie/q?genre=comedy"

"""

    [<Test>]
    let ``Triple quote string interpolation using nested string`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            "
let check msg a b = 
    if a = b then printfn \"%s succeeded\" msg else failwithf \"%s failed, expected %A, got %A\" msg b a
do
    let itvar=\"i\"
    let iterfrom=\"0\"
    let iterto=\"100\"
    let block= $\"\"\"printf(\"%%d\", {itvar});
    do({itvar});\"\"\"
    check \"vcewweh22m7\" $\"\"\"
for({itvar}={iterfrom};{itvar}<{iterto};++{itvar}) {{
    {block}
}}\"\"\" \"\"\"
for(i=0;i<100;++i) {
    printf(\"%d\", i);
    do(i);
}\"\"\"
"

    [<Test>]
    let ``Mixed quote string interpolation using nested string`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            "
let check msg a b = 
    if a = b then printfn \"%s succeeded\" msg else failwithf \"%s failed, expected %A, got %A\" msg b a

check \"vcewweh22n1\" 
    $\"\"\"
    PROCEDURE SEARCH;
    BEGIN
    END;\"\"\" 
    \"\"\"
    PROCEDURE SEARCH;
    BEGIN
    END;\"\"\"

check \"vcewweh22n2\" 
    $\"\"\"
    PROCEDURE SEARCH;
    BEGIN
        WRITELN({ $\"{21+21}\" });
    END;\"\"\" 
    \"\"\"
    PROCEDURE SEARCH;
    BEGIN
        WRITELN(42);
    END;\"\"\"
"

    [<Test>]
    let ``String interpolation using .NET Formats`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

check "vcewweh221q" $"abc {1}" "abc 1"
check "vcewweh222w" $"abc {1:N3}" "abc 1.000"
check "vcewweh223e" $"abc {1,10}" "abc          1"
check "vcewweh223r" $"abc {1,-10}" "abc 1         "
check "vcewweh224t" $"abc {1,10:N3}" "abc      1.000"
check "vcewweh224y" $"abc {1,-10:N3}" "abc 1.000     "
check "vcewweh225u" $"abc %d{1}" "abc 1"
check "vcewweh225u" $"abc %5d{1}" "abc     1"
check "vcewweh225u" $"abc %-5d{1}" "abc 1    "
            """

    [<Test>]
    let ``String interpolation of null`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

check "vcewweh221q1" $"{null}" ""
check "vcewweh221q2" $"%s{null}" ""
check "vcewweh221q3" $"abc %s{null}" "abc "
check "vcewweh221q4" $"abc %s{null} def" "abc  def"
            """

    [<Test>]
    let ``String interpolation of basic types`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

check "vcewweh221q11" $"{1y}" "1"
check "vcewweh221q12" $"{1uy}" "1"
check "vcewweh221q13" $"{1s}" "1"
check "vcewweh221q14" $"{1us}" "1"
check "vcewweh221q15" $"{1u}" "1"
check "vcewweh221q16" $"{1}" "1"
check "vcewweh221q17" $"{-1}" "-1"
check "vcewweh221q18" $"{1L}" "1"
check "vcewweh221q19" $"{1UL}" "1"
check "vcewweh221q1q" $"{1n}" "1"
check "vcewweh221q1w" $"{1un}" "1"
check "vcewweh221q1e" $"{1.0}" "1"
check "vcewweh221q1r" $"{1.01}" "1.01"
check "vcewweh221q1t" $"{-1.01}" "-1.01"
check "vcewweh221q1y" $"{1I}" "1"
check "vcewweh221q1i" $"{1M}" "1"

check "vcewweh221q1o" $"%d{1y}" "1"
check "vcewweh221q1p" $"%d{1uy}" "1"
check "vcewweh221q1a" $"%d{1s}" "1"
check "vcewweh221q1s" $"%d{1us}" "1"
check "vcewweh221q1d" $"%d{1u}" "1"
check "vcewweh221q1f" $"%d{1}" "1"
check "vcewweh221q1g" $"%d{-1}" "-1"
check "vcewweh221q1h" $"%d{1L}" "1"
check "vcewweh221q1j" $"%d{1UL}" "1"
check "vcewweh221q1k" $"%d{1n}" "1"
check "vcewweh221q1l" $"%d{1un}" "1"

check "vcewweh221q1" $"%f{1.0}" "1.000000"
            """
    [<Test>]
    let ``String interpolation using escaped braces`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

check "vcewweh226i" $"{{" "{"
check "vcewweh226o" $"{{{{" "{{"
check "vcewweh226p" $"{{{1}}}" "{1}"
check "vcewweh227a" $"}}" "}"
check "vcewweh227s" $"}}}}" "}}"
check "vcewweh228d" "{{" "{{"
check "vcewweh229f" "}}" "}}"
            """

    [<Test>]
    let ``String interpolation using verbatim strings`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

check "vcewweh226i" $"{{" "{"
check "vcewweh226o" $"{{{{" "{{"
check "vcewweh226p" $"{{{1}}}" "{1}"
check "vcewweh227a" $"}}" "}"
check "vcewweh227s" $"}}}}" "}}"
check "vcewweh228d" "{{" "{{"
check "vcewweh229f" "}}" "}}"
            """


    [<Test>]
    let ``String interpolation using record data`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
type R = { X : int  }
type R2 = { X : int ; Y: int }

let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

// Check record expression (parenthesized)
check "vcewweh18" $"abc{({contents=1}.contents)}def" "abc1def"

// Check {{ }} expression (un-parenthesized)
check "vcewweh19" $"abc{{contents=1}.contents}def" "abc{contents=1}.contents}def"

// Check record expression (un-parenthesized)
check "vcewweh20" $"abc{{X=1}}def" "abc{X=1}def"

// Check {{ }} expression (un-parenthesized, multi-line)
check "vcewweh21" $"abc{{X=1; Y=2}}def" "abc{X=1; Y=2}def"

            """


    [<Test>]
    let ``String interpolation using printf formats`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

let x = 12
let s = "sixsix"

// check %A
check "vcewweh22" $"x = %A{1}" "x = 1"

// check %d
check "vcewweh22b" $"x = %d{1}" "x = 1"

// check %x
check "vcewweh22c" $"x = %x{1}" "x = 1"

// check %o (octal)
check "vcewweh22d" $"x = %o{15}" "x = 17"

// check %b
check "vcewweh22e" $"x = %b{true}" "x = true"

// check %s
check "vcewweh22f" $"x = %s{s}" "x = sixsix"

// check %A of string
check "vcewweh22g" $"x = %A{s}" "x = \"sixsix\""

// check nested string with %s
check "vcewweh22l" $"x = %s{"1"}" "x = 1"

check "vcewweh20" $"x = %A{1}" "x = 1"

            """


    [<Test>]
    let ``String interpolation using list and array data`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

// check unannotated of list
check "vcewweh22i" $"x = {[0..100]} " "x = [0; 1; 2; ... ] "

let xs = [0..100]
// check unannotated of list
check "vcewweh22i" $"x = {xs} " "x = [0; 1; 2; ... ] "

// check %A of list
check "vcewweh22h" $"x = %0A{[0..100]} " "x = [0; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 11; 12; 13; 14; 15; 16; 17; 18; 19; 20; 21; 22; 23; 24; 25; 26; 27; 28; 29; 30; 31; 32; 33; 34; 35; 36; 37; 38; 39; 40; 41; 42; 43; 44; 45; 46; 47; 48; 49; 50; 51; 52; 53; 54; 55; 56; 57; 58; 59; 60; 61; 62; 63; 64; 65; 66; 67; 68; 69; 70; 71; 72; 73; 74; 75; 76; 77; 78; 79; 80; 81; 82; 83; 84; 85; 86; 87; 88; 89; 90; 91; 92; 93; 94; 95; 96; 97; 98; 99; ...] "

// check unannotated of array
check "vcewweh22j" $"x = {[|0..100|]} " "x = System.Int32[] "

let arr = [|0..100|]
// check unannotated of array
check "vcewweh22j" $"x = {arr} " "x = System.Int32[] "

// check %0A of array
check "vcewweh22k" $"x = %0A{[|0..100|]} " "x = [|0; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 11; 12; 13; 14; 15; 16; 17; 18; 19; 20; 21; 22; 23; 24; 25; 26; 27; 28; 29; 30; 31; 32; 33; 34; 35; 36; 37; 38; 39; 40; 41; 42; 43; 44; 45; 46; 47; 48; 49; 50; 51; 52; 53; 54; 55; 56; 57; 58; 59; 60; 61; 62; 63; 64; 65; 66; 67; 68; 69; 70; 71; 72; 73; 74; 75; 76; 77; 78; 79; 80; 81; 82; 83; 84; 85; 86; 87; 88; 89; 90; 91; 92; 93; 94; 95; 96; 97; 98; 99; ...|] "

            """


    [<Test>]
    let ``String interpolation using anonymous records`` () =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
let check msg a b = 
    if a = b then printfn "%s succeeded" msg else failwithf "%s failed, expected %A, got %A" msg b a

// Check anonymous record expression (parenthesized)
check "vcewweh23" $"abc{({| A=1 |})}def" "abc{ A = 1 }def"

            """


    [<Test>]
    let ``Basic string interpolation (no preview)`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions  [| |]
            """
let x = $"one" 
            """
            [|(FSharpErrorSeverity.Error, 3350, (2, 9, 2, 15),
                   "Feature 'string interpolation' is not available in F# 4.7. Please use language version 'preview' or greater.")|]
