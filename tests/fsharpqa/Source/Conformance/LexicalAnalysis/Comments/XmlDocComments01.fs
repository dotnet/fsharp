// #Conformance #LexicalAnalysis 
#light

// Verify no syntax errors or problems when using XML doc comments

/// Not associated with anything

/// let binding
let x = 1

/// function binding
let f x = 1

/// DU
type DU = A | B

/// Record
type R = { F : DU }

/// Class
type C = 
    class
    end


exit 0
