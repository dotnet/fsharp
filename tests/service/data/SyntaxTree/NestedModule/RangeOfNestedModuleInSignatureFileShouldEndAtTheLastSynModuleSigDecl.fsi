
namespace Microsoft.FSharp.Core

open System
open System.Collections.Generic
open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections
open System.Collections


module Tuple =

    type Tuple<'T1,'T2,'T3,'T4> =
        interface IStructuralEquatable
        interface IStructuralComparable
        interface IComparable
        new : 'T1 * 'T2 * 'T3 * 'T4 -> Tuple<'T1,'T2,'T3,'T4>
        member Item1 : 'T1 with get
        member Item2 : 'T2 with get
        member Item3 : 'T3 with get
        member Item4 : 'T4 with get


module Choice =

    /// <summary>Helper types for active patterns with 6 choices.</summary>
    [<StructuralEquality; StructuralComparison>]
    [<CompiledName("FSharpChoice`6")>]
    type Choice<'T1,'T2,'T3,'T4,'T5,'T6> =
      /// <summary>Choice 1 of 6 choices</summary>
      | Choice1Of6 of 'T1
      /// <summary>Choice 2 of 6 choices</summary>
      | Choice2Of6 of 'T2
      /// <summary>Choice 3 of 6 choices</summary>
      | Choice3Of6 of 'T3
      /// <summary>Choice 4 of 6 choices</summary>
      | Choice4Of6 of 'T4
      /// <summary>Choice 5 of 6 choices</summary>
      | Choice5Of6 of 'T5
      /// <summary>Choice 6 of 6 choices</summary>
      | Choice6Of6 of 'T6



/// <summary>Basic F# Operators. This module is automatically opened in all F# code.</summary>
[<AutoOpen>]
module Operators =

    type ``[,]``<'T> with
        /// <summary>Get the length of an array in the first dimension  </summary>
        [<CompiledName("Length1")>]
        member Length1 : int
        /// <summary>Get the length of the array in the second dimension  </summary>
        [<CompiledName("Length2")>]        
        member Length2 : int
        /// <summary>Get the lower bound of the array in the first dimension  </summary>
        [<CompiledName("Base1")>]
        member Base1 : int
        /// <summary>Get the lower bound of the array in the second dimension  </summary>
        [<CompiledName("Base2")>]
        member Base2 : int
