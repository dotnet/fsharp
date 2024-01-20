// #Conformance #TypesAndModules #Records 
#light


// Verify additional checks for validating that records cannot be null

type RecordType = { X : int }

// Works
let test1 = (box { X = 1} :?> RecordType)

// Will fail, the dynamic type cast will throw
let testPassed = 
    try
        let _ = (box null :?> RecordType)
        false
    with 
    :? System.NullReferenceException
        -> true

if not testPassed then failwith "Failed"