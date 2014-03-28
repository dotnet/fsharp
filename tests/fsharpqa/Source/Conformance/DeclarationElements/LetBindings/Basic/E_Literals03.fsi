// #Regression #Conformance #DeclarationElements #LetBindings 
//<Expects status="error" span="(6,5)" id="FS0034"></Expects>
//<Expects status="error" span="(9,5)" id="FS0034"></Expects>

// verify error when literal value specified in the signature file does not match actual literal value

module M
[<Literal>]
val test1 : string = "xy" ;;

[<Literal>]
val test2 : int = 12
