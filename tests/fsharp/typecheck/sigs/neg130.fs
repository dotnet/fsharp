module Neg130


// The code in this test starts to compile once FS-1043 is enabled.
// See https://github.com/dotnet/fsharp/issues/3814#issuecomment-395686007
module YetTestAnotherCaseOfSRTP = 
    type X =
        static member Method (a: int) = 2
        static member Method (a: int64) = 3


    let inline Test< ^t, ^a when ^t: (static member Method: ^a -> int)> (value: ^a) =
        ( ^t: (static member Method: ^a -> int)(value))

    let inline Test2< ^t> a = Test<X, ^t> a

    if Test2<int> 0 <> 2 then 
        printfn "test failed"
        exit 1


/// In this case, the presence of the "Equals" method on System.Object was causing method overloading to be resolved too
/// early, when ^t was not yet known.  The underlying problem was that we were proceeding with weak resolution
/// even for a single-support-type trait constraint.
module MethodOverloadingForTraitConstraintsWhereSomeMethodsComeFromObjectTypeIsNotDeterminedTooEarly =
    type Test() =
         member __.Equals (_: Test) = true

    let inline Equals(a: obj) (b: ^t) =
        match a with
        | :? ^t as x -> (^t: (member Equals: ^t -> bool) (b, x))
        | _-> false

    let a  = Test()
    let b  = Test()

    // NOTE, this was a bug fixed by RFC FS-1043, see https://github.com/Microsoft/visualfsharp/issues/3814
    //
    // The result should be true.  
    //
    // This test has been added to pin down current behaviour pending a future bug fix.
    if Equals a b <> true then 
        printfn "test failed"
        exit 1
       



