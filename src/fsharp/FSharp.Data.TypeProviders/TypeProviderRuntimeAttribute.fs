// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Data.TypeProviders.DesignTime

open Microsoft.FSharp.Core.CompilerServices

// This says that this assembly is a runtime DLL for a particular platform, where the design-time
// DLL is found alongside this DLL and has the given name.
[<assembly:TypeProviderAssemblyAttribute("FSharp.Data.TypeProviders.DesignTime.dll") >]
do()

