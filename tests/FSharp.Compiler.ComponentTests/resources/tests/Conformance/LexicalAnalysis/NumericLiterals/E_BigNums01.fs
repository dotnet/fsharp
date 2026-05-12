// #Regression #Conformance #LexicalAnalysis #Constants  
// Verify errors when trying to use non bigint big numbers

//<Expects id="FS0784" status="error" span="(10,9)">This numeric literal requires that a module 'NumericLiteralN'</Expects>
//<Expects id="FS0784" status="error" span="(11,9)">This numeric literal requires that a module 'NumericLiteralZ'</Expects>
//<Expects id="FS0784" status="error" span="(12,9)">This numeric literal requires that a module 'NumericLiteralQ</Expects>
//<Expects id="FS0784" status="error" span="(13,9)">This numeric literal requires that a module 'NumericLiteralR</Expects>
//<Expects id="FS0784" status="error" span="(14,9)">This numeric literal requires that a module 'NumericLiteralG</Expects>

let a = 1234567890N
let b = 1234567890Z
let c = 1234567890Q
let d = 1234567890R
let e = 1234567890G

