module Neg17






module M =
   val private privateValue : int
   val internal internalValue : int
   
   type private PrivateUnionType = 
       |          DefaultTagOfPrivateType of int
       //| private  PrivateTagOfPrivateType of int
       //| internal InternalTagOfPrivateType of int

   type UnionTypeWithPrivateRepresentation 

   type private PrivateRecordType = 
       { DefaultFieldOfPrivateType : int }

   type RecordTypeWithPrivateRepresentation 

   type internal InternalUnionType = 
       |          DefaultTagOfInternalType of int
       //| private  PrivateTagOfInternalType of int
       //| internal InternalTagOfInternalType of int

   type internal InternalRecordType = 
       { DefaultFieldOfInternalType : int }

   val internal noErrorInternalValueLaterInferredToInvolveInternalUnionType : InternalUnionType list ref
   
 
   type Type = 
       new : unit -> Type
       member        internal InternalProperty : int
       static member internal InternalStaticProperty : int
       member        internal InternalMethod: unit -> int
       static member internal InternalStaticMethod: unit -> int

module M2 =
   val internal privateValue : int // error 
   val private  internalValue : int // error
   
   type internal PrivateUnionType = // error
       |          DefaultTagOfPrivateType of int
