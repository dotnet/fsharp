// #Regression #Conformance #UnitsOfMeasure #ObjectOrientedTypes 
// Verify error when putting invalid attributes on type arguments
// (We should only allow [<Measure>].)
//<Expects id="FS0842" status="warning" span="(8,36)">This attribute is not valid for use on this language element</Expects>

open System

type GenericUOM<  'gen, [<Measure; System.Obsolete>] 'uom>(x : 'gen, y : decimal<'uom>) =
    member this.GenValue = x
    member this.UOMValue = y
    
[<Measure>]
type s
    
let test = new GenericUOM<_,_>("a string", 1.0M<s>)
