// #Regression #Conformance #DeclarationElements #Accessibility 
// Let bindings in classes are always private

type internal A() = class end

type public B() =
 let public foo (x:A) = ()
 do ()
