(*** hide ***)
#I "../../artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
(**
Compiler Services: Reacting to Changes
============================================

This tutorial discusses some technical aspects of how to make sure the F# compiler service is 
providing up-to-date results especially when hosted in an IDE. See also [project wide analysis](project.html)
for information on project analysis.

> **NOTE:** The FSharp.Compiler.Service API is subject to change when later versions of the nuget package are published.

The logical results of all "Check" routines (``ParseAndCheckFileInProject``, ``GetBackgroundCheckResultsForFileInProject``, 
``TryGetRecentTypeCheckResultsForFile``, ``ParseAndCheckProject``) depend on results reported by the file system,
especially the ``IFileSystem`` implementation described in the tutorial on [project wide analysis](project.html).
Logically speaking, these results would be different if file system changes occur.  For example,
referenced DLLs may change on disk, or referenced files may change.

There is [work-in-progress](https://github.com/dotnet/fsharp/issues/11976) to make all parsing and checking deliver results based on immutable snapshots of inputs.

*)
