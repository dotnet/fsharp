module Neg30
let f<'a when 'a : unmanaged> (x : 'a) = ()
;;
do f 10
do f 10u
;;
[<Struct>]
type SUnmanaged =
   val i : int
   val j : uint32
   member this.X = this.i, this.j
;;
[<Struct>]
type SUnmanagedRecursive =
   val s1 : SUnmanaged
   val i : int

[<Struct>]
type SManaged =
   val v : obj

[<Struct>]
type SManagedRecursive =
   val s1 : SUnmanaged
   val s2 : SManaged

[<Struct>]
type SBadRecursion =
   val s : SBadRecursion

[<Struct>]
type SGeneric<'T> =
   val s : int

type FSharpUnion =
| XA 
| XB
| XC


do f (new SUnmanaged())               // Ok
do f (new SUnmanagedRecursive())      // Ok
do f (new SManaged())                 // Error
do f (new SManagedRecursive())        // Error
do f (new obj())                      // Error
do f (new SBadRecursion())            // Should not crash
do f XA                               // Error
do f (new SGeneric<int>())            // Error

[<Measure>]
type kg

do f (1.5<kg>)                        // Ok

type C<'a when 'a : unmanaged>() = class end

let _ = new C<SUnmanaged>()               // Ok
let _ = new C<SUnmanagedRecursive>()      // Ok
let _ = new C<SManaged>()                 // Error
let _ = new C<SManagedRecursive>()        // Error
let _ = new C<obj>()                      // Error
let _ = new C<SBadRecursion>()            // Should not crash
let _ = new C<FSharpUnion>()               // Error
let _ = new C<SGeneric<int>>()               // Error


type CompilerMessageTest() = 
    [<CompilerMessage("hello!", 120)>] 
    member x.P = 1

let test = CompilerMessageTest().P


type CompilerMessageTest2() = 
    [<CompilerMessage("hello!", 10021, IsError=true)>]
    member x.P = 1

let test2 = CompilerMessageTest2().P

