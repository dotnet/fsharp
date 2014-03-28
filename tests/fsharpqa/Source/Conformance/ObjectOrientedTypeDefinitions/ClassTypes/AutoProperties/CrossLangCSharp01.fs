// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
// Regression test for 203936, auto prop dispatch slots
// Normal cases are covered well by the Cambridge suite, these are sanity tests for inheriting from a C# library

open BaseClass01

type Bar() =
    inherit BaseClass01()
    override val Foo = 10 with get, set

let x = BaseClass01()
if x.Foo <> 0 then exit 1
let y = Bar()
if y.Foo <> 10 then exit 1
y.Foo <- 11
if y.Foo <> 11 then exit 1

type Bar2() =
    interface Interface01 with
        override val Foo = 10 with get, set

let z = Bar2()
if (z :> Interface01).Foo <> 10 then exit 1
(z :> Interface01).Foo <- 11
if (z :> Interface01).Foo <> 11 then exit 1

type Bar3() =
    inherit AbstractClass01()
    override val Foo = 10 with get, set

let a = Bar()
if a.Foo <> 10 then exit 1
a.Foo <- 11
if a.Foo <> 11 then exit 1


exit 0