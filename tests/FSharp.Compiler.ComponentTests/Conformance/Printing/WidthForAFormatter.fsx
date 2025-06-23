// #Regression #NoMT #Printing 
#light

// Regression test for FSHARP1.0:4139 - %A formatter does not accept width, e.g. printf "%10000A"

let test = 
    sprintf "%1A" [1..5] = "[1;\n 2;\n 3;\n 4;\n 5]" &&
    sprintf "%13A" [|1..5|] = "[|1; 2; 3; 4;\n  5|]" &&
    sprintf "%3.1A" {1..3} = "seq\n  [1;\n   ...]"

match test with
| false -> raise (new System.Exception("LazyValues03 failed - this should never be forced"))
| _     -> ()
