// #Warnings
//<Expects status="Error" span="(5,1)" id="FS0027">This value is not mutable. Consider using the mutable keyword, e.g. 'let mutable x = expression'.</Expects>

let x = 10
x <- 20
    
exit 0