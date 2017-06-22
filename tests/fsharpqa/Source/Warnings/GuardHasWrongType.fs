// #Warnings
//<Expects status="Error" id="FS0001">A pattern match guard must be of type 'bool'</Expects>

let x = 1
match x with
| 1 when "s" -> true
| _ -> false
    
exit 0