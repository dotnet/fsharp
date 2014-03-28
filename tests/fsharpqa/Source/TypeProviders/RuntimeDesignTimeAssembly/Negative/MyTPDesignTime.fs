namespace MyTPDesignTime
open System
open Microsoft.FSharp.Core.CompilerServices

type AType() =
    inherit MyTPDesignTimeHelper.Class1()  // signature has type not found in TP-Designtime/VS-runtime

[<TypeProvider>]
type HelloWorldProvider() =
    let x = 0
    interface ITypeProvider with
        member this.ApplyStaticArguments(_,_,_) = failwith "not impl"
        member this.GetGeneratedAssemblyContents _ = failwith "not impl"
        member this.GetInvokerExpression(_,_) = failwith "not impl"
        member this.GetNamespaces() = failwith "not impl"
        member this.GetStaticParameters _ = failwith "not impl"
        [<CLIEvent>]
        member this.Invalidate : IEvent<EventHandler,EventArgs> = failwith "not impl"
    interface System.IDisposable with
        member x.Dispose() = ()
