// #Warnings
//<Expects status="Error" id="FS0039"></Expects>
//<Expects>Maybe you want one of the following:</Expects>
//<Expects>Name</Expects>

type Person = { Name : string; }

let x = { Person.Names = "Isaac" }
    
exit 0