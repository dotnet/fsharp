module TypeChecks.ReuseTcResultsTests

open System.Collections.Generic
open System.Diagnostics
open System.IO

open FSharp.Compiler.Diagnostics
open FSharp.Test.Compiler

open Xunit

open TestFramework


type Activities() =

    let tempPath = $"{getTemporaryFileName()}.fsx"

    let actualActivities = List<string>()

    let listener = new ActivityListener(
        ShouldListenTo = (fun source -> source.Name = ActivityNames.FscSourceName),
        Sample = (fun _ -> ActivitySamplingResult.AllData),
        ActivityStarted = (fun activity ->
            if activity.DisplayName.Contains Activity.Events.reuseTcResultsCachePrefix then
                actualActivities.Add activity.DisplayName))

    do
        ActivitySource.AddActivityListener listener


    [<Fact>]
    let ``Recompilation with changed sources``() =
        let expectedActivities = List<string> [
            Activity.Events.reuseTcResultsCacheAbsent
            Activity.Events.reuseTcResultsCachePresent
            Activity.Events.reuseTcResultsCacheMissed
        ]

        File.WriteAllText(tempPath, "42")

        let cUnit =
            FsxFromPath tempPath
            |> withReuseTcResults

        cUnit
        |> compileExisting
        |> shouldSucceed
        |> ignore

        File.WriteAllText(tempPath, "42")

        cUnit
        |> compileExisting
        |> shouldSucceed
        |> ignore

        Assert.Equal<List<string>>(expectedActivities, actualActivities)

    [<Fact>]
    let ``Recompilation with changed command line``() =
        let expectedActivities = List<string> [
            Activity.Events.reuseTcResultsCacheAbsent
            Activity.Events.reuseTcResultsCachePresent
            Activity.Events.reuseTcResultsCacheMissed
        ]

        File.WriteAllText(tempPath, "42")

        let cUnit =
            FsxFromPath tempPath
            |> withReuseTcResults

        cUnit
        |> compileExisting
        |> shouldSucceed
        |> ignore

        cUnit
        |> withNoOptimizationData // random option
        |> compileExisting
        |> shouldSucceed
        |> ignore

        Assert.Equal<List<string>>(expectedActivities, actualActivities)

    [<Fact>]
    let ``Recompilation with changed references``() =
        let updateFsharpCoreReference() =
            let fsharpCoreRef = typeof<_ list>.Assembly.Location
            let lastWriteTime = File.GetLastWriteTime fsharpCoreRef
            let earlier = lastWriteTime.AddMinutes -1

            // Have to do this via a copy as otherwise the file is locked on .NET framework.
            let fsharpCoreRefTemp = $"{fsharpCoreRef}.temp"
            File.Copy(fsharpCoreRef, fsharpCoreRefTemp)
            File.SetLastWriteTime(fsharpCoreRefTemp, earlier)
            File.Replace(fsharpCoreRefTemp, fsharpCoreRef, null)


        let expectedActivities = List<string> [
            Activity.Events.reuseTcResultsCacheAbsent
            Activity.Events.reuseTcResultsCachePresent
            Activity.Events.reuseTcResultsCacheMissed
        ]

        File.WriteAllText(tempPath, "42")

        let cUnit =
            FsxFromPath tempPath
            |> withReuseTcResults

        cUnit
        |> compileExisting
        |> shouldSucceed
        |> ignore

        updateFsharpCoreReference()
 
        cUnit
        |> compileExisting
        |> shouldSucceed
        |> ignore

        Assert.Equal<List<string>>(expectedActivities, actualActivities)

    [<Fact>]
    let ``Recompilation with everything same``() =
        let expectedActivities = List<string> [
            Activity.Events.reuseTcResultsCacheAbsent
            Activity.Events.reuseTcResultsCachePresent
            Activity.Events.reuseTcResultsCacheHit
        ]

        File.WriteAllText(tempPath, "42")

        let cUnit =
            FsxFromPath tempPath
            |> withReuseTcResults

        cUnit
        |> compileExisting
        |> shouldSucceed
        |> ignore

        cUnit
        |> compileExisting
        |> shouldSucceed
        |> ignore

        Assert.Equal<List<string>>(expectedActivities, actualActivities)
