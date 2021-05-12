// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core.CompilerServices

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE
open Microsoft.FSharp.Core
open System
open System.Runtime.CompilerServices
open Microsoft.FSharp.Collections

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
type MoveNextMethodImpl<'Template> = delegate of byref<'Template> -> unit

/// Defines the implementation of the SetStateMachine method for a struct state machine.
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
type SetStateMachineMethodImpl<'Template> = delegate of byref<'Template> * IAsyncStateMachine -> unit

/// Defines the implementation of the code reun after the creation of a struct state machine.
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
type AfterCode<'Template, 'Result> = delegate of byref<'Template> -> 'Result

/// Defines the implementation of the corresponding method on IResumableStateMachine for a struct state machine.
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
type GetResumptionPointMethodImpl<'Template> = delegate of byref<'Template> -> int

/// Defines the implementation of the corresponding method on IResumableStateMachine for a struct state machine.
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
type SetResumableStateMachineDataMethodImpl<'Template, 'Data> = delegate of byref<'Template> * 'Data -> unit

/// Defines the implementation of the corresponding method on IResumableStateMachine for a struct state machine.
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
type GetResumableStateMachineDataMethodImpl<'Template, 'Data> = delegate of byref<'Template> -> 'Data

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

type IResumableStateMachine<'Data> =
    /// Get the resumption point of the state machine
    abstract ResumptionPoint: int

    /// Copy-out or copy-in the data of the state machine
    abstract Data: 'Data with get, set

/// Contains compiler intrinsics related to the definition of state machines.
[<Microsoft.FSharp.Core.Experimental("Experimental library feature, requires '--langversion:preview'")>]
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
        moveNextMethod: MoveNextMethodImpl<'Template> -> 
        setStateMachineMethod: SetStateMachineMethodImpl<'Template> -> 
        otherMethodImpls: (Type * string * Delegate)[] -> 
        afterCode: AfterCode<'Template, 'Result> 
            -> 'Result

/// Acts as a template for struct state machines introduced by __structStateMachine, and also as a reflective implementation
[<Struct; NoComparison; NoEquality>]
type ResumableStateMachine<'Data> =

    /// When statically compiled, holds the continuation goto-label further execution of the state machine
    [<DefaultValue(false)>]
    val mutable Data: 'Data

    /// When statically compiled, holds the continuation goto-label further execution of the state machine
    [<DefaultValue(false)>]
    val mutable ResumptionPoint: int

    /// When interpreted, holds the continuation for the further execution of the state machine
    [<DefaultValue(false)>]
    val mutable ResumptionFuncData: ResumptionFunc<'Data> * ResumptionFuncExecutor<'Data> * SetStateMachineMethodImpl<ResumableStateMachine<'Data>>

    interface IResumableStateMachine<'Data>

    interface IAsyncStateMachine

/// Represents the runtime continuation of a resumable state machine created dynamically
and ResumptionFunc<'Data> = delegate of byref<ResumableStateMachine<'Data>> -> bool

/// Represents the runtime executor of a resumable state machine created dynamically
and ResumptionFuncExecutor<'Data> = delegate of byref<ResumableStateMachine<'Data>> * ResumptionFunc<'Data> -> unit

/// A special compiler-recognised delegate type for specifying blocks of resumable code
/// with access to the state machine.
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
[<ResumableCodeAttribute>]
type ResumableCode<'Data, 'T> = delegate of byref<ResumableStateMachine<'Data>> -> bool

/// A special compiler-recognised delegate type for specifying blocks of non-resumable code
/// with access to the state machine.
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
type NonResumableCode<'Data, 'T> = delegate of byref<ResumableStateMachine<'Data>> -> 'T

/// Contains functions for composing resumable code blocks
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
[<RequireQualifiedAccess>]
module ResumableCode =

    /// Sequences one section of resumable code after another
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val inline Combine: code1: ResumableCode<'Data, unit> * code2: ResumableCode<'Data, 'T> -> ResumableCode<'Data, 'T>

    /// Creates resumable code whose definition is a delayed function
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val inline Delay: f: (unit -> ResumableCode<'Data, 'T>) -> ResumableCode<'Data, 'T>

    /// Specifies resumable code which iterates an input sequence
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val inline For: sequence: seq<'T> * body: ('T -> ResumableCode<'Data, unit>) -> ResumableCode<'Data, unit>

    /// Specifies resumable code which iterates yields
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val inline Yield: unit -> ResumableCode<'Data, unit>

    /// Specifies resumable code which executes with try/finally semantics
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val inline TryFinally: body: ResumableCode<'Data, 'T> * compensation: NonResumableCode<'Data,unit> -> ResumableCode<'Data, 'T>

    /// Specifies resumable code which executes with try/finally semantics
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val inline TryFinallyAsync: body: ResumableCode<'Data, 'T> * compensation: ResumableCode<'Data,unit> -> ResumableCode<'Data, 'T>

    /// Specifies resumable code which executes with try/with semantics
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val inline TryWith: body: ResumableCode<'Data, 'T> * catch: (exn -> ResumableCode<'Data, 'T>) -> ResumableCode<'Data, 'T>

    /// Specifies resumable code which executes with 'use' semantics
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val inline Using: resource: 'Resource * body: ('Resource -> ResumableCode<'Data, 'T>) -> ResumableCode<'Data, 'T> when 'Resource :> IDisposable

    /// Specifies resumable code which executes a loop
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val inline While: [<InlineIfLambda>] condition: (unit -> bool) * body: ResumableCode<'Data, unit> -> ResumableCode<'Data, unit>

    /// Specifies resumable code which does nothing
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val inline Zero: unit -> ResumableCode<'Data, unit>

    /// The dynamic implementation of the corresponding operation. This operation should not be used directly.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val CombineDynamic: sm: byref<ResumableStateMachine<'Data>> * code1: ResumableCode<'Data, unit> * code2: ResumableCode<'Data, 'T> -> bool

    /// The dynamic implementation of the corresponding operation. This operation should not be used directly.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val WhileDynamic: sm: byref<ResumableStateMachine<'Data>> * condition: (unit -> bool) * body: ResumableCode<'Data, unit> -> bool

    /// The dynamic implementation of the corresponding operation. This operation should not be used directly.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val TryFinallyAsyncDynamic: sm: byref<ResumableStateMachine<'Data>> * body: ResumableCode<'Data, 'T> * compensation: ResumableCode<'Data,unit> -> bool

    /// The dynamic implementation of the corresponding operation. This operation should not be used directly.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val TryWithDynamic: sm: byref<ResumableStateMachine<'Data>> * body: ResumableCode<'Data, 'T> * handler: (exn -> ResumableCode<'Data, 'T>) -> bool

    /// The dynamic implementation of the corresponding operation. This operation should not be used directly.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val YieldDynamic: sm: byref<ResumableStateMachine<'Data>> -> bool

#endif
