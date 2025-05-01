// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Setter declared using curried syntax
// Expected: invalid declaration syntax

let mutable globalPt : obj = null

type Pt =
    { X : float; Y : float }
    member this.Move with set x, y = 
                                globalPt <- { X = this.X + x; Y = this.Y + y}
                                ()

let org = { X = 0.0; Y = 0.0 }
org.Move <- 1.0, 2.0
