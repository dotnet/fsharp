// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.Service.SurfaceArea

open Tests.Service.SurfaceArea.LibraryTestFx
open NUnit.Framework

type SurfaceAreaTest() =
    [<Test>]
    member _.VerifyArea() =
        let expectedFile = System.IO.Path.Combine(__SOURCE_DIRECTORY__,"FSharp.CompilerService.SurfaceArea.netstandard.expected")
        let actualFile = System.IO.Path.Combine(__SOURCE_DIRECTORY__,"FSharp.CompilerService.SurfaceArea.netstandard.actual")
        SurfaceArea.verify expectedFile actualFile
