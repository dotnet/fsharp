// #Warnings
//<Expects status="Error" id="FS0039">The value, namespace, type or module 'Path' is not defined. Maybe you want one of the following: Math$</Expects>

let _ = Path.GetFullPath "images"
    
exit 0