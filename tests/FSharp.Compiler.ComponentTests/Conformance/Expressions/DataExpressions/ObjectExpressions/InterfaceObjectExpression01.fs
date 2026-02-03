// #Regression #Conformance #DataExpressions #ObjectConstructors  
// Regression for FSHARP1.0:6510
// 
type I = 
    abstract ToString: unit -> string
type J = 
    abstract ToString: unit -> string

let x = 
   { new I with 
        member __.ToString () = "AAA"
     interface J with 
        member __.ToString () = "BBB" }

if (x :> obj).ToString () = "AAA" then exit 1       // used to print "AAA" instead of something like "FSI_0003+x@8"
if x.ToString() <> "AAA" then exit 1                 
if (x :> obj :?> I).ToString() <> "AAA" then exit 1  
if (x :> obj :?> J).ToString() <> "BBB" then exit 1 

exit 0