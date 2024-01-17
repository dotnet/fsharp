module FSharp.Benchmarks.ComputationExpressionBenchmarks

open System.IO
open BenchmarkDotNet.Attributes
open FSharp.Compiler.CodeAnalysis
open FSharp.Test.ProjectGeneration

[<Literal>]
let FSharpCategory = "fsharp"

[<MemoryDiagnoser>]
[<BenchmarkCategory(FSharpCategory)>]
type ComputationExpressionBenchmarks() =

    let oneHundred_a_nesting_of_1 = File.ReadAllText(__SOURCE_DIRECTORY__ ++ "CE_100x_nesting_of_1.fs")
    let oneHundred_a_nesting_of_5 = File.ReadAllText(__SOURCE_DIRECTORY__ ++ "CE_100x_nesting_of_5.fs")
    let twoHundred_a_nesting_of_5 = File.ReadAllText(__SOURCE_DIRECTORY__ ++ "CE_200x_nesting_of_5.fs")
    let oneHundred_a_nesting_of_10 = File.ReadAllText(__SOURCE_DIRECTORY__ ++ "CE_100x_nesting_of_10.fs")
    let nesting1WithCustOps = File.ReadAllText(__SOURCE_DIRECTORY__ ++ "CE_with_CustOp_nesting1.fs")
    let nesting5WithCustOps = File.ReadAllText(__SOURCE_DIRECTORY__ ++ "CE_with_CustOp_nesting5.fs")

    member val Benchmark = Unchecked.defaultof<_> with get, set

    member this.setup(project) =
        let checker = FSharpChecker.Create()
        this.Benchmark <- ProjectWorkflowBuilder(project, checker = checker).CreateBenchmarkBuilder()
        saveProject project false checker |> Async.RunSynchronously

    member this.SetupWithSource (source) =
        this.setup
            { SyntheticProject.Create() with
                SourceFiles =
                    [

                        { sourceFile "File" [] with
                            ExtraSource = source
                        }
                    ]
                OtherOptions = []
            }
    
    [<GlobalSetup(Target = "Check100xNestingOf1")>]
    member this.Check100xNestingOf1_Setup() = this.SetupWithSource (oneHundred_a_nesting_of_1)
    
    [<GlobalSetup(Target = "Check100xNestingOf5")>]
    member this.Check100xNestingOf5_Setup() = this.SetupWithSource (oneHundred_a_nesting_of_5)    
    
    [<GlobalSetup(Target = "Check200xNestingOf5")>]
    member this.Check200xNestingOf5_Setup() = this.SetupWithSource (twoHundred_a_nesting_of_5)    
    
    // [<GlobalSetup(Target = "Check100xNestingOf10")>]
    // member this.Check100xNestingOf10_Setup() = this.SetupWithSource (oneHundred_a_nesting_of_10)
    
    [<GlobalSetup(Target = "CheckNesting1WithCustOps")>]
    member this.CheckNesting1WithCustOps_Setup() = this.SetupWithSource (nesting1WithCustOps)
    
    [<GlobalSetup(Target = "CheckNesting5WithCustOps")>]
    member this.CheckNesting5WithCustOps_Setup() = this.SetupWithSource (nesting5WithCustOps)

    [<GlobalSetup(Target = "Compile100xNestingOf1")>]
    member this.Compile100xNestingOf1_Setup() = this.SetupWithSource (oneHundred_a_nesting_of_1)

    [<GlobalSetup(Target = "Compile100xNestingOf5")>]
    member this.Compile100xNestingOf5_Setup() = this.SetupWithSource (oneHundred_a_nesting_of_5)
    
    [<GlobalSetup(Target = "Compile200xNestingOf5")>]
    member this.Compile200xNestingOf5_Setup() = this.SetupWithSource (oneHundred_a_nesting_of_5)
    
    // [<GlobalSetup(Target = "Compile100xNestingOf10")>]
    // member this.Compile100xNestingOf10_Setup() = this.SetupWithSource (oneHundred_a_nesting_of_10)
    
    [<GlobalSetup(Target = "CompileNesting1WithCustOps")>]
    member this.CompileNesting1WithCustOps_Setup() = this.SetupWithSource (nesting1WithCustOps)

    [<GlobalSetup(Target = "CompileNesting5WithCustOps")>]
    member this.CompileNesting5WithCustOps_Setup() = this.SetupWithSource (nesting5WithCustOps)

    [<Benchmark>]
    member this.Check100xNestingOf1() = this.Benchmark { checkFile "File" expectOk }
    
    [<Benchmark>]
    member this.Check100xNestingOf5() = this.Benchmark { checkFile "File" expectOk }
    
    [<Benchmark>]
    member this.Check200xNestingOf5() = this.Benchmark { checkFile "File" expectOk }
    
    // [<Benchmark>]
    // member this.Check100xNestingOf10() = this.Benchmark { checkFile "File" expectOk }
    
    [<Benchmark>]
    member this.CheckNesting1WithCustOps() = this.Benchmark { checkFile "File" expectOk }    
    
    [<Benchmark>]
    member this.CheckNesting5WithCustOps() = this.Benchmark { checkFile "File" expectOk }
    
    [<Benchmark>]
    member this.Compile100xNestingOf1() = this.Benchmark { compileWithFSC }
    
    [<Benchmark>]
    member this.Compile100xNestingOf5() = this.Benchmark { compileWithFSC }

    [<Benchmark>]
    member this.Compile200xNestingOf5() = this.Benchmark { compileWithFSC }
    
    // [<Benchmark>] di
    // member this.Compile100xNestingOf10() = this.Benchmark { compileWithFSC }
    
    [<Benchmark>]
    member this.CompileNesting1WithCustOps() = this.Benchmark { checkFile "File" expectOk }
    
    [<Benchmark>]
    member this.CompileNesting5WithCustOps() = this.Benchmark { checkFile "File" expectOk }
    