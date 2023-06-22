// #Regression #Conformance #ObjectOrientedTypes #Classes #ObjectConstructors 
// Regression test for FSHARP1.0:3217
// Use misc constructs in explicit object constructor
// construct: while ... do ... done
#light

type T =
    new () = 
        while true do
           ()
        done
        new T()
