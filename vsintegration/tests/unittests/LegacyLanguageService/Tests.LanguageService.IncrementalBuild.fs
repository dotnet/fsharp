// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.LanguageService

open System
open System.IO
open System.Threading
open System.Threading.Tasks
open NUnit.Framework
#if NUNIT_V2
#else
open NUnit.Framework.Constraints
#endif
open Salsa.Salsa
open Salsa.VsOpsUtils               
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.IncrementalBuild
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library

// Useful methods that someday might go into IncrementalBuild
module internal Vector = 
    /// Convert from vector to a scalar
    let ToScalar<'I> (taskname:string) (input:Vector<'I>) : Scalar<'I array> =
        let Identity _ inArray = inArray |> cancellable.Return
        Vector.Demultiplex taskname Identity input
            
[<AutoOpen>]
module internal Values = 
    let ctok = AssumeCompilationThreadWithoutEvidence()    

[<TestFixture>]
[<Category "LanguageService">] 
[<Category("LanguageService.MSBuild")>]
[<Category "ProjectSystem">]
type IncrementalBuild() = 
    
    let save _ctok _ = ()
    
    /// Called per test
    [<SetUp>]
    member this.Setup() =
        //Trace.Log <- "IncrementalBuild"
        ()


    // This test is related to 
    //    835552  Language service loses track of files in project due to intermitent file read failures
    // It verifies that incremental builder can handle changes to timestamps that happen _before_ the
    // stamp function exists. This ensures there's not a race in the data gathered for tracking file
    // timestamps in parsing.
    [<Test; Category("Expensive")>]
    member public rb.StampUpdate() =
        let path = Path.GetTempFileName()

        let TouchFile() =
            printfn "Touching file"
            File.WriteAllText(path,"Some text")

        let updateStamp = ref true

        let StampFile _cache _ctok filename =
            let result = File.GetLastWriteTimeUtc(filename)
            if !updateStamp then
                // Here, simulate that VS is writing to our file.
                TouchFile()
            result

        let Map _ctok filename = 
            "map:"+filename

        let buildDesc = new BuildDescriptionScope()
        let input = InputVector<string> "InputVector"
        let stamped = Vector.Stamp "Stamp" StampFile input
        let mapped = Vector.Map "Map" Map stamped
        buildDesc.DeclareVectorOutput mapped
        let inputs = [ BuildInput.VectorInput(input, [path]) ]
        let bound = buildDesc.GetInitialPartialBuild inputs

        let DoCertainStep bound = 
            let cache = TimeStampCache(System.DateTime.UtcNow)
            match IncrementalBuild.Step cache ctok save (Target(mapped,None)) bound |> Cancellable.runWithoutCancellation with
            | Some bound -> bound
            | None -> failwith "Expected to be able to step"

        // While updateStamp is true we should be able to step as continuously
        // because there will always be more to bound.
        let mutable bound = bound
        for i in 0..5 do 
            printfn "Iteration %d" i
            bound <- DoCertainStep bound
            System.Threading.Thread.Sleep 2000

        // Now, turn off updateStamp and the build should just finish.
        updateStamp:=false
        bound <- DoCertainStep bound
        bound <- DoCertainStep bound
        let cache = TimeStampCache(System.DateTime.UtcNow)
        match IncrementalBuild.Step cache ctok save (Target (mapped, None)) bound  |> Cancellable.runWithoutCancellation with
        | Some bound -> failwith "Build should have stopped"
        | None -> () 

            
    /// Test that stamp works
    [<Test>]
    member public rb.StampScan() =
        
        let mapSuffix = ref "Suffix1"
        let Scan ctok acc filename = 
            eventually { return acc+"-"+filename+"-"+(!mapSuffix) }
            
        let stampAs = ref DateTime.UtcNow
        let StampFile _cache _ctok filename = 
            !stampAs
                            
        let buildDesc = new BuildDescriptionScope()
        let input = InputVector<string> "InputVector"
        let acc = InputScalar<string> "Accumulator"
        let stamped = Vector.Stamp "Stamp" StampFile input
        let scanned = Vector.ScanLeft "Scan" Scan acc stamped
        buildDesc.DeclareVectorOutput scanned
        let inputs = 
            [ BuildInput.VectorInput(input, ["File1.fs"; "File2.fs"; "File3.fs"]) 
              BuildInput.ScalarInput(acc, "AccVal") ]
        let bound = buildDesc.GetInitialPartialBuild inputs
            
        printf "-[Step1]----------------------------------------------------------------------------------------\n"
        // Evaluate the first time.
        let cache = TimeStampCache(System.DateTime.UtcNow)
        let bound = Eval cache ctok save scanned bound  |> Cancellable.runWithoutCancellation
        let r = GetVectorResult (scanned, bound)
        Assert.AreEqual("AccVal-File1.fs-Suffix1-File2.fs-Suffix1",r.[1])
            
        printf "-[Step2]----------------------------------------------------------------------------------------\n"
        // Evaluate the second time. No change should be seen.
        mapSuffix:="Suffix2"
        let cache = TimeStampCache(System.DateTime.UtcNow)
        let bound = Eval cache ctok save scanned bound  |> Cancellable.runWithoutCancellation
        let r = GetVectorResult (scanned,bound)
        Assert.AreEqual("AccVal-File1.fs-Suffix1-File2.fs-Suffix1",r.[1])

        printf "-[Step3]----------------------------------------------------------------------------------------\n"
        // Evaluate a third time with timestamps updated. Should cause a rebuild
        System.Threading.Thread.Sleep 10 // Sleep a little to avoid grabbing the same 'Now'
        stampAs:=DateTime.UtcNow
        let cache = TimeStampCache(System.DateTime.UtcNow)
        let bound = Eval cache ctok save scanned bound  |> Cancellable.runWithoutCancellation
        let r = GetVectorResult (scanned,bound)
        Assert.AreEqual("AccVal-File1.fs-Suffix2-File2.fs-Suffix2",r.[1])
             
            
    /// Test case of zero elements in a vector
    [<Test>]
    member public rb.aaZeroElementVector() = // Starts with 'aa' to put it at the front.
        let stamp = ref DateTime.UtcNow
        let Stamp _cache _ctok (s:string) = !stamp
        let Map ctok (s:string) = s
        let Demult ctok (a:string[]) = a.Length  |> cancellable.Return
            
        let buildDesc = new BuildDescriptionScope()
        let inputVector = InputVector<string> "InputVector"
        let stamped = Vector.Stamp "Stamp" Stamp inputVector
        let mapped = Vector.Map "Map" Map stamped
        let result = Vector.Demultiplex "Demult" Demult mapped
        buildDesc.DeclareVectorOutput stamped
        buildDesc.DeclareVectorOutput mapped
        buildDesc.DeclareScalarOutput result
            
        // Try first with one input
        let inputs1 = [ BuildInput.VectorInput(inputVector, [""]) ]
        let build1 = buildDesc.GetInitialPartialBuild inputs1

        let cache = TimeStampCache(System.DateTime.UtcNow)
        let build1Evaled = Eval cache ctok save result build1  |> Cancellable.runWithoutCancellation
        let r1 = GetScalarResult (result, build1Evaled)
        match r1 with
        | Some(v,dt) -> Assert.AreEqual(1,v) 
        | None -> failwith "Expected the value 1 to be returned."
            
        // Now with zero. This was the original bug.
        stamp := DateTime.UtcNow
        let inputs0 = [ BuildInput.VectorInput(inputVector, []) ]
        let build0 = buildDesc.GetInitialPartialBuild inputs0

        let cache = TimeStampCache(System.DateTime.UtcNow)
        let build0Evaled = Eval cache ctok save result build0  |> Cancellable.runWithoutCancellation
        let r0 = GetScalarResult (result, build0Evaled)
        match r0 with
        | Some(v,dt) -> Assert.AreEqual(0,v) 
        | None -> failwith "Expected the value 0 to be returned."  
        ()
         
            
    /// Here, we want a multiplex to increase the number of items processed.
    [<Test>]
    member public rb.MultiplexTransitionUp() =
        let elements = ref 1
        let timestamp = ref System.DateTime.UtcNow
        let Input() : string array =  [| for i in 1..!elements -> sprintf "Element %d" i |]
        let Stamp _cache ctok s = !timestamp
        let Map ctok (s:string) = sprintf "Mapped %s " s
        let Result ctok (a:string[]) = String.Join(",", a)  |> cancellable.Return
        let now = System.DateTime.UtcNow
        let FixedTimestamp _cache _ctok _  =  now
            
        let buildDesc = new BuildDescriptionScope()
        let input = InputVector<string> "InputVector"
        let stampedInput = Vector.Stamp "StampInput" Stamp input
        //let demultiplexedInput = Vector.Demultiplex "DemultInput" Demult stampedInput
        //let multiplexed = Scalar.Multiplex "Mult" Mult demultiplexedInput
        let mapped = Vector.Map "Map" Map stampedInput
        let mapped = Vector.Stamp "FixedTime" FixedTimestamp mapped // Change in vector size should x-ray through even if timestamps haven't changed in remaining items.
        let result = Vector.Demultiplex "DemultResult" Result mapped
        buildDesc.DeclareScalarOutput result
            
        // Create the build.
        let inputs = [ BuildInput.VectorInput(input, ["Input 0"]) ]
        let bound = buildDesc.GetInitialPartialBuild inputs
            
        // Evaluate it with value 1
        elements := 1
        let cache = TimeStampCache(System.DateTime.UtcNow)
        let bound = Eval cache ctok save result bound  |> Cancellable.runWithoutCancellation
        let r1 = GetScalarResult<string>(result, bound)
        match r1 with
        | Some(s,dt) -> printfn "%s" s
        | None -> failwith ""
            
        // Now, re-evaluate it with value 2
        elements := 2
        System.Threading.Thread.Sleep(100)
        timestamp := System.DateTime.UtcNow
            
        let cache = TimeStampCache(System.DateTime.UtcNow)
        let bound = Eval cache ctok save result bound  |> Cancellable.runWithoutCancellation
        let r2 = GetScalarResult (result, bound)
        match r2 with
        | Some(s,dt) -> Assert.AreEqual("Mapped Input 0 ",s)
        | None -> failwith ""
            
    (*
    /// Here, we want a multiplex to decrease the number of items processed.
    [<Test>]
    member public rb.MultiplexTransitionDown() =
        let elements = ref 1
        let timestamp = ref System.DateTime.UtcNow
        let Mult(s:string) : string array =  [| for i in 1..!elements -> sprintf "Element %d" i |]
        let Stamp(s) = !timestamp
        let Map(s:string) = 
            printfn "Map called with %s" s
            sprintf "Mapped %s " s
        let Demult(a:string array) : string = 
            printfn "Demult called with %d items" a.Length
            sprintf "Demult %s" (String.Join(",",a))
        let Result(a:string array) : string = 
            let result = String.Join(",", a)
            printfn "Result called with %d items returns %s" a.Length result
            result
        let now = System.DateTime.UtcNow
        let FixedTimestamp _  =  
            printfn "Fixing timestamp"
            now               
            
        let buildDesc = new BuildDescriptionScope()
        let input = InputVector<string> "InputVector"
        let stampedInput = Vector.Stamp "StampInput" Stamp input
        let demultiplexedInput = Vector.Demultiplex "DemultInput" Demult stampedInput
        let multiplexed = Scalar.Multiplex "Mult" Mult demultiplexedInput
        let mapped = Vector.Map "Map" Map multiplexed
        let fixedmapped = Vector.Stamp "FixedTime" FixedTimestamp mapped // Change in vector size should x-ray through even if timestamps haven't changed in remaining items.
        let result = Vector.Demultiplex "DemultResult" Result fixedmapped
            
        buildDesc.DeclareScalarOutput demultiplexedInput
        buildDesc.DeclareVectorOutput mapped
        buildDesc.DeclareVectorOutput fixedmapped
        buildDesc.DeclareScalarOutput result
            
        // Create the build.
        let bound = buildDesc.GetInitialPartialBuild(["InputVector",1,[box "Input 0"]],[])         
            
        // Evaluate it with value 2
        elements := 2
        let bound = Eval result bound
        let r1 = GetScalarResult<string>(result, bound)
        match r1 with
        | Some(s,dt) -> printfn "%s" s
        | None -> failwith ""
            
        // Now, re-evaluate it with value 1
        elements := 1
        System.Threading.Thread.Sleep(100)
        timestamp := System.DateTime.UtcNow
            
        let buildDemuxed = Eval demultiplexedInput bound
        let rdm = GetScalarResult (demultiplexedInput,buildDemuxed)
        match rdm with
        | Some(s,dt)->Assert.AreEqual("Demult Input 0", s)
        | None -> failwith "unexpected"
            
        let buildMapped = Eval mapped bound
        let mp = GetVectorResult (mapped,buildMapped)
        Assert.AreEqual(1,mp.Length)
        let melem = mp.[0]
        Assert.AreEqual("Mapped Element 1 ", melem)
            
        let buildFixedMapped = Eval fixedmapped buildMapped
        let mp = GetVectorResult (fixedmapped,buildFixedMapped)
        Assert.AreEqual(1,mp.Length)
        let melem = mp.[0]
        Assert.AreEqual("Mapped Element 1 ", melem)            
            
        let bound = Eval result bound
        let r2 = GetScalarResult<string>(result, bound)
        match r2 with
        | Some(s,dt) -> Assert.AreEqual("Mapped Element 1 ",s)
        | None -> failwith "unexpected"
         *)
            
    /// Test that stamp works
    [<Test>]
    member public rb.StampMap() =
        
        let mapSuffix = ref "Suffix1"
        let MapIt ctok filename = 
            filename+"."+(!mapSuffix)
            
        let stampAs = ref DateTime.UtcNow
        let StampFile _cache ctok filename =  
            !stampAs
                            
        let buildDesc = new BuildDescriptionScope()
        let input = InputVector<string> "InputVector"
        let stamped = Vector.Stamp "Stamp" StampFile input
        let mapped = Vector.Map "Map" MapIt stamped
        buildDesc.DeclareVectorOutput mapped
        let inputs = [ BuildInput.VectorInput(input, ["File1.fs";"File2.fs";"File3.fs"]) ]
        let bound = buildDesc.GetInitialPartialBuild inputs
            
        printf "-[Step1]----------------------------------------------------------------------------------------\n"
        // Evaluate the first time.
        let cache = TimeStampCache(System.DateTime.UtcNow)
        let bound = Eval cache ctok save mapped bound  |> Cancellable.runWithoutCancellation
        let r = GetVectorResult (mapped,bound)
        Assert.AreEqual("File2.fs.Suffix1",r.[1])
            
        printf "-[Step2]----------------------------------------------------------------------------------------\n"
        // Evaluate the second time. No change should be seen.
        mapSuffix:="Suffix2"
        let cache = TimeStampCache(System.DateTime.UtcNow)
        let bound = Eval cache ctok save  mapped bound  |> Cancellable.runWithoutCancellation
        let r = GetVectorResult (mapped,bound)
        Assert.AreEqual("File2.fs.Suffix1",r.[1])

        printf "-[Step3]----------------------------------------------------------------------------------------\n"
        // Evaluate a third time with timestamps updated. Should cause a rebuild
        let cache = TimeStampCache(System.DateTime.UtcNow)
        while !stampAs = DateTime.UtcNow do 
            System.Threading.Thread.Sleep 10 // Sleep a little to avoid grabbing the same 'Now'
        stampAs:=DateTime.UtcNow
        let bound = Eval cache ctok save mapped bound  |> Cancellable.runWithoutCancellation
        let r = GetVectorResult (mapped,bound)
        Assert.AreEqual("File2.fs.Suffix2",r.[1])
            
    /// Test that stamp works
    [<Test>]
    member public rb.StampDemultiplex() =
        
        let joinedResult = ref "Join1"
        let Join ctok (filenames:_[]) = 
            !joinedResult  |> cancellable.Return
            
        let stampAs = ref DateTime.UtcNow
        let StampFile _cache ctok filename = 
            !stampAs
                            
        let buildDesc = new BuildDescriptionScope()
        let input = InputVector<string> "InputVector"
        let stamped = Vector.Stamp "Stamp" StampFile input
        let joined = Vector.Demultiplex "Demultiplex" Join stamped
        buildDesc.DeclareScalarOutput joined
        let inputs = [ BuildInput.VectorInput(input, ["File1.fs";"File2.fs";"File3.fs"]) ]
        let bound = buildDesc.GetInitialPartialBuild inputs
            
        printf "-[Step1]----------------------------------------------------------------------------------------\n"
        // Evaluate the first time.
        let cache = TimeStampCache(System.DateTime.UtcNow)
        let bound = Eval cache ctok save joined bound  |> Cancellable.runWithoutCancellation
        let (r,_) = Option.get (GetScalarResult<string>(joined,bound))
        Assert.AreEqual("Join1",r)
            
        printf "-[Step2]----------------------------------------------------------------------------------------\n"
        // Evaluate the second time. No change should be seen.
        joinedResult:="Join2"
        let cache = TimeStampCache(System.DateTime.UtcNow)
        let bound = Eval cache ctok save joined bound  |> Cancellable.runWithoutCancellation
        let (r,_) = Option.get (GetScalarResult (joined,bound))
        Assert.AreEqual("Join1",r)

        printf "-[Step3]----------------------------------------------------------------------------------------\n"
        // Evaluate a third time with timestamps updated. Should cause a rebuild
        while !stampAs = DateTime.UtcNow do 
            System.Threading.Thread.Sleep 10 // Sleep a little to avoid grabbing the same 'Now'
        stampAs:=DateTime.UtcNow
        let cache = TimeStampCache(System.DateTime.UtcNow)
        let bound = Eval cache ctok save joined bound  |> Cancellable.runWithoutCancellation
        let (r,_) = Option.get (GetScalarResult (joined,bound))
        Assert.AreEqual("Join2",r)
            

    /// Test that Demultiplex followed by ScanLeft works
    [<Test>]
    member public rb.DemultiplexScanLeft() =
        let Size ctok (ar:_[]) = ar.Length  |> cancellable.Return
        let Scan ctok acc (file :string) = eventually { return acc + file.Length }
        let buildDesc = new BuildDescriptionScope()
        let inVector = InputVector<string> "InputVector"
        let vectorSize = Vector.Demultiplex "Demultiplex" Size inVector
        let scanned = Vector.ScanLeft "Scan" Scan vectorSize inVector
        buildDesc.DeclareScalarOutput vectorSize
        buildDesc.DeclareVectorOutput scanned
        let inputs = [ BuildInput.VectorInput(inVector, ["File1.fs";"File2.fs";"File3.fs"]) ]
        let bound = buildDesc.GetInitialPartialBuild inputs
            
        let cache = TimeStampCache(System.DateTime.UtcNow)
        let e = Eval cache ctok save scanned bound     |> Cancellable.runWithoutCancellation
        let r = GetScalarResult (vectorSize,e)  
        match r with 
        | Some(r,_) -> Assert.AreEqual(3,r)
        | None -> Assert.Fail("No size was returned")       
            

    /// Test that a simple scalar action works.
    [<Test>] 
    member public rb.Scalar() =
        let buildDesc = new BuildDescriptionScope()
        let inScalar = InputScalar<string> "Scalar"
        buildDesc.DeclareScalarOutput  inScalar
        let inputs = [ BuildInput.ScalarInput(inScalar, "A Scalar Value") ]
        let bound = buildDesc.GetInitialPartialBuild inputs

        let cache = TimeStampCache(System.DateTime.UtcNow)
        let e = Eval cache ctok save inScalar bound  |> Cancellable.runWithoutCancellation
        let r = GetScalarResult(inScalar,e)
        match r with 
            | Some(r,_) -> Assert.AreEqual("A Scalar Value", r)
            | None -> Assert.Fail()
            
    /// Test that ScanLeft works.
    [<Test>]
    member public rb.ScanLeft() =
        let DoIt ctok (a:int*string) (b:string) =
            eventually { return ((fst a)+1,b) }
            
        let buildDesc = new BuildDescriptionScope()
        let inScalar = InputScalar<int*string> "InputScalar"
        let inVector = InputVector<string> "InputVector"
        let result = Vector.ScanLeft "DoIt" DoIt inScalar inVector
        buildDesc.DeclareVectorOutput result
            
        let inputs = 
            [ BuildInput.VectorInput(inVector, ["File1.fs";"File2.fs";"File3.fs"]);
              BuildInput.ScalarInput(inScalar, (5,"")) ]

        let bound = buildDesc.GetInitialPartialBuild(inputs)
        let cache = TimeStampCache(System.DateTime.UtcNow)
        let e = Eval cache ctok save result bound  |> Cancellable.runWithoutCancellation
        let r = GetVectorResult(result,e)
        if [| (6,"File1.fs"); (7,"File2.fs"); (8, "File3.fs") |] <> r then 
            printfn "Got %A" r
            Assert.Fail()
        ()     
            
    /// Convert a vector to a scalar
    [<Test>]
    member public rb.ToScalar() =
        let buildDesc = new BuildDescriptionScope()
        let inVector = InputVector<string> "InputVector"
        let result = Vector.ToScalar "ToScalar" inVector
        buildDesc.DeclareScalarOutput result 
        let inputs = [ BuildInput.VectorInput(inVector, ["File1.fs";"File2.fs";"File3.fs"]) ]
        let bound = buildDesc.GetInitialPartialBuild(inputs)

        let cache = TimeStampCache(System.DateTime.UtcNow)
        let e = Eval cache ctok save result bound  |> Cancellable.runWithoutCancellation
        let r = GetScalarResult (result, e)
        match r with 
        | Some(r,ts)->
            if "File3.fs"<>(r.[2]) then
                printf "Got %A\n" (r.[2])
                Assert.Fail()
        | None -> Assert.Fail()

             
            
    /// Check a cancellation
    [<Test>]
    member public rb.``Can cancel Eval``() =
        let buildDesc = new BuildDescriptionScope()
        let inVector = InputVector<string> "InputVector"
        let result = Vector.ToScalar "ToScalar" inVector
        buildDesc.DeclareScalarOutput result 
        let inputs = [ BuildInput.VectorInput(inVector, ["File1.fs";"File2.fs";"File3.fs"]) ]
        let bound = buildDesc.GetInitialPartialBuild(inputs)

        let cts = new CancellationTokenSource()
        cts.Cancel() 
        let res = 
            let cache = TimeStampCache(System.DateTime.UtcNow)
            match Eval cache ctok save result bound |> Cancellable.run cts.Token with 
            | ValueOrCancelled.Cancelled _ -> true
            | ValueOrCancelled.Value _ -> false
        Assert.AreEqual(res, true)

            
    /// This test replicates the data flow of the assembly reference model. It includes several concepts 
    /// that were new at the time: Scalars, Invalidation, Disposal
    [<Test>]
    member public rb.AssemblyReferenceModel() =
        let ParseTask ctok filename = sprintf "Parse(%s)" filename
        let now = System.DateTime.UtcNow
        let StampFileNameTask _cache ctok filename = now 
        let TimestampReferencedAssemblyTask _cache ctok reference = now
        let ApplyMetaCommands ctok (parseResults:string[]) = "tcConfig-of("+String.Join(",",parseResults)+")"
        let GetReferencedAssemblyNames ctok (tcConfig) = [|"Assembly1.dll";"Assembly2.dll";"Assembly3.dll"|]
        let ReadAssembly ctok assemblyName = sprintf "tcImport-of(%s)" assemblyName
        let CombineImportedAssembliesTask ctok imports = "tcAcc"  |> cancellable.Return
        let TypeCheckTask ctok tcAcc parseResults = eventually { return tcAcc }
        let FinalizeTypeCheckTask ctok results = "finalized"  |> cancellable.Return

        // Build rules.
        let buildDesc = new BuildDescriptionScope()
        
        // Inputs
        let fileNamesNode = InputVector<string> "Filenames"
        let referencedAssembliesNode = InputVector<string * DateTime> "ReferencedAssemblies"
        
        //Build
        let stampedFileNamesNode        = Vector.Stamp "SourceFileTimeStamps" StampFileNameTask fileNamesNode
        let parseTreesNode              = Vector.Map "ParseTrees" ParseTask stampedFileNamesNode
        let stampedReferencedAssembliesNode = Vector.Stamp "TimestampReferencedAssembly" TimestampReferencedAssemblyTask referencedAssembliesNode

        let initialTcAccNode            = Vector.Demultiplex "CombineImportedAssemblies" CombineImportedAssembliesTask stampedReferencedAssembliesNode

        let tcStatesNode                = Vector.ScanLeft "TypeCheckingStates" TypeCheckTask initialTcAccNode parseTreesNode

        let finalizedTypeCheckNode      = Vector.Demultiplex "FinalizeTypeCheck" FinalizeTypeCheckTask tcStatesNode
        let buildDesc            = new BuildDescriptionScope ()

        do buildDesc.DeclareVectorOutput stampedFileNamesNode
        do buildDesc.DeclareVectorOutput stampedReferencedAssembliesNode
        do buildDesc.DeclareVectorOutput parseTreesNode
        do buildDesc.DeclareVectorOutput tcStatesNode
        do buildDesc.DeclareScalarOutput initialTcAccNode
        do buildDesc.DeclareScalarOutput finalizedTypeCheckNode

        let inputs = 
            [ BuildInput.VectorInput(fileNamesNode, ["File1.fs";"File2.fs";"File3.fs"]);
              BuildInput.VectorInput(referencedAssembliesNode, [("lib1.dll", now);("lib2.dll", now)]) ]
        let bound = buildDesc.GetInitialPartialBuild(inputs)
        let cache = TimeStampCache(System.DateTime.UtcNow)
        let e = Eval cache ctok save finalizedTypeCheckNode bound  |> Cancellable.runWithoutCancellation
        let r = GetScalarResult(finalizedTypeCheckNode,e)
            
        ()

    [<Test>]
    member public rb.OneToOneWorks() =
        let VectorModify ctok (input:int) : string =
            sprintf "Transformation of %d" input

        let buildDesc = new BuildDescriptionScope()
        let inputs = InputVector<int> "Inputs"
        let outputs = Vector.Map "Modify" VectorModify inputs
        buildDesc.DeclareVectorOutput outputs
        let inputs = [ BuildInput.VectorInput(inputs, [1;2;3;4]) ]
        let bound = buildDesc.GetInitialPartialBuild inputs

        let cache = TimeStampCache(System.DateTime.UtcNow)
        let evaled = Eval cache ctok save outputs bound  |> Cancellable.runWithoutCancellation
        let outputs = GetVectorResult(outputs,evaled)
        Assert.AreEqual("Transformation of 4", outputs.[3])
        ()   
            
    /// In this bug, the desired output is between other outputs.
    /// The getExprById function couldn't find it.            
    [<Test>]
    member public rb.HiddenOutputGroup() =
        let VectorModify ctok (input:int) : string =
            sprintf "Transformation of %d" input

        let buildDesc = new BuildDescriptionScope()
        let inputs = InputVector<int> "Inputs"
        let outputs = Vector.Map "Modify" VectorModify inputs
        buildDesc.DeclareVectorOutput inputs
        buildDesc.DeclareVectorOutput inputs
        buildDesc.DeclareVectorOutput inputs
        buildDesc.DeclareVectorOutput outputs
        buildDesc.DeclareVectorOutput inputs
        buildDesc.DeclareVectorOutput inputs
        buildDesc.DeclareVectorOutput inputs
        let inputs = [ BuildInput.VectorInput(inputs, [1;2;3;4]) ]
        let bound = buildDesc.GetInitialPartialBuild inputs

        let cache = TimeStampCache(System.DateTime.UtcNow)
        let evaled = Eval cache ctok save outputs bound  |> Cancellable.runWithoutCancellation
        let outputs = GetVectorResult(outputs,evaled)
        Assert.AreEqual("Transformation of 4", outputs.[3])
        ()               
            
    /// Empty build should just be a NOP.
    [<Test>]
    member public rb.EmptyBuildIsNop() =
        let VectorModify ctok (input:int) : string =
            sprintf "Transformation of %d" input

        let buildDesc = new BuildDescriptionScope()
        let inputs = InputVector<int> "Inputs"
        let outputs = Vector.Map "Modify" VectorModify inputs
        buildDesc.DeclareVectorOutput outputs
        let inputs = [ BuildInput.VectorInput(inputs, []) ]
        let bound = buildDesc.GetInitialPartialBuild inputs

        let cache = TimeStampCache(System.DateTime.UtcNow)
        let evaled = Eval cache ctok save outputs  bound  |> Cancellable.runWithoutCancellation
        let outputs = GetVectorResult(outputs,evaled)
        ()               
              
