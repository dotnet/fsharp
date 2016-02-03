// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Interactive.SRProperties 

let private resources = lazy (new System.Resources.ResourceManager("Properties", System.Reflection.Assembly.GetExecutingAssembly()))

let GetString(name:string) =        
    let s = resources.Value.GetString(name, System.Globalization.CultureInfo.CurrentUICulture)
#if DEBUG
    if null = s then
        System.Diagnostics.Debug.Assert(false, sprintf "**RESOURCE ERROR**: Resource token %s does not exist!" name)
#endif
    s.Replace(@"\n", System.Environment.NewLine)
    
[<Literal>]
let FSharpInteractive64Bit = "FSharpInteractive64Bit"

[<Literal>]
let FSharpInteractive64BitDescr = "FSharpInteractive64BitDescr"

[<Literal>]
let FSharpInteractiveOptions = "FSharpInteractiveOptions"

[<Literal>]
let FSharpInteractiveOptionsDescr = "FSharpInteractiveOptionsDescr"

[<Literal>]
let FSharpInteractiveShadowCopyDescr = "FSharpInteractiveShadowCopyDescr"

[<Literal>]
let FSharpInteractiveShadowCopy = "FSharpInteractiveShadowCopy"

[<Literal>]
let FSharpInteractiveDebugMode = "FSharpInteractiveDebugMode"

[<Literal>]
let FSharpInteractiveDebugModeDescr = "FSharpInteractiveDebugModeDescr"

[<Literal>]
let FSharpInteractiveMisc = "FSharpInteractiveMisc"

[<Literal>]
let FSharpInteractiveDebugging = "FSharpInteractiveDebugging"

type DisplayNameAttribute(resName) =
    inherit System.ComponentModel.DisplayNameAttribute(GetString(resName))

type DescriptionAttribute(resName) =
    inherit System.ComponentModel.DescriptionAttribute(GetString(resName))
        
type CategoryAttribute(resName) = 
    inherit System.ComponentModel.CategoryAttribute(GetString(resName))