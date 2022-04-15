// #FSharpQA #Conformance #TypeInference #ByRef  
//<Expects status="error" id="FS0424" span="(7,6-7,13)">The address of an array element cannot be used at this point</Expects>
let mutable xs = [|1|]

let applyWithOne f = 

   f &xs.[0]

applyWithOne (fun x -> x <- 2)

applyWithOne (fun x -> printf "%i" x)