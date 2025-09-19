type TupleAbbrev = float * bool

type MyUnion = 
    | CaseA of x: TupleAbbrev * y: TupleAbbrev

let testTupled value =
    match value with
    | CaseA(x = a, b, y = f, g) -> $"CaseA: x=%f{a}, %b{b}, y=%f{f}, %b{g}"

let testTupled2 value =
    match value with
    | CaseA(x = (a, b), y = f, g) -> $"CaseA: x=%f{a}, %b{b}, y=%f{f}, %b{g}"

let testTupled3 value =
    match value with
    | CaseA(x = _, _, y = _, _) -> $"CaseA: x=_, _, y=_, _"

let expected = "CaseA: x=3.140000, true, y=2.720000, false"

let expected2 = "CaseA: x=_, _, y=_, _"

let actual = testTupled (CaseA((3.14, true), (2.72, false)))

let actual2 = testTupled2 (CaseA((3.14, true), (2.72, false)))

let actual3 = testTupled3 (CaseA((3.14, true), (2.72, false)))

if actual <> expected then 
    failwithf "expected: %A, got: %A" expected actual

if actual2 <> expected then 
    failwithf "expected: %A, got: %A" expected actual2

if actual3 <> expected2 then
    failwith "expected: %A, got: %A" expected2 actual3