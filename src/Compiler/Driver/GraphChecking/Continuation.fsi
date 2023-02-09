[<RequireQualifiedAccess>]
module internal Continuation

/// This function sequences computations that have been expressed in continuation-passing style.
/// Concretely, when 'T is `int` as an example, can be expressed in continuation-passing style as a function,
/// taking as its input another function that is "how to proceed with a computation given the value of the integer",
/// and returning "the result of that computation".
/// That is, an integer is equivalently represented as a generic function (howToProceed : int -> 'TReturn) -> 'TReturn,
/// and the effect of the function corresponding to the integer 3 is simply to apply the input `howToProceed` to the value 3.
///
/// The motivation for Continuation.sequence is most easily understood when it is viewed without its second argument:
/// it is a higher-order function that takes "a list of 'T expressed in continuation-passing style", and returns "a 'T list expressed in continuation-passing style".
/// The resulting "continuation-passing 'T list" operates by chaining the input 'Ts together, and finally returning the result of continuing the computation after first sequencing the inputs.
///
/// Crucially, this technique can be used to enable unbounded recursion:
/// it constructs and invokes closures representing intermediate stages of the sequenced computation on the heap, rather than consuming space on the (more constrained) stack.
val sequence<'T, 'TReturn> :
    recursions: (('T -> 'TReturn) -> 'TReturn) list -> finalContinuation: ('T list -> 'TReturn) -> 'TReturn

/// Auxiliary function for `Continuation.sequence` that assumes the recursions return a 'T list.
/// In the final continuation the `'T list list` will first be concatenated into one list, before being passed to the (final) `continuation`.
val concatenate<'T, 'TReturn> :
    recursions: (('T list -> 'TReturn) -> 'TReturn) list -> finalContinuation: ('T list -> 'TReturn) -> 'TReturn
