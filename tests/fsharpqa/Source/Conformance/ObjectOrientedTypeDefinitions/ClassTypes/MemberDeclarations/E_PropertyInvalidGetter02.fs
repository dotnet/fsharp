// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions 
// Verify compiler disallows duplicate getters (and setters) on the same property
// Regression test for FSHARP1.0:4882
//<Expects id="FS0555" span="(11,27-11,39)" status="error">'get' and/or 'set' required</Expects>
//<Expects id="FS0555" span="(16,27-16,39)" status="error">'get' and/or 'set' required</Expects>

#light

type A() = 
    member this.Item with get (x: int, y: int) = 6 
                      and get (x: int) = 6


type B() = 
    member this.Item with set (x: int, y: int) = ()
                      and set (x: int) = ()
