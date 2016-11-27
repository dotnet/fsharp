// #Warnings
//<Expects status="Error" id="FS0001">This expression was expected to have</Expects>

let test = 100
let list = [1..10]
let y =
    if test > 10 then "test"
    else 
        for (x:string) in list do
            printfn "%s" x
            
        "test"
    
exit 0