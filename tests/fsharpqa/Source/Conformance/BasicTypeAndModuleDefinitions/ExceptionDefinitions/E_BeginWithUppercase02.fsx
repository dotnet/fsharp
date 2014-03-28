// #Regression #Conformance #TypesAndModules #Exceptions 
// Exception types
// Exception labels must begin with an uppercase letter
//<Expects id="FS0010" span="(8,11-8,12)" status="error">Unexpected integer literal in exception definition\. Expected identifier or other token</Expects>

#light

exception 1             // err: can't use
