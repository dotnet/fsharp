// #Regression #Conformance #DeclarationElements #Modules 

// Verify error if trying to use a module abbreviation inside of a function
//<Expects status="error" span="(7,5-7,11)" id="FS0010">Incomplete structured construct at or before this point in binding$</Expects>

let someFunc x y =
    module ListMod = Microsoft.FSharp.Collections.List
    
    ListMod.sum [0; x; y]

