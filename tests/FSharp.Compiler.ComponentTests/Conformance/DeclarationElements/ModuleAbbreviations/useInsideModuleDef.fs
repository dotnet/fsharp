// #Conformance #DeclarationElements #Modules 
// Use module abbreviation inside a module
//<Expects status="success"></Expects>
#light

module A =
    module B =
        module C =
           type X = | Red  = 1
                    | Blue = 2 
           let DirectorySeparatorChar = System.IO.Path.DirectorySeparatorChar 
            
module TestModule1 = 
    
    let f x y = x + y
    
    module ABC = A.B.C
    
    let dsc = ABC.DirectorySeparatorChar
    printfn "Current directory seperator char is %c" dsc

