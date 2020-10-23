// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// It should be possible to override interface implementations


type MyInt = int
[<Measure>] type kg

type IB<'a> =
    interface 
        abstract X : unit -> int
    end

type CBase() =
    interface IB<int> with
        member x.X() = 1
    interface IB<float> with
        member x.X() = 2

type C2() =
    inherit CBase()
    interface IB<int> with
        member x.X() = 3

type C3() =
    inherit C2()
    interface IB<float> with
        member x.X() = 4

type C4() =
    inherit C3()
    interface IB<int> with
        member x.X() = 5
    interface IB<float> with
        member x.X() = 6
    interface IB<float32> with
        member x.X() = 7

type C5() =
    inherit C4()
    interface IB<MyInt> with
        member x.X() = 8
    interface IB<float<kg>> with
        member x.X() = 9
    interface IB<float32> with
        member x.X() = 10

exit 0