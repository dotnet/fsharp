module DelegateUnitReturn

open System

// Target returns unit, compiled to void; delegate (Action) returns void.
[<NoCompilerInlining>]
let returnsUnit (x: int) (y: int) : unit = ()

// 28. non-eta unit-returning member (compiled to void)
let voidNonEta () = Action<int, int>(returnsUnit)

// 11. eta unit-returning member
let voidEta () = Action<int, int>(fun a b -> returnsUnit a b)

type C =
    // Generic method returning its own type variable; instantiated to unit below. The compiled method
    // returns the type variable (System.Unit once instantiated), not void - a distinct case from the
    // void-returning member above.
    [<NoCompilerInlining>]
    static member Echo<'T>(x: 'T) : 'T = x

// Generic return type variable instantiated to unit; the delegate likewise returns unit.
// 29. non-eta generic return tyvar instantiated to unit (compiled return is Unit, not void)
let unitGenericReturnNonEta () = Func<unit, unit>(C.Echo<unit>)

// 12. eta generic unit-returning method
let unitGenericReturnEta () = Func<unit, unit>(fun (x: unit) -> C.Echo<unit> x)
