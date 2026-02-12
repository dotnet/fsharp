// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Aliased types should correctly unify

// These errors could be improved, but verify that it errors out at all.
//<Expects status="error" span="(23,15-23,24)" id="FS0888">Duplicate specification of an interface</Expects>
//<Expects status="error" span="(21,15-21,22)" id="FS0362">Duplicate or redundant interface</Expects>
//<Expects status="error" span="(23,15-23,24)" id="FS0362">Duplicate or redundant interface</Expects>
//<Expects status="error" span="(22,18-22,19)" id="FS0855">No abstract or interface member was found that corresponds to this override</Expects>
//<Expects status="error" span="(24,18-24,19)" id="FS0855">No abstract or interface member was found that corresponds to this override</Expects>


type MyInt = int


type IB<'a> =
    interface 
        abstract X : unit -> int
    end

type C() =
    interface IB<int> with
        member x.X() = 1
    interface IB<MyInt> with
        member x.X() = 2
    
exit 1

