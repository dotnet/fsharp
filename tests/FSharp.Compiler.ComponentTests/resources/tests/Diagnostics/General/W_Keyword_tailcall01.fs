// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2204
//<Expects>\(13,11-13,19\).+warning FS0046: The keyword 'tailcall' is reserved for future use by F#</Expects>
//<Expects>\(14,12-14,20\).+warning FS0046: The keyword 'tailcall' is reserved for future use by F#</Expects>
//<Expects>\(15,14-15,22\).+warning FS0046: The keyword 'tailcall' is reserved for future use by F#</Expects>
//<Expects>\(15,27-15,35\).+warning FS0046: The keyword 'tailcall' is reserved for future use by F#</Expects>
//<Expects>\(17,13-17,21\).+warning FS0046: The keyword 'tailcall' is reserved for future use by F#</Expects>
//<Expects>\(17,24-17,32\).+warning FS0046: The keyword 'tailcall' is reserved for future use by F#</Expects>
//<Expects>\(17,33-17,41\).+warning FS0046: The keyword 'tailcall' is reserved for future use by F#</Expects>
//<Expects>\(18,22-18,30\).+warning FS0046: The keyword 'tailcall' is reserved for future use by F#</Expects>

#light

namespace tailcall
    module tailcall = 
        type tailcall = | tailcall   = 1
                        | tailcall_b = 2
        let tailcall = tailcall.tailcall
        printfn "%A" tailcall
