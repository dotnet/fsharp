// #Regression #Conformance #TypeInference #ByRef 
#light

// Verify error when setting object field to a byref value. 
// (This is disallowed by the CLR.)

//<Expects id="FS0437" span="(10,6-10,9)" status="error">A type would store a byref typed value\. This is not permitted by Common IL\.$</Expects>
//<Expects id="FS0412" span="(11,50-11,54)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>

type Foo() =
    let mutable m_byrefOpt : byref<int> option = None
