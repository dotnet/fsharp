// #Regression #Conformance #DeclarationElements #LetBindings 
#light

// Verify warning when 'do-bindings' do not return unit.
//<Expects id="FS0020" status="warning">The result of this expression has type 'int' and is implicitly ignored</Expects>

let square x = x * x

square 32
