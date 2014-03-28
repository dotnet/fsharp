// #Regression #Conformance #UnitsOfMeasure #ObjectOrientedTypes 
#light 

// Regression for FSB-3572
// false error 'Expected type, not a unit-of-measure' when it in fact is.
 
[<Measure>] type m
 
type IA = 
    interface 
        abstract Foo< [<Measure>]'a> : float<'a> -> float<'a>
    end 
 
type A() =  
    class
        interface IA with 
            member obj.Foo< [<Measure>]'a> (x:float<'a>) : float<'a> = 5.0 * x
        end
    end 
 
let x = A() :> IA  
let y = x.Foo<m> 1.0<m>


if y <> 5.0<m> then
    exit 1

exit 0
