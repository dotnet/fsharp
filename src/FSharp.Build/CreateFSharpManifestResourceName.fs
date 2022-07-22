// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build

open System
open System.IO
open Microsoft.Build.Tasks

type CreateFSharpManifestResourceName public () =
    inherit CreateCSharpManifestResourceName()

    // When set to true, generate resource names in the same way as C# with root namespace and folder names
    member val UseStandardResourceNames = false with get, set

    override this.CreateManifestName
        (
            fileName: string,
            linkFileName: string,
            rootNamespace: string,  // may be null
            dependentUponFileName: string,  // may be null
            binaryStream: Stream // may be null
        ) : string =

        // The Visual CSharp and XBuild CSharp toolchains transform resource names like this:
        //     SubDir\abc.resx --> SubDir.abc.resources
        //     SubDir\abc.txt --> SubDir.abc.txt
        //
        // For resx resources, both the Visual FSharp and XBuild FSHarp toolchains do the right thing, i.e.
        //     SubDir\abc.resx --> SubDir.abc.resources

        let fileName, linkFileName, rootNamespace =
            match this.UseStandardResourceNames with
            | true -> fileName, linkFileName, rootNamespace
            | false -> fileName, linkFileName, ""

        let embeddedFileName =
            match linkFileName with
            | null -> fileName
            | _ -> linkFileName

        // since we do not support resources dependent on a form, we always pass null for a binary stream
        let cSharpResult =
            base.CreateManifestName(fileName, linkFileName, rootNamespace, dependentUponFileName, null)
        // Workaround that makes us keep .resources extension on both 3.5 and 3.5SP1
        // 3.5 stripped ".resources", 3.5 SP1 does not. We should do 3.5SP1  thing
        let extensionToWorkaround = ".resources"

        if
            embeddedFileName.EndsWith(extensionToWorkaround, StringComparison.OrdinalIgnoreCase)
            && not (cSharpResult.EndsWith(extensionToWorkaround, StringComparison.OrdinalIgnoreCase))
        then
            cSharpResult + extensionToWorkaround
        else
            cSharpResult

    override _.IsSourceFile(fileName: string) =
        let extension = Path.GetExtension(fileName)

        (String.Equals(extension, ".fs", StringComparison.OrdinalIgnoreCase)
         || String.Equals(extension, ".ml", StringComparison.OrdinalIgnoreCase))
