// Original notice:
// To the extent possible under law, the author(s) have dedicated all copyright and related and neighboring rights
// to this software to the public domain worldwide. This software is distributed without any warranty.
//
// Updates:
// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

#nowarn "9"
#nowarn "51"
namespace Microsoft.FSharp.Core.CompilerServices

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE
    open System
    open System.Runtime.CompilerServices
    open Microsoft.FSharp.Core

    /// A marker interface to give priority to different available overloads
    type IPriority3 = interface end

    /// A marker interface to give priority to different available overloads
    type IPriority2 = interface inherit IPriority3 end

    /// A marker interface to give priority to different available overloads
    type IPriority1 = interface inherit IPriority2 end

    type MoveNextMethod<'Template> = delegate of byref<'Template> -> unit

    type SetMachineStateMethod<'Template> = delegate of byref<'Template> * IAsyncStateMachine -> unit

    type AfterMethod<'Template, 'Result> = delegate of byref<'Template> -> 'Result

    [<AttributeUsage (AttributeTargets.Delegate ||| AttributeTargets.Method,AllowMultiple=false)>]  
    [<Sealed>]
    type ResumableCodeAttribute() = 
        inherit System.Attribute()

    module StateMachineHelpers = 

        /// Statically determines whether resumable code is being used
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __useResumableCode<'T> : bool = false
        
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __resumableEntry () : int option = 
            failwith "__resumableEntry should always be guarded by __useResumableCode and only used in valid state machine implementations"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __resumeAt<'T> (programLabel: int) : 'T = 
            ignore programLabel
            failwith "__resumeAt should always be guarded by __useResumableCode and only used in valid state machine implementations"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __structStateMachine<'Template, 'Result> (moveNextMethod: MoveNextMethod<'Template>) (setMachineStateMethod: SetMachineStateMethod<'Template>) (afterMethod: AfterMethod<'Template, 'Result>): 'Result =
            ignore moveNextMethod
            ignore setMachineStateMethod
            ignore afterMethod
            failwith "__structStateMachine should always be guarded by __useResumableCode and only used in valid state machine implementations"
       
#endif
