namespace FSharp.Compiler.HotReload

open System
#if NET5_0_OR_GREATER
open System.Reflection.Metadata
#endif

open Internal.Utilities.Library

open FSharp.Compiler.EnvironmentHelpers

[<Flags>]
type internal HotReloadCapabilityFlags =
    | None = 0
    | Il = 1
    | Metadata = 2
    | PortablePdb = 4
    | MultipleGenerations = 8
    | RuntimeApply = 16

type internal HotReloadCapabilities = { Flags: HotReloadCapabilityFlags }

module internal HotReloadCapability =

    [<Literal>]
    let private RuntimeApplyFeatureFlagName =
        EnvironmentVariableNames.HotReloadEnableRuntimeApply

    let private runtimeApplySupported: bool =
#if NET5_0_OR_GREATER
        try
            MetadataUpdater.IsSupported
        with RecoverableException _ ->
            false
#else
        false
#endif

    let private runtimeApplyFeatureFlag: bool =
        isEnvVarTruthy RuntimeApplyFeatureFlagName

    let private runtimeApplyEnabled = runtimeApplySupported && runtimeApplyFeatureFlag

    let current: HotReloadCapabilities =
        let baseFlags =
            HotReloadCapabilityFlags.Il
            ||| HotReloadCapabilityFlags.Metadata
            ||| HotReloadCapabilityFlags.PortablePdb
            ||| HotReloadCapabilityFlags.MultipleGenerations

        let flags =
            if runtimeApplyEnabled then
                baseFlags ||| HotReloadCapabilityFlags.RuntimeApply
            else
                baseFlags

        { Flags = flags }
