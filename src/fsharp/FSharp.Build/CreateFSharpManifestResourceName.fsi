// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Build

[<Class>]
type CreateFSharpManifestResourceName =
    inherit Microsoft.Build.Tasks.CreateCSharpManifestResourceName
    public new : unit -> CreateFSharpManifestResourceName
