
let a = Module1.Test.bar()
let b = sprintf "%A" (Module1.Test.run())

let test1 = (a=b)
type D() = 

  [<ReflectedDefinition>]
  static member F x = (Module1.C(), D(), System.DateTime.Now)


let z2 = Quotations.Expr.TryGetReflectedDefinition(typeof<Module1.C>.GetMethod("F"))
let s2 = (sprintf "%2000A" z2) 
let test2 = (s2 = "Some Lambda (x, NewTuple (NewObject (C), PropertyGet (None, Now, [])))")

let z3 = Quotations.Expr.TryGetReflectedDefinition(typeof<D>.GetMethod("F"))
let s3 = (sprintf "%2000A" z3) 
let test3 = (s3 = "Some Lambda (x, NewTuple (NewObject (C), NewObject (D), PropertyGet (None, Now, [])))")

#if EXTRAS
// Add some references to System.ValueTuple, and add a test case which statically links this DLL
let test4 = struct (3,4)
let test5 = struct (z2,z3)
#endif

if not test1 then 
    stdout.WriteLine "*** test1 FAILED"; 
    eprintf "FAILED, in-module result %s is different from out-module call %s" a b

if not test2 then 
    stdout.WriteLine "*** test2 FAILED"; 
    eprintf "FAILED, %s is different from expected" s2
if not test3 then 
    stdout.WriteLine "*** test3 FAILED"; 
    eprintf "FAILED, %s is different from expected" s3


if test1 && test2 && test3 then
    stdout.WriteLine "Test Passed"; 
    printf "TEST PASSED OK"; 
    exit 0
else
    exit 1

