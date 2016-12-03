// #Warnings
//<Expects status="Warning" span="(10,5)" id="FS0020">If you intended to set a value to a property, then use the '<-' operator e.g. 'z.Enabled <- expression'</Expects>

open System
    
let z = System.Timers.Timer()
let y = "hello"

let changeProperty() =
    z.Enabled = true
    y = "test"
    
exit 0