// #Regression #Conformance #TypesAndModules #Unions 
// DU may include members
// Verify that duplicate methods are not allowed


#light
type T = | C of int * int
         | D of (int * int)
         member x.IsC(a) = match a with
                           | C(_,_) -> true
                           | D(_) -> false
                           
         static member IsC(b) = match b with
                                | C(_,_) -> true
                                | D(_) -> false
