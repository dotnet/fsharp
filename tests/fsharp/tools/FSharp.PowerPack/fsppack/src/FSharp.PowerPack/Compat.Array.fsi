namespace Microsoft.FSharp.Compatibility

    open System

    /// Compatibility operations on arrays.  
    [<RequireQualifiedAccess>]
    module Array = 

        /// Create a jagged 2 dimensional array.
        val createJaggedMatrix: int -> int -> 'T -> 'T array array

        /// Is an element in the array, uses (=) equality.
        val inline contains: 'T -> 'T array -> bool  when 'T : equality

        /// Like reduce, but return both the intermediary and final results
        val scanReduce : reduction:('T -> 'T -> 'T) -> 'T array -> 'T array

        /// Like reduceBack, but return both the intermediary and final results
        val scanReduceBack : reduction:('T -> 'T -> 'T) -> 'T array -> 'T array


        /// Pin the given array for the duration of a single call to the given function.  A native pointer to
        /// the first element in the array is passed to the given function.  Cleanup the GCHandle associated with the 
        /// pin when the function completes, even if an exception is raised.
        [<Unverifiable>]
        [<NoDynamicInvocation>]
        val inline pin: 'T[] -> (nativeptr<'T> -> 'Result) -> 'Result

        /// As for Array.pin, except that the caller is responsible for calling Free on the returned GCHandle in order
        /// to release the pin.
        [<Unverifiable>]
        [<NoDynamicInvocation>]
        val inline pinUnscoped: 'T[] -> nativeptr<'T> *  System.Runtime.InteropServices.GCHandle


        /// Create a jagged 2 dimensional array.  Synonym for createJaggedMatrix.
        [<CompilerMessage("This construct is for ML compatibility. The F# name for this function is 'createJaggedMatrix'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
        val create_matrix: int -> int -> 'T -> 'T array array

        [<CompilerMessage("This construct is for ML compatibility. The F# name for this function is 'contains'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
        val inline mem: 'T -> 'T array -> bool when 'T : equality

        [<Unverifiable>]
        [<NoDynamicInvocation>]
        [<Obsolete("This function has been renamed to 'pinUnscoped'")>]
        val inline pin_unscoped: 'T[] -> nativeptr<'T> *  System.Runtime.InteropServices.GCHandle

        [<Obsolete("This function will be removed. Use 'not Array.isEmpty' instead")>]
        val nonempty: 'T[] -> bool

        [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'zip' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
        val combine: array1:'T1 array -> array2:'T2 array -> ('T1 * 'T2) array

        [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'unzip' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
        val split: array:('T1 * 'T2) array -> ('T1 array * 'T2 array)

        [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'forall' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
        val for_all: predicate:('T -> bool) -> array:array<'T> -> bool

        [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'create' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
        val make: count:int -> value:'T -> array<'T>
         
        [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'fold' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
        val fold_left: folder:('State -> 'T -> 'State) -> state:'State -> array: array<'T> -> 'State

        /// Apply a function to each element of the array, threading an accumulator argument
        /// through the computation. If the input function is <c>f</c> and the elements are <c>i0...iN</c> 
        /// then computes <c>f i0 (...(f iN s))</c>
        [<CompilerMessage("This construct is for ML compatibility. This F# library function has been renamed. Use 'foldBack' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
        val fold_right: folder:('T -> 'State -> 'State) -> array:array<'T> -> state:'State -> 'State

        [<CompilerMessage("This construct is for ML compatibility. The F# library name for this function is now 'Array.toList'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
        val to_list: array:'T[] -> 'T list

        [<CompilerMessage("This construct is for ML compatibility. The F# library name for this function is now 'Array.ofList'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
        val of_list: list:'T list -> 'T[]

