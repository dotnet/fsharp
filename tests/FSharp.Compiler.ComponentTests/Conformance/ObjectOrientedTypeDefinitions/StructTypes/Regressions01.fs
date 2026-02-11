// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Regression test for FSB 4654, generic structs may not have explicit layout

type MultiMap<'k,'v> (x: int) = struct
   static member Empty : MultiMap<'k,'v> = MultiMap<_,_>.Empty
end

type MultiMap2<'k,'v> (x: int) = struct
   member this.Empty = 12
end

type MultiMap3<'k> (x: int) = struct
   member this.Empty = 12
end

type MultiMap4<'k>(x:int) = struct
   member this.Empty = 12
end

type MultiMap5<'k> = struct
   member this.Empty = 12
end

type MultiMap6 = struct
   member this.Empty = 12
end

exit 0
