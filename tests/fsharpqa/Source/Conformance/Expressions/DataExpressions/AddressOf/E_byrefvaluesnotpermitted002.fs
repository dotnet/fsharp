// #Regression #Conformance #DataExpressions 
// AddressOf Operator (warnings)
// Verify we can the compiler always issue warnings about using & and && operators
//<Expects status="warning" span="(9,11-9,14)" id="FS0051">The use of native pointers may result in unverifiable \.NET IL code$</Expects>
//<Expects status="error" span="(9,13-9,14)" id="FS0421">The address of the variable 'x' cannot be used at this point$</Expects>

module M =
  let mutable x = 10
  let w = &&x

