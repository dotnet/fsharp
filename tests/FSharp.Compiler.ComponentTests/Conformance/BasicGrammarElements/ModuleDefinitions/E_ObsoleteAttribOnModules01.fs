// #Regression #Conformance #TypesAndModules #Modules 
#light
// Regression test for: FSharp1.0:2030 - We should honor System.Obsolete on modules
//<Expects id="FS0101" span="(21,10-21,24)" status="error">This construct is deprecated\. Don't use this module\.</Expects>
//<Expects id="FS0101" span="(22,25-22,39)" status="error">This construct is deprecated\. Don't use this module\.</Expects>
//<Expects id="FS0101" span="(23,14-23,28)" status="error">This construct is deprecated\. Don't use this module\.</Expects>
//<Expects id="FS0044" span="(26,21-26,41)" status="warning">This construct is deprecated\. Don't use this nested module\.</Expects>
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

