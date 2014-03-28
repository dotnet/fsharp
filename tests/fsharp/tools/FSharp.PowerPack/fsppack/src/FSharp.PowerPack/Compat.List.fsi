namespace Microsoft.FSharp.Compatibility

open System

/// Compatibility operations on lists.  
[<RequireQualifiedAccess>]
module List = 

    /// Like reduce, but return both the intermediary and final results
    val scanReduce : reduction:('T -> 'T -> 'T) -> 'T list -> 'T list

    /// Like reduceBack, but return both the intermediary and final results
    val scanReduceBack : reduction:('T -> 'T -> 'T) -> 'T list -> 'T list

    /// Is an element in the list. Elements are compared using generic equality.
    val contains: 'T -> 'T list -> bool when 'T : equality

    /// Is an element in the list. Elements are compared using generic equality.
    [<CompilerMessage("This construct is for ML compatibility. The F# name for this function is 'contains'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val mem: 'T -> 'T list -> bool when 'T : equality

    /// Lookup key's data in association list, uses (=) equality.
    /// Raise <c>System.IndexOutOfRangeException</c> exception if key not found, in which case you should typically use <c>try_assoc</c> instead.
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val assoc: 'Key -> ('Key * 'Value) list -> 'Value when 'Key : equality

    /// Lookup key's data in association list, uses (=) equality,
    /// returning "Some data" or "None".  
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val try_assoc: 'Key -> ('Key * 'Value) list -> 'Value option when 'Key : equality

    /// Does the key have pair in the association list?
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val mem_assoc: 'Key -> ('Key * 'Value) list -> bool when 'Key : equality

    /// Remove pair for key from the association list (if it's there).
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val remove_assoc: 'Key -> ('Key * 'Value) list -> ('Key * 'Value) list when 'Key : equality

    /// See <c>assoc</c>, but uses the physical equality operator (==) for equality tests
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val assq: 'Key -> ('Key * 'Value) list -> 'Value when 'Key : not struct
      
    /// See <c>try_assoc</c>, but uses the physical equality operator (==) for equality tests.    
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val try_assq: 'Key -> ('Key * 'Value) list -> 'Value option when 'Key : not struct

    /// See <c>mem_assoc</c>, but uses the physical equality operator (==) for equality tests.      
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val mem_assq: 'Key -> ('Key * 'Value) list -> bool when 'Key : not struct

    /// See <c>remove_assoc</c>, but uses the physical equality operator (==) for equality tests.        
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val remove_assq: 'Key -> ('Key * 'Value) list -> ('Key * 'Value) list when 'Key : not struct

    /// See <c>mem</c>, but uses the physical equality operator (==) for equality tests.        
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val memq: 'T -> 'T list -> bool when 'T : not struct

    /// Return true if the list is not empty.
    [<Obsolete("This function will be removed. Use 'not List.isEmpty' instead")>]
    val nonempty: 'T list -> bool

    /// "rev_map f l1" evaluates to "map f (rev l1)"
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val rev_map: mapping:('T -> 'U) -> 'T list -> 'U list

    /// "rev_map2 f l1 l2" evaluates to "map2 f (rev l1) (rev l2)"
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val rev_map2: mapping:('T1 -> 'T2 -> 'U) -> 'T1 list -> 'T2 list -> 'U list

    /// "rev_append l1 l2" evaluates to "append (rev l1) l2"
    [<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val rev_append: 'T list -> 'T list -> 'T list

    [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'concat' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val flatten: lists:seq<'T list> -> 'T list

    [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'filter' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val find_all: predicate:('T -> bool) -> list:'T list -> 'T list

    [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'fold' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val fold_left: folder:('State -> 'T -> 'State) -> state:'State -> list:'T list -> 'State

    [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'fold2' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val fold_left2: folder:('State -> 'T1 -> 'T2 -> 'State) -> state:'State -> list1:'T1 list -> list2:'T2 list -> 'State

    [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'foldBack' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val fold_right: folder:('T -> 'State -> 'State) -> list:'T list -> state:'State -> 'State

    [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'foldBack2' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val fold_right2: folder:('T1 -> 'T2 -> 'State -> 'State) -> list1:'T1 list -> list2:'T2 list -> state:'State -> 'State

    [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'forall' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val for_all: predicate:('T -> bool) -> list:'T list -> bool

    [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'sortWith' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val stable_sort: list: ('T -> 'T -> int) -> 'T list -> 'T list

    [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'unzip' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val split: list:('T1 * 'T2) list -> ('T1 list * 'T2 list)

    [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'zip' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val combine: list1:'T1 list -> list2:'T2 list -> ('T1 * 'T2) list
    
    [<CompilerMessage("This construct is for ML compatibility. The F# library name for this function is now 'List.ofArray'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val of_array : array:'T[] -> 'T list

    [<CompilerMessage("This construct is for ML compatibility. The F# library name for this function is now 'List.toArray'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val to_array: list:'T list -> 'T[]

    /// Return the first element of the list.
    ///
    /// Raises <c>System.ArgumentException</c> if <c>list</c> is empty
    [<CompilerMessage("This construct is for ML compatibility. The F# library name for this function is now 'List.head'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val hd: list:'T list -> 'T

    /// Return the tail of the list.  
    ///
    /// Raises <c>System.ArgumentException</c> if <c>list</c> is empty
    [<CompilerMessage("This construct is for ML compatibility. The F# library name for this function is now 'List.tail'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val tl: list:'T list -> 'T list

