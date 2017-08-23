// #Warnings
//<Expects status="Error" span="(14,19,14,31)" id="FS0039">The field, constructor or member 'My Case2' is not defined.</Expects>
//<Expects>Maybe you want one of the following:\s+My Case1,?\s+Case2</Expects>


module N =
    type MyUnion =
    | ``My Case1``
    | Case2


open N

let x = N.MyUnion.``My Case2``