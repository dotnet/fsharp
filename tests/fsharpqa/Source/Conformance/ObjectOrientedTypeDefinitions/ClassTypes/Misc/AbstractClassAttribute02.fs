// #Regression #Conformance #ObjectOrientedTypes #Classes 
#light

// Verify error associated with putting the AbstractClass attribute o
// on non-class types.

//<Expects id="FS0939" status="error">Only classes may be given the 'AbstractClass' attribute</Expects>
//<Expects id="FS0939" status="error">Only classes may be given the 'AbstractClass' attribute</Expects>

[<AbstractClass>]
type IAmAnInterface = interface
    abstract DoStuff : unit -> unit
    end

[<AbstractClass>]
type StructType = struct
    val X : int
    end

exit 1
