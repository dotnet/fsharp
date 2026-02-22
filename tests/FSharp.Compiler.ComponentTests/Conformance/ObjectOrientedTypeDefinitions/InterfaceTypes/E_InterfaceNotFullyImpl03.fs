// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:3748
// Now we emit an error.

//<Expects id="FS0366" span="(14,14-14,20)" status="error">No implementation was given for 'abstract IThing\.Name: string'\. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e\.g\. 'interface \.\.\. with member \.\.\.'</Expects>

open System
type IThing =
    abstract Name: string
    abstract Action: List< string * (unit -> unit) >

[<AbstractClass>]
type Dog() =
   interface IThing with
     member x.Action = [("bites", fun () -> printfn "ouch")]
     /// type checker passes but compiler reports
     /// error FS0193: internal error: Method 'get_Name' in type 'MyDog' from assembly 'FSI-ASSEMBLY, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null' does not have an implementation.
     /// Please build a small example that reproduces this problem and report it to fsbugs@microsoft.com. 

type MyDog() =
   inherit Dog() 
    /// Not sure if this is useful for anything but I'm dutifully reporting.
    /// internal error: tcref_of_stripped_typ (Failure) 
    /// Please build a small example that reproduces this problem and report it to fsbugs@microsoft.com

