// #Conformance #TypesAndModules #Modules 
// Light syntax: begin/end can be omitted
//<Expects status="success"></Expects>
#light

module N1 =
                let f x = x + 1

module N2 =
                ()

module N3 = 
                type T = | A = 1
            

module N4 = 
                exception E of string
            

module N5 = 
                module M5 = begin
                            end
            

module N6 = 
                module M6 = Microsoft.FSharp.Collections.Array
            
           
module N7 = 
                open Microsoft.FSharp.Control
                
                
                
            
