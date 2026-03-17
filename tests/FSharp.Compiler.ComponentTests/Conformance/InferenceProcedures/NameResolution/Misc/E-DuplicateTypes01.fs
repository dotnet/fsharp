// #Regression #Conformance #TypeInference 
// Verify error if declaring a duplicate types
//<Expects id="FS0037" status="error" span="(9,6)">Duplicate definition of type, exception or module 'Foo`1'</Expects>

type Foo<'a>() =
    override this.ToString() = "Foo<'a>"


type Foo<'a>() =
    override this.ToString() = "Foo<'a> 2"
