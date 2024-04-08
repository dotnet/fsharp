// #Conformance #TypesAndModules #Unions 
// Union Types
// Discriminated union cases names must begin with an uppercase letter
//<Expects status="success"></Expects>

module M
[<Measure>] type Kg

type T = | Uppercase
         | U
         | I of int         // ok - regular I
         | İ of int         // ok - turkish i
         | System           // ok - no clash with namespace 'System'
         | M of float<Kg>   // ok - no clash with module 'M'

let p = İ(0)

