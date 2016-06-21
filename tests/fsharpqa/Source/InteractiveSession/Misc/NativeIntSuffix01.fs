// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:4118
// FSI: PrettyPrinting of nativeint or unativeint does not emit the suffix (n or un, respectively)
// <Expect status="success">val it : nativeint = 2n</Expect>

nativeint 2;;
#q;;

