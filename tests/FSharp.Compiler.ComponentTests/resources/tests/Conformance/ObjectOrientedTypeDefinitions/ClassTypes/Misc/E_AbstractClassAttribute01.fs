// #Regression #Conformance #ObjectOrientedTypes #Classes 
// Regression test for FSharp1.0:2272 - new 'a() should not be allowed on types with AbstractClassAttribute
//<Expects id="FS0001" span="(12,9-12,10)" status="error">A generic construct requires that the type 'A' be non-abstract</Expects>

#light
[<AbstractClass>]
type A() =
    class
    end
    
let f<'a when 'a : (new : unit -> 'a)>() = new 'a()
let x = f<A>()

exit 1
