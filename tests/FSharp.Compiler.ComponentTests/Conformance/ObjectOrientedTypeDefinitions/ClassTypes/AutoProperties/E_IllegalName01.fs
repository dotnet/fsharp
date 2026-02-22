// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
// <Expects status="error" id="FS1156" span="(4,16-4,18)">This is not a valid numeric literal. Valid numeric literals include</Expects>
type T() =  
    member val 1P = [] with get,set

exit 1
