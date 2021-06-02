namespace Internal.Utilities.Collections

/// Iterable functional collection with O(1) append-1 time. Useful for data structures where elements get added at the
/// end but the collection must occasionally be iterated. Iteration is slower and may allocate because 
/// a suffix of elements is stored in reverse order.
///
/// The type doesn't support structural hashing or comparison.
type internal QueueList<'T> =

    interface System.Collections.IEnumerable

    interface System.Collections.Generic.IEnumerable<'T>

    new: xs:'T list -> QueueList<'T>

    new: firstElementsIn:'T list * lastElementsRevIn:'T list * numLastElementsIn:int -> QueueList<'T>

    member Append: ys:seq<'T> -> QueueList<'T>

    member AppendOne: y:'T -> QueueList<'T>

    member ToList: unit -> 'T list

    member FirstElements: 'T list

    member LastElements: 'T list

    static member Empty: QueueList<'T>

module internal QueueList =

  val empty<'T> : QueueList<'T>

  val ofSeq: x:seq<'a> -> QueueList<'a>

  val iter: f:('a -> unit) -> x:QueueList<'a> -> unit

  val map: f:('a -> 'b) -> x:QueueList<'a> -> QueueList<'b>

  val exists: f:('a -> bool) -> x:QueueList<'a> -> bool

  val filter: f:('a -> bool) -> x:QueueList<'a> -> QueueList<'a>

  val foldBack: f:('a -> 'b -> 'b) -> x:QueueList<'a> -> acc:'b -> 'b

  val forall: f:('a -> bool) -> x:QueueList<'a> -> bool

  val ofList: x:'a list -> QueueList<'a>

  val toList: x:QueueList<'a> -> 'a list

  val tryFind: f:('a -> bool) -> x:QueueList<'a> -> 'a option

  val one: x:'a -> QueueList<'a>

  val appendOne: x:QueueList<'a> -> y:'a -> QueueList<'a>

  val append: x:QueueList<'a> -> ys:QueueList<'a> -> QueueList<'a>
