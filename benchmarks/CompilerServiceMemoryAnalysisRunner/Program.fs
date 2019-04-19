open System.Diagnostics
open System.Collections.Generic
open System.Threading.Tasks
open Microsoft.Diagnostics.Runtime

// WARNING: This type name can change.
let CompilerServiceProjectCacheTypeName = "Internal.Utilities.Collections.MruCache<FSharp.Compiler.AbstractIL.Internal.Library+CompilationThreadToken,FSharp.Compiler.SourceCodeServices.FSharpProjectOptions,System.Tuple<Microsoft.FSharp.Core.FSharpOption<FSharp.Compiler.IncrementalBuilder>,FSharp.Compiler.SourceCodeServices.FSharpErrorInfo[],System.IDisposable>>"

[<RequireQualifiedAccess>]
type ClrInstance =
    | Object of ClrObject
    | ValueClass of ClrValueClass

let rec getFullMemorySize (instance: ClrInstance) (objSet: HashSet<uint64>) =
    match instance with
    | ClrInstance.Object(clrInstance) ->

        if clrInstance.IsNull || not (objSet.Add clrInstance.Address) then 0
        else

        let instanceFieldFullMemorySize =
            clrInstance.Type.Fields
            |> Array.ofSeq
            |> Array.map (fun clrInstanceField ->
                if clrInstanceField.Type.IsValueClass then
                    let clrValueClass = clrInstance.GetValueClassField(clrInstanceField.Name)
                    fun () -> getFullMemorySize (ClrInstance.ValueClass clrValueClass) objSet
                elif clrInstanceField.Type.IsPrimitive then
                    fun () -> clrInstanceField.Type.BaseSize
                else
                    let clrObject = clrInstance.GetObjectField(clrInstanceField.Name)
                    fun () -> getFullMemorySize (ClrInstance.Object clrObject) objSet
            )
            |> Seq.sumBy (fun f -> f ())

        clrInstance.Type.BaseSize + instanceFieldFullMemorySize

    | ClrInstance.ValueClass(clrInstance) ->

        let instanceFieldFullMemorySize =
            clrInstance.Type.Fields
            |> Seq.map (fun clrInstanceField ->
                if clrInstanceField.Type.IsValueClass then
                    let clrValueClass = clrInstance.GetValueClassField(clrInstanceField.Name)
                    fun () -> getFullMemorySize (ClrInstance.ValueClass clrValueClass) objSet
                elif clrInstanceField.Type.IsPrimitive then
                    fun () -> clrInstanceField.Type.BaseSize
                else
                    let clrObject = clrInstance.GetObjectField(clrInstanceField.Name)
                    fun () -> getFullMemorySize (ClrInstance.Object clrObject) objSet
            )
            |> Seq.sumBy (fun f -> f ())

        clrInstance.Type.BaseSize + instanceFieldFullMemorySize

let analyzeCompilerServiceProjectCache (cacheObject: ClrObject) (runtime: ClrRuntime) =
    printfn "Analyzing compiler service project cache"

    let totalByteCount = getFullMemorySize (ClrInstance.Object cacheObject) (HashSet())
    printfn "Cache Size: %.00f MB" (single totalByteCount / 1024.f / 1024.f)

    printfn "Finished analyzing compiler service project cache"

let analyzeTestModule (runtime: ClrRuntime) (info: ModuleInfo) =
    printfn "Analyzing test module"

    for seg in runtime.Heap.Segments do
        let mutable objId = seg.FirstObject
        while objId <> 0UL do
            let clrObject = runtime.Heap.GetObject(objId)
            if not clrObject.IsNull && clrObject.Type.Name = CompilerServiceProjectCacheTypeName then
                analyzeCompilerServiceProjectCache clrObject runtime
                objId <- 0UL
            else
                objId <- seg.NextObject(objId)

    printfn "Finished analyzing test module"

let analyzeDataTarget (dataTarget: DataTarget) =
    dataTarget.EnumerateModules()
    |> Seq.iter (fun m ->
        if m.FileName.Contains("CompilerServiceMemoryAnalysisTest.dll") then
            analyzeTestModule (dataTarget.ClrVersions.[0].CreateRuntime()) m
        else
            ()
    )

[<EntryPoint>]
let main _ =
    use p = new Process()
    p.StartInfo.FileName <- "dotnet"
    p.StartInfo.Arguments <- "CompilerServiceMemoryAnalysisTest.dll"
    p.StartInfo.UseShellExecute <- false
    p.StartInfo.RedirectStandardOutput <- true
    p.StartInfo.RedirectStandardInput <- true
    p.OutputDataReceived.Add(fun args ->
        if args.Data = "CompilerServiceMemoryAnalysisTest-Finished" then
            try
                let dataTarget = DataTarget.AttachToProcess(p.Id, 1000u)
                analyzeDataTarget dataTarget
                dataTarget.Dispose()
                p.StandardInput.WriteLine()
            with
            | ex -> 
                printfn "%A" ex
                p.StandardInput.WriteLine()
        else
            printfn "\nCompilerServiceMemoryAnalysisTest Output: %A\n" args.Data 
    )

    printfn "Initializing compiler service test"
    p.Start() |> ignore
    p.BeginOutputReadLine()
    p.WaitForExit()
    printfn "CompilerServiceMemoryAnalysisRunner Finished"
    0
