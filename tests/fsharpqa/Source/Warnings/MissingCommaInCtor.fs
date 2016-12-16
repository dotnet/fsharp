// #Warnings
//<Expects status="Error" id="FS0501">The object constructor 'Person' takes 0 argument\(s\) but is here given 1. The required signature is 'new : unit -> Person'. If some of the arguments are meant to assign values to properties, consider separating those arguments with a comma \(','\).</Expects>

type Person() =
    member val Name = "" with get,set
    member val Age = 0 with get,set


let p =
    Person(Name = "Fred"
           Age = 18)