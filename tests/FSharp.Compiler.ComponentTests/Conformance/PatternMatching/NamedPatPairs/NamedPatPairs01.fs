// #Conformance #PatternMatching #NamedPatPairs 
type MyUnion = 
    | CaseA of a: int * b: string
    | CaseB of x: float * y: bool * z: char

let testComma value =
    match value with
    | CaseA(a = x, b = y) -> sprintf "CaseA: a=%d, b=%s" x y
    | CaseB(x = p, y = q, z = r) -> sprintf "CaseB: x=%f, y=%b, z=%c" p q r

let testSemicolon value =
    match value with
    | CaseA(a = x; b = y) -> sprintf "CaseA: a=%d, b=%s" x y
    | CaseB(x = p; y = q; z = r) -> sprintf "CaseB: x=%f, y=%b, z=%c" p q r

let testMixed value =
    match value with
    | CaseA(a = x; b = y) -> sprintf "CaseA: a=%d, b=%s" x y
    | CaseB(x = p, y = q; z = r) -> sprintf "CaseB: x=%f, y=%b, z=%c" p q r
    | CaseA(a = x, b = y) -> sprintf "CaseA: a=%d, b=%s" x y
    | CaseB(x = p; y = q, z = r) -> sprintf "CaseB: x=%f, y=%b, z=%c" p q r