// #Regression #Conformance #TypesAndModules #Modules 
#light
// Regression test for: FSharp1.0:2030 - We should honor System.Obsolete on modules




[<System.Obsolete("Don't use this module.", true)>]
module ObsoleteModule = 
    module NestedModule =
        let Level = 1
    let Level = 0
    
module Module = 
    [<System.Obsolete("Don't use this nested module.", false)>]
    module NestedObsoleteModule = 
        let Level = 1
    let Level = 0
    
module Program = 
    open ObsoleteModule
    let mutable level = ObsoleteModule.Level
    level <- ObsoleteModule.NestedModule.Level
    
    open Module
    level <- Module.NestedObsoleteModule.Level
    level <- Module.Level

exit 1

