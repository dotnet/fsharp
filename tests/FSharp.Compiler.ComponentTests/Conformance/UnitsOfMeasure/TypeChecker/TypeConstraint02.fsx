// #Regression #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints 
// FSB 3458: units ought to satisfy struct constraint

#light
[<Measure>] type m
 
let f<'a,'b when 'a : struct>() = 1
 
let x = f<float<m>,float>() 

exit 0
