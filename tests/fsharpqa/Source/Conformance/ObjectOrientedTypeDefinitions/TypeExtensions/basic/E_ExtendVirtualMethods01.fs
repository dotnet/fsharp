// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
#light

// It's error if the augmentation is of a type from a different assembly
// The is regression test for FSHARP1.0:3272

//<Expects id="FS0854" span="(10,14)" status="error">Method overrides and interface implementations are not permitted here</Expects>

type System.String with
    override this.ToString() = "Overridden"

printfn "\"foo\".ToString() = %s" ("foo".ToString())
    
if "foo".ToString() <> "foo" then exit 1

exit 0
