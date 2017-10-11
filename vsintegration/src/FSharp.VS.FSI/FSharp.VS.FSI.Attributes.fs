// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

type internal ResourceDisplayNameAttribute(resName) =
    inherit System.ComponentModel.DisplayNameAttribute(SRProperties.GetString(resName))

type internal ResourceDescriptionAttribute(resName) =
    inherit System.ComponentModel.DescriptionAttribute(SRProperties.GetString(resName))
        
type internal ResourceCategoryAttribute(resName) =
    inherit System.ComponentModel.CategoryAttribute(SRProperties.GetString(resName))
