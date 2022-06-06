// #Regression #Conformance #TypeInference #ByRef 
// Verify appropriate error if attempting to assign a ByRef value to an
// object field. (Disallowed by the CLR.)

//<Expects id="FS0437" span="(8,6-8,21)" status="error">A type would store a byref typed value\. This is not permitted by Common IL\.$</Expects>
//<Expects id="FS0412" span="(8,25-8,26)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>

type RecordWithByref = {A : byref<int> }


