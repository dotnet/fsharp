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

    /// Defines the implementation of the SetStateMachine method for a struct state machine.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    type SetStateMachineMethod<'Template> = delegate of byref<'Template> * IAsyncStateMachine -> unit

    /// Defines the implementation of the code reun after the creation of a struct state machine.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    type AfterCode<'Template, 'Result> = delegate of byref<'Template> -> 'Result

    /// <summary>Adding this attribute to a delegate type indicates implementations of the delegate may include resumable code.</summary>
    ///
    /// <category>Attributes</category>
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
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
        /// Statically generates a closure struct type based on a template struct type,
        /// implementing IAsyncStateMachine and, optionally, other interfaces.
        /// At runtime an instance of the struct is populated and 'afterMethod' is called
        /// to consume it.
        /// </summary>
        ///
        /// <remarks>
        /// The template type must be a struct type and, at compile-time, guides the generation of a new struct type by the F# compiler
        /// in a way similar to an object expression.  At compile-time a fresh, unique struct type is statically created copying the fields of the template type.
        /// Any mention of the template type in any the methods is rewritten to this fresh struct type.  The 'methodImpls' are used to implement
        /// the interfaces on the struct type corresponding to the interfaces of the template type and are also rewritten.
        /// The 'after' method is executed after the state machine has been created and must eliminate the template type, its return
        /// type should not mention the template type.
        /// </remarks>
        /// <param name="moveNextMethod">Gives the implementation of the MoveNext method on IAsyncStateMachine.</param>
        /// <param name="setStateMachineMethod">Gives the implementation of the SetStateMachine method on IAsyncStateMachine.</param>
        /// <param name="otherMethodImpls">Gives the fresh implementations of interfaces implemented by the template.</param>
        /// <param name="afterCode">Gives code to execute after the generation of the state machine and to produce the final result.</param>
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        val __structStateMachine<'Template, 'Result> :
            moveNextMethod: MoveNextMethod<'Template> -> 
            setStateMachineMethod: SetStateMachineMethod<'Template> -> 
            otherMethodImpls: (Type * string * Delegate)[] -> 
            afterCode: AfterCode<'Template, 'Result> 
                -> 'Result

#endif
