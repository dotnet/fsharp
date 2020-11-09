// #Regression #Conformance #DeclarationElements #ObjectConstructors 
// Regression test for FSHARP1.0:2939
// No warning when generic code is less generic than indicated by annotations
//<Expects id="FS0064" status="warning">This construct causes code to be less generic than indicated by the type annotations</Expects>
//<Expects id="FS0064" status="warning">This construct causes code to be less generic than indicated by the type annotations</Expects>

let f1 (x:'a,y:'b) = [x;y]

let f2 (x:'a,y:'b list) = [x;y]

exit 0

