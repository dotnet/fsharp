// #Regression #NoMT #Import 
//<Expects status="error" span="(8,13)" id="FS1092">The type 'Accessibility' is not defined</Expects>
namespace NS

type T() =
    // note: the assembly 'Accessibility' does NOT have an IVT to this assembly, so it is expected to fail.
    inherit Accessibility()
    member x.Test() = base.FamAndAssembly
