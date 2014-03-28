// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions 
// Verify that omitting "()" for the getter simply yields an error (instead of internal error)
// (Note: in 1.9.6.16, the compiler was throwing an ICE). This is regression test for FSHARP1.0:4537
// Regression test for FSHARP1.0:5375
//<Expects id="FS0557" span="(11,36-11,39)" status="error">A getter property is expected to be a function, e\.g\. 'get\(\) = \.\.\.' or 'get\(index\) = \.\.\.'</Expects>

#light

type action() =
    let mutable m_addToExisting = false
    member this.AddToExisting with get   = m_addToExisting
                              and  set x = m_addToExisting <- x

    member this.Execute () =
        let ifAdd = this.AddToExisting
        ()

