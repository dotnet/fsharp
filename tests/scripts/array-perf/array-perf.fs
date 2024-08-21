/// BenchmarkDotNet Notes:
/// Docs/GitHub: https://github.com/PerfDotNet/BenchmarkDotNet#getting-started
///
/// This benchmarking suite will perform JIT warmups, collect system environment data
/// run multiple trials, and produce convenient reports.  
/// You will find csv, markdown, and html versions of the reports in .\BenchmarkDotNet.Artifacts\results
/// after running the tests.
///
/// Be sure to run tests in Release mode, optimizations on, etc.


module Program

open System
open System.Threading.Tasks

/// BenchmarkDotNet on Nuget at:
/// https://www.nuget.org/packages/BenchmarkDotNet/
/// https://www.nuget.org/packages/BenchmarkDotNet.Diagnostics.Windows/
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkDotNet.Configs
open BenchmarkDotNet.Jobs

#if MONO
#else
open BenchmarkDotNet.Diagnostics.Windows
#endif
                
/// When pulling functions out of the complete fsharp solution to test
/// Some things are not easily available, such as Array.zeroCreateUnchecked
/// We mock these here using their checked variations
module Array =
    let inline zeroCreateUnchecked (count:int) = 
        Array.zeroCreate count

    let inline subUnchecked startIndex count (array : 'T[]) =
        Array.sub array startIndex count

// Almost every array function calls this, so mock it with 
// the exact same code
let inline checkNonNull argName arg = 
            match box arg with 
            | null -> nullArg argName 
            | _ -> ()


/// Here you can add the functions you want to test against.
/// Below I use Parallel.Partition as an example


// Original partition
let partition predicate (array : 'T[]) =
                checkNonNull "array" array
                let inputLength = array.Length
                let lastInputIndex = inputLength - 1

                let isTrue = Array.zeroCreateUnchecked inputLength
                Parallel.For(0, inputLength, 
                    fun i -> isTrue.[i] <- predicate array.[i]
                    ) |> ignore
                
                let mutable trueLength = 0
                for i in 0 .. lastInputIndex do
                    if isTrue.[i] then trueLength <- trueLength + 1
                
                let trueResult = Array.zeroCreateUnchecked trueLength
                let falseResult = Array.zeroCreateUnchecked (inputLength - trueLength)
                let mutable iTrue = 0
                let mutable iFalse = 0
                for i = 0 to lastInputIndex do
                    if isTrue.[i] then
                        trueResult.[iTrue] <- array.[i]
                        iTrue <- iTrue + 1
                    else
                        falseResult.[iFalse] <- array.[i]
                        iFalse <- iFalse + 1

                (trueResult, falseResult)

// New variation
let partitionNew predicate (array : 'T[]) =
                
                checkNonNull "array" array
                let inputLength = array.Length                
                let isTrue = Array.zeroCreateUnchecked inputLength                
                let mutable trueLength = 0
                                                
                Parallel.For(0, 
                             inputLength, 
                             (fun () -> 0),
                             (fun i _ trueCount -> 
                                if predicate array.[i] then
                                    isTrue.[i] <- true
                                    trueCount + 1
                                else
                                    trueCount),                        
                             Action<int> (fun x -> System.Threading.Interlocked.Add(&trueLength,x) |> ignore) ) |> ignore
                                
                let res1 = Array.zeroCreateUnchecked trueLength
                let res2 = Array.zeroCreateUnchecked (inputLength - trueLength)
                let mutable iTrue = 0
                let mutable iFalse = 0
                for i = 0 to isTrue.Length-1 do
                    if isTrue.[i] then
                        res1.[iTrue] <- array.[i]
                        iTrue <- iTrue + 1
                    else
                        res2.[iFalse] <- array.[i]
                        iFalse <- iFalse + 1

                res1, res2




/// Configuration for a given benchmark
type ArrayPerfConfig () =
    inherit ManualConfig()
    do 
        base.Add Job.RyuJitX64
        //base.Add Job.LegacyJitX86 // If you want to also test 32bit. It will run tests on both if both of these are here.
        #if MONO
        #else
        base.Add(new MemoryDiagnoser())  // To get GC and allocation data
        #endif

[<Config (typeof<ArrayPerfConfig>)>]
type ArrayBenchmark () =    
    let r = Random()

    let mutable array = [||]
    let mutable array2 = [||]
    let mutable array3 = [||]
    


    //When the test runs, it will run once per each param.
    //This is used to run the test on arrays of size 10, 100, etc
    //You can create a 2nd Params section and the test will run over all permutations
    [<Params (10,100,10000,1000000,10000000)>] 
    member val public Length = 0 with get, set

    //This is run before each iteration of a test
    [<Setup>]
    member self.SetupData () =       
             
        // Initialize whatever arrays or other data you want to test on here.
        // Random is often handy but some algorithms you may want to test on 
        // Sorted or other structured data. Ints/doubles/objects etc
        // Create however many you need.  Setup time is not included in 
        // runtime statistics, however allocations may be, due to a bug 
        // as of this comment.

        array <- Array.init self.Length (fun i -> int(r.NextDouble()*1000.0))
        //array2 <- Array.create self.Length 5
        
    
    
    
    // If you set a benchmark as baseline, the results output will add a 
    // column showing runtime of all other benchmarks as a percentage of the baseline
    // care must be taken here to be sure your benchmark is not JITTed into nothingness
    // because it is identified as dead code.      
    [<Benchmark(Baseline=true)>]
    member self.Original () =
        array |> partition (fun x -> x % 2 = 0) 

    [<Benchmark>]
    member self.New () =
        array |> partitionNew (fun x -> x % 2 = 0)
        
    //Create any number of benchmarks                           
                                       
   

    

[<EntryPoint>]
let main argv =              
  
    // Run the executable with the argument "ArrayPerfBenchmark" 
    
    let switch = 
        BenchmarkSwitcher [|
            typeof<ArrayPerfBenchmark>
        |]

    switch.Run argv |> ignore
    0

