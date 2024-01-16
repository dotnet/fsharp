// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

//--------------------------------------------------------------------------------------
// Attributes used to mark up editable properties

[<AttributeUsage(AttributeTargets.All)>]
type internal SRDescriptionAttribute(description: string) =
    inherit DescriptionAttribute(description)
    let mutable replaced = false

    override x.Description =
        if not (replaced) then
            replaced <- true
            x.DescriptionValue <- SR.GetString(base.Description)

        base.Description

[<AttributeUsage(AttributeTargets.All)>]
type internal SRCategoryAttribute(category: string) =
    inherit CategoryAttribute(category)
    override x.GetLocalizedString(value: string) = SR.GetString(value)

[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Property ||| AttributeTargets.Field, Inherited = false, AllowMultiple = false)>]
type internal SRDisplayNameAttribute(name: string) =
    inherit DisplayNameAttribute()

    override x.DisplayName =
        match SR.GetString(name) with
        | null ->
            Debug.Assert(false, "String resource '" + name + "' is missing")
            name
        | result -> result
