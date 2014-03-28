// #Regression #Conformance #DeclarationElements #LetBindings 
#light

// Verify warning when 'do-bindings' do not return unit.
//<Expects id="FS0020" status="warning">This expression should have type 'unit', but has type 'int'</Expects>

let square x = x * x

square 32

exit 0
