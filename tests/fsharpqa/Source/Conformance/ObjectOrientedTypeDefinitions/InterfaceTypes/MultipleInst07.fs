// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Aliased types should correctly unify, even in combination with a measure.

//<Expects status="error" id="FS3302" span="(14,6-14,7)">'C' cannot implement the interface 'IB<_>' with the two instantiations 'IB<MyInt>' and 'IB<int<kg>>' because they may unify.</Expects>

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