// #Warnings
//<Expects status="Error" span="(7,9)" id="FS3214">Method or object constructor 'X' is not static</Expects>

type Class1() = 
    member this.X() = "F#"

let x = Class1.X()

exit 0