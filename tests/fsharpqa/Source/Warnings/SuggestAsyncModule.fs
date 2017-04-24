// #Warnings
//<Expects status="Error" id="FS0039"> The value, namespace, type or module 'Asnc' is not defined.</Expects>
//<Expects>Maybe you want one of the following:\s+Async,?\s+async,?\s+asin</Expects>

let f =
    Asnc.Sleep 1000


exit 0