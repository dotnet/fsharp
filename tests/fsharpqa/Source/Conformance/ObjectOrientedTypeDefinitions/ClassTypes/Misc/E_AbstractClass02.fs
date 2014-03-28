// #Regression #Conformance #ObjectOrientedTypes #Classes 
// FSB 1272, New-ing a sub class with unimplemented abstract members should not be allowed.

//<Expects id="FS0759" status="error" span="(18,9)">Instances of this type cannot be created since it has been marked abstract or not all methods have been given implementations\. Consider using an object expression '{ new \.\.\. with \.\.\. }' instead</Expects>

[<AbstractClass>]
type Foo = class
    new () = {}
    abstract f : int -> int
end

[<AbstractClass>]
type Bar = class
    inherit Foo
    new () = {}
end

let x = new Bar ()
let y = x.f 1

