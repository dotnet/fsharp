// #Regression #Conformance #DataExpressions #ObjectConstructors 
// Regression test for FSHARP1.0:5141
// Title: spurious warning with slot inference?
// Desc: Not expecting to get a 'less generic than indicated by the annotation' message at the point pointed by marked

#light

type IParameterUser<'t> =
  abstract Use<'b> : list<'b> -> 't
 
let getName5 () = { new IParameterUser<'t> with member x.Use<'b> (p:list<'b> (*HERE*)) : 't = failwith "" }

exit 0
