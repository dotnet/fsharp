// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace EmptyAssembly

open Microsoft.FSharp.Core.CompilerServices

// The point is to test a warning diagnostic about assemblies with the attribute below but with no type providers defined

[<assembly:TypeProviderAssembly>]
do()