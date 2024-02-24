open Prelude

module ByRefExtensionMethods1 = 

    open System
    open System.Runtime.CompilerServices

    [<Extension>]
    type Ext = 
    
        [<Extension>]
        static member ExtDateTime2(dt: inref<DateTime>, x:int) = dt.AddDays(double x)
    
    module UseExt = 
        let now = DateTime.Now
        let dt2 = now.ExtDateTime2(3)
        check "£f3mllkm2" dt2 (now.AddDays(3.0))
        