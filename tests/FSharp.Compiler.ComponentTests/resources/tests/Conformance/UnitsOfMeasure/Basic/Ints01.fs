// #Conformance #UnitsOfMeasure 
// Sanity check units of measure on signed integers

[<Measure>]
type Widget

[<Measure>]
type s

let makeWidget08() = 1y<Widget>
let makeWidget16() = 1s<Widget>
let makeWidget32() = 1<Widget>
let makeWidget64() = 1L<Widget>

let factoryRate1 = makeWidget08() / 1y<s>
let factoryRate2 = makeWidget16() / 1s<s>
let factoryRate3 = makeWidget32() / 1<s>
let factoryRate4 = makeWidget64() / 1L<s>

exit 0

