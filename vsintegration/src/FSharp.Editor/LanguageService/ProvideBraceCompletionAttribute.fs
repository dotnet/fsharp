// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open Microsoft.VisualStudio.Shell

[<Sealed>]
[<AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)>]
type internal ProvideBraceCompletionAttribute(languageName: string) =
    inherit RegistrationAttribute()

    override _.Register(context) =
        use key = context.CreateKey(@"Languages\Language Services\" + languageName)
        key.SetValue("ShowBraceCompletion", 1)

    override _.Unregister(_) = ()
