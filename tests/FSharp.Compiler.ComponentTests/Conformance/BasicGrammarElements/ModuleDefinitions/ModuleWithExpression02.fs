// #Regression #Conformance #TypesAndModules #Modules 
// Regression test for FSHARP1.0:2644 (a module may start with an expression)
// This is unfortunate, but this is the current behavior for this release.
// So, the workaround is to use begin... end
//<Expects status="success"></Expects>
#light

module M2 = begin
               System.DateTime.Now
            end
