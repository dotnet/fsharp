// #Warnings
//<Expects status="Error" span="(10,5)" id="FS0193">Type constraint mismatch. The type</Expects>

open System.Collections.Generic

let orig = Dictionary<obj,obj>()

let c = 
  match orig with
  | :? IDictionary<obj,obj> -> "yes"
  | _ -> "no"
    
exit 0