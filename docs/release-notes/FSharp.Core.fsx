(** ---
category: Release Notes
categoryindex: 600
index: 3
title: FSharp.Core
---

# FSharp.Core
*)
(*** hide ***)
#load "./.aux/Common.fsx"

open System.IO
open Markdig
open Common

let path = Path.Combine(__SOURCE_DIRECTORY__, ".FSharp.Core")

// FSharp.Core mostly follows the F# version, but not always: the F# 10 packages shipped as 10.1.x
// to signal breaking changes, while the notes file stays 10.0.x. The lookup by source commit
// handles that. Packages before 8.0 predate the release notes folder.
renderPackageReleaseNotes "FSharp.Core" path (System.Version(8, 0, 0)) id upcomingFSharpVersion
(*** include-it-raw ***)
