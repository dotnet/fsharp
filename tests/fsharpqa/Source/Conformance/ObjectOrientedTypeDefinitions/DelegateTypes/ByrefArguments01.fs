// #Conformance #ObjectOrientedTypes #Delegates 
#light

// FSB 151, Cannot construct delegate values that accept byref arguments

type Foo = delegate of byref<int> * byref<int> -> string

let instanceOfDelegate = new Foo (fun x y -> (x + y).ToString())

let mutable a = 16
let mutable b = 64
let result = instanceOfDelegate.Invoke( (&a), (&b) )

if result <> "80" then exit 1
exit 0
