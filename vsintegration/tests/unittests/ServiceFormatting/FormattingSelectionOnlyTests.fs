module FSharp.Compiler.Service.Tests.ServiceFormatting.FormattingSelectionOnlyTests

open NUnit.Framework
open FsUnit
open TestHelper

[<Test>]
let ``should format a part of a line correctly``() =
    formatSelectionOnly false (makeRange 3 8 3 10) """
let x = 2 + 3
let y = 1+2
let z = x + y""" config
    |> should equal """1 + 2"""

[<Test>]
let ``should format a whole line correctly and preserve indentation``() =
    formatSelectionOnly false (makeRange 3 0 3 36) """
    let base1 = d1 :> Base1
    let derived1 = base1 :?> Derived1""" config
    |> should equal """    let derived1 = base1 :?> Derived1"""

[<Test>]
let ``should format a few lines correctly and preserve indentation``() =
    formatSelectionOnly false (makeRange 3 4 5 51) """
let rangeTest testValue mid size =
    match testValue with
    | var1 when var1 >= mid - size/2 && var1 <= mid + size/2 -> printfn "The test value is in range."
    | _ -> printfn "The test value is out of range."

let (var1, var2) as tuple1 = (1, 2)""" config
    |> append newline
    |> should equal """match testValue with
    | var1 when var1 >= mid - size / 2 && var1 <= mid + size / 2 -> 
        printfn "The test value is in range."
    | _ -> printfn "The test value is out of range."
"""

[<Test>]
let ``should format a top-level let correctly``() =
    formatSelectionOnly false (makeRange 3 0 3 10) """
let x = 2 + 3
let y = 1+2
let z = x + y""" config
    |> should equal """let y = 1 + 2"""

[<Test>]
let ``should skip whitespace at the beginning of lines``() =
    formatSelectionOnly false (makeRange 3 3 3 27) """
type Product' (backlogItemId) =
    let mutable ordering = 0
    let mutable version = 0
    let backlogItems = []""" config
    |> should equal """ let mutable ordering = 0"""

[<Test>]
let ``should parse a complete expression correctly``() =
    formatSelectionOnly false (makeRange 4 0 5 35) """
open Fantomas.CodeFormatter

let config = { FormatConfig.Default with 
                IndentSpaceNum = 2 }

let source = "
    let Multiple9x9 () = 
      for i in 1 .. 9 do
        printf \"\\n\";
        for j in 1 .. 9 do
          let k = i * j in
          printf \"%d x %d = %2d \" i j k;
          done;
      done;;
    Multiple9x9 ();;"
"""     config
    |> should equal """let config = { FormatConfig.Default with IndentSpaceNum = 2 }"""

[<Test>]
let ``should format the selected pipeline correctly``() =
    formatSelectionOnly false (makeRange 3 4 7 18) """
let r = 
    [ "abc"
      "a"
      "b"
      "" ]
    |> List.map id""" config
    |> should equal """[ "abc"; "a"; "b"; "" ] |> List.map id"""

[<Test>]
let ``should preserve line breaks before and after selection``() =
    formatSelectionOnly false (makeRange 3 0 4 25) """
assert (3 > 2)

let result = lazy (x + 10)

do printfn "Hello world"
"""     config
    |> should equal """let result = lazy (x + 10)"""

[<Test>]
let ``should detect members and format appropriately``() =
    formatSelectionOnly false (makeRange 4 0 5 32) """
type T () = 
  let items = []
  override x.Reorder () = 
        items |> List.iter ignore
"""     config
    |> should equal """  override x.Reorder() = items |> List.iter ignore"""

[<Test>]
let ``should format the and branch of recursive functions``() =
    formatSelectionOnly false (makeRange 3 0 4 34) """
let rec createJArray x = createJObject

and createJObject y = createJArray
"""     config
    |> should equal """and createJObject y = createJArray
"""

[<Test>]
let ``should format recursive types correctly``() =
    formatSelectionOnly false (makeRange 7 0 10 48) """
type Folder(pathIn : string) = 
    let path = pathIn
    let filenameArray : string array = System.IO.Directory.GetFiles(path)
    member this.FileArray = 
        Array.map (fun elem -> new File(elem, this)) filenameArray

and File(filename: string, containingFolder: Folder) = 
   member __.Name = filename
   member __.ContainingFolder = containingFolder
"""     config
    |> prepend newline
    |> should equal """
and File(filename : string, containingFolder : Folder) =
    member __.Name = filename
    member __.ContainingFolder = containingFolder
"""

[<Test>]
let ``should not add trailing whitespaces and preserve indentation``() =
    formatSelectionOnly false (makeRange 4 0 7 15) """
module Enums = 
    // Declaration of an enumeration. 
    type Colour = 
      | Red = 0
      | Green = 1
      | Blue = 2
"""     config
    |> prepend newline
    |> should equal """
    type Colour =
        | Red = 0
        | Green = 1
        | Blue = 2"""