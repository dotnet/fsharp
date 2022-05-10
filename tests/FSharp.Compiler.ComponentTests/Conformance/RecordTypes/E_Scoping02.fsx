// #Regression #Conformance #TypesAndModules #Records 
// Field labels have module scope
//<Expects id="FS3391" span="(9,15-9,17)" status="warning">This expression uses the implicit conversion 'System\.Decimal\.op_Implicit\(value: int\) : decimal' to convert type 'int' to type 'decimal'. See https:.*$</Expects>
type T1 = { a : decimal }

module M0 =
    type T1 = { a : int;}

let x = { a = 10 }              // error - 'expecting decimal' (which means we are not seeing M0.T1)

