// #Regression #Conformance #ObjectOrientedTypes #Classes 
// FSB 1467, What to do about marking types as abstract?
//<Expects id="FS0759" status="error" span="(10,12)">Instances of this type cannot be created since it has been marked abstract or not all methods have been given implementations\. Consider using an object expression '{ new \.\.\. with \.\.\. }' instead</Expects>

[<AbstractClass>]
type Foo() =
    override this.ToString() = "stuff"
    
// Should fail, since Foo is abstract
let test = new Foo()

// Should have compile errors
exit 1
