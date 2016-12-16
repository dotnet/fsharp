// #Warnings
//<Expects status="Error" id="FS0039">The namespace or module 'Collectons' is not defined</Expects>
//<Expects>Maybe you want one of the following:</Expects>
//<Expects>Collections</Expects>

module Collections =

    let f () = printfn "%s" "Hello"

open Collectons

exit 0