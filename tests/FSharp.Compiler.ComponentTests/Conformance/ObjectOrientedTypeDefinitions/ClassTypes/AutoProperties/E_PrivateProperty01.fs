// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #Accessibility
//<Expects status="error" span="(6,23-6,37)" id="FS0410">The type 'X' is less accessible than the value, member or type 'member XX\.PublicProperty: X' it is used in.$</Expects>
type private X() = class end

type XX() =
    member val public PublicProperty = X() with get,set 

exit 1