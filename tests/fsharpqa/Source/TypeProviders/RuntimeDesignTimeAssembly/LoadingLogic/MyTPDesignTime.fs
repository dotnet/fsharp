namespace MyTPDesignTime
open System
open Microsoft.FSharp.Core.CompilerServices

//type AType() =
//    inherit MyTPDesignTimeHelper.Class1()  // signature has type not found in TP-Designtime/VS-runtime

[<TypeProvider>]
type HelloWorldProvider() =
    let x = 0
    interface ITypeProvider with
        member this.ApplyStaticArguments(_,_,_) = failwith "not impl 1"
        member this.GetGeneratedAssemblyContents _ = failwith "not impl 2"
        member this.GetInvokerExpression(_,_) = failwith "not impl 3"
        member this.GetNamespaces() = failwith "not impl 4"
        member this.GetStaticParameters _ = failwith "not impl 5"
        [<CLIEvent>]
        member this.Invalidate : IEvent<EventHandler,EventArgs> = 
           let inGAC = System.Reflection.Assembly.GetExecutingAssembly().GlobalAssemblyCache
           failwithf "not impl [%s]" (if inGAC then "INGAC" else "LOCALFILE")
    interface System.IDisposable with
        member x.Dispose() = ()

#if VER1111
[<assembly:System.Reflection.AssemblyVersion("1.1.1.1")>]
#else
[<assembly:System.Reflection.AssemblyVersion("2.2.2.2")>]
#endif
do
()