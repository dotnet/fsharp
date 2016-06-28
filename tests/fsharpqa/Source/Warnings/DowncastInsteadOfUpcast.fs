// #Warnings
//<Expects status="Error" span="(7,9)" id="FS0193">Type constraint mismatch. The type</Expects>

open System.Collections.Generic

let orig = Dictionary<obj,obj>() :> IDictionary<obj,obj>
let c = orig :> Dictionary<obj,obj>
    
exit 0