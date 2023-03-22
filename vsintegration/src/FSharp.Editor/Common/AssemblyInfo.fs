// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.VisualStudio.Shell

// This is needed to load XAML resource dictionaries from FSharp.UIResources assembly because ProvideCodeBase attribute does not work for that purpose.
// This adds $PackageFolder$ to the directories probed for assemblies to load.
// The attribute is inexplicably class-targeted, hence the dummy class.
[<ProvideBindingPath>]
type private BindingPathForUIResources =
    class
    end
