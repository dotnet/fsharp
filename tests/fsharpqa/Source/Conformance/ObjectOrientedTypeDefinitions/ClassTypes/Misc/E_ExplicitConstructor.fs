// #Regression #Conformance #ObjectOrientedTypes #Classes 
// Bug 3540 - this used to crash the compiler
//<Expects id="FS0787" span="(9,27-9,58)" status="error">The inherited type is not an object model type</Expects>
//<Expects id="FS0001" span="(9,60-9,61)" status="error">This expression was expected to have type.    'A'    .but here has type.    'int'</Expects>
//<Expects id="FS0696" span="(9,60-9,61)" status="error">This is not a valid object construction expression\. Explicit object constructors must either call an alternate constructor or initialize all fields of the object and specify a call to a super class constructor\.$</Expects>

type A =
  inherit System.Exception 
  new (x : string) as y = { inherit System.Exception(x) }; 1
