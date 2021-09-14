// #Regression #Conformance #UnitsOfMeasure #ObjectOrientedTypes #ReqNOMT 

//<Expects status="success">interface Unit<length></Expects>
//<Expects id="FS0366" status="error" span="(12,13-12,25)">No implementation was given for 'abstract Unit\.Factor: unit -> float'\. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e\.g\. 'interface \.\.\. with member \.\.\.'</Expects>

[<Measure>] type length

type Unit< [<Measure>] 'a > =
  abstract Factor : unit -> float

type Meter() =
  interface Unit<length>
    member this.Factor() = 1.0

type Value< [<Measure>] 'a > = float<'a> * Unit<'a>

#if INTERACTIVE
;;
exit 1;;
#endif

