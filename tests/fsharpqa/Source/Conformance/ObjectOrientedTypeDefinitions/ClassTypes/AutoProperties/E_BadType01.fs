// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
// <Expects status="error" id="FS0039" span="(4,20-4,21)">The value or constructor 'a' is not defined</Expects>
type T() =  
    member val P = a with get,set

exit 1
