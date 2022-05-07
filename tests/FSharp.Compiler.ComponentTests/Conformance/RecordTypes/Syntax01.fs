// #Conformance #TypesAndModules #Records 
#light

// Verify sytanx associated with defining and creating records

// Same line
type Rec1 = { A1 : int; B1 : string }

// Newline delimited
and Rec2 = {
    A2 : int
    B2 : string
}

// Mixed
and Rec3 = { A3 : int;
             B3 : string }
    
 

let test1 = { A1 = 1; B1 = "1" }

let test2 = {
    A2 = 2
    B2 = "2"
}

let test3 = { A3 = 3;
              B3 = "3" }

