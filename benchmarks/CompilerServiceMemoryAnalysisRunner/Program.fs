open System.Diagnostics
open System.Collections.Generic
open System.Threading.Tasks
open Microsoft.Diagnostics.Runtime

// WARNING: This type name can change.
let CompilerServiceProjectCacheTypeName = "Internal.Utilities.Collections.MruCache<FSharp.Compiler.AbstractIL.Internal.Library+CompilationThreadToken,FSharp.Compiler.SourceCodeServices.FSharpProjectOptions,System.Tuple<Microsoft.FSharp.Core.FSharpOption<FSharp.Compiler.IncrementalBuilder>,FSharp.Compiler.SourceCodeServices.FSharpErrorInfo[],System.IDisposable>>"

let getClrObjectSize (clrRuntime: ClrRuntime) (clrObject: ClrObject) =
    let heap = clrRuntime.Heap
    let eval = Stack<uint64>()
    let considered = ObjectSet(heap)

    let mutable count = 0
    let mutable size = 0UL
    eval.Push(clrObject.Address)

    while eval.Count > 0 do
        let objRef = eval.Pop()

        if considered.Add objRef then

            let typ = heap.GetObjectType(objRef)
            match typ with
            | null -> ()
            | typ ->
                count <- count + 1
                size <- size + typ.GetSize(objRef)

                typ.EnumerateRefsOfObject(objRef, fun childObjRef _ ->
                    if childObjRef <> 0UL && not (considered.Contains childObjRef) then
                        eval.Push(childObjRef)
                )

    size

let analyzeCompilerServiceProjectCache clrRuntime cacheObject =
    let totalByteCount = getClrObjectSize clrRuntime cacheObject
    printfn "Cache Size: %.00f MB" (single totalByteCount / 1024.f / 1024.f)

let analyzeTestModule (runtime: ClrRuntime) (info: ModuleInfo) =
    for seg in runtime.Heap.Segments do
        let mutable objId = seg.FirstObject
        while objId <> 0UL do
            let clrObject = runtime.Heap.GetObject(objId)
            if not clrObject.IsNull && clrObject.Type.Name = CompilerServiceProjectCacheTypeName then
                analyzeCompilerServiceProjectCache runtime clrObject 
                objId <- 0UL
            else
                objId <- seg.NextObject(objId)

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
