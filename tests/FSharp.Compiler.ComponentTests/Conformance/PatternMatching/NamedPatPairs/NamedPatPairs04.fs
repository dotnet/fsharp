exception Error1 of x: string
exception Error2 of x: string * y: int
exception Error3 of x: string * y: int * z: float

let testSemicolon (value: exn) =
   try
      raise value
   with
   | Error1(x = x) -> sprintf "Error1: a=%s" x
   | Error2(x = x; y = y) -> sprintf "Error2: a=%s, b=%d" x y
   | Error3(x = p; y = q; z = r) -> sprintf "Error3: x=%s, y=%d, z=%f" p q r

let expected = 
   [ "Error1: a=error one"
     "Error2: a=error two, b=2"
     "Error3: x=error three, y=3, z=3.140000" ]

let actual = 
   [ testSemicolon (Error1 "error one")
     testSemicolon (Error2("error two", 2))
     testSemicolon (Error3("error three", 3, 3.14)) ]

if actual <> expected then
   failwithf "expected: %A, got: %A" expected actual