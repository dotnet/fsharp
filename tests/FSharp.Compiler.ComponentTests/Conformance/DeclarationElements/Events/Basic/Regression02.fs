// #Regression #Conformance #DeclarationElements #Events 
// Regression test for FSHARP1.0:5192
// The following code should not ICE!

let postEventInContext,sendEventInContext = 
    let post (event:Event<'a>) = ()
    let send (event:Event<'a>) = ()
    post,send
