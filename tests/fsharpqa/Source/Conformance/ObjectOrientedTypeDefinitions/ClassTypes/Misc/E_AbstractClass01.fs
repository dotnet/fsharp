// #Regression #Conformance #ObjectOrientedTypes #Classes 


// FSB 1272, New-ing a sub class with unimplemented abstract members should not be allowed.

//<Expects id="FS0054" status="error" span="(12,6)">This type is 'abstract' since some abstract members have not been given an implementation\. If this is intentional then add the '\[<AbstractClass>\]' attribute to your type</Expects>
//<Expects id="FS0365" status="error" span="(12,6)">No implementation was given for 'abstract Foo\.f: int -> int'</Expects>
//<Expects id="FS0054" status="error" span="(17,6)">This type is 'abstract' since some abstract members have not been given an implementation\. If this is intentional then add the '\[<AbstractClass>\]' attribute to your type</Expects>
//<Expects id="FS0365" status="error" span="(17,6)">No implementation was given for 'abstract Foo\.f: int -> int'</Expects>


type Foo = class
    new () = {}
    abstract f : int -> int
end

type Bar = class
    inherit Foo
    new () = {}
end

let x = new Bar ()
let y = x.f 1
