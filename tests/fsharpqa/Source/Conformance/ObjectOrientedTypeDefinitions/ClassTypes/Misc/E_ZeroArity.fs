// #Regression #Conformance #ObjectOrientedTypes #Classes 
// Verify error when providing an arity-zero type instantiation (requires a space)
// FSB 4076

//<Expects id="FS0010" status="error" span="(13,16-13,18)">Unexpected infix operator in expression$</Expects>

type Foo() =
    member this.Value = 1


let a = new Foo()    // Fine
let b = new Foo< >() // Fine
let c = new Foo<>()  // ERROR
