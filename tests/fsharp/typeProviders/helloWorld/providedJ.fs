namespace global

/// The type provided when GenerativeProviderWithStaticParameter is instantiated at "J"
type TheGeneratedTypeJ() = 
    member x.Item1 = 1


// some code with an intra-assembly reference to a generated type
module IntraAssemblyCode = 
   let f (x:TheGeneratedTypeJ) = x

