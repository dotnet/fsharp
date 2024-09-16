// #Regression #Conformance #Binding 
namespace One

#light

module One =
    // Regression test for bug 4083: Extend "inferred type is less generic than the given type annotations" warning to cover cases where a user-authored 'b is constrained to be an 'a
    

    let f2 (x : 'a) (y : 'b) = x : 'b
