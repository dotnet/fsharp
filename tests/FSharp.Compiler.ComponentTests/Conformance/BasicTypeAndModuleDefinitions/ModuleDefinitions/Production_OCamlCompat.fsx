// #Conformance #TypesAndModules #Modules 
// Productions
// Compile with: --mlcompatibility -a --warnaserror+
//<Expects status="success"></Expects>
#light

module N1 = struct
                let f x = x + 1
            end

module N2 = struct
                ()
            end

module N3 = struct
                type T = | A = 1
            end

module N4 = struct
                exception E of string
            end

module N5 = struct
                module M5 = begin
                            end
            end

module N6 = struct 
                module M6 = Microsoft.FSharp.Collections.Array
            end
           
module N7 = struct
                open Microsoft.FSharp.Control
            end
