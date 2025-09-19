type MyUnion = 
    | CaseA of x: int
    | CaseB of x: int * y: string
    | CaseC of x: float * y: bool * z: char

let testSemicolon2 value =
    match value with
    | CaseA(x = x) -> sprintf "CaseA: a=%d" x
    | CaseB(x = x; _) -> sprintf "CaseA: a=%d" x
    | CaseC(x = p; y = q; _) -> sprintf "CaseB: x=%f, y=%b" p q

let expected = 
    [ "CaseA: a=42"
      "CaseA: a=7"
      "CaseB: x=3.140000, y=true" ]

let actual = 
    [ testSemicolon2 (CaseA 42) 
      testSemicolon2 (CaseB(7, "hello")) 
      testSemicolon2 (CaseC(3.14, true, 'z')) ]

if actual <> expected then 
    failwithf "expected: %A, got: %A" expected actual