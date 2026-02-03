// #Regression #Conformance #TypeInference 
// Former regression test for FSharp1.0:5685
// Title: squiggles under 'this' - unfortunate type inference
// Descr: Make sure error span is correct for type inference error
// Now, after fix for 832789 this compiles just fine.

//<Expects status="success"></Expects>

type Foo<'a>(bar : Bar<'a>) =
    member this.Blah() = bar.Foo() 
and Bar<'a>() =
    member this.Foo() = ()

exit 0
