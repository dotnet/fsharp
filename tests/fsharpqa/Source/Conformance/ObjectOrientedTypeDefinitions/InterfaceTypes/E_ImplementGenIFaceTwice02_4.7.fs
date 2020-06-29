// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Verify error when trying to implement the same generic interface twice
//<Expects status="error" id="FS3350" span="(10,6-10,9)">Feature 'interfaces with multiple generic instantiation' is not available in F# 4.7. Please use language version 'preview' or greater.</Expects>

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
