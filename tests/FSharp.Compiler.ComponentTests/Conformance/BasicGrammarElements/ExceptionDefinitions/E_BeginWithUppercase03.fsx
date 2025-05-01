// #Regression #Conformance #TypesAndModules #Exceptions 
// Exception types
// Exception labels must begin with an uppercase letter


#light

exception (* *) A       // ok
exception "A"           // err
