// #Warnings
//<Expects status="Warning" span="(10,5)" id="FS0020">If you intended to set a value to a property, then use the '<-' operator e.g. 'x <- expression'</Expects>

open System
    
let x = System.Timers.Timer()
let y = "hello"

let changeProperty() =
    x.Enabled = true
    y = "test"
    
exit 0