// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
type IFoo<'a> =
    interface
        abstract DoStuff : unit -> string
    end

type Bar() =
    interface IFoo<string> with
        member this.DoStuff() = "IFoo<string>"
    interface IFoo<int64> with
        member this.DoStuff() = "IFoo<int64>"
