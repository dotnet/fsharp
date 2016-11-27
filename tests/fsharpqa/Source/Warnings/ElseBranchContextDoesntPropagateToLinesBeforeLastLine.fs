// #Warnings
//<Expects status="Error" id="FS0001">This expression was expected to have type</Expects>

let test = 100
let list = [1..10]
let y =
    if test > 10 then "test"
    else 
        printfn "%s" 1
            
        "test"
    
exit 0