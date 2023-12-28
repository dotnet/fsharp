module Microsoft.FSharp.Core

    open System
    
    [<AttributeUsage(AttributeTargets.Method)>]
    type TailCallAttribute() = inherit Attribute()
