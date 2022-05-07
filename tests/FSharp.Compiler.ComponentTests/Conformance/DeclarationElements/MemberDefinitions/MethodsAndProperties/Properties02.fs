// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties
// Verify write-only properties
// See also FSHARP1.0:4163
let mutable globalPt : obj = null

type Pt =
    { X : float; Y : float }
    member this.Move with set x y = 
                                globalPt <- { X = this.X + x; Y = this.Y + y}
                                ()
    member this.Move2 with set ((x, y)) = 
                                globalPt <- { X = this.X + x; Y = this.Y + y}
                                ()

let org = { X = 0.0; Y = 0.0 }
org.Move(1.0) <- 2.0

let cp = globalPt :?> Pt
if cp.X <> 1.0 then failwith "Failed: 1"
if cp.Y <> 2.0 then failwith "Failed: 2"

org.Move2 <- (-1.0,-2.0)

let cp2 = globalPt :?> Pt
if cp2.X <> -1.0 then failwith "Failed: 3"
if cp2.Y <> -2.0 then failwith "Failed: 4"

