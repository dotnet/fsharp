// #Regression #Conformance #ObjectOrientedTypes #Enums 
// FS1 992: ilreflect error triggered with Enum value__ calls.
//<Expects id="FS0039" status="error">The field, constructor or member 'value__' is not defined</Expects>

type EnumType = 
    | A = 1
    | B = 2
    | C = 3

do  EnumType.A.value__

exit 1
