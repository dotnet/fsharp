module FSharp.Compiler.ComponentTests.HotReload.TestEnv

open System

// Ensure runtime allows metadata updates for test assemblies loaded in this process.
// These must be set before assemblies are JITed, so do this at module load time.
do
    Environment.SetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES", "debug")
    Environment.SetEnvironmentVariable("COMPlus_ForceEnc", "1")
    Environment.SetEnvironmentVariable("COMPlus_ReadyToRun", "0")
    Environment.SetEnvironmentVariable("COMPlus_ZapDisable", "1")
