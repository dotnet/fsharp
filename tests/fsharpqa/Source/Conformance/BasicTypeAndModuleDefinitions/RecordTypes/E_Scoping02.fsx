// #Regression #Conformance #TypesAndModules #Records 
// Field labels have module scope
//<Expects id="FS0001" span="(9,15-9,17)" status="error">This expression was expected to have type.    'decimal'    .but here has type.    'int'</Expects>
type T1 = { a : decimal }

module M0 =
    type T1 = { a : int;}

let x = { a = 10 }              // error - 'expecting decimal' (which means we are not seeing M0.T1)

