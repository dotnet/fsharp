// #Regression #Conformance #ObjectOrientedTypes #Classes #Inheritance 
// Regression test for FSHARP1.0:3782
// Make sure we do not ICE when trying to
// inherit from a type variable.
//<Expects id="FS0753" span="(9,19)" status="error">Cannot inherit from a variable type</Expects>
#light

type X<'a>() = class
                  inherit 'a()
               end
