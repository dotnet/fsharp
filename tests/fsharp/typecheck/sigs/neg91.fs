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

module TestCompilerMessageSet = 
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

module TestExtrinsicExtensionConstructor = 
    // See https://github.com/Microsoft/visualfsharp/issues/659

    type AugmentMe() = class end

    module M = 
        type AugmentMe with new(i) = AugmentMe()

// don't expect an error here
module TestIntrinsicExtensionConstructor1 = 
    type AugmentMe = val _i : int
    type AugmentMe with member i.I = i._i
    type AugmentMe with new(i) = { _i = i } // don't expect an error here
    let v = AugmentMe(42).I

// don't expect an error here
module TestIntrinsicExtensionConstructor2 = 
    type AugmentMe() = class end
    type AugmentMe with member i.I = 1
    type AugmentMe with new(i) = AugmentMe()  // don't expect an error here
    let v = AugmentMe(42).I

