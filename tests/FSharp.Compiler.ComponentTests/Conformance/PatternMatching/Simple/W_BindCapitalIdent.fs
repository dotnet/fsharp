// #Regression #Conformance #PatternMatching 
// Verify warning when capturing values with capital identifier
// FSB 3954




let test x = function 
             | Foo :: []      -> 1 
             | Bar :: _ :: [] -> 2 
             | _ -> 3
