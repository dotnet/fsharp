type CaseB = 
    | CaseB of x: int * y: string

let (CaseB(x = a, y = b)) = CaseB(7, "hello")

let actual2 = a, b

if actual2 <> (7, "hello") then
    failwithf "expected: %A, got: %A" [7, "hello"] actual2

type CaseC = 
    | CaseC of x: float * y: bool * z: char

let (CaseC(x = p, y = q, z = r)) = CaseC(3.14, true, 'z')

let actual3 = (p, q, r)

if actual3 <> (3.14, true, 'z') then
    failwithf "expected: %A, got: %A" (3.14, true, 'z') actual3