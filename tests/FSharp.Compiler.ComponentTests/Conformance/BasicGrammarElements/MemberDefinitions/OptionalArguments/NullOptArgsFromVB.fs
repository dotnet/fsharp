module M

open Test
open System

let validate case actual expected =
    printfn "Checking case '%s'" case
    if actual <> expected then
        printfn "  Actual: %O" actual
        printfn "  Expected: %O" expected
        failwith "Failed: 1"

let a = VBTestClass()

validate "ByRefNullable1" (a.OptionalNullableRefParam()) (Nullable(130))
validate "ByRefNullable2" (a.OptionalNullableRefParam(ref (Nullable()))) (Nullable(55))
validate "ByRefNullable3" (a.OptionalNullableRefParam(ref (Nullable(-100)))) (Nullable(0))

validate "OptionalRefParam1" (a.OptionalRefParam()) "superduper edited"
validate "OptionalRefParam2" (a.OptionalRefParam(ref "qwerty")) "qwerty edited"
let mutable x1 = "aaa"
validate "OptionalRefParam3" (a.OptionalRefParam(ref x1)) "aaa edited"
validate "OptionalRefParam4" x1 "aaa"
validate "OptionalRefParam5" (a.OptionalRefParam(&x1)) "aaa edited"
validate "OptionalRefParam6" x1 "aaa edited"
