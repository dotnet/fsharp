// #Conformance #ObjectOrientedTypes #TypeExtensions  
//<Expects id="FS0854" status="error" span="(8,25-8,37)">Method overrides and interface implementations are not permitted here</Expects>

module M = 
    type R() = class end
module U =
   open M
   type R with override x.ToString() = "hi"
 
 
open M
open U
let x = R()
printfn "%A" (x.ToString())
