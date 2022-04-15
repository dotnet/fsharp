// #Regression #Conformance #BasicGrammarElements #Operators 
#light

// FSB 3993, Operator overloading of == and != is inconsistent

type Test(x: int) =
     member t.X = x
     static member op_EqualsEquals (t1: Test, t2: Test) = t1.X = t2.X
     static member op_BangEquals   (t1: Test, t2: Test) = t1.X <> t2.X

let a, b = Test(1), Test(2)

let _ = ( a == b ) 
let _ = ( a != b )
