// #Regression #Conformance #DeclarationElements #ObjectConstructors 
// FSB 1749, Interfaces should not allow implicit construction pattern. Bad codegen.

//<Expects id="FS0866" status="error" span="(8,6)">Interfaces cannot contain definitions of object constructors$</Expects>

// Test that interfaces cannot use object constructors.

type X() =
    interface
    end
