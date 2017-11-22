#if INTERACTIVE
#r "../../Debug/fcs/net45/FSharp.Compiler.Service.dll" // note, run 'build fcs debug' to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../packages/NUnit.3.5.0/lib/net45/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.Service.StructureTests
#endif

open System.IO
open NUnit.Framework
open Microsoft.FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Service.Tests.Common
open System.Text

let fileName = Path.Combine (__SOURCE_DIRECTORY__, __SOURCE_FILE__)
type Line = int
type Col = int

let (=>) (source: string) (expectedRanges: (Line * Col * Line * Col) list) =
    let lines =
        use reader = new StringReader(source)
        [| let line = ref (reader.ReadLine())
           while not (isNull !line) do
               yield !line
               line := reader.ReadLine()
           if source.EndsWith "\n" then
               // last trailing space not returned
               // http://stackoverflow.com/questions/19365404/stringreader-omits-trailing-linebreak
               yield "" |]

    let formatList (xs: _ list) =
        let sb = StringBuilder("[ ")
        for r in xs do
            sb.AppendLine (sprintf "%A" r) |> ignore
        sprintf "%O ]" sb

    let ast = parseSourceCode(fileName, source)
    try
        match ast with
        | Some tree ->
            let actual =
                Structure.getOutliningRanges lines tree
                |> Seq.filter (fun sr -> sr.Range.StartLine <> sr.Range.EndLine)
                |> Seq.map (fun r ->  r.Range.StartLine, r.Range.StartColumn, r.Range.EndLine, r.Range.EndColumn)
                |> Seq.sort
                |> List.ofSeq
            let expected = List.sort expectedRanges
            if actual <> expected then
            failwithf "Expected %s, but was %s" (formatList expected) (formatList actual)
        | None -> failwithf "Expected there to be a parse tree for source:\n%s" source
    with _ ->
        printfn "AST:\n%+A" ast
        reraise()

[<Test>]
let ``empty file``() = "" => [ (1, 0, 2, 0) ]

[<Test>]
let ``nested module``() =
    """
module MyModule =
    ()
"""
    => [ (1, 0, 4, 0)
         (2, 0, 3, 6) ]

[<Test>]
let ``module with multiline function``() =
    """
module MyModule =
    let foo() =
        foo()
"""
    => [ (1, 0, 5, 0)
         (2, 0, 4, 13)
         (3, 4, 4, 13)
         (3, 8, 4, 13) ]

[<Test>]
let ``DU``() =
    """
type Color =
    | Red
    | Green
    | Blue
"""
    => [ (1, 0, 6, 0)
         (2, 5, 5, 10)
         (3, 4, 5, 10) ]

[<Test>]
let ``DU with interface``() =
    """
type Color =
    | Red
    | Green
    | Blue

    interface IDisposable with
        member __.Dispose() =
            (docEventListener :> IDisposable).Dispose()
"""
    => [ (1, 0, 10, 0)
         (2, 5, 9, 55)
         (3, 4, 5, 10)
         (7, 4, 9, 55)
         (8, 15, 9, 55)
         (8, 15, 9, 55) ]

[<Test>]
let ``record with interface``() =
    """
type Color =
    { Red: int
        Green: int
        Blue: int 
    }

    interface IDisposable with
        member __.Dispose() =
            (docEventListener :> IDisposable).Dispose()
"""
    =>
    [ (1, 0, 11, 0)
      (2, 5, 10, 55)
      (3, 4, 4, 14)
      (3, 6, 4, 13)
      (8, 4, 10, 55)
      (9, 15, 10, 55)
      (9, 15, 10, 55) ]

[<Test>]
let ``type with a do block``() =
    """
type Color() =   // 2
    let foo() =
        ()

    do
        foo()
        ()       // 8
"""
    => [ (1, 0, 9, 0)
         (2, 5, 8, 10)
         (3, 8, 4, 10)
         (6, 4, 8, 10) ]

[<Test>]
let ``complex outlining test``() =
    """
module MyModule =       // 2
    let foo() = ()
    let bar() =
        ()

    type Color =        // 7
        { Red: int
          Green: int
          Blue: int 
        }

        interface IDisposable with      // 13
            member __.Dispose() =
                (docEventListener :> IDisposable).Dispose()

    module MyInnerModule =              // 17

        type RecordColor =              // 19
            { Red: int
              Green: int
              Blue: int 
            }

            interface IDisposable with  // 25
                member __.Dispose() =
                    (docEventListener :> IDisposable).Dispose()
""" 
    => [ (1, 0, 28, 0)
         (2, 0, 27, 63)
         (4, 4, 5, 10)
         (4, 8, 5, 10)
         (7, 9, 15, 59)
         (8, 8, 11, 9)
         (13, 8, 15, 59)
         (14, 19, 15, 59)
         (14, 19, 15, 59)
         (17, 4, 27, 63)
         (19, 13, 27, 63)
         (20, 12, 23, 13)
         (25, 12, 27, 63)
         (26, 23, 27, 63)
         (26, 23, 27, 63) ]

    
[<Test>]
let ``open statements``() =
    """
open M             
open N             
                   
module M =         
    let x = 1      
                   
    open M         
    open N         
                   
    module M =     
        open M     
                   
        let x = 1  
                   
    module M =     
        open M     
        open N     
        let x = 1  
                   
open M             
open N             
open H             
                   
open G             
open H              
"""
    => [ (1, 0, 26, 6)
         (2, 5, 3, 6)
         (5, 0, 19, 17)
         (8, 9, 9, 10)
         (11, 4, 14, 17)
         (16, 4, 19, 17)
         (17, 13, 18, 14)
         (21, 5, 26, 6) ]

[<Test>]
let ``hash directives``() =
    """
#r @"a"   
#r "b"    
          
#r "c"    
          
#r "d"    
#r "e"    
let x = 1 
          
#r "f"    
#r "g"    
#load "x" 
#r "y"    
          
#load "a" 
      "b" 
      "c" 
          
#load "a" 
      "b" 
      "c" 
#r "d"     
"""
    => [ (1, 0, 23, 6)
         (2, 3, 8, 6)
         (11, 3, 23, 6) ]

[<Test>]
let ``nested let bindings``() =
    """
let f x =       // 2
    let g x =   // 3
        let h = // 4
            ()  // 5
        ()      // 6
    x           // 7
"""
    => [ (1, 0, 8, 0)
         (2, 0, 7, 5)
         (2, 4, 7, 5)
         (3, 8, 6, 10)
         (4, 12, 5, 14) ]

[<Test>]
let ``match``() =
    """
match None with     // 2
| Some _ ->         // 3
    ()              // 4
| None ->           // 5
    match None with // 6
    | Some _ -> ()  // 7
    | None ->       // 8
        let x = ()  // 9
        ()          // 10
"""
    => [ (1, 0, 11, 0)
         (2, 0, 10, 10)
         (6, 4, 10, 10)
         (6, 4, 10, 10)
         (9, 8, 10, 10) ]
         
[<Test>]
let ``computation expressions``() =
    """
seq {              // 2
    yield ()       // 3
    let f x =      // 4
        ()         // 5
    yield! seq {   // 6
        yield () } // 7
}                  // 8
"""
    => [ (1, 0, 8, 1)
         (2, 0, 8, 1)
         (4, 8, 5, 10)
         (6, 4, 7, 18)
         (6, 11, 7, 18) ]

[<Test>]
let ``list``() =
    """
let _ = 
    [ 1; 2
      3 ]
"""
  => [ (1, 0, 5, 0)
       (2, 0, 4, 9)
       (2, 4, 4, 9)
       (3, 4, 4, 9) ]

[<Test>]
let ``object expressions``() =
    """
let _ =
    { new System.IDisposable with
        member __.Dispose() = () }
"""
    => [ (1, 0, 5, 0)
         (2, 0, 4, 34)
         (2, 4, 4, 34)
         (3, 4, 4, 34) ]
         
[<Test>]
let ``try - with``() =
    """
try           // 2
    let f x = // 3
        ()    // 4
with _ ->     // 5
    let f x = // 6
        ()    // 7
    ()        // 8
"""
    => [ (1, 0, 9, 0)
         (2, 0, 5, 0)
         (2, 0, 8, 6)
         (3, 8, 4, 10)
         (5, 0, 8, 6)
         (6, 4, 8, 6)
         (6, 8, 7, 10) ]

[<Test>]
let ``try - finally``() =
    """
try           // 2
    let f x = // 3
        ()    // 4
finally       // 5
    let f x = // 6
        ()    // 7
    ()        // 8
"""
    => [ (1, 0, 9, 0)
         (2, 0, 8, 6)
         (3, 8, 4, 10)
         (5, 0, 8, 6)
         (6, 8, 7, 10) ]

[<Test>]
let ``if - then - else``() =
    """
if true then
    let f x = 
        ()
    ()
else
    let f x =
        ()
    ()
"""
    => [ (1, 0, 10, 0)
         (2, 0, 9, 6)
         (2, 8, 5, 6)
         (3, 8, 4, 10)
         (7, 8, 8, 10) ]

[<Test>]
let ``code quotation``() =
    """
<@
  "code"
        @>
"""
    => [ 1, 0, 4, 10
         2, 0, 4, 10 ]

[<Test>]
let ``raw code quotation``() =
    """
<@@
  "code"
        @@>
"""
    => [ (1, 0, 4, 11)
         (2, 0, 4, 11) ]

[<Test>]
let ``match lambda aka function``() =
    """
function
| 0 ->  ()
        ()
"""
    => [ (1, 0, 5, 0)
         (2, 0, 4, 10)
         (3, 8, 4, 10) ]

[<Test>]
let ``match guarded clause``() =
    """
let matchwith num =
    match num with
    | 0 -> ()
           ()
"""
    =>  [ (1, 0, 6, 0)
          (2, 0, 5, 13)
          (2, 4, 5, 13)
          (3, 4, 5, 13)
          (4, 11, 5, 13) ]

[<Test>]
let ``for loop``() =
    """
for x = 100 downto 10 do
    ()
    ()
"""
    => [ (1, 0, 5, 0)
         (2, 0, 4, 6) ]

[<Test>]
let ``for each``() =
    """
for x in 0 .. 100 -> 
            ()
            ()
"""
    =>  [ (1, 0, 5, 0)
          (2, 0, 4, 14)
          (2, 18, 4, 14) ]
   
[<Test>]
let ``tuple``() =
    """
( 20340
, 322
, 123123 )
"""
    => [ (1, 0, 4, 10)
         (2, 2, 4, 8) ]

[<Test>]
let ``do!``() =
    """
do! 
    printfn "allo"
    printfn "allo"
"""
    =>  [(1, 0, 5, 0)
         (2, 0, 4, 18)]

[<Test>]
let ``cexpr yield yield!``() =
    """
cexpr{
    yield! 
        cexpr{
                    yield 
                                
                        10
                }
    }
"""
    =>  [1, 0, 9, 5
         2, 0, 9, 5
         3, 4, 8, 17
         4, 8, 8, 17
         5, 20, 7, 26]

[<Test>]
let ``XML doc comments``() =
    """
/// Line 1
/// Line 2
module M =
    /// Line 3
    /// Line 4
    type T() =
        /// Line 5
        /// Line 6
        /// Line 7
        let f x = x
    /// Single line comment
    let f x = x
"""
    => [ (1, 0, 14, 0)
         (2, 0, 3, 10)
         (4, 0, 13, 15)
         (5, 4, 6, 14)
         (7, 9, 11, 19)
         (8, 8, 10, 18) ]
         
[<Test>]
let ``regular comments``() =
    """
// Line 1
// Line 2
module M =
    // Line 3
    // Line 4
    type T() =
        // Line 5
        // Line 6
        // Line 7
        let f x = x
    // Single line comment
    let f x = x
"""
    => [ (1, 0, 14, 0)
         (2, 0, 3, 9)
         (4, 0, 13, 15)
         (5, 4, 6, 13)
         (7, 9, 11, 19)
         (8, 8, 10, 17) ]
         
[<Test>]
let ``XML doc and regular comments in one block``() =
    """
// Line 1
// Line 2
/// Line 3
/// Line 4
// Line 5
/// Line 6
/// Line 7
/// Line 8
/// Line 9
"""
    => [ (1, 0, 11, 0)
         (2, 0, 3, 9)
         (4, 0, 5, 10)
         (7, 0, 10, 10) ]