// #Regression #Conformance #DeclarationElements #Events 
// Regression test for FSHARP1.0:5192
// The following code should not ICE!
open System.Threading
 
let postEventInContext,sendEventInContext = 
    let callBack (event:Event<'a>) arg =
        let f _ = event.Trigger(arg)
        new SendOrPostCallback(f)
    let post (context:SynchronizationContext) (event:Event<'a>) arg =
        context.Post((callBack event arg),null)
    let send (context:SynchronizationContext) (event:Event<'a>) arg =
        context.Send((callBack event arg),null)
    post,send
