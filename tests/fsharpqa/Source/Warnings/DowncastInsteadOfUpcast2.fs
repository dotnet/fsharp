// #Warnings
//<Expects status="Error" span="(6,33)" id="FS0001">This expression was expected to have type</Expects>

open System.Collections.Generic

let orig : IDictionary<'a,'b> = Dictionary<'a,'b>()
    
exit 0