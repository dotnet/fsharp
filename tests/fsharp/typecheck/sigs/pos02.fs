module Test
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

type FSharpEnum =
| A = 1
| B = 2
| C = 3


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



do f (new SUnmanaged())               // Ok
do f (new SUnmanagedRecursive())      // Ok
do f FSharpEnum.A                     // Ok
do f (System.DayOfWeek.Monday)        // Ok

type SAbbrev = SUnmanaged

do f (new SAbbrev())

[<Measure>]
type kg

do f (1.5<kg>)                        // Ok

type C<'a when 'a : unmanaged>() = class end

let _ = new C<SUnmanaged>()               // Ok
let _ = new C<SUnmanagedRecursive>()      // Ok
let _ = new C<FSharpEnum>()               // Ok
let _ = new C<System.DayOfWeek>()         // Ok
let _ = new C<SAbbrev>()                  // Ok
