// #Regression #NoMT #Import 
//<Expects status="error" span="(8,28)" id="FS0039">The field, constructor or member 'FamAndAssembly' is not defined.</Expects>
namespace NS

type T() =
    // note: the assembly 'Accessibility' does NOT have an IVT to this assembly, so it is expected to fail.
    inherit Accessibility()
    member x.Test() = base.FamAndAssembly
