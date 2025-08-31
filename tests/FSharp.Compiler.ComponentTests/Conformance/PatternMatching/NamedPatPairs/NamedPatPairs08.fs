type SynLongIdent = 
| SynLongIdent of id: string * dotRanges: string list

type RecordFieldName = SynLongIdent * bool

type MyUnion = 
    | CaseA of x: RecordFieldName

let testTupled value =
    match value with
    | CaseA(x = SynLongIdent(a, b), _) -> $"CaseA: x=%s{a} %A{b}"

let testTupled2 value =
    match value with
    | CaseA(x = (SynLongIdent(a, b), _)) -> $"CaseA: x=%s{a} %A{b}"

let expected = "CaseA: x=3.140000 [\"\"]"

let actual = testTupled (CaseA((SynLongIdent("3.140000", [""]), true)))

if actual <> expected then 
    failwithf "expected: %A, got: %A" expected actual

let actual2 = testTupled2 (CaseA((SynLongIdent("3.140000", [""]), true)))

if actual2 <> expected then 
    failwithf "expected: %A, got: %A" expected actual2