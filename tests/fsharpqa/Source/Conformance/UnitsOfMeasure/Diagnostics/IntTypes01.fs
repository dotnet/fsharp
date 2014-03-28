// #Conformance #UnitsOfMeasure #Diagnostics 
// Verify it is ok to use signed integers
//<Expects status="success"></Expects>
module M
[<Measure>] type Kg

let a = 1L<Kg> - 1L<Kg>     // int64
let b = 1<Kg> * 1<Kg>       // int=int32
let c = 1s<Kg> / 1s<Kg>     // int16
let d = 1y<Kg> + 1y<Kg>     // int8

exit 0
