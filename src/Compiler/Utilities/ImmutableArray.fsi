[<AutoOpen>]
module internal Internal.Utilities.Library.Block

open System.Collections.Immutable

[<RequireQualifiedAccess>]
module ImmutableArrayBuilder =

    val create: size: int -> ImmutableArray<'T>.Builder

[<RequireQualifiedAccess>]
module ImmutableArray =

    [<GeneralizableValue>]
    val empty<'T> : ImmutableArray<'T>

    val init: n: int -> f: (int -> 'T) -> ImmutableArray<'T>

    val iter: f: ('T -> unit) -> ImmutableArray<'T> -> unit

    val iteri: f: (int -> 'T -> unit) -> ImmutableArray<'T> -> unit

    val iter2: f: ('T1 -> 'T2 -> unit) -> ImmutableArray<'T1> -> ImmutableArray<'T2> -> unit

    val iteri2: f: (int -> 'T1 -> 'T2 -> unit) -> ImmutableArray<'T1> -> ImmutableArray<'T2> -> unit

    val map: mapper: ('T1 -> 'T2) -> ImmutableArray<'T1> -> ImmutableArray<'T2>

    val mapi: mapper: (int -> 'T1 -> 'T2) -> ImmutableArray<'T1> -> ImmutableArray<'T2>

    val concat: ImmutableArray<ImmutableArray<'T>> -> ImmutableArray<'T>

    val forall: predicate: ('T -> bool) -> ImmutableArray<'T> -> bool

    val forall2: predicate: ('T1 -> 'T2 -> bool) -> ImmutableArray<'T1> -> ImmutableArray<'T2> -> bool

    val tryFind: predicate: ('T -> bool) -> ImmutableArray<'T> -> 'T option

    val tryFindIndex: predicate: ('T -> bool) -> ImmutableArray<'T> -> int option

    val tryPick: chooser: ('T1 -> 'T2 option) -> ImmutableArray<'T1> -> 'T2 option

    val ofSeq: seq<'T> -> ImmutableArray<'T>

    val append: ImmutableArray<'T> -> ImmutableArray<'T> -> ImmutableArray<'T>

    val createOne: 'T -> ImmutableArray<'T>

    val filter: predicate: ('T -> bool) -> ImmutableArray<'T> -> ImmutableArray<'T>

    val exists: predicate: ('T -> bool) -> ImmutableArray<'T> -> bool

    val choose: chooser: ('T -> 'U option) -> ImmutableArray<'T> -> ImmutableArray<'U>

    val isEmpty: ImmutableArray<'T> -> bool

    val fold: folder: ('State -> 'T -> 'State) -> 'State -> ImmutableArray<'T> -> 'State
