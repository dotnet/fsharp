// #Warnings
//<Expects status="Error" id="FS0501">The object constructor 'Person' takes 0 argument\(s\) but is here given 1. The required signature is 'new : unit -> Person'.$</Expects>

type Person() =
    member val Name = "" with get,set
    member val Age = 0 with get,set


let p =
    Person(1)