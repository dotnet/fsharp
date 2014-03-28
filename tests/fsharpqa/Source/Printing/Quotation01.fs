// #NoMT #Printing 
// Regression test for FSHARP1.0:524
//<Expects status=success>val it : Quotations.Expr<int> = Value \(1\) {CustomAttributes = \[||\];</Expects>
//<Expects status=success>                                           Raw = \.\.\.;</Expects>
//<Expects status=success>                                           Type = System\.Int32;}</Expects>
<@ 1 @>;;
exit 0;;
