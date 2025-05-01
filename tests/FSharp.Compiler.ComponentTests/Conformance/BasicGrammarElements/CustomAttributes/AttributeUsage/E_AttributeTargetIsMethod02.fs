// This tests that AttributeTargets.Method is not allowed in let bound values

open System

[<AttributeUsage(AttributeTargets.Method)>]
type MethodOnlyAttribute() = 
   inherit Attribute()

[<MethodOnly>] // Should fail
let val1 = "someValue"

[<MethodOnly>] // Should fail
let i, j, k = (1, 2, 3) 

[<MethodOnly>]  // Should fail
let val2 = nameof(MethodOnlyAttribute)

[<MethodOnly>] // Should fail
let rec val3 = nameof(val2) 

[<MethodOnly>] // Should fail
let ``val4`` = "someValue" 

[<MethodOnly>]  // Should fail
let rec val5 = 0
and [<MethodOnly>] val6 = [] // Should fail
 
[<MethodOnly>]  // Should fail
let (a :: _) = []

[<MethodOnly>] // Should fail
let (d, e) as val7 = 1, 2 

[<MethodOnly>] // Should fail
let 1 = 0 

type X = { X: int }

[<MethodOnly>] // Should fail
let { X = _ } = { X = 1 } 
