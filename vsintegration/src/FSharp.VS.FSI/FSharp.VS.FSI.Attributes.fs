// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

type DisplayNameAttribute(resName) =
    inherit System.ComponentModel.DisplayNameAttribute(SRProperties.GetString(resName))

type DescriptionAttribute(resName) =
    inherit System.ComponentModel.DescriptionAttribute(SRProperties.GetString(resName))
        
type CategoryAttribute(resName) = 
    inherit System.ComponentModel.CategoryAttribute(SRProperties.GetString(resName))
