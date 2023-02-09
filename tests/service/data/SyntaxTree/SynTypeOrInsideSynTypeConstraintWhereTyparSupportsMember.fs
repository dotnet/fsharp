let inline f_StaticMethod<'T1, 'T2 when ('T1 or 'T2) : (static member StaticMethod: int -> int) >() : int = 
    ()