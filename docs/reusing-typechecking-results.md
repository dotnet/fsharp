---
title: Reusing typechecking results
category: Compiler Internals
categoryindex: 200
index: 42
---

# Reusing typechecking results between compiler runs

Caching and reusing compilation results between compiler runs will be an optimization technique aimed at improving the performance and efficiency of the F# development experience.

## Motivation

- **Performance**. Things like recomputing type information for large projects are time-consuming. By caching the results, subsequent compilation can skip some steps, leading to faster builds.

- **Better IDE cold start**. IDEs can provide faster IntelliSense, code navigation, and other features if they can quickly access cached compilation information. The bigger and the less coupled the project is, the greater will be the gains.

### Example real-world scenarios

The optimization will benefit the most in large and not very interdependent projects. For instance, a 100-files test project where a single test is changed. MSBuild will mark the project for recompilation but thanks to the improvement only one file would be recompiled in the full-blown manner.

## Premises

Here are some assumptions I am coming with after tinkering with the topic and investigating the current state of the art. 

### Premise 1: current compiler design

The heart of the compiler, [fsc.fs](src\Compiler\Driver\fsc.fs), is split into 6 phases (`main1` - `main6`). The code is designed to pass minimum information between phases, using the `Args` structure, which is essentially a data bag. The first phase takes info from the program arguments.

```fsharp
main1 (...args...)
|> main2
|> main3
|> main4 (tcImportsCapture, dynamicAssemblyCreator)
|> main5
|> main6 dynamicAssemblyCreator
```

### Premise 2: current compilation time consumption

Let's measure (`fsc --times`) the fresh compilation of large F# files. We'll use examples from our benchmarks - those are different but should be good to see general trends.

**SomethingToCompile.fs**
```
--------------------------------------------------------------------------------------------------------
|Phase name                          |Elapsed |Duration| WS(MB)|  GC0  |  GC1  |  GC2  |Handles|Threads|
|------------------------------------|--------|--------|-------|-------|-------|-------|-------|-------|
// main1
|Import mscorlib+FSharp.Core         |  0,2720|  0,2604|     96|      0|      0|      0|    365|     30|     <-- long
|Parse inputs                        |  0,3378|  0,0560|    108|      0|      0|      0|    372|     30|
|Import non-system references        |  0,3932|  0,0476|    127|      0|      0|      0|    372|     30|
|Typecheck                           |  0,9020|  0,5025|    164|      3|      2|      1|    456|     46|     <-- longest
|Typechecked                         |  0,9087|  0,0002|    164|      0|      0|      0|    456|     46|
// main2
|Write Interface File                |  0,9191|  0,0000|    164|      0|      0|      0|    456|     46|
|Write XML doc signatures            |  0,9280|  0,0000|    164|      0|      0|      0|    456|     46|
|Write XML docs                      |  0,9342|  0,0002|    164|      0|      0|      0|    456|     46|
// main3
|Encode Interface Data               |  0,9778|  0,0345|    164|      0|      0|      0|    456|     46|
|Optimizations                       |  1,2312|  0,2463|    178|      2|      2|      1|    456|     46|     <-- long
|Ending Optimizations                |  1,2386|  0,0000|    178|      0|      0|      0|    456|     46|
|Encoding OptData                    |  1,2549|  0,0093|    178|      0|      0|      0|    456|     46|
|TailCall Checks                     |  1,2687|  0,0054|    178|      0|      0|      0|    456|     46|
// main4, main5
|TAST -> IL                          |  1,3657|  0,0883|    180|      0|      0|      0|    456|     46|
// main6
|Write .NET Binary                   |  1,6016|  0,2284|    183|      0|      0|      0|    462|     46|     <-- long
--------------------------------------------------------------------------------------------------------
```

**decentlySizedStandAloneFile.fs**
```
--------------------------------------------------------------------------------------------------------
|Phase name                          |Elapsed |Duration| WS(MB)|  GC0  |  GC1  |  GC2  |Handles|Threads|
|------------------------------------|--------|--------|-------|-------|-------|-------|-------|-------|
// main1
|Import mscorlib+FSharp.Core         |  0,3285|  0,3120|    101|      0|      0|      0|    365|     30|     <-- longest
|Parse inputs                        |  0,3673|  0,0292|    108|      0|      0|      0|    374|     30|
|Import non-system references        |  0,4354|  0,0622|    128|      0|      0|      0|    374|     30|
|Typecheck                           |  0,6464|  0,2045|    144|      1|      1|      1|    378|     30|     <-- long
|Typechecked                         |  0,6522|  0,0004|    144|      0|      0|      0|    378|     30|
// main2
|Write Interface File                |  0,6597|  0,0000|    144|      0|      0|      0|    378|     30|
|Write XML doc signatures            |  0,6661|  0,0000|    144|      0|      0|      0|    378|     30|
|Write XML docs                      |  0,6710|  0,0002|    144|      0|      0|      0|    378|     30|
// main3
|Encode Interface Data               |  0,7273|  0,0503|    154|      0|      0|      0|    378|     30|
|Optimizations                       |  0,8757|  0,1425|    172|      1|      1|      0|    378|     30|     <-- long
|Ending Optimizations                |  0,8815|  0,0000|    172|      0|      0|      0|    378|     30|
|Encoding OptData                    |  0,8899|  0,0024|    173|      0|      0|      0|    378|     30|
|TailCall Checks                     |  0,8990|  0,0025|    173|      0|      0|      0|    378|     30|
// main4, main5
|TAST -> IL                          |  0,9487|  0,0447|    176|      0|      0|      0|    378|     30|
// main6
|Write .NET Binary                   |  1,1530|  0,1972|    180|      0|      0|      0|    384|     30|     <-- long
--------------------------------------------------------------------------------------------------------
```

**CE100xnest5.fs**
```
--------------------------------------------------------------------------------------------------------
|Phase name                          |Elapsed |Duration| WS(MB)|  GC0  |  GC1  |  GC2  |Handles|Threads|
|------------------------------------|--------|--------|-------|-------|-------|-------|-------|-------|
// main1
|Import mscorlib+FSharp.Core         |  0,4092|  0,4084|    101|      0|      0|      0|    365|     30|     <-- long
|Parse inputs                        |  0,4635|  0,0445|    108|      0|      0|      0|    374|     30|
|Import non-system references        |  0,5475|  0,0775|    128|      0|      0|      0|    374|     30|
|Typecheck                           |  3,1157|  2,5612|    198|     18|     15|      3|    712|     45|     <-- long
|Typechecked                         |  3,1219|  0,0002|    198|      0|      0|      0|    712|     45|
// main2
|Write Interface File                |  3,1280|  0,0000|    198|      0|      0|      0|    712|     45|
|Write XML doc signatures            |  3,1363|  0,0000|    198|      0|      0|      0|    712|     45|
|Write XML docs                      |  3,1435|  0,0002|    198|      0|      0|      0|    712|     45|
// main3
|Encode Interface Data               |  3,1803|  0,0296|    198|      0|      0|      0|    712|     45|
|Optimizations                       |  8,9949|  5,8049|    216|     43|     42|      2|    457|     45|     <-- longest
|Ending Optimizations                |  9,0015|  0,0000|    216|      0|      0|      0|    457|     45|
|Encoding OptData                    |  9,0090|  0,0010|    216|      0|      0|      0|    457|     45|
|TailCall Checks                     |  9,0190|  0,0013|    216|      0|      0|      0|    457|     45|
// main4, main5
|TAST -> IL                          |  9,0463|  0,0210|    217|      0|      0|      0|    458|     46|
// main6
|Write .NET Binary                   |  9,2463|  0,1930|    218|      0|      0|      0|    464|     46|     <-- long
--------------------------------------------------------------------------------------------------------
```

The phases taking the longest are marked in the right. Those are assembly import, type check, optimizations and IL writing.

Optimizations are not relevant in the dev loop with run/debug/test cycles so we won't take those into account.

## Experiment: force-generated signature files efficiency

In the initial discussions, we decided to make an experiment to see if force-generating and caching signature files in a project saves much time during recompilations.

My setup was a project of 25 independent big files (copies of `Utilities.fs`).
There, I measured (few times, with cleaning artifacts, using `--times`):
1. Typechecking - **time1**
2. Typechecking + all signature generation (`--allsigs`) - **time2**
3. Typechecking with signatures generated (left from previous run and added to the project) - **time3**
4. Typechecking with partial signature usage and generation (20 sigs used, 5 generated) - **time4**

I picked a slow machine to better see the differences. But _all the differences_ were largely **marginal** (including between **time1** and **time3**). I can imagine squeezing stable single-digit % performance differences in a cleaner experiment - that said:
- It's not clear if it's going to be for better or for worse, since the signature generation penalty can be bigger than the benefit of having them.
- Even if it's for better, such a small improvement won't be worse the hassle.

Therefore, I think it's not the way to go here.

<details>

<summary>Numbers, for completeness</summary>

```
Normal compilation:
1. Typecheck: 23.7531
2. Typecheck: 27.4234
3. Typecheck: 24.5202

Normal compilation + generating signatures (added extra `ReportTime` for measuring the latter):
1. Typecheck: 26.5991, Siggen: 1.9369
2. Typecheck: 25.0246, Siggen: 1.7517
3. Typecheck: 27.0057, Siggen: 1.9093

Compilation with given signatures:
1. Typecheck: 25.7741
2. Typecheck: 26.2904
3. Typecheck: 24.3852

Compilation with given 80% signatures + generating 20% signatures:
1. Typecheck: 24.1338, Siggen: 0.5284
2. Typecheck: 27.6037, Siggen: 0.7526
3. Typecheck: 25.6967, Siggen: 0.6244
```

</details>


## Implementation plan

The conclusion from the above is that there is a lot of potential in caching - at the same time, implementing things to the full potential right away will require serious changes in the compiler code, which will be dangerous, hard to test and review. Therefore, I propose to implement the feature in stages where each stage brings gains in some scenarios.

**Stage 1 - force-gen and compare typechecking graphs**

We already create the TC graph in `main1` and we are able to dump it (in Mermaid) via the compiler flags.

That means we can force-gen and save the graph which will allow us to skip retypechecking if we detect that the graph is not changed. We should also track all the compilation information (the argument string) and the last update times of the files.

This step won't bring big observable benefits, yet it will create necessary MSBuild hooks to communicate the intermediate files folder towards the compiler, add time-based and hash-based cache invalidation logic, and create testing rails which will include the `clean` and `rebuild` tests to make sure the cache is easily invalidated on demand.

**Stage 2 - skip retypechecking for some files**

In `main1`, we do unpickling (= deserializing) and pickling (= serializing) of the code. This currently happens on the module/namespace basis. 

In this stage, we will fully implement pickling and unpickling of all typechecked files (`CheckedImplFile` per file as well as all other outputs of `main1` which cannot be cheaply recreated like `topAttrs` and `CcuThunk`). Parts of the pickling/unpickling can reuse the primitives already existing in `TypedTreePickle` and built upon them. So we can save acquired typechecking information to the intermediate folder after the typechecking - and for the files not needing retypechecking as per graph diff, skip the phase, instead restoring the typechecking info. We also need to identify the side effects happening during typechecking and probably replay them in such cases.

This is likely the biggest amount of work expected but it will hugely benefit the scenarios when only the edge of the graph is affected (think one test in a test project).

**Stage 3 - reduce signature data generation**

In `main3`, we encode signature data in the assemblies.

Since we are addressing run/debug/test scenarios, a lot of projects can avoid signature data at all, because signature data is only needed for cross-project compilation. This could be detected automatically for certain types of projects (think web apps, console apps, test projects).

For libraries, the signature information resource blob (a byte array) would have to be split into per-file data and we'll and logic to recombine the splits with freshly typechecked information into a single binary resource back.

**Stage 4 - reuse import data**

In `main1`, we restore imported assemblies to get their IL (e.g. system assemblies).

So here, we can serialize and cache imported IL - we'll need to evaluate and benchmark if per assembly or per assembly block (cases like  System.*.dll). We'll apply this for the cross-project case, to not reimport IL for repeating assemblies but instead restore it from the cache - by adding a cross-project intermediate file location to be coordinated with MSBuild properties.

This will be a smaller gain for any particular project but a big accumulated one for large multi-project solutions.

## Testing and benchmarking

We should have a compiler switch for this: applying it to current typechecking tests shouldn't make any difference in results. Tests should test that restored cached results + reusing them should be equivalent to fresh typechecking results.

Benchmarks should be added or run at every stage to the `FCSBenchmarks` project. A good inspiration can be workflow-style tests for updating files done by @0101 in Transparent Compiler.
