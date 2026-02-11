// #Regression #TypeInference
//<Expects id="FS0193" span="(5,9-5,10)" status="error">Type constraint mismatch. The type.+''a'.+is not compatible with type.+System\.IDisposable</Expects>
// Regression test for F# 3.0 bug 130523
let _ =
    use x = null
    1
