// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

module internal ItemDescriptionIcons = 

    // Hardwired constants from older versions of Visual Studio. These constants were used with Babel and VS internals.
    let iIconGroupClass = 0x0000
    let iIconGroupConstant = 0x0001
    let iIconGroupDelegate = 0x0002 
    let iIconGroupEnum = 0x0003
    let iIconGroupEnumMember = 0x0004
    let iIconGroupEvent = 0x0005
    let iIconGroupException = 0x0006
    let iIconGroupFieldBlue = 0x0007
    let iIconGroupInterface = 0x0008 // Absolute = 48
    let iIconGroupTextLine = 0x0009
    let iIconGroupScript = 0x000a
    let iIconGroupScript2 = 0x000b
    let iIconGroupMethod = 0x000c
    let iIconGroupMethod2 = 0x000d
    let iIconGroupModule = 0x000e
    let iIconGroupNameSpace = 0x000f // Absolute = 90
    let iIconGroupFormula = 0x0010
    let iIconGroupProperty = 0x00011
    let iIconGroupStruct = 0x00012
    let iIconGroupTemplate = 0x00013
    let iIconGroupTypedef = 0x00014
    let iIconGroupType = 0x00015
    let iIconGroupUnion = 0x00016
    let iIconGroupVariable = 0x00017
    let iIconGroupValueType = 0x00018 // Absolute = 144
    let iIconGroupIntrinsic = 0x00019
    let iIconGroupError = 0x0001f
    let iIconGroupFieldYellow = 0x0020
    let iIconGroupMisc1 = 0x00021
    let iIconGroupMisc2 = 0x0022
    let iIconGroupMisc3 = 0x00023

    let iIconItemPublic = 0x0000
    let iIconItemInternal = 0x0001
    let iIconItemSpecial = 0x0002
    let iIconItemProtected = 0x0003
    let iIconItemPrivate = 0x0004
    let iIconItemShortCut = 0x0005
    let iIconItemNormal  = iIconItemPublic

    let iIconBlackBox = 162
    let iIconLibrary = 163
    let iIconProgram = 164
    let iIconWebProgram = 165
    let iIconProgramEmpty = 166
    let iIconWebProgramEmpty = 167

    let iIconComponents = 168
    let iIconEnvironment = 169
    let iIconWindow = 170
    let iIconFolderOpen = 171
    let iIconFolder = 172
    let iIconArrowRight = 173

    let iIconAmbigious = 174
    let iIconShadowClass = 175
    let iIconShadowMethodPrivate = 176
    let iIconShadowMethodProtected = 177
    let iIconShadowMethod = 178
    let iIconInCompleteSource = 179
