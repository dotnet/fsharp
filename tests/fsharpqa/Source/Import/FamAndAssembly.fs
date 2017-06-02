// #Regression #NoMT #Import 

namespace NS

type T() =
    // note: the assembly 'Accessibility' has an IVT to this assembly
    inherit Accessibility()
    member x.Test() = base.FamAndAssembly
