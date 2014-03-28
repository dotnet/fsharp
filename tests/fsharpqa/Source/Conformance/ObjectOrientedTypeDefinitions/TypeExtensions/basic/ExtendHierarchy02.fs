// #Conformance #ObjectOrientedTypes #TypeExtensions 
#light

// Verify type extensions extend the full hierarchy of a type
// Verify that if you extend a base class, derived classes are extended

type BaseClass() =
    member this.BaseValue = 1


type DerivedClass() =
    inherit BaseClass()
    member this.DerivedValue = 2

// Type Extension
type BaseClass with
    member this.ExtendedValue = 3

let test = new DerivedClass()
if test.BaseValue     <> 1 then exit 1
if test.DerivedValue  <> 2 then exit 1
if test.ExtendedValue <> 3 then exit 1

exit 0
