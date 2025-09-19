type MyUnion = 
    | CaseA of x: int
    | CaseB of x: int * y: string
    | CaseC of x: float * y: bool * z: char

let testComma value =
    match value with
    | CaseA(x = x) -> sprintf "CaseA: a=%d" x
    | CaseB(x = x, y = y) -> sprintf "CaseA: a=%d, b=%s" x y
    | CaseC(x = p, y = q, z = r) -> sprintf "CaseB: x=%f, y=%b, z=%c" p q r

let expected = 
    [ "CaseA: a=42"
      "CaseA: a=7, b=hello"
      "CaseB: x=3.140000, y=true, z=z" ]

let actual = 
    [ testComma (CaseA 42)
      testComma (CaseB(7, "hello"))
      testComma (CaseC(3.14, true, 'z')) ]

if actual <> expected then
    failwithf "expected: %A, got: %A" expected actual