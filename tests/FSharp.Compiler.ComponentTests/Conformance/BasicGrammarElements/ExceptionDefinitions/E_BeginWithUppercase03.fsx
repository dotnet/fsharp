// #Regression #Conformance #TypesAndModules #Exceptions 
// Exception types
// Exception labels must begin with an uppercase letter
//<Expects id="FS0010" span="(9,11-9,14)" status="error">Unexpected string literal in exception definition\. Expected identifier or other token</Expects>

#light

exception (* *) A       // ok
exception "A"           // err
