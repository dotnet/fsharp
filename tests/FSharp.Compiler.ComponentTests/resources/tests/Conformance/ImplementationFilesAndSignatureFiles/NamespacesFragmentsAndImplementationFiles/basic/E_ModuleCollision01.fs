// #Regression #Conformance #SignatureFiles #Namespaces 
// Verify error if the same fully-qualified type is defined twice
//<Expects id="FS0037" status="error" span="(13,1)">Duplicate definition of type, exception or module 'B'</Expects>

module A 

module B =

   type DU =
        | X of float
        | Y of float

module B =

    type DU =
        | X' of float
        | Y' of float

