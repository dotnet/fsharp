// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions 
// Regression test for 342901
//<Expects id="FS3172" span="(6,17-6,18)" status="error">A property's getter and setter must have the same type\. Property 'X' has getter of type 'obj option' but setter of type 'obj'\.</Expects>
type Foo(x) =
    let mutable x = x
    member this.X 
        with get() = x
        and set(value) = x <- Some value

