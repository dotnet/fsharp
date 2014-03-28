// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
#light

// Verify you can use inheritance with interfaces
type IAge =
    abstract GetAge : unit -> int

type IThing =
    inherit IAge
    abstract Name : string

type FSharpLanguage() =
    interface IThing with
        member this.GetAge() = 3
        member this.Name = "The F# Programming Language"    

let test = new FSharpLanguage()
let ia = test :> IAge
let it = test :> IThing

if ia.GetAge() <> 3 then exit 1

if it.GetAge() <> 3 then exit 1
if it.Name <> "The F# Programming Language" then exit 1

exit 0
