// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Diagnostics
open Microsoft.VisualStudio.Shell

type internal ProvideFSharpVersionRegistrationAttribute(packageGuidString: string, productName: string) =
    inherit RegistrationAttribute()

    let keyName = "InstalledProducts\\" + productName

    override this.Register(context: RegistrationAttribute.RegistrationContext) =
        // Get the version of this build.  This code is executed by CreatePkgDef.exe at build time and NOT at runtime.
        let version =
            FileVersionInfo.GetVersionInfo(typeof<ProvideFSharpVersionRegistrationAttribute>.Assembly.Location)

        use key = context.CreateKey(keyName)
        key.SetValue("Package", Guid.Parse(packageGuidString).ToString("B"))
        key.SetValue("PID", version.ProductVersion)
        key.SetValue("UseInterface", false)
        key.SetValue("UseVSProductID", false)

    override this.Unregister(context: RegistrationAttribute.RegistrationContext) = context.RemoveKey(keyName)
