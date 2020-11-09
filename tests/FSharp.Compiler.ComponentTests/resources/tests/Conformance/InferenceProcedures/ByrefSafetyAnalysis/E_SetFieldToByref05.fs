// #Regression #Conformance #TypeInference #ByRef 
// Verify appropriate error if attempting to assign a ByRef value to an
// object field. (Disallowed by the CLR.)

//<Expects id="FS0437" span="(8,6-8,17)" status="error">A type would store a byref typed value\. This is not permitted by Common IL\.$</Expects>
//<Expects id="FS0412" span="(9,7-9,8)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>

type DUWithByref =
    | A of int * byref<int>
