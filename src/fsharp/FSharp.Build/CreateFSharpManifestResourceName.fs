// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Build
open System
open System.IO
open System.Text
open Microsoft.Build.Tasks
open Microsoft.Build.Utilities

type CreateFSharpManifestResourceName public () =
    inherit CreateCSharpManifestResourceName()
    
    override this.CreateManifestName 
                ((fileName:string), 
                    (linkFileName:string),
                    (rootNamespace:string), (* may be null *)  
                    (dependentUponFileName:string), (* may be null *) 
                    (binaryStream:System.IO.Stream) (* may be null *)) : string = 
        let embeddedFileName = 
            match linkFileName with
            |   null -> fileName
            |   _ -> linkFileName
        // since we do not support resources dependent on a form, we always pass null for a binary stream 
        // rootNamespace is always empty - we do not support it
        let cSharpResult = 
            base.CreateManifestName(fileName, linkFileName,  "", dependentUponFileName, null)
        // Workaround that makes us keep .resources extension on both 3.5 and 3.5SP1
        // 3.5 stripped ".resources", 3.5 SP1 does not. We should do 3.5SP1  thing
        let extensionToWorkaround = ".resources"
        if embeddedFileName.EndsWith(extensionToWorkaround, StringComparison.OrdinalIgnoreCase) 
                && not (cSharpResult.EndsWith(extensionToWorkaround, StringComparison.OrdinalIgnoreCase)) then
            cSharpResult + extensionToWorkaround
        else
            cSharpResult
            
        
    override this.IsSourceFile (filename:string) = 
        let extension = Path.GetExtension(filename)
        (String.Equals(extension, ".fs", StringComparison.OrdinalIgnoreCase) ||
            String.Equals(extension, ".ml", StringComparison.OrdinalIgnoreCase))