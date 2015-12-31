// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace UnitTests.Tests.LanguageService

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
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open IncrementalBuild
    
/// Useful methods that someday might go into IncrementalBuild
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
    [<Test>]
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

        let build = new BuildDescriptionScope()
        let input = InputVector<string> "InputVector"
        let stamped = Vector.Stamp "Stamp" StampFile input
        let mapped = Vector.Map "Map" Map stamped
        build.DeclareVectorOutput("Mapped", mapped)
        let build = build.GetInitialPartialBuild(["InputVector",1,[box path]],[])

        let DoCertainStep build = 
            match IncrementalBuild.Step "Mapped" build with
            | Some(build) -> build
            | None -> failwith "Expected to be able to step"

        // While updateStamp is true we should be able to step as continuously
        // because there will always be more to build.
        let mutable build = build
        for i in 0..5 do 
            printfn "Iteration %d" i
            build <- DoCertainStep build
            System.Threading.Thread.Sleep 2000

        // Now, turn off updateStamp and the build should just finish.
        updateStamp:=false
        build <- DoCertainStep build
        build <- DoCertainStep build
        match IncrementalBuild.Step "Mapped" build with
        | Some(build) -> failwith "Build should have stopped"
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
                            
        let build = new BuildDescriptionScope()
        let input = InputVector<string> "InputVector"
        let acc = InputScalar<string> "Accumulator"
        let stamped = Vector.Stamp "Stamp" StampFile input
        let scanned = Vector.ScanLeft "Scan" Scan acc stamped
        build.DeclareVectorOutput("Scanned", scanned)
        let build = build.GetInitialPartialBuild(["InputVector",3,[box "File1.fs";box "File2.fs";box "File3.fs"]],["Accumulator",box "AccVal"])
            
        printf "-[Step1]----------------------------------------------------------------------------------------\n"
        // Evaluate the first time.
        let build = Eval "Scanned" build
        let r = GetVectorResult<string>("Scanned",build)
        Assert.AreEqual("AccVal-File1.fs-Suffix1-File2.fs-Suffix1",r.[1])
            
        printf "-[Step2]----------------------------------------------------------------------------------------\n"
        // Evaluate the second time. No change should be seen.
        mapSuffix:="Suffix2"
        let build = Eval "Scanned" build
        let r = GetVectorResult<string>("Scanned",build)
        Assert.AreEqual("AccVal-File1.fs-Suffix1-File2.fs-Suffix1",r.[1])

        printf "-[Step3]----------------------------------------------------------------------------------------\n"
        // Evaluate a third time with timestamps updated. Should cause a rebuild
        System.Threading.Thread.Sleep 10 // Sleep a little to avoid grabbing the same 'Now'
        stampAs:=DateTime.Now
        let build = Eval "Scanned" build
        let r = GetVectorResult<string>("Scanned",build)
        Assert.AreEqual("AccVal-File1.fs-Suffix2-File2.fs-Suffix2",r.[1])
             
            
    /// Test case of zero elements in a vector
    [<Test>]
    member public rb.aaZeroElementVector() = // Starts with 'aa' to put it at the front.
        let stamp = ref DateTime.Now
        let Mult(i:int) : string array = Array.create i ""
        let Stamp(s:string) = !stamp
        let Map(s:string) = s
        let Demult(a:string array) : int = a.Length
            
        let build = new BuildDescriptionScope()
        let input = InputScalar<int> "InputScalar"
        let multiplexed = Scalar.Multiplex "Mult" Mult input
        let stamped = Vector.Stamp "Stamp" Stamp multiplexed
        let mapped = Vector.Map "Map" Map stamped
        let demultiplexed = Vector.Demultiplex "Demult" Demult mapped
        build.DeclareVectorOutput("Multiplexed", multiplexed)
        build.DeclareVectorOutput("Stamped", stamped)
        build.DeclareVectorOutput("Mapped", mapped)
        build.DeclareScalarOutput("Result", demultiplexed)
            
        // Try first with one input
        let build1 = build.GetInitialPartialBuild([],["InputScalar", box 1])    
        let build1Evaled = Eval "Result" build1
        let r1 = GetScalarResult<int>("Result",build1Evaled)
        match r1 with
        | Some(v,dt) -> Assert.AreEqual(1,v) 
        | None -> failwith "Expected the value 1 to be returned."
            
        // Now with zero. This was the original bug.
        stamp := DateTime.Now
        let build0 = build.GetInitialPartialBuild([],["InputScalar", box 0])            
        let build0Evaled = Eval "Result" build0
        let r0 = GetScalarResult<int>("Result",build0Evaled)
        match r0 with
        | Some(v,dt) -> Assert.AreEqual(0,v) 
        | None -> failwith "Expected the value 0 to be returned."  
        ()
         
            
    /// Here, we want a multiplex to increase the number of items processed.
    [<Test>]
    member public rb.MultiplexTransitionUp() =
        let elements = ref 1
        let timestamp = ref System.DateTime.Now
        let Mult(s:string) : string array =  [| for i in 1..!elements -> sprintf "Element %d" i |]
        let Stamp(s) = !timestamp
        let Map(s:string) = sprintf "Mapped %s " s
        let Demult(a:string array) : string = "Demult"
        let Result(a:string array) : string = String.Join(",", a)
        let now = System.DateTime.Now
        let FixedTimestamp _  =  now
            
        let build = new BuildDescriptionScope()
        let input = InputVector<string> "InputVector"
        let stampedInput = Vector.Stamp "StampInput" Stamp input
        let demultiplexedInput = Vector.Demultiplex "DemultInput" Demult stampedInput
        let multiplexed = Scalar.Multiplex "Mult" Mult demultiplexedInput
        let mapped = Vector.Map "Map" Map multiplexed
        let mapped = Vector.Stamp "FixedTime" FixedTimestamp mapped // Change in vector size should x-ray through even if timestamps haven't changed in remaining items.
        let demultiplexed = Vector.Demultiplex "DemultResult" Result mapped
        build.DeclareScalarOutput("Result", demultiplexed)
            
        // Create the build.
        let build = build.GetInitialPartialBuild(["InputVector",1,[box "Input 0"]],[])         
            
        // Evaluate it with value 1
        elements := 1
        let build = Eval "Result" build
        let r1 = GetScalarResult<string>("Result", build)
        match r1 with
        | Some(s,dt) -> printfn "%s" s
        | None -> failwith ""
            
        // Now, re-evaluate it with value 2
        elements := 2
        System.Threading.Thread.Sleep(100)
        timestamp := System.DateTime.Now
            
            
            
        let build = Eval "Result" build
        let r2 = GetScalarResult<string>("Result", build)
        match r2 with
        | Some(s,dt) -> Assert.AreEqual("Mapped Element 1 ,Mapped Element 2 ",s)
        | None -> failwith ""
            
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
            
        let build = new BuildDescriptionScope()
        let input = InputVector<string> "InputVector"
        let stampedInput = Vector.Stamp "StampInput" Stamp input
        let demultiplexedInput = Vector.Demultiplex "DemultInput" Demult stampedInput
        let multiplexed = Scalar.Multiplex "Mult" Mult demultiplexedInput
        let mapped = Vector.Map "Map" Map multiplexed
        let fixedmapped = Vector.Stamp "FixedTime" FixedTimestamp mapped // Change in vector size should x-ray through even if timestamps haven't changed in remaining items.
        let demultiplexed = Vector.Demultiplex "DemultResult" Result fixedmapped
            
        build.DeclareScalarOutput("DemultiplexedInput", demultiplexedInput)
        build.DeclareVectorOutput("Mapped", mapped)
        build.DeclareVectorOutput("FixedMapped", fixedmapped)            
        build.DeclareScalarOutput("Result", demultiplexed)
            
        // Create the build.
        let build = build.GetInitialPartialBuild(["InputVector",1,[box "Input 0"]],[])         
            
        // Evaluate it with value 2
        elements := 2
        let build = Eval "Result" build
        let r1 = GetScalarResult<string>("Result", build)
        match r1 with
        | Some(s,dt) -> printfn "%s" s
        | None -> failwith ""
            
        // Now, re-evaluate it with value 1
        elements := 1
        System.Threading.Thread.Sleep(100)
        timestamp := System.DateTime.Now
            
        let buildDemuxed = Eval "DemultiplexedInput" build
        let rdm = GetScalarResult<string>("DemultiplexedInput",buildDemuxed)
        match rdm with
        | Some(s,dt)->Assert.AreEqual("Demult Input 0", s)
        | None -> failwith "unexpected"
            
        let buildMapped = Eval "Mapped" build
        let mp = GetVectorResult<string>("Mapped",buildMapped)
        Assert.AreEqual(1,mp.Length)
        let melem = mp.[0]
        Assert.AreEqual("Mapped Element 1 ", melem)
            
        let buildFixedMapped = Eval "FixedMapped" buildMapped
        let mp = GetVectorResult<string>("FixedMapped",buildFixedMapped)
        Assert.AreEqual(1,mp.Length)
        let melem = mp.[0]
        Assert.AreEqual("Mapped Element 1 ", melem)            
            
        let build = Eval "Result" build
        let r2 = GetScalarResult<string>("Result", build)
        match r2 with
        | Some(s,dt) -> Assert.AreEqual("Mapped Element 1 ",s)
        | None -> failwith "unexpected"
            
    /// Test that stamp works
    [<Test>]
    member public rb.StampMap() =
        
        let mapSuffix = ref "Suffix1"
        let MapIt(filename) = 
            filename+"."+(!mapSuffix)
            
        let stampAs = ref DateTime.Now
        let StampFile(filename) =  
            !stampAs
                            
        let build = new BuildDescriptionScope()
        let input = InputVector<string> "InputVector"
        let stamped = Vector.Stamp "Stamp" StampFile input
        let mapped = Vector.Map "Map" MapIt stamped
        build.DeclareVectorOutput("Mapped", mapped)
        let build = build.GetInitialPartialBuild(["InputVector",3,[box "File1.fs";box "File2.fs";box "File3.fs"]],[])
            
        printf "-[Step1]----------------------------------------------------------------------------------------\n"
        // Evaluate the first time.
        let build = Eval "Mapped" build
        let r = GetVectorResult<string>("Mapped",build)
        Assert.AreEqual("File2.fs.Suffix1",r.[1])
            
        printf "-[Step2]----------------------------------------------------------------------------------------\n"
        // Evaluate the second time. No change should be seen.
        mapSuffix:="Suffix2"
        let build = Eval "Mapped" build
        let r = GetVectorResult<string>("Mapped",build)
        Assert.AreEqual("File2.fs.Suffix1",r.[1])

        printf "-[Step3]----------------------------------------------------------------------------------------\n"
        // Evaluate a third time with timestamps updated. Should cause a rebuild
        while !stampAs = DateTime.Now do 
            System.Threading.Thread.Sleep 10 // Sleep a little to avoid grabbing the same 'Now'
        stampAs:=DateTime.Now
        let build = Eval "Mapped" build
        let r = GetVectorResult<string>("Mapped",build)
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
                            
        let build = new BuildDescriptionScope()
        let input = InputVector<string> "InputVector"
        let stamped = Vector.Stamp "Stamp" StampFile input
        let joined = Vector.Demultiplex "Demultiplex" Join stamped
        build.DeclareScalarOutput("Joined", joined)
        let build = build.GetInitialPartialBuild(["InputVector",3,[box "File1.fs";box "File2.fs";box "File3.fs"]],[])
            
        printf "-[Step1]----------------------------------------------------------------------------------------\n"
        // Evaluate the first time.
        let build = Eval "Joined" build
        let (r,_) = Option.get (GetScalarResult<string>("Joined",build))
        Assert.AreEqual("Join1",r)
            
        printf "-[Step2]----------------------------------------------------------------------------------------\n"
        // Evaluate the second time. No change should be seen.
        joinedResult:="Join2"
        let build = Eval "Joined" build
        let (r,_) = Option.get (GetScalarResult<string>("Joined",build))
        Assert.AreEqual("Join1",r)

        printf "-[Step3]----------------------------------------------------------------------------------------\n"
        // Evaluate a third time with timestamps updated. Should cause a rebuild
        while !stampAs = DateTime.Now do 
            System.Threading.Thread.Sleep 10 // Sleep a little to avoid grabbing the same 'Now'
        stampAs:=DateTime.Now
        let build = Eval "Joined" build
        let (r,_) = Option.get (GetScalarResult<string>("Joined",build))
        Assert.AreEqual("Join2",r)
            

    /// Test that Demultiplex followed by ScanLeft works
    [<Test>]
    member public rb.DemultiplexScanLeft() =
        let Size(ar:_[]) = ar.Length
        let Scan acc (file :string) = eventually { return acc + file.Length }
        let build = new BuildDescriptionScope()
        let inVector = InputVector<string> "InputVector"
        let vectorSize = Vector.Demultiplex "Demultiplex" Size inVector
        let scanned = Vector.ScanLeft "Scan" Scan vectorSize inVector
        build.DeclareScalarOutput("Size", vectorSize)
        build.DeclareVectorOutput("Scanned", scanned)
        let build = build.GetInitialPartialBuild(["InputVector",3,[box "File1.fs";box "File2.fs";box "File3.fs"]],[])
            
        let e = Eval "Scanned" build   
        let r = GetScalarResult<int>("Size",e)  
        match r with 
        | Some(r,_)->Assert.AreEqual(3,r)
        | None -> Assert.Fail("No size was returned")       
            
            
    /// Test that Scalar.Multiplex works.
    [<Test>] 
    member public rb.ScalarMultiplex() =
        let MultiplexScalar inp = [|inp+":1";inp+":2";inp+":3"|]
        
        let build = new BuildDescriptionScope()
        let inScalar = InputScalar<string> "Scalar"
        let multiplexed = Scalar.Multiplex "MultiplexScalar" MultiplexScalar inScalar
        build.DeclareVectorOutput("Output", multiplexed)
            
        let b = build.GetInitialPartialBuild([],["Scalar",box "A Scalar Value"])
        let e = Eval "Output" b
        let r = GetVectorResult("Output",e)
        Assert.AreEqual("A Scalar Value:2", r.[1])
            
    /// Test that Scalar.Map works.
    [<Test>] 
    member public rb.ScalarMap() =
        let MapScalar inp = "out:"+inp
        
        let build = new BuildDescriptionScope()
        let inScalar = InputScalar<string> "Scalar"
        let mappedScalar = Scalar.Map "MapScalar" MapScalar inScalar
        build.DeclareScalarOutput("Output", mappedScalar)
            
        let b = build.GetInitialPartialBuild([],["Scalar",box "A Scalar Value"])
        let e = Eval "Output" b
        let r = GetScalarResult("Output",e)
        match r with 
            | Some(r,_) -> Assert.AreEqual("out:A Scalar Value", r)
            | None -> Assert.Fail()                 
    
    /// Test that a simple scalar action works.
    [<Test>] 
    member public rb.Scalar() =
        let build = new BuildDescriptionScope()
        let inScalar = InputScalar<string> "Scalar"
        build.DeclareScalarOutput("Output", inScalar)
        let b = build.GetInitialPartialBuild([],["Scalar",box "A Scalar Value"])
        let e = Eval "Output" b
        let r = GetScalarResult("Output",e)
        match r with 
            | Some(r,_) -> Assert.AreEqual("A Scalar Value", r)
            | None -> Assert.Fail()
            
    /// Test that ScanLeft works.
    [<Test>]
    member public rb.ScanLeft() =
        let DoIt (a:int*string) (b:string) =
            eventually { return ((fst a)+1,b) }
            
        let build = new BuildDescriptionScope()
        let inScalar = InputScalar<int*string> "InputScalar"
        let inVector = InputVector<string> "InputVector"
        let scanned = Vector.ScanLeft "DoIt" DoIt inScalar inVector
        build.DeclareVectorOutput("Output", scanned)
            
        let build = build.GetInitialPartialBuild(["InputVector",3,[box "File1.fs";box "File2.fs";box "File3.fs"]],["InputScalar",box (5,"")])
        let e = Eval "Output" build
        let r = GetVectorResult("Output",e)
        if [| (6,"File1.fs"); (7,"File2.fs"); (8, "File3.fs") |] <> r then 
            printfn "Got %A" r
            Assert.Fail()
        ()     
            
    /// Convert a vector to a scalar
    [<Test>]
    member public rb.ToScalar() =
        let build = new BuildDescriptionScope()
        let inVector = InputVector<string> "InputVector"
        let asScalar = Vector.ToScalar "ToScalar" inVector
        build.DeclareScalarOutput("Output", asScalar)
        let build = build.GetInitialPartialBuild(["InputVector",3,[box "File1.fs";box "File2.fs";box "File3.fs"]],[])
        let e = Eval "Output" build
        let r = GetScalarResult<string array>("Output",e)
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
        let Parse(filename) = sprintf "Parse(%s)" filename
        let ApplyMetaCommands(parseResults:string[]) = "tcConfig-of("+String.Join(",",parseResults)+")"
        let GetReferencedAssemblyNames(tcConfig) = [|"Assembly1.dll";"Assembly2.dll";"Assembly3.dll"|]
        let ReadAssembly(assemblyName) = sprintf "tcImport-of(%s)" assemblyName
        let CombineImports(imports) = "tcAcc"
        let TypeCheck tcAcc parseResults = eventually { return tcAcc }

        // Build rules.
        let build = new BuildDescriptionScope()
        let filenames = InputVector<string> "Filenames"
        let parseTrees = Vector.Map "Parse" Parse filenames
        let parseTreesAsScalar = Vector.ToScalar<string> "ScalarizeParseTrees" parseTrees
        let tcConfig = Scalar.Map "ApplyMetaCommands" ApplyMetaCommands parseTreesAsScalar
        let referencedAssemblyNames = Scalar.Multiplex "GetReferencedAssemblyNames" GetReferencedAssemblyNames tcConfig
        let readAssemblies = Vector.Map "ReadAssembly" ReadAssembly referencedAssemblyNames
        let tcAcc = Vector.Demultiplex "CombineImports" CombineImports readAssemblies
        let tcResults = Vector.ScanLeft "TypeCheck" TypeCheck tcAcc parseTrees
        build.DeclareVectorOutput("TypeCheckingStates",tcResults)
            
        let build = build.GetInitialPartialBuild(["Filenames",3,[box "File1.fs";box "File2.fs";box "File3.fs"]],[])
        let e = Eval "TypeCheckingStates" build
        let r = GetVectorResult("TypeCheckingStates",e)
            
        ()

#if NUNIT_V2
    [<Test;ExpectedException(typeof<Exception>)>]
    member public rb.DuplicateExpressionNamesNotAllowed() =
            let DoIt s = s
            ()
#else
    [<Test>]
    member public rb.DuplicateExpressionNamesNotAllowed() =
            Assert.That((fun () -> let DoIt s = s
                                   let b = 
                                       let build = new BuildDescriptionScope()
                                       let i = InputVector<string> "Input"
                                       let r = Vector.Map "Input" DoIt i
                                       build.DeclareVectorOutput("Output",r)
                                       build.GetInitialPartialBuild(["Input",1,[box ""]],[])

                                   let e = Eval "Output" b
                                   ()), NUnit.Framework.Throws.TypeOf(typeof<Exception>))
#endif

    [<Test>]
    member public rb.OneToOneWorks() =
        let VectorModify (input:int) : string =
            sprintf "Transformation of %d" input

        let bound = 
            let build = new BuildDescriptionScope()
            let inputs = InputVector<int> "Inputs"
            let outputs = Vector.Map "Modify" VectorModify inputs
            build.DeclareVectorOutput("Outputs",outputs)
            build.GetInitialPartialBuild(["Inputs",4,[box 1;box 2;box 3;box 4]],[])

        let evaled = bound |> Eval "Outputs" 
        let outputs = GetVectorResult("Outputs",evaled)
        Assert.AreEqual("Transformation of 4", outputs.[3])
        ()   
            
    /// In this bug, the desired output is between other outputs.
    /// The getExprById function couldn't find it.            
    [<Test>]
    member public rb.HiddenOutputGroup() =
        let VectorModify (input:int) : string =
            sprintf "Transformation of %d" input

        let bound = 
            let build = new BuildDescriptionScope()
            let inputs = InputVector<int> "Inputs"
            let outputs = Vector.Map "Modify" VectorModify inputs
            build.DeclareVectorOutput("Inputs1", inputs)
            build.DeclareVectorOutput("Inputs2", inputs)
            build.DeclareVectorOutput("Inputs3", inputs)
            build.DeclareVectorOutput("Outputs", outputs)
            build.DeclareVectorOutput("Inputs4", inputs)
            build.DeclareVectorOutput("Inputs5", inputs)
            build.DeclareVectorOutput("Inputs6", inputs)
            build.GetInitialPartialBuild(["Inputs",4,[box 1;box 2;box 3;box 4]],[])

        let evaled = bound |> Eval "Outputs" 
        let outputs = GetVectorResult("Outputs",evaled)
        Assert.AreEqual("Transformation of 4", outputs.[3])
        ()               
            
    /// Empty build should just be a NOP.
    [<Test>]
    member public rb.EmptyBuildIsNop() =
        let VectorModify (input:int) : string =
            sprintf "Transformation of %d" input

        let bound = 
            let build = new BuildDescriptionScope()
            let inputs = InputVector<int> "Inputs"
            let outputs = Vector.Map "Modify" VectorModify inputs
            build.DeclareVectorOutput("Outputs", outputs)
            build.GetInitialPartialBuild(["Inputs",0,[]],[])

        let evaled = bound |> Eval "Outputs" 
        let outputs = GetVectorResult("Outputs",evaled)
        ()               
              
