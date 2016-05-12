// #Warnings
//<Expects status="Error" span="(5,18)" id="FS0001">This expression was expected to have type.    'string'    .but here has type.    ''a * 'b * 'c'    A ';' is used to separate field values in records. Consider replacing ',' with ';'.</Expects>

type Person = { Name : string; Age : int; City : string }
let x = { Name = "Isaac", Age = 21, City = "London" }
    
exit 0