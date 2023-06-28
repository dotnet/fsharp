// #Regression #Conformance #TypesAndModules #Unions 
// DU may include members
// Verify that duplicate methods are not allowed
//<Expects id="FS0023" span="(9,19-9,22)" status="error">The member 'IsC' can not be defined because the name 'IsC' clashes with the default augmentation of the union case 'C' in this type or module</Expects>
//<Expects id="FS0023" span="(13,24-13,27)" status="error">The member 'IsC' can not be defined because the name 'IsC' clashes with the default augmentation of the union case 'C' in this type or module</Expects>
#light
type T = | C of int * int
         | D of (int * int)
         member x.IsC(a) = match a with
                           | C(_,_) -> true
                           | D(_) -> false
                           
         static member IsC(b) = match b with
                                | C(_,_) -> true
                                | D(_) -> false
