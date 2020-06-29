// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Aliased types should correctly unify, even in combination with a measure.

//<Expects status="error" id="FS0443" span="(14,6-14,7)">This type implements the same interface at different generic instantiations 'IB<MyInt>' and 'IB<int<kg>>'. This is not permitted in this version of F#.</Expects>

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
