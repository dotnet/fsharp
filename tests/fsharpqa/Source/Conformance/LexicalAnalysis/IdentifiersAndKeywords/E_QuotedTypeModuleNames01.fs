// #Regression #Conformance #LexicalAnalysis 
#light

// Regression test for FSharp1.0:4854
// Title: Unable to introduce types with certain invalid symbols, like '[' or '+'
//<Expects id="FS0883" span="(33,6-33,23)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(34,6-34,23)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(35,6-35,23)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(36,6-36,23)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(37,6-37,23)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(38,6-38,23)" status="error">Invalid namespace, module, type or union case name</Expects>


//<Expects id="FS0883" span="(41,6-41,23)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(42,6-42,23)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(44,8-44,26)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(45,8-45,26)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(46,8-46,26)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(47,8-47,26)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(48,8-48,26)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(49,8-49,26)" status="error">Invalid namespace, module, type or union case name</Expects>

//<Expects id="FS0883" span="(52,8-52,26)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(53,8-53,26)" status="error">Invalid namespace, module, type or union case name</Expects>

//<Expects id="FS0883" span="(55,6-55,23)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(56,6-56,23)" status="error">Invalid namespace, module, type or union case name</Expects>

//<Expects id="FS0883" span="(58,8-58,26)" status="error">Invalid namespace, module, type or union case name</Expects>
//<Expects id="FS0883" span="(59,8-59,26)" status="error">Invalid namespace, module, type or union case name</Expects>


type ``invalid.class`` () = class end
type ``invalid+class`` () = class end
type ``invalid$class`` () = class end
type ``invalid&class`` () = class end
type ``invalid[class`` () = class end
type ``invalid]class`` () = class end


type ``invalid*class`` () = class end
type ``invalid"class`` () = class end

module ``invalid.module`` = let a = 1
module ``invalid+module`` = let a = 1
module ``invalid$module`` = let a = 1
module ``invalid&module`` = let a = 1
module ``invalid[module`` = let a = 1
module ``invalid]module`` = let a = 1


module ``invalid*module`` = let a = 1
module ``invalid"module`` = let a = 1

type ``invalid/class`` () = class end
type ``invalid\class`` () = class end

module ``invalid/module`` = let a = 1
module ``invalid\module`` = let a = 1

exit 1
