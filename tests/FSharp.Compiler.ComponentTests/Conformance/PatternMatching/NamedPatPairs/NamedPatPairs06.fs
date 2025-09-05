type MyUnion = 
    | CaseA of x: (float * bool)

let testTupled value =
    match value with
    | CaseA(x = a, b) -> $"CaseA: x=%f{a}, %b{b}"

let expected = "CaseA: x=3.140000, true"

let actual = testTupled (CaseA(3.14, true))

if actual <> expected then 
    failwithf "expected: %A, got: %A" expected actual