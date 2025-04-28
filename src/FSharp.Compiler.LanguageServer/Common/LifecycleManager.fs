namespace FSharp.Compiler.LanguageServer.Common

open Microsoft.CommonLanguageServerProtocol.Framework
open System
open StreamJsonRpc
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection

#nowarn "3261"

type LspServiceLifeCycleManager() =

    interface ILifeCycleManager with
        member _.ShutdownAsync(_message: string) =
            task {
                try
                    printfn "Shutting down"
                with
                | :? ObjectDisposedException
                | :? ConnectionLostException -> ()
            }

        member _.ExitAsync() = Task.CompletedTask

type FSharpLspServices(serviceCollection: IServiceCollection) as this =

    do serviceCollection.AddSingleton<ILspServices>(this) |> ignore

    let serviceProvider = serviceCollection.BuildServiceProvider()

    interface ILspServices with
        member _.GetRequiredService() = serviceProvider.GetRequiredService()

        member _.GetService() = serviceProvider.GetService()

        member _.GetRequiredServices() = serviceProvider.GetServices()

        member _.Dispose() = serviceProvider.Dispose()

        member _.TryGetService(``type``, service) =
            match serviceProvider.GetService(``type``) with
            | NonNull x ->
                service <- x
                true
            | Null -> false
