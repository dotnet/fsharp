(**
---
title: Snapshots, incrementality and reacting to changes
category: Compiler Service
categoryindex: 2
index: 14
---
*)
(*** hide ***)
#I "../../artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
(**
Compiler Services: Snapshots, incrementality and reacting to changes
============================================

FCS is an incremental execution engine. The aim is to make it Roslyn-like. We're not quite there.

The logical results of all "Check" routines (``ParseAndCheckFileInProject``, ``GetBackgroundCheckResultsForFileInProject``, 
``TryGetRecentTypeCheckResultsForFile``, ``ParseAndCheckProject``) depend on results reported by the file system,
especially the ``IFileSystem`` implementation described in the tutorial on [project wide analysis](project.html).
Logically speaking, these results would be different if file system changes occur.  For example,
referenced DLLs may change on disk, or referenced files may change.

There is [work-in-progress](https://github.com/dotnet/fsharp/issues/11976) to make all parsing and checking deliver results based on immutable snapshots of inputs.

*)
