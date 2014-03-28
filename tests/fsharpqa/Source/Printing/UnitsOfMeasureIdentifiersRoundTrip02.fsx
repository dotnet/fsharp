// #Regression #NoMT #Printing 
// Regression test for FSHARP1.0:3300
// Verify that Pretty-printing of measure identifiers round-trips, i.e. displays the long identified (Namespace.Module.Type)
//<Expects status=success>val it : decimal<A\.B\.C\.M1\.Kg> = -2\.0M</Expects>
//<Expects status=success>val it : float32<A\.B\.C\.M2\.Kg> = 2\.0f</Expects>
//<Expects status=success>val it : float<A\.B\.C\.M1\.Kg> = 1\.2</Expects>
#light

#r "UnitsOfMeasureIdentifiersRoundTrip02.dll"

-2.0M<A.B.C.M1.Kg>;;
2.0f<A.B.C.M2.Kg>;;
1.2<A.B.C.M1.Kg>;;

exit 0;;



