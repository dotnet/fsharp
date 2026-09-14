(** ---
category: Release Notes
categoryindex: 600
index: 4
title: FSharp.Compiler.Service
---

# FSharp.Compiler.Service
*)
(*** hide ***)
#load "./.aux/Common.fsx"

open System.IO
open System.Xml.Linq
open Markdig
open Common

let path = Path.Combine(__SOURCE_DIRECTORY__, ".FSharp.Compiler.Service")

// The FCS package version cannot be derived from the release notes file name: its minor number
// is bumped independently of the F# version (43.12.100 is F# 11.0.100, 43.12.204 is F# 10.0.204).
// The lookup by source commit handles that; the F# major is only used to place notes that never
// shipped. Packages before 43.8 predate the release notes folder.
renderPackageReleaseNotes
    "FSharp.Compiler.Service"
    path
    (System.Version(43, 8, 0))
    (fun notes -> System.Version(43, notes.Major, notes.Build))
    upcomingFcsVersion
(*** include-it-raw ***)
