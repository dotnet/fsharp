// #Warnings
//<Expects status="Warning" span="(12,5)" id="FS0020">If you intended to set a value to a property, then use the '<-' operator e.g. 'x.Property2 <- expression'</Expects>

type MyClass(property1 : int) =
    member val Property1 = property1
    member val Property2 = "" with get, set
    
let x = MyClass(1)
let y = "hello"

let changeProperty() =
    x.Property2 = "20"
    y = "test"
    
exit 0