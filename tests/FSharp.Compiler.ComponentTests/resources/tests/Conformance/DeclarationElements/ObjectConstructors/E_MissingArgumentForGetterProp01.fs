// #Regression #Conformance #DeclarationElements #ObjectConstructors 
//<Expects id="FS0557" span="(7,32-7,35)" status="error">A getter property is expected to be a function, e\.g\. 'get\(\) = \.\.\.' or 'get\(index\) = \.\.\.'</Expects>
// Regression test for FSHARP1.0:5375
type T =
    class
        val mutable m_value : int
        member this.Value with get = this.m_value  // missing ()
    end

exit 1

