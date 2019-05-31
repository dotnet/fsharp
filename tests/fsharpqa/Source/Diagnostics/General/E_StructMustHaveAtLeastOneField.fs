// #Regression #Diagnostics 
#light

// Somehow related to the fix for FSHARP1.0:3143: as a result, now struct may be empty
//<Expects status="success"></Expects>

type StructType = struct
                  end



