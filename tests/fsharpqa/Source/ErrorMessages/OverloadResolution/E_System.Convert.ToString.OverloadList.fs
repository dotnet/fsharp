module Overloads
System.Convert.ToString('a',0) |> ignore
System.Convert.ToString(provider='a', value=0) |> ignore
System.Convert.ToString(provider=null, value=0) |> ignore

//<Expects id="FS0039" status="error" span="(2,1-2,31)">Arguments given:</Expects>
//<Expects id="FS0039" status="error" span="(2,1-2,31)"> - char</Expects>
//<Expects id="FS0039" status="error" span="(2,1-2,31)"> - int</Expects>
//<Expects id="FS0039" status="error" span="(3,1-3,47)">Arguments given:</Expects>
//<Expects id="FS0039" status="error" span="(3,1-3,47)"> - (provider) : char</Expects>
//<Expects id="FS0039" status="error" span="(3,1-3,47)"> - (value) : int</Expects>
//<Expects id="FS0039" status="error" span="(4,1-4,48)"> - (provider) : obj</Expects>
//<Expects id="FS0039" status="error" span="(4,1-4,48)"> - (value) : int</Expects>
