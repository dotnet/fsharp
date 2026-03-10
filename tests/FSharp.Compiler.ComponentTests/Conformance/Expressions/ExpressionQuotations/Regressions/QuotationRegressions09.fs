// #Regression #Conformance #Quotations 
// Regression test for FSHARP1.0:3999 - Issue with quotations over <- operator inside type constructor

open System
open Microsoft.FSharp.Quotations

//no exception:
let mutable x = 50
printfn "%A" <@ x <- 5 @>
printf "\n\n"

// no exception and no compile time error
type Test() =
   let y = ref 50
   do printfn "%A" <@ y := 5 @>
      printf "\n\n"
let t = Test()

// no exception and no compile time error
// let-declaration-in-a-class 
type Test2() =
   let mutable z = 50
   do printfn "%A" <@ z <- 5 @>
      printf "\n"
let t2 = Test2()

// no exception and no compile time error
// Mutating a record field is ok
type R = { mutable X : int }
type Test3() =
   let r = { X = 10 }
   do printfn "%A" <@ r.X <- 5 @>
      // PropertySet (Some (Value ({X = 10;})), X, [Value (5)])
let t3 = new Test3()

// no exception and no compile time error
// Mutating a record field is ok
let Test4() =
   let r = { X = 10 }
   do printfn "%A" <@ r.X <- 5 @>
      // PropertySet (Some (Value ({X = 10;})), X, [Value (5)])
let t4 = Test4()

// This does not compile anymore (and it is covered in neg63 under fsharp suite)
//no exception:
//let Test3() = 
//   let mutable z = 50
//   do printfn "%A" <@ z <- 1  @>
//      printf "\n"
//
//let t3 = Test3()

exit 0
