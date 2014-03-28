// #Regression #Conformance #TypeConstraints 
#light

// Odd type: System.Void
let x1 : System.Void = null

// struct
let x2 : System.Guid = null
let x4 : System.DateTime = null

// native types
let x3 : int32 = null

//<Expects id="FS0043" span="(5,24-5,28)" status="error">The type 'System.Void' does not have 'null' as a proper value</Expects>
//<Expects id="FS0043" span="(8,24-8,28)" status="error">The type 'System.Guid' does not have 'null' as a proper value</Expects>
//<Expects id="FS0043" span="(9,28-9,32)" status="error">The type 'System.DateTime' does not have 'null' as a proper value</Expects>
//<Expects id="FS0043" span="(12,18-12,22)" status="error">The type 'int32' does not have 'null' as a proper value</Expects>
