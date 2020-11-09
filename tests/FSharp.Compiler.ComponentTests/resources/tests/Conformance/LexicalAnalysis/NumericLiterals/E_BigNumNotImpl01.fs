// #Regression #Conformance #LexicalAnalysis #Constants 
// Verify ability to write custom big num libraries
//<Expects id="FS0039" span="(11,23)" status="error">The value, constructor, namespace or type 'FromInt32' is not defined</Expects>
//<Expects id="FS0039" span="(12,23)" status="error">The value, constructor, namespace or type 'FromInt64' is not defined</Expects>
//<Expects id="FS0039" span="(13,23)" status="error">The value, constructor, namespace or type 'FromString' is not defined</Expects>

module NumericLiteralG =
    let IAmNotComplete = true
    
    
let int32Range      = 2147483647G
let int64Range      = 9223372036854775807G
let fromStringRange = 12345678901234567890G

exit 1
