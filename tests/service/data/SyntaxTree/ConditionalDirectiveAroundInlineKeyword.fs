
let 
#if !FOO
    inline
#endif
    map f ar = Async.map (Result.map f) ar
