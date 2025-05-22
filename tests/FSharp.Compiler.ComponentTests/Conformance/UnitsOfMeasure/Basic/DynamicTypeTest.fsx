// #Regression #Conformance #UnitsOfMeasure 
#light

// Bug 3688 - This was crashing the compiler before.

[<Measure>]
type sprocket

[<Measure>]
type widget

let printMeasure x =
    match box x with
    | :? float<sprocket>   as s -> sprintf "float sprocket with value %.2f" (float s)
    | :? float32<sprocket>   as s -> sprintf "float32 sprocket with value %.3f" (float32 s)
    | :? decimal<sprocket> as s -> sprintf "decimal sprocket with value %M" (decimal s)
    | :? float<widget>     as w -> sprintf "float widget with value %.2f" (float w)
    | :? float32<widget>     as w -> sprintf "float32 widget with value %.3f" (float32 w)
    | :? decimal<widget>   as w -> sprintf "decimal widget with value %M" (decimal w)
    | _    -> "Unknown - wildcard"

let testSprocket() =
    let ret1 = printMeasure 1.23<sprocket>
    if ret1 <> "float sprocket with value 1.23" then exit 1

    let ret2 = printMeasure 0.001f<sprocket>
    if ret2 <> "float32 sprocket with value 0.001" then exit 1

    let ret3 = printMeasure 1e3m<sprocket>
    if ret3 <> "decimal sprocket with value 1000" then exit 1
    
let testWidget() =
    // NOTE: that currently, measures are ignored at runtime. See bug 3688.
    let ret1 = printMeasure 1.23<widget>
    if ret1 <> "float sprocket with value 1.23" then exit 1

    let ret2 = printMeasure 0.001f<widget>
    if ret2 <> "float32 sprocket with value 0.001" then exit 1

    let ret3 = printMeasure 1e3m<widget>
    if ret3 <> "decimal sprocket with value 1000" then exit 1    

testSprocket()
testWidget()

exit 0
