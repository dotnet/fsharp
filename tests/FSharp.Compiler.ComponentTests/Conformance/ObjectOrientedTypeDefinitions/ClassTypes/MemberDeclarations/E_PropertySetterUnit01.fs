// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions 
// Verify that it is ok to omitting the argument for the setter (no error)
// (Note: in 1.9.6.2, the compiler was throwing an ICE). This is regression test for FSHARP1.0:3751
// See also bug #5456
// Since Dev11, A property's getter and setter must have the same type, so error would be reported.
// See bug #342901
//<Expects id="FS3172" span="(12,27-12,30)" status="error">A property's getter and setter must have the same type\. Property 'ini' has getter of type 'bool' but setter of type 'unit'\.</Expects>
//<Expects id="FS3172" span="(19,27-19,30)" status="error">A property's getter and setter must have the same type\. Property 'ini' has getter of type 'bool' but setter of type 'unit'\.</Expects>
module M
type test = class
              val mutable Init : bool
              member this.ini
               with get () = this.Init
               and set () = this.Init <- false              // <- missing arg to set()
            end

type test2= class
              val mutable Init : bool
              member this.ini
               with get () = this.Init
               and set (v:unit) = this.Init <- false        // <- exact same thing as above
            end