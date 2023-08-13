// #Regression #Conformance #PatternMatching 
// Regression test for DevDiv:198999 ("Warning messages for incomplete matches involving enum types are wrong")
// Not really regression tests for the bug, but these 2 cases cover code that was touched
//<Expects status="success"></Expects>

module M

// Unterminated try-with
let h1 =
    try
        ()
        raise (new System.NotImplementedException())
    with
    | :? System.NotFiniteNumberException -> ()

// Unterminated try-with in a computation expr
let a = 
    async {
        try
            ()
            raise (new System.NotImplementedException())
        with
        | :? System.NotFiniteNumberException -> ()
    }

