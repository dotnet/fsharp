// #Regression #NoMT #Printing 
// Regression test for FSHARP1.0:3300
// Verify that Pretty-printing of measure identifiers round-trips, i.e. displays the long identified (Namespace.Module.Type)
//<Expects status="success">val it: decimal<M1\.Kg> = -2\.0M</Expects>
//<Expects status="success">val it: float32<M2\.Kg> = 2\.0f</Expects>
//<Expects status="success">val it: float<M1\.Kg> = 1\.2</Expects>
#light

module M1 =
  [<Measure>] type Kg

module M2 =
  [<Measure>] type Kg

;;

-2.0M<M1.Kg>;;
2.0f<M2.Kg>;;
1.2<M1.Kg>;;

exit 0;;





