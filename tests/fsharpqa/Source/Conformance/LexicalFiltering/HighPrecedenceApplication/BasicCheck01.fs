// #Conformance #LexFilter #Precedence 
#light

// Verify high precedence applications. That is, if no space is between the function
// and its first method, then verify a different precedence.

// B(e).C  => (B(e)).C
// B (e).C => B ((e).C)

type Foo(x : string) =
    member this.Prop  = new Foo(x + "P")
    member this.Value = x + "V"
    static member B (x : Foo) : Foo = new Foo(x.Value + "B")


let result1 = Foo.B(new Foo("")).Prop
let result2 = Foo.B (new Foo("")).Prop

if result1.Value <> "VBPV" then exit 1
if result2.Value <> "PVBV" then exit 1

exit 0
