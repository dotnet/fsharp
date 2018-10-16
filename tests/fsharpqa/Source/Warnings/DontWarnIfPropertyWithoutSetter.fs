// #Warnings
//<Expects status="Warning" span="(11,5)" id="FS0020">The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'.</Expects>

type MyClass(property1 : int) =
    member val Property2 = "" with get
    
let x = MyClass(1)
let y = "hello"

let changeProperty() =
    x.Property2 = "22"
    y = "test"
    
exit 0