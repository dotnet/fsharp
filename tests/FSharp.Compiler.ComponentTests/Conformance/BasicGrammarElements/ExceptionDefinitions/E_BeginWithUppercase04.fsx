// #Regression #Conformance #TypesAndModules #Exceptions 
// Exception types
// Exception labels must begin with an uppercase letter
//<Expects id="FS0010" span="(7,11-7,12)" status="error">Unexpected reserved keyword in exception definition\. Expected identifier or other token</Expects>
#light

exception ``null
