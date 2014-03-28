// #Conformance #LexFilter 
// Verify correct lexing of a complex type application

let typeApp = typeof<Map<Map<Map<Map<_,_>[],Map<_,_[]>>,_>,_>>.FullName
let result  = typeApp.StartsWith("Microsoft.FSharp.Collections.FSharpMap`2[[Microsoft.FSharp.Collections.FSharpMap`2[[Microsoft.FSharp.Collections.FSharpMap`2[[Microsoft.FSharp.Collections.FSharpMap`2[[System.IComparable")

if not result then exit 1

exit 0
