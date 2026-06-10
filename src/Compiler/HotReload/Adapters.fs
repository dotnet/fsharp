namespace FSharp.Compiler.HotReload

open System.Threading
open System.Threading.Tasks

module internal DotnetWatchBridge =

    let emitDelta request =
        FSharpEditAndContinueLanguageService.Instance.EmitDelta request

module internal IdeBridge =

    let emitDeltaAsync (request: DeltaEmissionRequest) (cancellationToken: CancellationToken) : Task<Result<DeltaEmissionResult, HotReloadError>> =
        cancellationToken.ThrowIfCancellationRequested()
        Task.FromResult(FSharpEditAndContinueLanguageService.Instance.EmitDelta request)
