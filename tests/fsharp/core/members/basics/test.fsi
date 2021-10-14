module Global

type ClassType1 =
  class
     inherit System.Object
     val someField : string

     abstract VirtualMethod1: string -> int
     abstract VirtualMethod2: string * string -> int
     default VirtualMethod1: string -> int
     default VirtualMethod2: string * string -> int
     override VirtualMethod1PostHoc : s:string -> int
     abstract VirtualMethod1PostHoc: string -> int
     abstract VirtualMethod2PostHoc: string * string -> int
     override VirtualMethod2PostHoc : s1:string * s2:string -> int

     new : string -> ClassType1

  end

type ClassType2 =
  class
    inherit ClassType1
    val someField2 : string

    override VirtualMethod1: string -> int
    override VirtualMethod2: string * string -> int

    new : string -> ClassType2
  end


module RecordTypeTest = begin

    [<Sealed>]
    type AbstractType =
       begin // properties
          member InstanceProperty : string
          member MutableInstanceProperty : string
             with get,set

          member InstanceIndexer : int -> string
             with get
          member InstanceIndexerCount: int

          member InstanceIndexer2 : int * int -> string
             with get
          member InstanceIndexer2Count1: int
          member InstanceIndexer2Count2: int

          member MutableInstanceIndexer : int -> string
             with get,set 
          member MutableInstanceIndexerCount: int

          member MutableInstanceIndexer2 : int * int  -> string
             with get,set 
          member MutableInstanceIndexer2Count1: int
          member MutableInstanceIndexer2Count2: int

          static member StaticProperty : string
          
          static member MutableStaticProperty : string
             with get,set
             
          static member StaticIndexer : int -> string
             with get
          static member StaticIndexerCount : int
             
          static member MutableStaticIndexer: int -> string
             with get,set 
          static member MutableStaticIndexerCount : int

          // methods
          member InstanceMethod : string -> string
          
          static member StaticMethod : string * string -> string

       end

end

module UnionTypeTest = begin

    [<Sealed>]
    type AbstractType = 
      begin
          // properties
          member InstanceProperty : string
          member MutableInstanceProperty : string
             with get,set

          member InstanceIndexer : int -> string
             with get
          member InstanceIndexerCount: int

          member InstanceIndexer2 : int * int -> string
             with get
          member InstanceIndexer2Count1: int
          member InstanceIndexer2Count2: int

          member MutableInstanceIndexer : int -> string
             with get,set 
          member MutableInstanceIndexerCount: int

          member MutableInstanceIndexer2 : int * int  -> string
             with get,set 
          member MutableInstanceIndexer2Count1: int
          member MutableInstanceIndexer2Count2: int

          static member StaticProperty : string
          
          
          static member MutableStaticProperty : string
             with get,set
             
          static member StaticIndexer : int -> string
             with get
          static member StaticIndexerCount : int
             
          static member MutableStaticIndexer: int -> string
             with get,set 
          static member MutableStaticIndexerCount : int

          // methods
          member InstanceMethod : string -> string
          
          static member StaticMethod : string * string -> string

    end

end

module ToStringOnUnionTest = begin
   type MyUnion = A of string | B
 
end

module ToStringOnUnionTestOverride = begin
   type MyUnion = A of string | B
 
end