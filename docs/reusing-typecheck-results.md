---
title: Reusing typecheck results
category: Compiler Internals
categoryindex: 200
index: 42
---

# Reusing typecheck results between compiler runs

Caching and reusing compilation results between compiler runs will be an optimization technique aimed at improving the performance and efficiency of the F# development experience.

## Motivation

- **Performance**. Things like recomputing type information for large projects are time-consuming. By caching the results, subsequent compilation can skip some steps, leading to faster builds.

- **Better IDE cold start**. IDEs can provide faster IntelliSense, code navigation, and other features if they can quickly access cached compilation information.

### Example real-world scenarios

The optimization will benefit the most in large and not very interdependent projects. For instance, a 100-files test project where a single test is changed. MSBuild will mark the project for recompilation but thanks to the improvement only one file would be recompiled in the full-blown manner.

## Premises

Here are some assumptions I am coming with after tinkering with the topic and investigating the current state of the art. 

### Premise 1: current compiler design

The heart of the compiler, `fsc.fs`, is split into 6 phases (`main1` - `main6`). The code is designed to pass minimum information between phases, using the `Args` structure, which is essentially a data bag. The first phase takes info from the program arguments.

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
|>Write Started                      |  1,3817|  0,0016|    180|      0|      0|      0|    455|     46|
|>Module Generation Preparation      |  1,3909|  0,0002|    180|      0|      0|      0|    455|     46|
|>Module Generation Pass 1           |  1,3987|  0,0015|    181|      0|      0|      0|    455|     46|
|>Module Generation Pass 2           |  1,4266|  0,0216|    182|      0|      0|      0|    455|     46|
|>Module Generation Pass 3           |  1,4353|  0,0024|    182|      0|      0|      0|    455|     46|
|>Module Generation Pass 4           |  1,4423|  0,0004|    182|      0|      0|      0|    455|     46|
|>Finalize Module Generation Results |  1,4515|  0,0003|    182|      0|      0|      0|    455|     46|
|>Generated Tables and Code          |  1,4607|  0,0004|    182|      0|      0|      0|    455|     46|
|>Layout Header of Tables            |  1,4663|  0,0001|    182|      0|      0|      0|    455|     46|
|>Build String/Blob Address Tables   |  1,4750|  0,0013|    182|      0|      0|      0|    455|     46|
|>Sort Tables                        |  1,4838|  0,0000|    182|      0|      0|      0|    455|     46|
|>Write Header of tablebuf           |  1,4931|  0,0015|    182|      0|      0|      0|    455|     46|
|>Write Tables to tablebuf           |  1,4999|  0,0000|    182|      0|      0|      0|    455|     46|
|>Layout Metadata                    |  1,5070|  0,0000|    182|      0|      0|      0|    455|     46|
|>Write Metadata Header              |  1,5149|  0,0000|    182|      0|      0|      0|    455|     46|
|>Write Metadata Tables              |  1,5218|  0,0001|    182|      0|      0|      0|    455|     46|
|>Write Metadata Strings             |  1,5297|  0,0000|    182|      0|      0|      0|    455|     46|
|>Write Metadata User Strings        |  1,5369|  0,0002|    182|      0|      0|      0|    455|     46|
|>Write Blob Stream                  |  1,5433|  0,0002|    182|      0|      0|      0|    455|     46|
|>Fixup Metadata                     |  1,5516|  0,0003|    182|      0|      0|      0|    455|     46|
|>Generated IL and metadata          |  1,5606|  0,0015|    182|      0|      0|      0|    455|     46|
|>Layout image                       |  1,5713|  0,0018|    183|      0|      0|      0|    455|     46|
|>Writing Image                      |  1,5799|  0,0009|    183|      0|      0|      0|    454|     46|
|>Signing Image                      |  1,5871|  0,0000|    183|      0|      0|      0|    454|     46|
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
|>Write Started                      |  0,9664|  0,0023|    177|      0|      0|      0|    377|     30|
|>Module Generation Preparation      |  0,9728|  0,0001|    177|      0|      0|      0|    377|     30|
|>Module Generation Pass 1           |  0,9821|  0,0022|    177|      0|      0|      0|    377|     30|
|>Module Generation Pass 2           |  1,0012|  0,0138|    178|      0|      0|      0|    377|     30|
|>Module Generation Pass 3           |  1,0098|  0,0012|    179|      0|      0|      0|    377|     30|
|>Module Generation Pass 4           |  1,0180|  0,0002|    179|      0|      0|      0|    377|     30|
|>Finalize Module Generation Results |  1,0264|  0,0007|    179|      0|      0|      0|    377|     30|
|>Generated Tables and Code          |  1,0344|  0,0003|    179|      0|      0|      0|    377|     30|
|>Layout Header of Tables            |  1,0403|  0,0000|    179|      0|      0|      0|    377|     30|
|>Build String/Blob Address Tables   |  1,0481|  0,0010|    179|      0|      0|      0|    377|     30|
|>Sort Tables                        |  1,0543|  0,0000|    179|      0|      0|      0|    377|     30|
|>Write Header of tablebuf           |  1,0608|  0,0003|    179|      0|      0|      0|    377|     30|
|>Write Tables to tablebuf           |  1,0681|  0,0000|    179|      0|      0|      0|    377|     30|
|>Layout Metadata                    |  1,0738|  0,0000|    179|      0|      0|      0|    377|     30|
|>Write Metadata Header              |  1,0796|  0,0000|    179|      0|      0|      0|    377|     30|
|>Write Metadata Tables              |  1,0875|  0,0000|    179|      0|      0|      0|    377|     30|
|>Write Metadata Strings             |  1,0933|  0,0000|    179|      0|      0|      0|    377|     30|
|>Write Metadata User Strings        |  1,1016|  0,0001|    179|      0|      0|      0|    377|     30|
|>Write Blob Stream                  |  1,1076|  0,0001|    179|      0|      0|      0|    377|     30|
|>Fixup Metadata                     |  1,1135|  0,0004|    179|      0|      0|      0|    377|     30|
|>Generated IL and metadata          |  1,1208|  0,0009|    179|      0|      0|      0|    377|     30|
|>Layout image                       |  1,1269|  0,0010|    179|      0|      0|      0|    377|     30|
|>Writing Image                      |  1,1347|  0,0003|    179|      0|      0|      0|    376|     30|
|>Signing Image                      |  1,1402|  0,0000|    179|      0|      0|      0|    376|     30|
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
|>Write Started                      |  9,0586|  0,0018|    217|      0|      0|      0|    457|     46|
|>Module Generation Preparation      |  9,0655|  0,0001|    217|      0|      0|      0|    457|     46|
|>Module Generation Pass 1           |  9,0719|  0,0005|    217|      0|      0|      0|    457|     46|
|>Module Generation Pass 2           |  9,0820|  0,0031|    218|      0|      0|      0|    457|     46|
|>Module Generation Pass 3           |  9,0894|  0,0005|    218|      0|      0|      0|    457|     46|
|>Module Generation Pass 4           |  9,0968|  0,0002|    218|      0|      0|      0|    457|     46|
|>Finalize Module Generation Results |  9,1032|  0,0003|    218|      0|      0|      0|    457|     46|
|>Generated Tables and Code          |  9,1120|  0,0005|    218|      0|      0|      0|    457|     46|
|>Layout Header of Tables            |  9,1189|  0,0000|    218|      0|      0|      0|    457|     46|
|>Build String/Blob Address Tables   |  9,1276|  0,0006|    218|      0|      0|      0|    457|     46|
|>Sort Tables                        |  9,1339|  0,0000|    218|      0|      0|      0|    457|     46|
|>Write Header of tablebuf           |  9,1430|  0,0001|    218|      0|      0|      0|    457|     46|
|>Write Tables to tablebuf           |  9,1503|  0,0000|    218|      0|      0|      0|    457|     46|
|>Layout Metadata                    |  9,1588|  0,0000|    218|      0|      0|      0|    457|     46|
|>Write Metadata Header              |  9,1656|  0,0000|    218|      0|      0|      0|    457|     46|
|>Write Metadata Tables              |  9,1731|  0,0000|    218|      0|      0|      0|    457|     46|
|>Write Metadata Strings             |  9,1819|  0,0000|    218|      0|      0|      0|    457|     46|
|>Write Metadata User Strings        |  9,1897|  0,0000|    218|      0|      0|      0|    457|     46|
|>Write Blob Stream                  |  9,1976|  0,0000|    218|      0|      0|      0|    457|     46|
|>Fixup Metadata                     |  9,2033|  0,0002|    218|      0|      0|      0|    457|     46|
|>Generated IL and metadata          |  9,2102|  0,0015|    218|      0|      0|      0|    457|     46|
|>Layout image                       |  9,2195|  0,0025|    218|      0|      0|      0|    457|     46|
|>Writing Image                      |  9,2272|  0,0006|    218|      0|      0|      0|    456|     46|
|>Signing Image                      |  9,2338|  0,0000|    218|      0|      0|      0|    456|     46|
// main6
|Write .NET Binary                   |  9,2463|  0,1930|    218|      0|      0|      0|    464|     46|     <-- long
--------------------------------------------------------------------------------------------------------
```

The phases taking the longest are marked in the right. Those are assembly import, type check, optimization and IL writing.

## Implementation plan

The conclusion from the above is that there is a lot of potential in caching - at the same time, implementing things to the full potential right away will require serious changes in the compiler code, which will be dangerous, hard to test and review. Therefore, I propose to implement the feature in stages where each stage brings gains in some scenarios.

**Stage 1 - force-gen and compare typechecking graphs**

We already create the TC graph in `main1` and we are able to dump it (in Mermaid) via the compiler flags.

So we can do the following:
- force-gen and save the graph
- append all the extra compilation info (basically, the compilation arguments string)
- before doing the retypecheck, detect if there is any change to above:
  - if so, skip the retypecheck
  - otherwise, apply some algorithm to get the graph diff. The result should be the list of files for retypecheck (just ignore it for now)

This will allow to skip a huge part of the compilation in the scenarios like reopening Visual Studio for a project when nothing has changed.

**Stage 2 - skip retypecheck for some files**

In `main1`, we do unpickling (= deserializing) and pickling (= serializing) of the code. This currently happens on the module/namespace basis. 

So here we can do the following:
- extend the pickler logic so that it can work on the file basis
- save acquired typecheck information to the intermediate folder after the typecheck
- for the files not needing retypecheck as per graph diff, skip the phase, instead restore the typecheck info

This will hugely benefit the scenarios when only the edge of the graph is affected (think one test in a test project).

**Stage 3 - reduce signature data generation**

In `main3`, we encode signature data in the assemblies.

Since we'll have typecheck graph at this point, we can detect its "edges" - files which nothing depends on. Those don't need to include signature data, hence we can skip the encoding step for them.

Here, the gain largely depends on the type of the project, but this would be anyway a good move in the direction of the compiler less relying on signatures for its operations.

**Stage 4 - reuse import data**

In `main1`, we restore imported assemblies to get their IL (e.g. system assemblies).

So here we can:
- serialize and cache imported IL (to be evaluated and benchmarked if per-assembly or per assembly-block, such as all System.*.dll into a single persisted file)
- apply this for the cross-project case, to not reimport IL for repeating assemblies but instead restore it from the cache

This will be a smaller gain for any particular project but a big accummulated one for large multi-project solutions.

## Testing and benchmarking

We should have a compiler switch for this: applying it to current typechecking tests shouldn't make any difference in results. Tests should test that restored cached results + reusing them should be equivalent to fresh typecheck results.

Benchmarks should be added or run at every stage to the `FCSBenchmarks` project. A good inspiration can be workflow-style tests for updating files done by @0101 in Transparent Compiler.
