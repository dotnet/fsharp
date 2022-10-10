// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

open System
open System.Diagnostics
open System.Diagnostics.Tracing

[<RequireQualifiedAccess>]
module Activity =    

    let private activitySource = new ActivitySource("fsc")

    let Start name (tags:(string * string) seq) : IDisposable = 
            let act = activitySource.StartActivity(name)
            for key,value in tags do
                act.AddTag(key,value) |> ignore
            act

    let StartNoTags name: IDisposable = activitySource.StartActivity(name)