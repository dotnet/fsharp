// #Regression #Conformance #ObjectOrientedTypes #Classes 
// Verify you don't run into value restriction for values in class
// constructors (both implicit and explicit constructor syntax)
// FSB 4584

type Explicit<'a> =
    val m_x : obj
    public new x = { m_x = x }

type Implicit<'a>(x) =
    let m_x : obj = x

// Both should compile just fine

exit 0
