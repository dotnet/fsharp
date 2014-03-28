// #CodeGen #Optimizations #Assemblies 
namespace N

module L1 =
    type T() =
        let rec f n = if n>0 then string n + "," + f (n-1) else "stop"
        member this.Method(n) = f n
