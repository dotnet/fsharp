// #Warnings
//<Expects status="Error" id="FS0001">All branches of an 'if' expression must return values of the same type as the first branch, which here is 'string'. This branch returns a value of type 'int'.</Expects>

let f x = x + 4

let y = 
    if true then
        ""
    else
        "" |> ignore
        (f 5)
    
exit 0
