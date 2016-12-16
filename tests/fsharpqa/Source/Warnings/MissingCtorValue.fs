// #Warnings
//<Expects status="Error" id="FS0496">The member or object constructor 'Person' requires 1 argument\(s\). The required signature is 'new : x:int -> Person'.</Expects>

type Person(x:int) =
    member val Name = "" with get,set
    member val Age = x with get,set


let p =
    Person(Name = "Fred", 
           Age = 18)