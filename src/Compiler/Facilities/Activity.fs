// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

open System
open System.Diagnostics

[<RequireQualifiedAccess>]
module Activity =    
    
    let private activitySourceName = "fsc"
    let private activitySource = new ActivitySource(activitySourceName)

    let start name (tags : (string * string) seq) : IDisposable = 
        match activitySource.StartActivity(name) |> Option.ofObj with
        | Some activity ->
            for key, value in tags do
                activity.AddTag(key, value) |> ignore
            activity :> IDisposable
        | None ->
            null

    let startNoTags name: IDisposable = activitySource.StartActivity(name)