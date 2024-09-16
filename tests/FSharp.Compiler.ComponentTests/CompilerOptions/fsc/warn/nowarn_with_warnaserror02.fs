// #Regression #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:4867
// nowarn has no effect if "Warning level = 4" and "Warnings as errors"

#nowarn "21"

let rec f = new System.EventHandler(fun _ _ -> f.Invoke(null,null))
