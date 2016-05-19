// #Regression #Conformance #TypeInference #TypeConstraints 
// Verify no implicit downlcast
//<Expects id="FS0001" status="error" span="(14,15-14,24)">This expression was expected to have type.    'Foo'    .but here has type.    'Bar'</Expects>

type Foo() =
    override this.ToString() = "Foo"

type Bar() =
    inherit Foo()
    override this.ToString() = "Bar"


let a = new Foo()
let b : Foo = new Bar()    // Should fail
