(**
---
title: Incrementality
category: Language Service Internals
categoryindex: 300
index: 1400
---
*)
(*** hide ***)
#I "../../artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
(**
Snapshots, incrementality and reacting to changes
============================================

FCS is an incremental execution engine. The aim is to make it Roslyn-like. We're not quite there.

There are two dimensions of incrementality:

* The inputs change, e.g. the source filed are edited or a referenced assembly changes, appears or disappears
* The results of analysis on the inputs (e.g. a parse tree) are further enriched with information (e.g. symbol uses are requested) and this information is held, i.e. not re-computed, perhaps by a returned object.

The logical results of all "Check" routines (``ParseAndCheckFileInProject``, ``GetBackgroundCheckResultsForFileInProject``, 
``TryGetRecentTypeCheckResultsForFile``, ``ParseAndCheckProject``) depend on results reported by the file system,
especially the ``IFileSystem`` implementation described in the tutorial on [project wide analysis](project.html).
Logically speaking, these results would be different if file system changes occur.  For example,
referenced DLLs may change on disk, or referenced files may change.

There is [work-in-progress](https://github.com/dotnet/fsharp/issues/11976) to make all parsing and checking deliver results based on immutable snapshots of inputs.

*)
