// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions 
// Verify that one should not be able to have two properties called �X� with different arities (number of arguments).
// This is regression test for FSHARP1.0:4529

//<Expects id="FS0436" span="(11,17-11,18)" status="error">The property 'X' has the same name as another property in this type</Expects>


type TestType1( x : int ) =  
    let mutable x = x
    
    member this.X with get () = x
                  and  set x' = x <- x'
    member this.X with set x' x'' = x <- x' + x''
    
exit 1
