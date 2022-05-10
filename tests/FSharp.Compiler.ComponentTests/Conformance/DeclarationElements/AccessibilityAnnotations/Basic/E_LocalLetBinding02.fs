// #Regression #Conformance #DeclarationElements #Accessibility 
// Let bindings in classes are always private
//<Expects id="FS0646" span="(7,13-7,16)" status="error">Multiple visibility attributes have been specified for this identifier\. 'let' bindings in classes are always private, as are any 'let' bindings inside expressions</Expects>
type internal A() = class end

type public B() =
 let public foo (x:A) = ()
 do ()
