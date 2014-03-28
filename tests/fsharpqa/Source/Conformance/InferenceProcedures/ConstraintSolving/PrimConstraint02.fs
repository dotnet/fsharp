// #Conformance #TypeInference #TypeConstraints 

#light

// Test primitive constraints

// Test ':>' constraints

type Foo(x : int) =
    member   this.Value      = x
    override this.ToString() = "Foo"

type Bar(x : int) =
    inherit Foo(-1)
    member   this.Value2     = x
    override this.ToString() = "Bar"

type Ram(x : int) =
    inherit Foo(10)
    member   this.ValueA     = x
    override this.ToString() = "Ram"

let test (x : Foo) = (x.Value, x.ToString())

let f = new Foo(128)
let b = new Bar(256)
let r = new Ram(314)

if test f <> (128, "Foo") then exit 1
if test b <> (-1, "Bar") then exit 1
if test r <> (10, "Ram") then exit 1

exit 0
