module FSharp.Compiler.Service.Tests.ServiceFormatting.FormattingSelectionTests

open NUnit.Framework
open FsUnit
open TestHelper

[<Test>]
let ``should format a part of a line correctly``() =
    formatSelectionFromString false (makeRange 3 8 3 10) """
let x = 2 + 3
let y = 1+2
let z = x + y""" config
    |> should equal """
let x = 2 + 3
let y = 1 + 2
let z = x + y"""

[<Test>]
let ``should format a whole line correctly and preserve indentation``() =
    formatSelectionFromString false (makeRange 3 4 3 34) """
    let base1 = d1 :> Base1
    let derived1 = base1 :?> Derived1""" config
    |> should equal """
    let base1 = d1 :> Base1
    let derived1 = base1 :?> Derived1"""

[<Test>]
let ``should format a few lines correctly and preserve indentation``() =
    formatSelectionFromString false (makeRange 3 5 5 51) """
let rangeTest testValue mid size =
    match testValue with
    | var1 when var1 >= mid - size/2 && var1 <= mid + size/2 -> printfn "The test value is in range."
    | _ -> printfn "The test value is out of range."

let (var1, var2) as tuple1 = (1, 2)""" config
    |> should equal """
let rangeTest testValue mid size =
    match testValue with
    | var1 when var1 >= mid - size / 2 && var1 <= mid + size / 2 -> 
        printfn "The test value is in range."
    | _ -> printfn "The test value is out of range."

let (var1, var2) as tuple1 = (1, 2)"""

[<Test>]
let ``should format a top-level let correctly``() =
    formatSelectionFromString false (makeRange 3 0 3 11) """
let x = 2 + 3
let y = 1+2
let z = x + y""" config
    |> should equal """
let x = 2 + 3
let y = 1 + 2
let z = x + y"""

[<Test>]
let ``should skip whitespace at the beginning of lines``() =
    formatSelectionFromString false (makeRange 3 3 3 27) """
type Product' (backlogItemId) =
    let mutable ordering = 0
    let mutable version = 0
    let backlogItems = []""" config
    |> should equal """
type Product' (backlogItemId) =
    let mutable ordering = 0
    let mutable version = 0
    let backlogItems = []"""

[<Test>]
let ``should parse a complete expression correctly``() =
    formatSelectionFromString false (makeRange 4 0 5 35) """
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
    |> should equal """
open Fantomas.CodeFormatter

let config = { FormatConfig.Default with IndentSpaceNum = 2 }

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
"""

[<Test>]
let ``should format the selected pipeline correctly``() =
    formatSelectionFromString false (makeRange 3 4 7 16) """
let r =
    [ "abc"
      "a"
      "b"
      "" ]
    |> List.map id""" config
    |> should equal """
let r =
    [ "abc"; "a"; "b"; "" ] |> List.map id"""

[<Test>]
let ``should preserve line breaks before and after selection``() =
    formatSelectionFromString false (makeRange 3 0 5 0) """
assert (3 > 2)

let result = lazy (x + 10)

do printfn "Hello world"
"""     config
    |> should equal """
assert (3 > 2)

let result = lazy (x + 10)

do printfn "Hello world"
"""

[<Test>]
let ``should detect members and format appropriately``() =
    formatSelectionFromString false (makeRange 4 0 5 32) """
type T () =
  let items = []
  override x.Reorder () = 
        items |> List.iter ignore
"""     config
    |> should equal """
type T () =
  let items = []
  override x.Reorder() = items |> List.iter ignore
"""

[<Test>]
let ``should format the and branch of recursive functions``() =
    formatSelectionFromString false (makeRange 3 0 4 34) """
let rec createJArray x = createJObject

and createJObject y = createJArray
"""     config
    |> should equal """
let rec createJArray x = createJObject

and createJObject y = createJArray
"""

[<Test>]
let ``should format recursive types correctly``() =
    formatSelectionFromString false (makeRange 7 0 10 48) """
type Folder(pathIn : string) =
    let path = pathIn
    let filenameArray : string array = System.IO.Directory.GetFiles(path)
    member this.FileArray =
        Array.map (fun elem -> new File(elem, this)) filenameArray

and File(filename: string, containingFolder: Folder) =
   member __.Name = filename
   member __.ContainingFolder = containingFolder
"""     config
    |> should equal """
type Folder(pathIn : string) =
    let path = pathIn
    let filenameArray : string array = System.IO.Directory.GetFiles(path)
    member this.FileArray =
        Array.map (fun elem -> new File(elem, this)) filenameArray

and File(filename : string, containingFolder : Folder) =
    member __.Name = filename
    member __.ContainingFolder = containingFolder
"""

[<Test>]
let ``should format around the cursor inside a list``() =
    formatAroundCursor false (makePos 4 4) """
let r =
    [ "abc"
      "a"
      "b"
      "" ]
    |> List.map id""" config
    |> should equal """
let r =
    [ "abc"; "a"; "b"; "" ]
    |> List.map id"""

[<Test>]
let ``should format around the cursor inside a tuple``() =
    formatAroundCursor false (makePos 4 8) """
let r =
    [ ("abc",1)
      ("a",2)
      ("b",3)
      ("",4) ]
    |> List.map id""" config
    |> should equal """
let r =
    [ ("abc",1)
      ("a", 2)
      ("b",3)
      ("",4) ]
    |> List.map id"""

[<Test>]
let ``should format around the cursor inside an array``() =
    formatAroundCursor false (makePos 3 20) """
let a3 =
    [| for n in 1 .. 100 do if isPrime n then yield n |]""" config
    |> should equal """
let a3 =
    [| for n in 1..100 do
           if isPrime n then yield n |]"""

[<Test>]
let ``should format around the cursor inside an object expression``() =
    formatAroundCursor false (makePos 2 20) """let obj1 =
    { new System.Object() with member x.ToString() = "F#" }""" config
    |> prepend newline
    |> should equal """
let obj1 =
    { new System.Object() with
          member x.ToString() = "F#" }"""

[<Test>]
let ``should format around the cursor inside a computation expression``() =
    formatAroundCursor false (makePos 4 20) """
let comp =
    eventually { for x in 1 .. 2 do
                    printfn " x = %d" x
                 return 3 + 4 }""" config
    |> should equal """
let comp =
    eventually { 
        for x in 1..2 do
            printfn " x = %d" x
        return 3 + 4
    }"""

[<Test>]
let ``should format around the cursor inside a record``() =
    formatAroundCursor false (makePos 3 10) """
type Car = {
    Make : string;
    Model : string;
    mutable Odometer : int;
    }""" config
    |> should equal """
type Car =
    { Make : string
      Model : string
      mutable Odometer : int }"""