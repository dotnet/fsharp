// #Regression #Conformance #DeclarationElements #ObjectConstructors 
#light

// Verify explicit constructors work for class types
// FSB 1.0 2927, grammar collision for accessibility modifiers on representation and members

type MyClass1 =    
    new(x)     = let v : int = x in {}

type MyClass2 =    
    internal new(x)     = let v : int = x in {}

exit 0
