// #Regression #Conformance #ObjectOrientedTypes #Classes 
#light

// FS1 1326: Generic unit types do not work with the implicit class syntax

type MyComplex<'a>(re : 'a, im : 'a) =
    member this.Real = re
    member this.Imaginary = im

let t1 = new MyComplex<string>("a", "b")
if t1.Real <> "a" then exit 1
if t1.Imaginary <> "b" then exit 1

let t2 = new MyComplex<int>(1, -1)
if t2.Real <> 1 then exit 1
if t2.Imaginary <> -1 then exit 1

exit 0
