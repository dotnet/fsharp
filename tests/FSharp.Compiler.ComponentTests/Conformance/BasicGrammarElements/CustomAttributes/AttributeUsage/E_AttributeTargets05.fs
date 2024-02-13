
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
   
   [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
   member this.IsNone = match 0 with 1 -> true | _ -> false

