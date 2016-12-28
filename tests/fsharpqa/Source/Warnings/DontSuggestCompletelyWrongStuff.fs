// #Warnings
//<Expects status="Error" id="FS0039">The namespace or module 'Path' is not defined. Maybe you want one of the following: Math$</Expects>

let _ = Path.GetFullPath "images"
    
exit 0