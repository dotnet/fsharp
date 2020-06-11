// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// It is now allowed to implement the same interface multiple times (RFC FS-1031).

type IFoo<'a> =
    interface
        abstract DoStuff : unit -> unit
    end

type Bar() =
    interface IFoo<string> with
        member this.DoStuff() = printfn "IFoo<string>"
    interface IFoo<int64> with
        member this.DoStuff() = printfn "IFoo<int64>"


let t = new Bar()
(t :> IFoo<string>).DoStuff()
(t :> IFoo<int64>).DoStuff()

exit 0
