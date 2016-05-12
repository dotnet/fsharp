// #Warnings
//<Expects status="Error" span="(6,18)" id="FS0001">This expression was expected to have type</Expects>
//<Expects status="Error" span="(6,9)" id="FS0764">No assignment given for field 'Age' of type 'CommaInRecCtor.Person'</Expects>

type Person = { Name : string; Age : int; City : string }
let x = { Name = "Isaac", Age = 21, City = "London" }
    
exit 0