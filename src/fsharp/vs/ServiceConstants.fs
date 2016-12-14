// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

module ItemDescriptionIcons = 

    type GlyphMajor = 
        | Class = 0x0000
        | Constant = 0x0001
        | Delegate = 0x0002 
        | Enum = 0x0003
        | EnumMember = 0x0004
        | Event = 0x0005
        | Exception = 0x0006
        | FieldBlue = 0x0007
        | Interface = 0x0008 // Absolute = 48
        | Method = 0x000c
        | Method2 = 0x000d
        | Module = 0x000e
        | NameSpace = 0x000f // Absolute = 90
        | Property = 0x00011
        | Struct = 0x00012
        | Typedef = 0x00014
        | Type = 0x00015
        | Union = 0x00016
        | Variable = 0x00017
        | ValueType = 0x00018 // Absolute = 144
        | Error = 0x0001f


    type GlyphMinor = 
        | Special = 0x0002
        | Normal  = 0x0000








