// #Regression #Conformance #ObjectOrientedTypes #Enums 
// Verify that you cannot mix underlying types

//<Expects id="FS0001" status="error" span="(8,11-8,13)">This expression was expected to have type.    'int'    .but here has type.    'int64'</Expects>

type EnumType = 
    | D = 3
    | A = 0L
    | B = 1UL
    | C = 2u
