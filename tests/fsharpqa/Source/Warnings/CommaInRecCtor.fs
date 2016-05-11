// #Warnings
//<Expects status="Error" span="(5,18)" id="FS0001">Consider replacing</Expects>

type Person = { Name : string; Age : int; City : string }
let x = { Name = "Isaac", Age = 21, City = "London" }
    
exit 0