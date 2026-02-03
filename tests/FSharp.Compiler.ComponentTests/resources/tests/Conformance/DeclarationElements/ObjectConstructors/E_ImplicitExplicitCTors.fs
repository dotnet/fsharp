// #Regression #Conformance #DeclarationElements #ObjectConstructors 
// Regression test for FSHARP1.0:1341
// Compiler should reject explicit class construction when an implicit one is defined
//<Expects id="FS0762" span="(11,13)" status="error">Constructors for the type 't' must directly or indirectly call its implicit object constructor\. Use a call to the implicit object constructor instead of a record expression</Expects>

type s = class 
   new () = {}
end

type t (x:s) = class
   new () = { }                 (* <- error here! *)
//   member self.get () = x
end
