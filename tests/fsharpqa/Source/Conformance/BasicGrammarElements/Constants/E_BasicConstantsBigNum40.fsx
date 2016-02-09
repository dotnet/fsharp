// #Regression #Conformance #BasicGrammarElements #Constants #NoMono #ReqNOMT #NETFX40Only 

// Verify the ability to specify basic constants - continued


// error FS0191: This numeric literal requires that a module 'NumericLiteralN' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope
//<Expects id="FS0784" span="(9,19-9,21)" status="error">This numeric literal requires that a module 'NumericLiteralN' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope</Expects>

let bignumConst = 1N

exit 1
