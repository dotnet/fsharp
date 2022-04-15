// #Regression #Conformance #TypeInference #ByRef 
// Verify you cannot set an object field to store a byref value

//<Expects id="FS0431" span="(8,9-8,17)" status="error">A byref typed value would be stored here\. Top-level let-bound byref values are not permitted</Expects>

module ModuleFoo =
    let mutable x = 0
    let byrefVal = &x

    
