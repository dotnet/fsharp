// #Regression #Conformance #TypesAndModules #Records 
#light

// Verify records cannot have null as a proper value


type RecordType = { X : int }

let x : RecordType = null
