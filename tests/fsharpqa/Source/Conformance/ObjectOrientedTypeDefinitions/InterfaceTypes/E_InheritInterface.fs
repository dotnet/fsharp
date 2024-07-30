// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:5962
// Verify ability to inherit from interface with "interface", which allows to specify implementations for static abstracts

module M

type I = interface
         end

type I' = interface I         
