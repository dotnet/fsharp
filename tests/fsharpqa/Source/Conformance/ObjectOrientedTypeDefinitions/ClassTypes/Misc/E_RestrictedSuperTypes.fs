// #Regression #Conformance #ObjectOrientedTypes #Classes 
// Regression test for FSharp1.0:3636 - PEVerify error when deriving from ValueType, Enum, Delegate, MulticastDelegate, and Array
//<Expects id="FS0771" span="(20,16)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(21,16)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(22,16)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(23,16)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(24,16)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(26,25)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(26,25)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(27,25)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(27,25)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(28,25)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(28,25)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(29,25)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(29,25)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(30,25)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>
//<Expects id="FS0771" span="(30,25)" status="error">The types System\.ValueType, System\.Enum, System\.Delegate, System\.MulticastDelegate and System\.Array cannot be used as super types in an object expression or class</Expects>

#light
let o1 = { new System.ValueType with member x.ToString() = "" }
let o2 = { new System.Array with member x.ToString() = "" }
let o3 = { new System.Enum with member x.ToString() = "" }
let o4 = { new System.Delegate with member x.ToString() = "" }
let o5 = { new System.MulticastDelegate with member x.ToString() = "" }

type C1 = class inherit System.ValueType override x.ToString() = ""  end
type C2 = class inherit System.Array override x.ToString() = ""  end
type C3 = class inherit System.Enum override x.ToString() = ""  end
type C4 = class inherit System.Delegate override x.ToString() = ""  end
type C5 = class inherit System.MulticastDelegate override x.ToString() = ""  end

exit 1
