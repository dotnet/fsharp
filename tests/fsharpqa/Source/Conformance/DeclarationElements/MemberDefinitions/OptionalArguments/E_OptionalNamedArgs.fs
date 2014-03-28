// #435263: compiler crash with .net optional parameters and F# optional syntax
//<Expects id="FS0193" status="error">Type constraint mismatch\. The type </Expects>
//<Expects>    'a option    </Expects>
//<Expects>is not compatible with type</Expects>
//<Expects>    string    </Expects>
//<Expects>The type ''a option' is not compatible with the type 'string'</Expects>

let _ = (TestLib.T()).NonNullOptArg(?x = "text")