// This tests that AttributeTargets.Method is not allowed in class let bound values

open System
open System.Diagnostics

[<AttributeUsage(AttributeTargets.Method)>]
type MethodOnlyAttribute() = 
   inherit Attribute()

type TestClass() = 

   [<MethodOnly>] // Should fail
   static let val1 = "someValue" 

   [<MethodOnly>]  // Should fail
   static let rec val2 = "someValue"

   [<MethodOnly>] // Should fail
   static let rec val3 = "someValue" 
   and [<MethodOnly>] val4 = "someValue" // Should fail

   [<MethodOnly>] // Should fail
   let val5 = "someValue" 

   [<MethodOnly>]  // Should fail
   let i, j, k = (1, 2, 3)

   [<MethodOnly>] // Should fail
   let val5 = nameof(MethodOnlyAttribute) 

   [<MethodOnly>] // Should fail
   let rec val6 = nameof(val5) 

   [<MethodOnly>] // Should fail
   let ``val7`` = "someValue" 

   [<MethodOnly>]  // Should fail
   let rec val8 = 0
   and [<MethodOnly>] val9 = [] // Should fail
 
   [<MethodOnly>] // Should fail
   let (a :: _) = [] 

   [<MethodOnly>]  // Should fail
   let (d, e) as foo = 1, 2

   [<MethodOnly>]  // Should fail
   let 1 = 0

   type X = { X: int }

   [<MethodOnly>]  // Should fail
   let { X = _ } = { X = 1 }

