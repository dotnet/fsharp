// #Regression #CodeGen #Optimizations #Assemblies 
// Reference type with method accessing private data for regressions for Dev10:850602

module A

type VectorFloat(x:float,y:float,z:float) = 
    member a.X = x

[<NoComparison>]
type OctantBoundary = 
 { Min : VectorFloat; 
   Max : VectorFloat }
  
let inline isContaining (b:OctantBoundary) = 
   b.Min.X 
