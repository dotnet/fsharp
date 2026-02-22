// #Regression #Conformance #TypesAndModules #Exceptions 
// Exception types
// Exception names must not contain illegal characters



exception ``My*Exception``     // err: contains '*'
exception ``My"Exception``     // err: contains '"'