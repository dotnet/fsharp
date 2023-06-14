// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

type Record = 
    { A : int; B : int; C : int; D : int }
    member this.AddTo a b c d = (this.A + a, this.B + b, this.C + c, this.D + d)

let testRec = {A = 10; B = 100; C = 1000; D = 10000}

let curriedAddTo1 = testRec.AddTo 1
let curriedAddTo2 = curriedAddTo1 11
let curriedAddTo3 = curriedAddTo2 111

let result = curriedAddTo3 1111
if result <> (11, 111, 1111, 11111) then failwith "Failed: 1"

// Just to double check
let (a, b, c, d) = result
if a <> 11 || b <> 111 || c <> 1111 || d <> 11111 then failwith "Failed: 2"
