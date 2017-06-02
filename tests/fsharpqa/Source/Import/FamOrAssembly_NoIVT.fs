// #Regression #NoMT #Import 
//<Expects status="error" span="(12,18)" id="FS0491">The member or object constructor 'FamOrAssembly' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.</Expects>
namespace NS

// with FamOrAssembly, this should succeed, even though there is no IVT
type T() =
    inherit Accessibility()
    member x.Test() = base.FamOrAssembly

module M =
    // note: the assembly 'Accessibility' does NOT have an IVT to this assembly, so it is expected to fail.
    let Test() = Accessibility().FamOrAssembly
