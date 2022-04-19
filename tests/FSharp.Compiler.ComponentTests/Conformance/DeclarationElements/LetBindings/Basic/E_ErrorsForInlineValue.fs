// #Regression #Conformance #DeclarationElements #LetBindings 
#light

// Regression test for FSharp1.0:4165
// Sweep all deprecation warnings and change to errors where reasonable [WAS: Inline values should be an error, not a warning]
//<Expects id="FS0832" span="(9,12-9,15)" status="error">Only functions may be marked 'inline'</Expects>
//<Expects id="FS0832" span="(11,12-11,16)" status="error">Only functions may be marked 'inline'</Expects>

let inline inc = (+) 1;;

let inline fs73 = (+) 1 >> (=) 0;;

