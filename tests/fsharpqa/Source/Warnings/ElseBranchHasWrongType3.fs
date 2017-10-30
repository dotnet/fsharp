// #Warnings
//<Expects status="Error" id="FS0001">All branches of an 'if' expression must have the same type. This expression was expected to have type 'string', but here has type 'int'.</Expects>

let f x = x + 4

let y = 
    if true then
        ""
    else
        "" |> ignore
        (f 5)
    
exit 0
