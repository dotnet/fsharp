// #Warnings
//<Expects status="Error" id="FS0039">The type 'Lst' is not defined in 'System.Collections.Generic'</Expects>
//<Expects>Maybe you want one of the following:</Expects>
//<Expects>List</Expects>

let x : System.Collections.Generic.Lst = ResizeArray()
    
exit 0