// #Regression #Conformance #PatternMatching 
#light



let isIntArray (o: obj) =
     match o with
     | :? int[] -> true
     | _ -> false

exit 1
