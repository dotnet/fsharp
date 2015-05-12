module Test

module T1 = 
    type Enum1 = 
        | E = 1
        with
        member this.Foo() = 1 // not ok

module T2 =    
    type Enum2 = 
        | E2 = 1
    type Enum2
        with
        member this.Foo() = 1 // not ok

module TestPrivateSet = 
    // See https://github.com/Microsoft/visualfsharp/issues/27
    module A =
        let mutable private x = 0

    module B = 
        let test () =
            // let _ = A.x  // accessibility error on read, as expected
            A.x <- 1     // but write works!

module TestObsoleteSet = 
    // See https://github.com/Microsoft/visualfsharp/issues/27
    module A =
        [<System.ObsoleteAttribute("Don't touch me")>]
        let mutable x = 0

    module B = 
        let test () =
            A.x <- 1     

module TestCompilerMessgeSet = 
    // See https://github.com/Microsoft/visualfsharp/issues/27
    module A =
        [<CompilerMessageAttribute("Don't touch me", 3003)>]
        let mutable x = 0

    module B = 
        let test () =
            A.x <- 1     

module TestExperimentalSet = 
    // See https://github.com/Microsoft/visualfsharp/issues/27
    module A =
        [<ExperimentalAttribute("It was just an experiment!")>]
        let mutable x = 0

    module B = 
        let test () =
            A.x <- 1     


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

