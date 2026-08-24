// #Conformance #ObjectOrientedTypes #TypeExtensions

// Verify that a tuple type extension of arity greater than 7 is rejected with a clear
// diagnostic. F# represents tuples of arity >= 8 with a nested System.Tuple TRest slot,
// so the flat System.Tuple<T1..T8> the desugaring would build could never unify with a
// real 8-tuple. Rather than silently produce a non-functional extension, the compiler
// reports an error.

type (int * int * int * int * int * int * int * int) with
    static member Foo() = 8
