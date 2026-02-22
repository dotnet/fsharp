// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2890
//<Expects id="FS0077" span="(6,14-6,61)" status="warning">Member constraints with the name 'Pow' are given special status by the F# compiler as certain \.NET types are implicitly augmented with this member\. This may result in runtime failures if you attempt to invoke the member constraint from your own code\.$</Expects>
module M
let inline trapPow  (x: ^a) (y: ^a) : ^a = 
             (^a: (static member Pow : ^a * ^a -> ^a) (x,y))

(trapPow 1.1 2.2) |> ignore


