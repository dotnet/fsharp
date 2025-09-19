type CaseB = 
    | CaseB of x: int * y: string * z: bool

let (CaseB(x = a; y = b; _)) = CaseB(7, "hello", true)

let actual2 = a, b

if actual2 <> (7, "hello") then
    failwithf "expected: %A, got: %A" [7, "hello"] actual2