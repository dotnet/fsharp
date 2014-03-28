// #Conformance #TypeInference #TypeConstraints 
#light

// Test primitive constraints

// Test ':' constraints

type Foo(x : int) =
    member   this.Value      = x
    override this.ToString() = "Foo"

type Bar(x : int) =
    inherit Foo(-1)
    member   this.Value2     = x
    override this.ToString() = "Bar"

let test1 (x : Foo) = x.Value
let test2 (x : Bar) = (x.Value, x.Value2)

let f = new Foo(128)
let b = new Bar(256)

if test1 f <> 128       then exit 1
if test2 b <> (-1, 256) then exit 1

exit 0
