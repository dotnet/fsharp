// #Regression #NoMT #Import 

namespace NS

// note: the assembly 'Accessibility' has an IVT to this assembly

type T() =
    inherit Accessibility()
    member x.Test() = base.FamOrAssembly

module M =
    let Test() = Accessibility().FamOrAssembly
