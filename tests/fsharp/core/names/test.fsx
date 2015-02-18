// #Regression #Conformance #Accessibility #SignatureFiles #Regression #Records #Unions 
#if Portable
module Core_access
#endif

#light
let failures = ref false
let report_failure () = 
  stderr.WriteLine " NO"; failures := true
let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure() 

(*--------------------*)

// Test cases for Github Issue # 10  module cannot contain ,

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

do 
    printfn "%d" ``,``.x
    if ``,``.x <> 3 then failures := true

    printfn "%d" ``,,,,,,,,,,,``.x
    if ``,,,,,,,,,,,``.x <> 5 then failures := true

    printfn "%d" ``One, 2, Three, 4, Five``.x
    if ``One, 2, Three, 4, Five``.x <> 7 then failures := true

    if ``Nested modules with commas``.``One, ``.``Two, ``.``Three, ``.x <> 9 then  failures := true
    printfn "%d" ``Nested modules with commas``.``One, ``.``Two, ``.``Three, ``.x

    (*--------------------*)  

// Test cases for Github Issue # 10  deeper look typenames cannot contain ', in fsi

// Comma in type name
module ``Comma in type name`` =
    type ``,`` () =
        static member x() = 13

// Lots of comma's in type name
module ``Lots of comma's in type name`` =
    type ``,,,,,,,`` () =
        static member x() = 15

// Lots of comma's and other characters in type name
module ``Lots of comma's and other characters in type name`` =
    type ``One, 2, Three, 4, Five`` () =
        static member x() = 17

do
    if ``Comma in type name``.``,``.x() <> 13 then failures := true
    printfn "%d" (``Comma in type name``.``,``.x());;

    if ``Lots of comma's in type name``.``,,,,,,,``.x() <> 15 then failures := true
    printfn "%d" (``Lots of comma's in type name``.``,,,,,,,``.x());;

    if ``Lots of comma's and other characters in type name``.``One, 2, Three, 4, Five``.x() <> 17 then failures := true
    printfn "%d" (``Lots of comma's and other characters in type name``.``One, 2, Three, 4, Five``.x());;


let aa =
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 

do  (stdout.WriteLine "Test Passed"; 
    System.IO.File.WriteAllText("test.ok","ok"); 
    exit 0)
