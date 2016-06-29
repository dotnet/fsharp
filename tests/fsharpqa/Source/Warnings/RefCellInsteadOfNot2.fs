// #Warnings
//<Expects status="Error" id="FS0001">This expression was expected to have type</Expects>
//<Expects status="success">The '!' operator is used to dereference a ref cell. Consider using 'not expr' here.</Expects>

let x = true
let y = !x
    
exit 0