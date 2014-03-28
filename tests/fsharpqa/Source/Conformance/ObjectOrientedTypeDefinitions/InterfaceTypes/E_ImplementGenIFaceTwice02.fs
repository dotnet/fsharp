// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Verify error when trying to implement the same generic
// interface twice
//<Expects status="error" span="(11,6)" id="FS0443">This type implements or inherits the same interface at different generic instantiations 'IFoo<int64>' and 'IFoo<string>'\. This is not permitted in this version of F#</Expects>

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

exit 1
