// TaskBuilder.fs - TPL task computation expressions for F#
//
// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core.CompilerServices

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE
    open Microsoft.FSharp.Core
    open System
    open System.Runtime.CompilerServices

    /// A marker interface to give priority to different available overloads.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    type IPriority3 = interface end

    /// A marker interface to give priority to different available overloads. Overloads using a
    /// parameter of this type will be preferred to overloads with IPriority3,
    /// all else being equal.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    type IPriority2 = interface inherit IPriority3 end

    /// A marker interface to give priority to different available overloads. Overloads using a
    /// parameter of this type will be preferred to overloads with IPriority2 or IPriority3,
    /// all else being equal.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    type IPriority1 = interface inherit IPriority2 end

    /// Defines the implementation of the MoveNext method for a struct state machine.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    type MoveNextMethod<'Template> = delegate of byref<'Template> -> unit

    /// Defines the implementation of the SetMachineState method for a struct state machine.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    type SetMachineStateMethod<'Template> = delegate of byref<'Template> * IAsyncStateMachine -> unit

    /// Defines the implementation of the code reun after the creation of a struct state machine.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    type AfterMethod<'Template, 'Result> = delegate of byref<'Template> -> 'Result

    /// <summary>Adding this attribute to a record type causes it to be compiled to a CLI representation
    /// with a default constructor with property getters and setters.</summary>
    ///
    /// <category>Attributes</category>
    [<AttributeUsage (AttributeTargets.Delegate ||| AttributeTargets.Method,AllowMultiple=false)>]  
    [<Sealed>]
    type ResumableCodeAttribute =
        inherit Attribute

        /// <summary>Creates an instance of the attribute</summary>
        /// <returns>ResumableCodeAttribute</returns>
        new : unit -> ResumableCodeAttribute

    /// Contains compiler intrinsics related to the definition of state machines.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    module StateMachineHelpers = 

        /// <summary>
        /// Statically determines whether resumable code is being used
        /// </summary>
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        val __useResumableCode<'T> : bool 

        /// <summary>
        /// Indicates a resumption point within resumable code
        /// </summary>
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        val __resumableEntry: unit -> int option

        /// <summary>
        /// Indicates to jump to a resumption point within resumable code.
        /// If the 'pc' is statically known then this is a 'goto' into resumable code.  If the 'pc' is not statically
        /// known it must be a valid resumption point within this block of resumable code.
        /// </summary>
        /// <param name="programLabel"></param>
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        val __resumeAt : programLabel: int -> 'T

        /// <summary>
        /// Indicates the given methods provide implementations of the
        /// IAsyncStateMachine functionality for a struct state machine.
        /// </summary>
        ///
        /// <remarks>
        /// The template type must be a struct type and guides the generation of a new struct type by the F# compiler.  Any mention of the template in
        /// any of the code is rewritten to this newly generated struct type.  'moveNext' and 'setMachineState' are
        /// used to implement the methods on the interface implemented by the struct type. The 'after'
        /// method is executed after the state machine has been created.
        /// </remarks>
        /// <param name="moveNextMethod">Gives the implementation of the MoveNext method on IAsyncResult.</param>
        /// <param name="setMachineStateMethod">Gives the implementation of the SetStateMachine method on IAsyncResult.</param>
        /// <param name="afterMethod">Gives code to execute after the generation of the state machine and to produce the final result.</param>
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        val __structStateMachine<'Template, 'Result> : moveNextMethod: MoveNextMethod<'Template> -> setMachineStateMethod: SetMachineStateMethod<'Template> -> afterMethod: AfterMethod<'Template, 'Result> -> 'Result

#endif
