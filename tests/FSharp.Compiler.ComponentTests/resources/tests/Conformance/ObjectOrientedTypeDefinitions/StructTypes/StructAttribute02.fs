// #Regression #Conformance #ObjectOrientedTypes #Structs 
#light

// Verify errors associated with putting the struct attribute on non structs
//<Expects id="FS0927" status="error">The kind of the type specified by its attributes does not match the kind implied by its definition</Expects>

[<Struct>]
type IAmAnInterface = interface
    abstract DoStuff1 : unit -> unit
    abstract DoStuff2 : unit -> unit
    end


exit 1
