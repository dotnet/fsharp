// #Conformance #ObjectOrientedTypes #MethodsAndProperties #MemberDefinitions 
module FSLibrary

// Interface with a member
type I = abstract M : unit -> unit

type C = 
    new() = {}
    abstract member M : unit -> unit
    default this.M() = System.Console.WriteLine("I am my method")
    interface I with
        member this.M() = 
            System.Console.WriteLine("I am going via the interface")
            this.M()
