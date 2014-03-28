// #Regression #Conformance #PatternMatching #ActivePatterns 
// Verify error if Active Patterns do not start with an upper case letter
//<Expects id="FS0623" status="error" span="(6,7)">Active pattern case identifiers must begin with an uppercase letter</Expects>
//<Expects id="FS0623" status="error" span="(6,16)">Active pattern case identifiers must begin with an uppercase letter</Expects>

let (|positive|negative|) n = if n < 0 then positive else negative

exit 1
