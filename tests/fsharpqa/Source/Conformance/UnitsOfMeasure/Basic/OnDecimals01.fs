// #Regression #Conformance #UnitsOfMeasure 
#light

// Regression for FSB 3432, Arithmetic operations appear not to work on decimals with units of measure.

[<Measure>]
type Widgets

[<Measure>]
type Sprockets

let widgetsPerSprocket = 5.0M<Widgets> / 1.25M<Sprockets>

// Addition
let addition =
    if widgetsPerSprocket + widgetsPerSprocket <> (10.0M / 1.25M) * 1M<Widgets / Sprockets> then exit 1

// Multiplication
let multiplication = 
    let x = 2.50000000M<Sprockets>
    if x * widgetsPerSprocket <> 10.0M<Widgets> then exit 1

// Subtraction
let subtraction = 
    if widgetsPerSprocket - widgetsPerSprocket <> 0.0M<Widgets / Sprockets> then exit 1

// Division
let division =
    if widgetsPerSprocket / 5.0M<Widgets> <> (1.0M / 1.25M) * 1M<Sprockets^-1> then exit 1

exit 0
         
