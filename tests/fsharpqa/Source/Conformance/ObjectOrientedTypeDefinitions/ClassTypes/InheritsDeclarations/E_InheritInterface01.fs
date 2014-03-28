// #Regression #Conformance #ObjectOrientedTypes #Classes #Inheritance 
#light

// Verify error when trying to inherit from interface type.

// Note the plethora of errors, see FSB 1330 about reducing the number of errors.

//<Expects id="FS0961" status="error">This 'inherit' declaration specifies the inherited type but no arguments\. Consider supplying arguments, e\.g\. 'inherit BaseType\(args\)'</Expects>
//<Expects id="FS0946" status="error">Cannot inherit from interface type\. Use interface \.\.\. with instead</Expects>

type I =
    abstract VirtMethod : unit -> int
    
type A() =
    inherit I with
        override this.VirtMethod () = 1

exit 1
