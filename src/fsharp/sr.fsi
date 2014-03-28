// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler 
    
    module internal SR =
        val GetString : string -> string
        val GetObject : string -> System.Object
            
        
    module internal DiagnosticMessage =
        type ResourceString<'T> =
          new : string * Printf.StringFormat<'T> -> ResourceString<'T>
          member Format : 'T

        val DeclareResourceString : string * Printf.StringFormat<'T> -> ResourceString<'T>