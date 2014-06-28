// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.


namespace Microsoft.VisualStudio.FSharp.LanguageService

open System.Runtime.InteropServices

/// Keyboard functions
module internal Keyboard =

    /// Determine the state of a particular key on the keyboard.
    /// See Win32 API
    [<DllImport("User32.dll")>]   
    let private GetKeyState(_key:int) : int16 = failwith ""
    
    /// Well-known keys
    type Keys = Shift = 16 | Whatever = 0 // VK_SHIFT

    /// Private hook vector for key state.
    let mutable private GetKeyStateHook = (fun (key:Keys)->GetKeyState(int32 key))
    
    /// Hook the key press handler
    let HookGetKeyState hook = 
        let orig = GetKeyStateHook
        GetKeyStateHook<-hook
        orig

    /// Determine whether a key is pressed.
    let IsKeyPressed key = (GetKeyStateHook key) < 0s
    
