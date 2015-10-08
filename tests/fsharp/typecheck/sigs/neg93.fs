namespace Neg93

// See https://github.com/Microsoft/visualfsharp/issues/32
module Repro1 = 
 type T1<'TError>(xx:'TError) =
    member x.Foo() = T2.Bar(xx)
 and T2 =
    static member Bar(arg) = 0

//let rec f1<'TError>(xx:'TError) = f2(xx)
//and f2(arg) = 0

module Repro2 = 
 type T1<'TError>(thisActuallyHasToBeHere:'TError) =
  member x.Foo() = T2.Bar(failwith "!" : option<'TError>)
 and T2 =
  static member Bar(arg:option<_>) = 0

module Repro3 = 

 let rec foo< > c = bar c
 and bar c = 0

module Bug528_Ex1 = 
    module Color =
        let [<Literal>] Yellow = "Yellow"

    let isRed inp = 
        match inp with
        | Color.Yellow arg ->   // should get an error here
            failwith "Don't know this color"
        | _ -> ""

module Bug528_Ex2 = 
    type Color = | Yellow = 1 | Red = 2

    let isRed inp = 
        match inp with
        | Color.Yellow arg ->   // should get an error here
            failwith "Don't know this color"
        | _ -> ""            

module Bug528_Ex3 = 

    let isRed inp = 
        match inp with
        | System.DayOfWeek.Monday arg ->   // should get an error here
            failwith "Don't know this day"
        | _ -> ""

