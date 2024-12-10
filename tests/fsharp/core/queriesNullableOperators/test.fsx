// #Query
#if TESTS_AS_APP
module Core_queriesNullableOperators
#endif


#nowarn "57"

open System
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.ExprShape
open Microsoft.FSharp.Linq.RuntimeHelpers

[<AutoOpen>]
module Infrastructure =
    let failures = ref []

    let report_failure (s : string) = 
        stderr.Write" NO: "
        stderr.WriteLine s
        failures := !failures @ [s]

    let check  s v1 v2 = 
       if v1 = v2 then 
           printfn "test %s...passed " s 
       else 
           report_failure (sprintf "test %s...failed, expected %A got %A" s v2 v1)

    let test s b = check s b true

module NullableOperatorTests = 

    open Microsoft.FSharp.Linq.NullableOperators
    check "op2oin209v20" (Nullable 2 ?=? Nullable 3) false
    check "op2oin209v21" (Nullable 3 ?=? Nullable 3) true
    check "op2oin209v22" (Nullable 3 ?=? Nullable()) false
    check "op2oin209v23" (Nullable () ?=? Nullable 3) false
    check "op2oin209v24" (Nullable () ?=? Nullable ()) true

    check "op2oin209v11" (2 =? Nullable 3) false
    check "op2oin209v12" (3 =? Nullable 3) true
    check "op2oin209v13" (3 =? Nullable()) false

    check "op2oin209v30" (Nullable 2 ?= 3) false
    check "op2oin209v31" (Nullable 3 ?= 3) true
    check "op2oin209v33" (Nullable () ?= 3) false

    check "op2oin209v301" (Nullable 2 ?> 3) false
    check "op2oin209v312" (Nullable 3 ?> 3) false
    check "op2oin209v313" (Nullable 4 ?> 3) true
    check "op2oin209v334" (Nullable () ?> 3) false

    check "op2oin209v304" (Nullable 2 ?>= 3) false
    check "op2oin209v315" (Nullable 3 ?>= 3) true
    check "op2oin209v316" (Nullable 4 ?>= 3) true
    check "op2oin209v337" (Nullable () ?>= 3) false

    check "op2oin209v308" (Nullable 2 ?< 3) true
    check "op2oin209v319" (Nullable 3 ?< 3) false
    check "op2oin209v31q" (Nullable 4 ?< 3) false
    check "op2oin209v33w" (Nullable () ?< 3) false

    check "op2oin209v30e" (Nullable 2 ?<= 3) true
    check "op2oin209v31r" (Nullable 3 ?<= 3) true
    check "op2oin209v31t" (Nullable 4 ?<= 3) false
    check "op2oin209v33y" (Nullable () ?<= 3) false


    check "op2oin209v30u" (Nullable 2 ?>? Nullable 3) false
    check "op2oin209v31i" (Nullable 3 ?>? Nullable 3) false
    check "op2oin209v31o" (Nullable 4 ?>? Nullable 3) true
    check "op2oin209v33p" (Nullable () ?>? Nullable 3) false

    check "op2oin209v30a" (Nullable 2 ?>=? Nullable 3) false
    check "op2oin209v31s" (Nullable 3 ?>=? Nullable 3) true
    check "op2oin209v31d" (Nullable 4 ?>=? Nullable 3) true
    check "op2oin209v33f" (Nullable () ?>=? Nullable 3) false

    check "op2oin209v30g" (Nullable 2 ?<? Nullable 3) true
    check "op2oin209v31h" (Nullable 3 ?<? Nullable 3) false
    check "op2oin209v31j" (Nullable 4 ?<? Nullable 3) false
    check "op2oin209v33k" (Nullable () ?<? Nullable 3) false

    check "op2oin209v30l" (Nullable 2 ?<=? Nullable 3) true
    check "op2oin209v31a" (Nullable 3 ?<=? Nullable 3) true
    check "op2oin209v31s" (Nullable 4 ?<=? Nullable 3) false
    check "op2oin209v33d" (Nullable () ?<=? Nullable 3) false



    check "op2oin209v30f" (Nullable 2 ?>? Nullable ()) false
    check "op2oin209v31g" (Nullable 3 ?>? Nullable ()) false
    check "op2oin209v31h" (Nullable 4 ?>? Nullable ()) false
    check "op2oin209v33j" (Nullable () ?>? Nullable ()) false

    check "op2oin209v30k" (Nullable 2 ?>=? Nullable ()) false
    check "op2oin209v31l" (Nullable 3 ?>=? Nullable ()) false
    check "op2oin209v31z" (Nullable 4 ?>=? Nullable ()) false
    check "op2oin209v33x" (Nullable () ?>=? Nullable ()) false

    check "op2oin209v30c" (Nullable 2 ?<? Nullable ()) false
    check "op2oin209v31v" (Nullable 3 ?<? Nullable ()) false
    check "op2oin209v31b" (Nullable 4 ?<? Nullable ()) false
    check "op2oin209v33n" (Nullable () ?<? Nullable ()) false

    check "op2oin209v30m" (Nullable 2 ?<=? Nullable ()) false
    check "op2oin209v31Q" (Nullable 3 ?<=? Nullable ()) false
    check "op2oin209v31W" (Nullable 4 ?<=? Nullable ()) false
    check "op2oin209v33E" (Nullable () ?<=? Nullable ()) false



module NullableAddInt = 
    open Microsoft.FSharp.Linq.NullableOperators

    check "p2oin209v304" (2 +? Nullable 3) (Nullable 5)
    check "p2oin209v315" (3 +? Nullable 3) (Nullable 6)
    check "p2oin209v316" (4 +? Nullable 3) (Nullable 7)
    check "p2oin209v337" (3 +? Nullable()) (Nullable ())

    check "p2oin209v304" (Nullable 2 ?+ 3) (Nullable 5)
    check "p2oin209v315" (Nullable 3 ?+ 3) (Nullable 6)
    check "p2oin209v316" (Nullable 4 ?+ 3) (Nullable 7)
    check "p2oin209v337" (Nullable () ?+ 3) (Nullable ())

    check "p2oin209v30a" (Nullable 2 ?+? Nullable 3) (Nullable 5)
    check "p2oin209v31s" (Nullable 3 ?+? Nullable 3) (Nullable 6)
    check "p2oin209v31d" (Nullable 4 ?+? Nullable 3) (Nullable 7)
    check "p2oin209v33f" (Nullable () ?+? Nullable 3) (Nullable ())

    check "p2oin209v30k" (Nullable 2 ?+? Nullable ()) (Nullable ())
    check "p2oin209v31l" (Nullable 3 ?+? Nullable ()) (Nullable ())
    check "p2oin209v31z" (Nullable 4 ?+? Nullable ()) (Nullable ())
    check "p2oin209v33x" (Nullable () ?+? Nullable ()) (Nullable ())

    // Some tests to check the type inference when the left and right types are not identical
    let now = System.DateTime.Now
    check "p2oin209v304dt" (Nullable now ?+ System.TimeSpan.Zero) (Nullable now)
    check "p2oin209v304dt" (now +? Nullable System.TimeSpan.Zero) (Nullable now)
    check "p2oin209v304dt" (now +? Nullable ()) (Nullable ())
    check "p2oin209v30adt" (Nullable now ?+? Nullable System.TimeSpan.Zero) (Nullable now)
    check "p2oin209v30kdt" (Nullable now ?+? Nullable ()) (Nullable ())


module NullableAddDouble = 
    open Microsoft.FSharp.Linq.NullableOperators

    check "p2oin209v304" (2.0 +? Nullable 3.0) (Nullable 5.0)
    check "p2oin209v315" (3.0 +? Nullable 3.0) (Nullable 6.0)
    check "p2oin209v316" (4.0 +? Nullable 3.0) (Nullable 7.0)
    check "p2oin209v337" (3.0 +? Nullable()) (Nullable ())

    check "p2oin209v304" (Nullable 2.0 ?+ 3.0) (Nullable 5.0)
    check "p2oin209v315" (Nullable 3.0 ?+ 3.0) (Nullable 6.0)
    check "p2oin209v316" (Nullable 4.0 ?+ 3.0) (Nullable 7.0)
    check "p2oin209v337" (Nullable () ?+ 3.0) (Nullable ())

    check "p2oin209v30a" (Nullable 2.0 ?+? Nullable 3.0) (Nullable 5.0)
    check "p2oin209v31s" (Nullable 3.0 ?+? Nullable 3.0) (Nullable 6.0)
    check "p2oin209v31d" (Nullable 4.0 ?+? Nullable 3.0) (Nullable 7.0)
    check "p2oin209v33f" (Nullable () ?+? Nullable 3.0) (Nullable ())

    check "p2oin209v30k" (Nullable 2.0 ?+? Nullable ()) (Nullable ())
    check "p2oin209v31l" (Nullable 3.0 ?+? Nullable ()) (Nullable ())
    check "p2oin209v31z" (Nullable 4.0 ?+? Nullable ()) (Nullable ())
    check "p2oin209v33x" (Nullable () ?+? Nullable ()) (Nullable ())



module NullableMinus = 

    open Microsoft.FSharp.Linq.NullableOperators

    check "p2oin209v304" (2 -? Nullable 3) (Nullable -1)
    check "p2oin209v315" (3 -? Nullable 3) (Nullable 0)
    check "p2oin209v316" (4 -? Nullable 3) (Nullable 1)
    check "p2oin209v337" (3 -? Nullable()) (Nullable ())

    check "p2oin209v304" (Nullable 2 ?- 3) (Nullable -1)
    check "p2oin209v315" (Nullable 3 ?- 3) (Nullable 0)
    check "p2oin209v316" (Nullable 4 ?- 3) (Nullable 1)
    check "p2oin209v337" (Nullable () ?- 3) (Nullable ())

    check "p2oin209v30a" (Nullable 2 ?-? Nullable 3) (Nullable -1)
    check "p2oin209v31s" (Nullable 3 ?-? Nullable 3) (Nullable 0)
    check "p2oin209v31d" (Nullable 4 ?-? Nullable 3) (Nullable 1)
    check "p2oin209v33f" (Nullable () ?-? Nullable 3) (Nullable ())

    check "p2oin209v30k" (Nullable 2 ?-? Nullable ()) (Nullable ())
    check "p2oin209v31l" (Nullable 3 ?-? Nullable ()) (Nullable ())
    check "p2oin209v31z" (Nullable 4 ?-? Nullable ()) (Nullable ())
    check "p2oin209v33x" (Nullable () ?-? Nullable ()) (Nullable ())

    // Some tests to check the type inference when the left and right types are not identical
    let now = System.DateTime.Now
    check "p2oin209v304dt" (Nullable now ?- System.TimeSpan.Zero) (Nullable now)
    check "p2oin209v304dt" (now -? Nullable System.TimeSpan.Zero) (Nullable now)
    check "p2oin209v304dt" (now -? Nullable<System.TimeSpan> ()) (Nullable ())
    check "p2oin209v30adt" (Nullable now ?-? Nullable System.TimeSpan.Zero) (Nullable now)
    check "p2oin209v30kdt" (Nullable now ?-? Nullable<System.TimeSpan> ()) (Nullable ())

module NullableMultiply = 

    open Microsoft.FSharp.Linq.NullableOperators

    check "p2oin209v304" (2 *? Nullable 3) (Nullable 6)
    check "p2oin209v315" (3 *? Nullable 3) (Nullable 9)
    check "p2oin209v316" (4 *? Nullable 3) (Nullable 12)
    check "p2oin209v337" (3 *? Nullable()) (Nullable ())

    check "p2oin209v304" (Nullable 2 ?* 3) (Nullable 6)
    check "p2oin209v315" (Nullable 3 ?* 3) (Nullable 9)
    check "p2oin209v316" (Nullable 4 ?* 3) (Nullable 12)
    check "p2oin209v337" (Nullable () ?* 3) (Nullable ())

    check "p2oin209v30a" (Nullable 2 ?*? Nullable 3) (Nullable 6)
    check "p2oin209v31s" (Nullable 3 ?*? Nullable 3) (Nullable 9)
    check "p2oin209v31d" (Nullable 4 ?*? Nullable 3) (Nullable 12)
    check "p2oin209v33f" (Nullable () ?*? Nullable 3) (Nullable ())

    check "p2oin209v30k" (Nullable 2 ?*? Nullable ()) (Nullable ())
    check "p2oin209v31l" (Nullable 3 ?*? Nullable ()) (Nullable ())
    check "p2oin209v31z" (Nullable 4 ?*? Nullable ()) (Nullable ())
    check "p2oin209v33x" (Nullable () ?*? Nullable ()) (Nullable ())



module NullableDivide = 

    open Microsoft.FSharp.Linq.NullableOperators

    check "p2oin209v304" (2 /? Nullable 3) (Nullable 0)
    check "p2oin209v315" (3 /? Nullable 3) (Nullable 1)
    check "p2oin209v316" (4 /? Nullable 3) (Nullable 1)
    check "p2oin209v337" (3 /? Nullable()) (Nullable ())

    check "p2oin209v304" (Nullable 2 ?/ 3) (Nullable 0)
    check "p2oin209v315" (Nullable 3 ?/ 3) (Nullable 1)
    check "p2oin209v316" (Nullable 4 ?/ 3) (Nullable 1)
    check "p2oin209v337" (Nullable () ?/ 3) (Nullable ())

    check "p2oin209v30a" (Nullable 2 ?/? Nullable 3) (Nullable 0)
    check "p2oin209v31s" (Nullable 3 ?/? Nullable 3) (Nullable 1)
    check "p2oin209v31d" (Nullable 4 ?/? Nullable 3) (Nullable 1)
    check "p2oin209v33f" (Nullable () ?/? Nullable 3) (Nullable ())

    check "p2oin209v30k" (Nullable 2 ?/? Nullable ()) (Nullable ())
    check "p2oin209v31l" (Nullable 3 ?/? Nullable ()) (Nullable ())
    check "p2oin209v31z" (Nullable 4 ?/? Nullable ()) (Nullable ())
    check "p2oin209v33x" (Nullable () ?/? Nullable ()) (Nullable ())


module NullableModulo = 

    open Microsoft.FSharp.Linq.NullableOperators

    check "p2oin209v304" (2 %? Nullable 3) (Nullable 2)
    check "p2oin209v315" (3 %? Nullable 3) (Nullable 0)
    check "p2oin209v316" (4 %? Nullable 3) (Nullable 1)
    check "p2oin209v337" (3 %? Nullable()) (Nullable ())

    check "p2oin209v304" (Nullable 2 ?% 3) (Nullable 2)
    check "p2oin209v315" (Nullable 3 ?% 3) (Nullable 0)
    check "p2oin209v316" (Nullable 4 ?% 3) (Nullable 1)
    check "p2oin209v337" (Nullable () ?% 3) (Nullable ())

    check "p2oin209v30a" (Nullable 2 ?%? Nullable 3) (Nullable 2)
    check "p2oin209v31s" (Nullable 3 ?%? Nullable 3) (Nullable 0)
    check "p2oin209v31d" (Nullable 4 ?%? Nullable 3) (Nullable 1)
    check "p2oin209v33f" (Nullable () ?%? Nullable 3) (Nullable ())

    check "p2oin209v30k" (Nullable 2 ?%? Nullable ()) (Nullable ())
    check "p2oin209v31l" (Nullable 3 ?%? Nullable ()) (Nullable ())
    check "p2oin209v31z" (Nullable 4 ?%? Nullable ()) (Nullable ())
    check "p2oin209v33x" (Nullable () ?%? Nullable ()) (Nullable ())

module NullableConversions = 
    open Microsoft.FSharp.Linq
    open Microsoft.FSharp.Linq.NullableOperators
    open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

    check "opp2oin209v3041" (Nullable.byte (Nullable 2)) (Nullable 2uy)
    check "opp2oin209v3041" (Nullable.uint8 (Nullable 2)) (Nullable 2uy)
    check "opp2oin209v3042" (Nullable.sbyte (Nullable 2)) (Nullable 2y)
    check "opp2oin209v3042" (Nullable.int8 (Nullable 2)) (Nullable 2y)
    check "opp2oin209v3043" (Nullable.uint16(Nullable 2 )) (Nullable 2us)
    check "opp2oin209v3044" (Nullable.int16(Nullable 2 )) (Nullable 2s)
    check "opp2oin209v3045" (Nullable.uint32 (Nullable 2s)) (Nullable 2u)
    check "opp2oin209v3046" (Nullable.int32 (Nullable 2s)) (Nullable 2)
    check "opp2oin209v3047" (Nullable.uint64(Nullable 2 )) (Nullable 2UL)
    check "opp2oin209v3048" (Nullable.int64(Nullable 2 )) (Nullable 2L)
    check "opp2oin209v3049" (Nullable.decimal(Nullable 2 )) (Nullable 2M)
    check "opp2oin209v304q" (Nullable.char(Nullable (int '2') )) (Nullable '2')
    check "opp2oin209v304w" (Nullable.enum(Nullable 2 ): System.Nullable<System.DayOfWeek>) (Nullable System.DayOfWeek.Tuesday )

    check "opp2oin209v304e" (Nullable.sbyte (Nullable 2<kg>)) (Nullable 2y)
    check "opp2oin209v304e" (Nullable.int8  (Nullable 2<kg>)) (Nullable 2y)
    check "opp2oin209v304r" (Nullable.int16 (Nullable 2<kg>)) (Nullable 2s)
    check "opp2oin209v304t" (Nullable.int32 (Nullable 2s<kg>)) (Nullable 2)
    check "opp2oin209v304y" (Nullable.int64 (Nullable 2<kg>)) (Nullable 2L)
    check "opp2oin209v304u" (Nullable.float (Nullable 2<kg>)) (Nullable 2.0)
    check "opp2oin209v304u" (Nullable.double (Nullable 2<kg>)) (Nullable 2.0)
    check "opp2oin209v304i" (Nullable.float32 (Nullable 2<kg>)) (Nullable 2.0f)
    check "opp2oin209v304i" (Nullable.single (Nullable 2<kg>)) (Nullable 2.0f)


#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      printf "TEST PASSED OK" ;
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

