open Prelude

module ByRefExtensionMethodsOverloading = 

    open System
    open System.Runtime.CompilerServices

    [<Extension>]
    type Ext = 
        [<Extension>]
        static member ExtDateTime(dt: DateTime, x:int) = dt.AddDays(double x)
    
        [<Extension>]
        static member ExtDateTime(dt: inref<DateTime>, x:int) = dt.AddDays(2.0 * double x)
    
    module UseExt = 
        let dt = DateTime.Now.ExtDateTime(3)
        let dt2 = DateTime.Now.ExtDateTime(3)