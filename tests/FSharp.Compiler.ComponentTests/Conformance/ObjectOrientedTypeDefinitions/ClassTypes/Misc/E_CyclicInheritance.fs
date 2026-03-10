// #Regression #Conformance #ObjectOrientedTypes #Classes 
// FSB 1233, Cyclic inheritance bug
//<Expects status="error" span="(8,6-8,7)" id="FS0954">This type definition involves an immediate cyclic reference through a struct field or inheritance relation$</Expects>
//<Expects status="error" span="(10,3-10,6)" id="FS0871">Constructors cannot be defined for this type$</Expects>
//<Expects status="error" span="(10,12-10,28)" id="FS0787">The inherited type is not an object model type$</Expects>
//<Expects status="error" span="(10,12-10,28)" id="FS0696">This is not a valid object construction expression\. Explicit object constructors must either call an alternate constructor or initialize all fields of the object and specify a call to a super class constructor\.$</Expects>

type C = class
  inherit C
  new () = { inherit C () }
end

let x = new C ()



