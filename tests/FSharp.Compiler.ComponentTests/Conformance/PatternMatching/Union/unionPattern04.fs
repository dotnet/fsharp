// #Regression #Conformance #PatternMatching #Unions 
#light

// Regression test for FSHARP1.0:3914 - Invalid value when returning a pattern match-bound parameter in the enclosure of a lambda (does that make sense?)

type XDuTag = XDuTag

let foo XDuTag = fun y -> XDuTag

let bar = foo XDuTag 0.0

exit 0
