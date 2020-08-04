// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// int<Measure> does not unify with in.

//<Expects status="error" id="FS3360" span="(14,6-14,7)">'C' cannot implement the interface 'IB<_>' with the two instantiations 'IB<MyInt>' and 'IB<int<kg>>' because they may unify.</Expects>

type MyInt = int
[<Measure>] type kg

type IB<'a> =
    interface 
        abstract X : unit -> int
    end

type C() =
    interface IB<int<kg>> with
        member x.X() = 1
    interface IB<MyInt> with
        member x.X() = 2
    
exit 1
