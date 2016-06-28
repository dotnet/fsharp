// #Warnings
//<Expects status="Error" span="(7,9)" id="FS3198">The conversion from Dictionary<obj,obj> to IDictionary<obj,obj> is a compile-time safe upcast, not a downcast.</Expects>

open System.Collections.Generic

let orig = Dictionary<obj,obj>()
let c = orig :?> IDictionary<obj,obj>
    
exit 0