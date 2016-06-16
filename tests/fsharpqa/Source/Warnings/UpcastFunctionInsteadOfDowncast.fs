// #Warnings
//<Expects status="Error" span="(7,32)" id="FS3198">The conversion from Dictionary<obj,obj> to IDictionary<obj,obj> is a compile-time safe upcast, not a downcast. Consider using 'upcast' instead of 'downcast'.</Expects>

open System.Collections.Generic

let orig = Dictionary<obj,obj>()
let c : IDictionary<obj,obj> = downcast orig 
    
exit 0