
open System
open System.Diagnostics

[<AttributeUsage(AttributeTargets.Field)>]
type FieldOnlyAttribute() = 
   inherit Attribute()

[<AttributeUsage(AttributeTargets.Method)>]
type MethodOnlyAttribute() = 
   inherit Attribute()

type Test() = 
   [<FieldOnly>]
   let mutable value = 0

   [<MethodOnly>]
   let myFunction() = ()
   
   [<FieldOnly>]
   let myFunction2 () = ()

   [<MethodOnly>]
   let value2 = 0

   [<FieldOnly>]
   static let value3 = 0

   [<MethodOnly>]
   static let myFunction3 () = ()

   [<FieldOnly>]
   static let myFunction4 () = ()

   [<MethodOnly>]
   static let value4 = 0

   [<MethodOnly>]
   let rec f = 0
   and [<FieldOnly>] g() = []

   [<FieldOnly>]
   let rec f1 = 0
   and [<MethodOnly>] g2() = []

   [<FieldOnly>]
   static let rec value5 = 0

   [<MethodOnly>]
   static let rec myFunction5() = ()

   [<FieldOnly>]
   static let rec myFunction6() = 0

   [<MethodOnly>]
   static let rec value6 = ()
   
   [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
   member this.IsNone = match 0 with 1 -> true | _ -> false

   [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
   static member None = None

