// #Warnings
//<Expects status="Warning" span="(8,5)" id="FS0020">If you intended to mutate a value, then use the '<-' operator e.g. 'x <- expression'.</Expects>

let mutable x = 10
let y = "hello"

let changeX() =
    x = 20
    y = "test"
    
exit 0