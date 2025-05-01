// #Regression #Conformance #TypesAndModules #Records 
// Dev11: 293290, previously record fields didn't satisfy static member constraints

type A = { Age: int;    Name: string }
type B = { name: string } with member this.Name = this.name
 
let inline name (x:^T) = (^T : (member Name : string) x)
 
let a = { Age = 29; Name = "Steffen" }
let b = { name = "Gary" }
 
let r = name a
if r <> "Steffen" then failwith "Failed"