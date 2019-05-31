// #Warnings
//<Expects status="Warning" span="(8,5)" id="FS0020">The result of this expression has type 'bool' and is implicitly ignored</Expects>

let mutable x = 0
while x < 1 do
    printfn "unneeded"
    x <- x + 1
    true
    
exit 0