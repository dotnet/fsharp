// #Regression #Conformance #Binding 
namespace One

#light

module One =
    // Regression test for bug 4083: Extend "inferred type is less generic than the given type annotations" warning to cover cases where a user-authored 'b is constrained to be an 'a
    //<Expects id="FS0064" span="(7,28-7,29)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type ''a'</Expects>

    let f2 (x : 'a) (y : 'b) = x : 'b
