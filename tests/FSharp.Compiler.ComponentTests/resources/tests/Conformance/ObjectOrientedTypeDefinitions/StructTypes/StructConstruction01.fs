// #Regression #Conformance #ObjectOrientedTypes #Structs 
#light

// FS1 1337: "new" is still required for the default struct constructor

type S =
    struct
        val x : int
    end

// Works
let x : S = new S()

// Should also work, using default bit pattern
let y : S = S()


exit 0
