// #Regression #Conformance #PatternMatching 




let isIntArray (o: obj) =
     match o with
     | :? int[] -> true
     | _ -> false

exit 1
