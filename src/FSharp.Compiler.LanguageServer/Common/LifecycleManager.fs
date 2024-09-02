namespace FSharp.Compiler.LanguageServer.Common

open Microsoft.CommonLanguageServerProtocol.Framework
open System
open StreamJsonRpc
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection

type LspServiceLifeCycleManager() =

    interface ILifeCycleManager with
        member this.ShutdownAsync(message: string) =
            task {
                try
                    printfn "Shutting down"
                with
                | :? ObjectDisposedException
                | :? ConnectionLostException -> ()
            }

        member this.ExitAsync() = Task.CompletedTask

type FSharpLspServices(serviceCollection: IServiceCollection) as this =

    do serviceCollection.AddSingleton<ILspServices>(this) |> ignore

    let serviceProvider = serviceCollection.BuildServiceProvider()

    interface ILspServices with
        member this.GetRequiredService<'T>() : 'T =
            serviceProvider.GetRequiredService<'T>()

        member this.TryGetService(t) = serviceProvider.GetService(t)

        member this.GetRequiredServices() = serviceProvider.GetServices()

        member this.GetRegisteredServices() = failwith "Not implemented"

        member this.SupportsGetRegisteredServices() = false

        member this.Dispose() = ()
