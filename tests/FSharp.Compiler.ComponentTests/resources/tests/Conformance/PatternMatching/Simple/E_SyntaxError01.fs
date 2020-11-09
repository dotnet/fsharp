// #Regression #Conformance #PatternMatching 
#light

//<Expects id="FS0010" status="error" span="(8,14)">Unexpected symbol'\[' in pattern matching. Expected '->' or other token</Expects>

let isIntArray (o: obj) =
     match o with
     | :? int[] -> true
     | _ -> false

exit 1
