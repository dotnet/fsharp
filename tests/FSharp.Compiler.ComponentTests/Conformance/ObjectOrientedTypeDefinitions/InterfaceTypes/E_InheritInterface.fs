// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:5962
//<Expects status="error" span="(10,21-10,22)" id="FS1207">Interfaces inherited by other interfaces should be declared using 'inherit \.\.\.' instead of 'interface \.\.\.'$</Expects>

module M

type I = interface
         end

type I' = interface I

         
