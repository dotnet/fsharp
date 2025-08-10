// #Conformance #PatternMatching #ActivePatterns 
#light

type Shape =
| Rectangle of width : float * length : float
| Circle of radius : float
| Prism of width : float * float * height : float
    
let getShapeWidth shape =
    match shape with
    | Rectangle(width = w) -> w
    | Circle(radius = r) -> 2. * r
    | Prism(width = w, height = h) -> w * h

let result = getShapeWidth (Rectangle(10., 20.))

if result <> 10 then exit 1

exit 0   