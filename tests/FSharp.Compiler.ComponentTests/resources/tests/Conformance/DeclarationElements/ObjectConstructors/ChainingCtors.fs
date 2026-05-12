// #Conformance #DeclarationElements #ObjectConstructors 
open System

// Test chaining of constructors.

type TestType(arg1 : int, arg2 : int) =
    let m_value = arg1 + arg2
    member this.Value = m_value
    
    new (sarg1 : string, sarg2 : string) = TestType(Int32.Parse(sarg1), Int32.Parse(sarg2))
    
    new (x : int list) = TestType((List.nth x 0), (List.nth x 1))

    new (x : string list) = TestType((List.nth x 0), (List.nth x 1))

let test1 = new TestType(1, 1)
if test1.Value <> 2 then exit 1

let test2 = new TestType("1", "2")
if test2.Value <> 3 then exit 1

let test3 = new TestType ([3; 5])
if test3.Value <> 8 then exit 1

let test4 = new TestType (["5"; "8"])
if test4.Value <> 13 then exit 1

exit 0
