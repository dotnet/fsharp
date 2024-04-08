// #Conformance #TypesAndModules #Records 
#light

// Test type inference for records

type Point = { X : float; Y : float }

// Notice no type annotations required for pt1, pt2
let add pt1 pt2 = { X = pt1.X + pt2.X; Y = pt1.Y + pt2.Y }

let tenten     = { X = 10.0; Y = 10.0 }
let negtenten  = { X = -10.0; Y = 10.0 }

let result = add tenten negtenten
if result.X <> 0.0 || result.Y <> 20.0 then failwith "Failed"
