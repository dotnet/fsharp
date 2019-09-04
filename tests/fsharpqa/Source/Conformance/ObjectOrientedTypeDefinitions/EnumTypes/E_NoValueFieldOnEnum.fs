// #Regression #Conformance #ObjectOrientedTypes #Enums 
// FS1 992: ilreflect error triggered with Enum value__ calls.
//<Expects id="FS0039" status="error">The type 'EnumType' does not define the field, constructor or member 'value__'</Expects>

type EnumType = 
    | A = 1
    | B = 2
    | C = 3

do  EnumType.A.value__

exit 1
