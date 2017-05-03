// #Warnings
//<Expects status="Error" id="FS0039">The value, namespace, type or module 'Path' is not defined.</Expects>
//<Expects>Maybe you want one of the following:\s+Math</Expects>

let _ = Path.GetFullPath "images"
    
exit 0