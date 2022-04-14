// #Regression #Conformance #BasicGrammarElements #Operators 
//<Expects id="FS0035" span="(7,6-7,23)" status="error">This construct is deprecated: '\$' is not permitted as a character in operator names and is reserved for future use</Expects>
//<Expects id="FS0035" span="(8,5-8,22)" status="error">This construct is deprecated: '\$' is not permitted as a character in operator names and is reserved for future use</Expects>

module TestModule
// Const
let (!$%&*+-./<=>?@^|~) = "jibberish"
if (!$%&*+-./<=>?@^|~) <> "jibberish" then ()

failwith "Failed: : 1"
