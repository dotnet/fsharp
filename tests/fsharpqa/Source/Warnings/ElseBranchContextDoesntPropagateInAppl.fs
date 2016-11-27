// #Warnings
//<Expects status="Error" id="FS0001">This expression was expected to have</Expects>

let test = 100
let f x : string = x
let y =
    if test > 10 then "test"
    else 
        f 123
    
exit 0