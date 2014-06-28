// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService
    open Microsoft.VisualStudio.TextManager.Interop
    open Microsoft.VisualStudio.FSharp.LanguageService
    open Microsoft.VisualStudio.Shell.Interop
    
    module internal Source = 
       // ~- These are unittest-only ~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~
       /// Create a source which delegates to functions in support of unittesting.
       val internal CreateDelegatingSource :
                      recolorizeWholeFile:(unit -> unit) * 
                      recolorizeLine:(int -> unit) * 
                      fileName:string *
                      isClosed:(unit -> bool) * 
                      fileChangeEx:IVsFileChangeEx -> IdealSource
        // ~- These are unittest-only ~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~
            
        /// Create a real production source object.
        val internal CreateSource : 
                        service:LanguageService * 
                        textLines:IVsTextLines * 
                        colorizer:Colorizer * 
                        filechange:IVsFileChangeEx -> IdealSource
