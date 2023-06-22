// #Regression #Conformance #TypesAndModules #Records 
#light

// Verify records cannot have null as a proper value
//<Expects id="FS0043" status="error">The type 'RecordType' does not have 'null' as a proper value</Expects>

type RecordType = { X : int }

let x : RecordType = null
