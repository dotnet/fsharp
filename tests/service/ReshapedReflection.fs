namespace FSharp.Compiler.Service.Tests

#if FX_RESHAPED_REFLECTION
module internal ReflectionAdapters =
    open System.Reflection
    
    type System.Type with
        member this.Assembly = this.GetTypeInfo().Assembly
#endif
