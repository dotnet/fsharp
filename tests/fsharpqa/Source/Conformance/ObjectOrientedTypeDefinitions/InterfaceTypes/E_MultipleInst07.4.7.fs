// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Aliased types should correctly unify, even in combination with a measure.

//<Expects status="error" id="FS3350" span="(14,6-14,7)">Feature 'interfaces with multiple generic instantiation' is not available in F# 4.7. Please use language version 'preview' or greater.</Expects>

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
