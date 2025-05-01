// #Regression #Conformance #TypeConstraints 
// Verify the [<AllowNullLiteral>] attribute is only value on class and interface types

//<Expects id="FS0934" span="(10,6-10,15)" status="error">Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute</Expects>
//<Expects id="FS0934" span="(22,6-22,12)" status="error">Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute</Expects>
//<Expects id="FS0934" span="(25,6-25,12)" status="error">Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute</Expects>
//<Expects id="FS0934" span="(30,6-30,16)" status="error">Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute</Expects>

[<AllowNullLiteral>]
type DiscUnion = A | B

[<AllowNullLiteral>]
type ClassType() = 
    class
    end

[<AllowNullLiteral>]
type Interface =
    abstract DoStuff : unit -> unit

[<AllowNullLiteral>]
type Record = { Field1 : int; Field2 : float }

[<AllowNullLiteral>]
type Struct = 
    struct
    end

[<AllowNullLiteral>]
type TypeAbbrev = string * int 

exit 1
