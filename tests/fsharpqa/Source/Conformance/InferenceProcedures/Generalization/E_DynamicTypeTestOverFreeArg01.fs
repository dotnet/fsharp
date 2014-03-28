// #Regression #Conformance #TypeInference 


//<Expects id="FS0064" span="(8,14-8,15)" status="warning">This construct causes code to be less generic than indicated by its type annotations\. The type variable implied by the use of a '#', '_' or other type annotation at or near '.+\.fs\(8,13\)-\(8,14\)' has been constrained to be type 'obj'</Expects>

let f () = 
   match box 1 with 
   | :? list<_> -> 3  // this line should give a warning
   | _ -> 4

exit 0
