// #Conformance #TypeInference #ApplicationExpressions 
#light

// Verify you can pipe a complex expression through the F# compiler

let result = (Map.empty : Map<int, string>).Add(1,typeof<Map<_,_>>.Name).Add(2,typeof<List<Map<List<_>,Option<_>>>>.Name).Count

if result <> 2 then exit 1
exit 0
