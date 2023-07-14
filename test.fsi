
namespace Microsoft.FSharp.Core
    open System
    [<AttributeUsage (AttributeTargets.Method ||| AttributeTargets.Property,AllowMultiple=false)>]  
    [<Sealed>]
    type NoDynamicInvocationAttribute =
        inherit Attribute
        new: unit -> NoDynamicInvocationAttribute
        internal new: isLegacy: bool -> NoDynamicInvocationAttribute

    module Operators = 
        [<CompiledName("GetId")>]
        val inline id: value: 'T -> 'T
            