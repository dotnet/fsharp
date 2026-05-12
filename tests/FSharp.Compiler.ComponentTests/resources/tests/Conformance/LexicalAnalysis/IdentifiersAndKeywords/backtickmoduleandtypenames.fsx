//<Expects status="success"></Expects>
open System
open System.Reflection

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns

let failures = ref false
let report_failure () = 
  stderr.WriteLine " NO"; failures := true
let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure() 

(*--------------------*)

// Test cases for GitHub Issue # 10  module cannot contain ,

// Comma in module name
module ``,`` =
    let x = 3

// Lots of comma's in module name
module ``,,,,,,,,,,,`` =
    let x = 5

// Lots of comma's and other characters in module name
module ``One, 2, Three, 4, Five`` =
    let x = 7

// Nested modules with commas
module ``Nested modules with commas`` =
  module ``One, `` =
    module ``Two, `` =
      module ``Three, `` =
        let x = 9

module ``Comma in type name`` =
    type ``,`` =
        static member x = 13

// Lots of comma's in type name
module ``Lots of comma's in type name`` =
    type ``,,,,,,,`` =
        static member x = 15

// Lots of comma's and other characters in type name
module ``Lots of comma's and other characters in type name`` =
    type ``One, 2, Three, 4, Five`` =
        static member x = 17

do 
    let eval expr value = if (expr) = value then true else false

    test "Comma in module name" (eval ``,``.x 3)
    test "Lots of comma's in module name" (eval ``,,,,,,,,,,,``.x 5)
    test "Lots of comma's and other characters in module name" (eval ``One, 2, Three, 4, Five``.x 7)
    test "Nested modules with commas" (eval ``Nested modules with commas``.``One, ``.``Two, ``.``Three, ``.x 9)

    test "Comma in type name" (eval ``Comma in type name``.``,``.x 13)
    test "Lots of comma's in type name" (eval ``Lots of comma's in type name``.``,,,,,,,``.x 15)
    test "Lots of comma's and other characters in type name" (eval ``Lots of comma's and other characters in type name``.``One, 2, Three, 4, Five``.x 17)

    let eval expr value = (match expr with | PropertyGet (a, b, c) -> (if (b.GetGetMethod(true) :> MethodBase).Invoke(null, null) :?>int = value then true else false) | _ -> false)

    test "Comma in module name from quotation"                                (eval <@@ ``,``.x @@> 3)
    test "Lots of comma's in module name from quotation"                      (eval <@@ ``,,,,,,,,,,,``.x @@> 5)
    test "Lots of comma's and other characters in module name from quotation" (eval <@@ ``One, 2, Three, 4, Five``.x @@> 7)
    test "Nested modules with commas from quotation"                          (eval <@@ ``Nested modules with commas``.``One, ``.``Two, ``.``Three, ``.x @@> 9)

    test "Comma in type name from quotation"                                  (eval <@@ ``Comma in type name``.``,``.x @@> 13)
    test "Lots of comma's in type name from quotation"                        (eval <@@ ``Lots of comma's in type name``.``,,,,,,,``.x @@> 15)
    test "Lots of comma's and other characters in type name from quotation"   (eval <@@ ``Lots of comma's and other characters in type name``.``One, 2, Three, 4, Five``.x @@> 17)

let aa =
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 

do (stdout.WriteLine "Test Passed";
    printf "TEST PASSED OK";
    exit 0)
