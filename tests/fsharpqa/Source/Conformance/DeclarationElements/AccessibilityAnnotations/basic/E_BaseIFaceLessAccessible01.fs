// #Regression #Conformance #DeclarationElements #Accessibility 
// Regression test for FSHARP1.0:5265
// It is *ok* to implement internalized interfaces
//<Expects status="error" span="(11,8-11,23)" id="FS0410">The type 'I1' is less accessible than the value, member or type 'IAmAnInterface1' it is used in.$</Expects>

module N.M
  // Interface inheritance:
  type private I1 =
    abstract P: int

  type IAmAnInterface1 =
    inherit I1
    abstract Q : int
