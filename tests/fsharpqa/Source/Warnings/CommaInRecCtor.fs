// #Warnings
//<Expects status="Error" span="(10,18)" id="FS0001">This expression was expected to have type</Expects>
//<Expects status="success">    'string'</Expects>
//<Expects status="success">but here has type</Expects>
//<Expects status="success">    ''a \* 'b \* 'c'</Expects>
//<Expects status="success">A ';' is used to separate field values in records. Consider replacing ',' with ';'.</Expects>


type Person = { Name : string; Age : int; City : string }
let x = { Name = "Isaac", Age = 21, City = "London" }
    
exit 0