// #Conformance #TypesAndModules #Unions 
// Scope - Data Constructor / Pattern Matching
// This testcase verifies that the discriminated union cases have module scope
// It also covers the fact that they can be used both as data constructors
// and to form patterns.
// This is the positive case.
//<Expects status="success"></Expects>
#light

    [<Measure>] type ı

    module M1 =

        type T1 = | CaseLabel1 of (int * char) | CaseLabel2 of char
        
        // used as a data constructor 
        let foo = CaseLabel1(10,'c')

        // in pattern matching
        let pred = match foo with
                    | CaseLabel1(x,_) -> x
                    | _ -> 1

    module M2 =
        type T2 = | CaseLabel1 of (float<ı> * float<ı>) | CaseLabel2 of float<ı>
       
                                            
        // used as a data constructor 
        let foo = CaseLabel1(10.0<ı>,10.0<ı>)    // no confusion with label1 in M1.T1

        // in pattern matching
        let pred = match foo with
                    | CaseLabel1(x,_) -> x
                    | _ -> 11.0<ı>
