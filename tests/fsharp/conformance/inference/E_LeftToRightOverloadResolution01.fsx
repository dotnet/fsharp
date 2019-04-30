// #Regression #TypeInference 
// Regression for FSHARP1.0:5749
// Better error message for overload resolution to help ease pain associated with mismatch of intellisense information


let array = [| "Ted"; "Katie"; |]
Array.iter (fun it -> System.Console.WriteLine(it))

Array.iter (fun it -> System.Console.WriteLine(it)) array

array |> Array.iter (fun it -> System.Console.WriteLine(it))

