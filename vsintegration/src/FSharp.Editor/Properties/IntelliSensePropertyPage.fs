// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Runtime.InteropServices
open Microsoft.VisualStudio.Shell

module internal IntelliSenseSettings =
    let mutable ShowAfterCharIsTyped = true

[<ComVisible(true)>]
[<CLSCompliant(false)>]
[<ClassInterface(ClassInterfaceType.AutoDual)>]
[<Guid("285A7B09-D942-464E-B69E-3BD06D665F5E")>]
type IntelliSensePropertyPage() = 
    inherit DialogPage()    
       
    [<SR.SRCategoryAttribute(SR.IntelliSensePropertyPageMiscCategory)>]
    [<SR.SRDisplayNameAttribute(SR.IntelliSensePropertyPageShowAfterCharIsTyped)>] 
    [<SR.SRDescription(SR.IntelliSensePropertyPageShowAfterCharIsTypedDescr)>] 
    member this.ShowAfterCharIsTyped with get() = IntelliSenseSettings.ShowAfterCharIsTyped and set (x:bool) = IntelliSenseSettings.ShowAfterCharIsTyped <- x

