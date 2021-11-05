---
title: Memory usage
category: General
categoryindex: 100
index: 600
---
# Compiler Memory Usage

Overall memory usage is a primary determinant of the usability of the F# compiler and instances of the F# compiler service. 

## Why memory usage matters

Overly high memory usage results in poor throughput (particularly due to increased GC times) and low user interface responsivity in tools such as Visual Studio or other editing environments. In some extreme cases, it can lead to Visual Studio crashing or another IDE becoming unusable due to constant paging from absurdly high memory usage. Luckily, these extreme cases are very rare.

When you do a single compilation to produce a binary, memory usage typically doesn't matter much. It's often fine to allocate a lot of memory because it will just be reclaimed after compilation is over.

However, the F# compiler is not simply a batch process that accepts source code as input and produces an assembly as output. When you consider the needs of editor and project tooling in IDEs, the F# compiler is:

* A database of syntactic and semantic data about the code hosted in an IDE
* An API for tools to request tooling-specific data (e.g. F# tooltip information)
* In FsAutoComplete or other LSP implementations, it's a server process that accepts requests for syntactic and semantic information

Thinking about the F# compiler in these ways makes performance far more complicated than just throughput of a batch compilation process.

## Analyzing compiler memory usage

In general, the F# compiler allocates a lot of memory. More than it needs to. However, most of the "easy" sources of allocations have been squashed out and what remains are many smaller sources of allocations. The remaining "big" pieces allocate as a result of their current architecture, so it isn't straightforward to address them.

Some allocations are much more than others
* Large Object Heap (LOH) allocations (> ~80K) are rarely collected and should only be used for long-lived items. 
* Ephemeral allocations that never escape the Gen0 seem to not matter that much, though of course should be considered.
* Don't try to remove all allocations, and don't asseume copy large structs is better than allocating a reference type. Measure instead.

To analyze memory usage of F# tooling, you have two primary avenues:

1. Take a process dump on your machine and analyze it 
2. Use sampling to collect a trace of your system while you perform various tasks in an IDE, ideally for 60 seconds or more.

You can analyze dumps and take samples with [dotMemory](https://www.jetbrains.com/dotmemory/) or [PerfView](https://github.com/Microsoft/perfview).

### Analyzing a process dump file

Process dump files are extremely information-rich data files that can be used to see the distribution of memory usage across various types. Tools like [dotMemory](https://www.jetbrains.com/dotmemory/) will show these distributions and intelligently group things to help identify the biggest areas worth improving. Additionally, they will notice things like duplicate strings and sparse arrays, which are often great ways to improve memory usage since it means more memory is being used than is necessary.

### Analyzing a sample trace of IDE usage

The other important tool to understand memory and CPU usage for a given sample of IDE usage is a trace file. These are collected and analyzed by tools like [PerfView](https://github.com/Microsoft/perfview) and [dotTrace](https://www.jetbrains.com/profiler/).

When analyzing a trace, there are a few things to look out for:

1. Overall GC statistics for the sample to give an overall picture of what was going on in the IDE for your sample:
   a. How much CPU time was spent in the GC as a percentage of total CPU time for the IDE process?
   b. What was the peak working set (total memory usage)?
   c. What was the peak allocations per second?
   d. How many allocations were Gen0? Gen1? Gen2?
2. Memory allocations for the sample, typically also ignoring object deaths:
   a. Is `LargeObject` showing up anywhere prominently? If so, that's a problem!
   b. Which objects show up highest on the list? Does their presence that high make sense?
   c. For a type such as `System.String`, which caller allocates it the most? Can that be improved?
3. CPU sampling data, sorted by most CPU time
   a. Are any methods showing up that correspond with high memory allocations? Something showing up prominently in both places is often a sign that it needs work!

After analyzing a trace, you should have a good idea of places that could see improvement. Often times a tuple can be made into a struct tuple, or some convenient string processing could be adjusted to use a `ReadonlySpan<'T>` or turned into a more verbose loop that avoids allocations.

