// #Regression #Conformance #DeclarationElements #MemberDefinitions #OptionalArguments  
// Dev10: 811195. Internal parse error when calling C#/VB methods with optional args whose default value is null
// Should be no errors here
module M

open TestLib
open System
open System.Collections.Generic

let validate case actual expected =
    printfn "Checking case '%s'" case
    if actual <> expected then
        printfn "  Actual: %O" actual
        printfn "  Expected: %O" expected
        failwith "Failed: 1"

let a = T()

validate "ValueTypeOptArg1" (a.ValueTypeOptArg()) 100
validate "ValueTypeOptArg2" (a.ValueTypeOptArg(2)) 2

validate "NullableOptArgNullDefault1" (a.NullableOptArgNullDefault()) (Nullable())
validate "NullableOptArgNullDefault2" (a.NullableOptArgNullDefault(Nullable(10))) (Nullable(10))

validate "NullableOptArgWithDefault1" (a.NullableOptArgWithDefault()) (Nullable(5.7))
validate "NullableOptArgWithDefault2" (a.NullableOptArgWithDefault(Nullable(10.5))) (Nullable(10.5))

validate "NullOptArg1" (a.NullOptArg()) null
validate "NullOptArg2" (a.NullOptArg("qwerty")) "qwerty"

validate "NonNullOptArg1" (a.NonNullOptArg()) "abc"
validate "NonNullOptArg2" (a.NonNullOptArg("qwerty2")) "qwerty2"

let intList = List<int>()
let stringList = List<string>()
validate "GenericOptArg1" (a.GenericOptArg<int>()) null
validate "GenericOptArg2" (a.GenericOptArg<int>(intList)) intList
validate "GenericOptArg3" (a.GenericOptArg<string>()) null
validate "GenericOptArg4" (a.GenericOptArg<string>(stringList)) stringList

validate "Combo1" (a.ComboOptionals("abc")) "[] [abc] [100] [200] [] []"
validate "Combo2" (a.ComboOptionals("abc", a = "123")) "[123] [abc] [100] [200] [] []"
validate "Combo3" (a.ComboOptionals("abc", b = "xyz")) "[] [xyz] [100] [200] [] []"
validate "Combo4" (a.ComboOptionals("abc", c = 33)) "[] [abc] [33] [200] [] []"
validate "Combo5" (a.ComboOptionals("abc", d = Nullable(33))) "[] [abc] [100] [33] [] []"
validate "Combo6" (a.ComboOptionals("abc", e = Nullable(33.3))) "[] [abc] [100] [200] [33.3] []"
validate "Combo7" (a.ComboOptionals("abc", f = intList)) "[] [abc] [100] [200] [] [System.Collections.Generic.List`1[System.Int32]]"
