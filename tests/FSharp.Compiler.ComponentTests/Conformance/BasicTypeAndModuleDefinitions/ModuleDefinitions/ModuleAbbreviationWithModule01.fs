// #Regression #Conformance #TypesAndModules #Modules 
// Regression test for FSHARP1.0:2644 (a module may start with an expression)
// Module abbreviation:
// Trying to abbreviate a module is ok
//<Expects status="success"></Expects>
#light

// Module abbreviations
module AbbreviateModule = 
    Microsoft.FSharp.Collections.List
    
