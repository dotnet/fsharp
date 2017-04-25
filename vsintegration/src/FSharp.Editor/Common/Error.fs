// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor
open System

/// Localizable strings.
module internal Strings = 
    open System.Resources
    open System.Reflection

    let private resources = new ResourceManager("FSharp.Editor", Assembly.GetExecutingAssembly())

    /// Exceptions:
    let ExceptionsHeader = resources.GetString("ExceptionsHeader", System.Globalization.CultureInfo.CurrentUICulture)

/// Assert helpers
type internal Assert() = 
    /// Display a good exception for this error message and then rethrow.
    static member Exception(e:Exception) =  
        System.Diagnostics.Debug.Assert(false, "Unexpected exception seen in language service", e.ToString())
    
        
    

