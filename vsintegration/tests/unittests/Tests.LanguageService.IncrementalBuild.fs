// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Tests.LanguageService

open System
open System.IO
open NUnit.Framework
#if NUNIT_V2
#else
open NUnit.Framework.Constraints
#endif
open Salsa.Salsa
open Salsa.VsOpsUtils               
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.IncrementalBuild
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library

// Useful methods that someday might go into IncrementalBuild
module internal Vector = 
    /// Convert from vector to a scalar
    let ToScalar<'I> (taskname:string) (input:Vector<'I>) : Scalar<'I array> =
        let Identity inArray = inArray
        Vector.Demultiplex taskname Identity input
            
    
[<TestFixture>] 
[<Category("LanguageService.MSBuild")>]
[<Category("LanguageService.ProjectSystem")>]
type IncrementalBuild() = 
    
    
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

        let StampFile filename =
            let result = File.GetLastWriteTime(filename)
            if !updateStamp then
                // Here, simulate that VS is writing to our file.
                TouchFile()
            result

        let Map filename = 
            "map:"+filename

        let buildDesc = new BuildDescriptionScope()
        let input = InputVector<string> "InputVector"
        let stamped = Vector.Stamp "Stamp" StampFile input
        let mapped = Vector.Map "Map" Map stamped
        buildDesc.DeclareVectorOutput mapped
        let inputs = [ BuildInput.VectorInput(input, [path]) ]
        let bound = buildDesc.GetInitialPartialBuild inputs

        let DoCertainStep bound = 
            match IncrementalBuild.Step (Target(mapped,None)) bound with
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
        match IncrementalBuild.Step (Target (mapped, None)) bound with
        | Some bound -> failwith "Build should have stopped"
        | None -> () 

            
    /// Test that stamp works
    [<Test>]
    member public rb.StampScan() =
        
        let mapSuffix = ref "Suffix1"
        let Scan acc filename = 
            eventually { return acc+"-"+filename+"-"+(!mapSuffix) }
            
        let stampAs = ref DateTime.Now
        let StampFile(filename) = 
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
        let bound = Eval scanned bound
        let r = GetVectorResult (scanned, bound)
        Assert.AreEqual("AccVal-File1.fs-Suffix1-File2.fs-Suffix1",r.[1])
            
        printf "-[Step2]----------------------------------------------------------------------------------------\n"
        // Evaluate the second time. No change should be seen.
        mapSuffix:="Suffix2"
        let bound = Eval scanned bound
        let r = GetVectorResult (scanned,bound)
        Assert.AreEqual("AccVal-File1.fs-Suffix1-File2.fs-Suffix1",r.[1])

        printf "-[Step3]----------------------------------------------------------------------------------------\n"
        // Evaluate a third time with timestamps updated. Should cause a rebuild
        System.Threading.Thread.Sleep 10 // Sleep a little to avoid grabbing the same 'Now'
        stampAs:=DateTime.Now
        let bound = Eval scanned bound
        let r = GetVectorResult (scanned,bound)
        Assert.AreEqual("AccVal-File1.fs-Suffix2-File2.fs-Suffix2",r.[1])
             
            
    /// Test case of zero elements in a vector
    [<Test>]
    member public rb.aaZeroElementVector() = // Starts with 'aa' to put it at the front.
        let stamp = ref DateTime.Now
        let Stamp(s:string) = !stamp
        let Map(s:string) = s
        let Demult(a:string array) : int = a.Length
            
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

        let build1Evaled = Eval result build1
        let r1 = GetScalarResult (result, build1Evaled)
        match r1 with
        | Some(v,dt) -> Assert.AreEqual(1,v) 
        | None -> failwith "Expected the value 1 to be returned."
            
        // Now with zero. This was the original bug.
        stamp := DateTime.Now
        let inputs0 = [ BuildInput.VectorInput(inputVector, []) ]
        let build0 = buildDesc.GetInitialPartialBuild inputs0

        let build0Evaled = Eval result build0
        let r0 = GetScalarResult (result, build0Evaled)
        match r0 with
        | Some(v,dt) -> Assert.AreEqual(0,v) 
        | None -> failwith "Expected the value 0 to be returned."  
        ()
         
            
    /// Here, we want a multiplex to increase the number of items processed.
    [<Test>]
    member public rb.MultiplexTransitionUp() =
        let elements = ref 1
        let timestamp = ref System.DateTime.Now
        let Input() : string array =  [| for i in 1..!elements -> sprintf "Element %d" i |]
        let Stamp(s) = !timestamp
        let Map(s:string) = sprintf "Mapped %s " s
        let Result(a:string[]) : string = String.Join(",", a)
        let now = System.DateTime.Now
        let FixedTimestamp _  =  now
            
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
        let bound = Eval result bound
        let r1 = GetScalarResult<string>(result, bound)
        match r1 with
        | Some(s,dt) -> printfn "%s" s
        | None -> failwith ""
            
        // Now, re-evaluate it with value 2
        elements := 2
        System.Threading.Thread.Sleep(100)
        timestamp := System.DateTime.Now
            
        let bound = Eval result bound
        let r2 = GetScalarResult (result, bound)
        match r2 with
        | Some(s,dt) -> Assert.AreEqual("Mapped Input 0 ",s)
        | None -> failwith ""
            
    (*
    /// Here, we want a multiplex to decrease the number of items processed.
    [<Test>]
    member public rb.MultiplexTransitionDown() =
        let elements = ref 1
        let timestamp = ref System.DateTime.Now
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
        let now = System.DateTime.Now
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
        timestamp := System.DateTime.Now
            
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
        let MapIt(filename) = 
            filename+"."+(!mapSuffix)
            
        let stampAs = ref DateTime.Now
        let StampFile(filename) =  
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
        let bound = Eval mapped bound
        let r = GetVectorResult (mapped,bound)
        Assert.AreEqual("File2.fs.Suffix1",r.[1])
            
        printf "-[Step2]----------------------------------------------------------------------------------------\n"
        // Evaluate the second time. No change should be seen.
        mapSuffix:="Suffix2"
        let bound = Eval mapped bound
        let r = GetVectorResult (mapped,bound)
        Assert.AreEqual("File2.fs.Suffix1",r.[1])

        printf "-[Step3]----------------------------------------------------------------------------------------\n"
        // Evaluate a third time with timestamps updated. Should cause a rebuild
        while !stampAs = DateTime.Now do 
            System.Threading.Thread.Sleep 10 // Sleep a little to avoid grabbing the same 'Now'
        stampAs:=DateTime.Now
        let bound = Eval mapped bound
        let r = GetVectorResult (mapped,bound)
        Assert.AreEqual("File2.fs.Suffix2",r.[1])
            
    /// Test that stamp works
    [<Test>]
    member public rb.StampDemultiplex() =
        
        let joinedResult = ref "Join1"
        let Join(filenames:_[]) = 
            !joinedResult
            
        let stampAs = ref DateTime.Now
        let StampFile(filename) = 
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
        let bound = Eval joined bound
        let (r,_) = Option.get (GetScalarResult<string>(joined,bound))
        Assert.AreEqual("Join1",r)
            
        printf "-[Step2]----------------------------------------------------------------------------------------\n"
        // Evaluate the second time. No change should be seen.
        joinedResult:="Join2"
        let bound = Eval joined bound
        let (r,_) = Option.get (GetScalarResult (joined,bound))
        Assert.AreEqual("Join1",r)

        printf "-[Step3]----------------------------------------------------------------------------------------\n"
        // Evaluate a third time with timestamps updated. Should cause a rebuild
        while !stampAs = DateTime.Now do 
            System.Threading.Thread.Sleep 10 // Sleep a little to avoid grabbing the same 'Now'
        stampAs:=DateTime.Now
        let bound = Eval joined bound
        let (r,_) = Option.get (GetScalarResult (joined,bound))
        Assert.AreEqual("Join2",r)
            

    /// Test that Demultiplex followed by ScanLeft works
    [<Test>]
    member public rb.DemultiplexScanLeft() =
        let Size(ar:_[]) = ar.Length
        let Scan acc (file :string) = eventually { return acc + file.Length }
        let buildDesc = new BuildDescriptionScope()
        let inVector = InputVector<string> "InputVector"
        let vectorSize = Vector.Demultiplex "Demultiplex" Size inVector
        let scanned = Vector.ScanLeft "Scan" Scan vectorSize inVector
        buildDesc.DeclareScalarOutput vectorSize
        buildDesc.DeclareVectorOutput scanned
        let inputs = [ BuildInput.VectorInput(inVector, ["File1.fs";"File2.fs";"File3.fs"]) ]
        let bound = buildDesc.GetInitialPartialBuild inputs
            
        let e = Eval scanned bound   
        let r = GetScalarResult (vectorSize,e)  
        match r with 
        | Some(r,_) -> Assert.AreEqual(3,r)
        | None -> Assert.Fail("No size was returned")       
            
            
    (*
    /// Test that Scalar.Multiplex works.
    [<Test>] 
    member public rb.ScalarMultiplex() =
        let MultiplexScalar inp = [|inp+":1";inp+":2";inp+":3"|]
        
        let buildDesc = new BuildDescriptionScope()
        let inScalar = InputScalar<string> "Scalar"
        let result = Scalar.Multiplex "MultiplexScalar" MultiplexScalar inScalar
        buildDesc.DeclareVectorOutput result 
            
        let b = buildDesc.GetInitialPartialBuild([],["Scalar",box "A Scalar Value"])
        let e = Eval result  b
        let r = GetVectorResult(result,e)
        Assert.AreEqual("A Scalar Value:2", r.[1])
    
            
    /// Test that Scalar.Map works.
    [<Test>] 
    member public rb.ScalarMap() =
        let MapScalar inp = "out:"+inp
        
        let buildDesc = new BuildDescriptionScope()
        let inScalar = InputScalar<string> "Scalar"
        let result  = Scalar.Map "MapScalar" MapScalar inScalar
        buildDesc.DeclareScalarOutput  result 
            
        let inputs = [ BuildInput.ScalarInput(inScalar, "A Scalar Value") ]
        let bound = buildDesc.GetInitialPartialBuild inputs

        let b = buildDesc.GetInitialPartialBuild([],["Scalar",box "A Scalar Value"])
        let e = Eval result bound
        let r = GetScalarResult(result,e)
        match r with 
            | Some(r,_) -> Assert.AreEqual("out:A Scalar Value", r)
            | None -> Assert.Fail()                 
    *)

    /// Test that a simple scalar action works.
    [<Test>] 
    member public rb.Scalar() =
        let buildDesc = new BuildDescriptionScope()
        let inScalar = InputScalar<string> "Scalar"
        buildDesc.DeclareScalarOutput  inScalar
        let inputs = [ BuildInput.ScalarInput(inScalar, "A Scalar Value") ]
        let bound = buildDesc.GetInitialPartialBuild inputs

        let e = Eval inScalar bound
        let r = GetScalarResult(inScalar,e)
        match r with 
            | Some(r,_) -> Assert.AreEqual("A Scalar Value", r)
            | None -> Assert.Fail()
            
    /// Test that ScanLeft works.
    [<Test>]
    member public rb.ScanLeft() =
        let DoIt (a:int*string) (b:string) =
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
        let e = Eval result bound
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

        let e = Eval result bound
        let r = GetScalarResult (result, e)
        match r with 
        | Some(r,ts)->
            if "File3.fs"<>(r.[2]) then
                printf "Got %A\n" (r.[2])
                Assert.Fail()
        | None -> Assert.Fail()

             
            
    /// This test replicates the data flow of the assembly reference model. It includes several concepts 
    /// that were new at the time: Scalars, Invalidation, Disposal
    [<Test>]
    member public rb.AssemblyReferenceModel() =
        let ParseTask(filename) = sprintf "Parse(%s)" filename
        let now = System.DateTime.Now
        let StampFileNameTask filename = now 
        let TimestampReferencedAssemblyTask reference = now
        let ApplyMetaCommands(parseResults:string[]) = "tcConfig-of("+String.Join(",",parseResults)+")"
        let GetReferencedAssemblyNames(tcConfig) = [|"Assembly1.dll";"Assembly2.dll";"Assembly3.dll"|]
        let ReadAssembly(assemblyName) = sprintf "tcImport-of(%s)" assemblyName
        let CombineImportedAssembliesTask(imports) = "tcAcc"
        let TypeCheckTask tcAcc parseResults = eventually { return tcAcc }
        let FinalizeTypeCheckTask results = "finalized"

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
        let e = Eval finalizedTypeCheckNode bound
        let r = GetScalarResult(finalizedTypeCheckNode,e)
            
        ()

    [<Test>]
    member public rb.OneToOneWorks() =
        let VectorModify (input:int) : string =
            sprintf "Transformation of %d" input

        let buildDesc = new BuildDescriptionScope()
        let inputs = InputVector<int> "Inputs"
        let outputs = Vector.Map "Modify" VectorModify inputs
        buildDesc.DeclareVectorOutput outputs
        let inputs = [ BuildInput.VectorInput(inputs, [1;2;3;4]) ]
        let bound = buildDesc.GetInitialPartialBuild inputs

        let evaled = Eval outputs bound
        let outputs = GetVectorResult(outputs,evaled)
        Assert.AreEqual("Transformation of 4", outputs.[3])
        ()   
            
    /// In this bug, the desired output is between other outputs.
    /// The getExprById function couldn't find it.            
    [<Test>]
    member public rb.HiddenOutputGroup() =
        let VectorModify (input:int) : string =
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

        let evaled = Eval outputs bound
        let outputs = GetVectorResult(outputs,evaled)
        Assert.AreEqual("Transformation of 4", outputs.[3])
        ()               
            
    /// Empty build should just be a NOP.
    [<Test>]
    member public rb.EmptyBuildIsNop() =
        let VectorModify (input:int) : string =
            sprintf "Transformation of %d" input

        let buildDesc = new BuildDescriptionScope()
        let inputs = InputVector<int> "Inputs"
        let outputs = Vector.Map "Modify" VectorModify inputs
        buildDesc.DeclareVectorOutput outputs
        let inputs = [ BuildInput.VectorInput(inputs, []) ]
        let bound = buildDesc.GetInitialPartialBuild inputs

        let evaled = Eval outputs  bound
        let outputs = GetVectorResult(outputs,evaled)
        ()               
              
