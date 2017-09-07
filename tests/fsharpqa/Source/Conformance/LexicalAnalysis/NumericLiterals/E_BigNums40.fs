// #Regression #Conformance #LexicalAnalysis #Constants #NoMono  #ReqNOMT 
// Verify errors when trying to use non bigint big numbers

//No deprecation on NetFx4.0/Dev10 (see FSHARP1.0:4599)
//<Expects id="FS0784" status="error">This numeric literal requires that a module 'NumericLiteralN' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope</Expects>
//<Expects id="FS0784" status="error">This numeric literal requires that a module 'NumericLiteralZ' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope</Expects>
//<Expects id="FS0784" status="error">This numeric literal requires that a module 'NumericLiteralQ' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope</Expects>
//<Expects id="FS0784" status="error">This numeric literal requires that a module 'NumericLiteralR' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope</Expects>
//<Expects id="FS0784" status="error">This numeric literal requires that a module 'NumericLiteralG' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope</Expects>
let a = 1234567890N
let b = 1234567890Z
let c = 1234567890Q
let d = 1234567890R
let e = 1234567890G

exit 1
