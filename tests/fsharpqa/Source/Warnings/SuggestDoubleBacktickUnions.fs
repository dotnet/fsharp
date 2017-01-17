// #Warnings
//<Expects status="Error" span="(13,19,13,31)" id="FS0039">The field, constructor or member 'My Case2' is not defined. Maybe you want one of the following: My Case1, Case2$</Expects>


module N =
    type MyUnion =
    | ``My Case1``
    | Case2


open N

let x = N.MyUnion.``My Case2``