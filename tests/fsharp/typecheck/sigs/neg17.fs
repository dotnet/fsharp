module Neg17






module M =
   let private privateValue = 1
   let internal internalValue = 1
   
   type private PrivateUnionType = 
       |          DefaultTagOfPrivateType of int
       //| private  PrivateTagOfPrivateType of int
       //| internal InternalTagOfPrivateType of int

   type UnionTypeWithPrivateRepresentation = 
       private | DefaultTagOfUnionTypeWithPrivateRepresentation of int

   type private PrivateRecordType = 
       { DefaultFieldOfPrivateType : int }

   type RecordTypeWithPrivateRepresentation = 
       private { DefaultFieldOfRecordTypeWithPrivateRepresentation : int }

   type internal InternalUnionType = 
       |          DefaultTagOfInternalType of int
       //| private  PrivateTagOfInternalType of int
       //| internal InternalTagOfInternalType of int

   type internal InternalRecordType = 
       { DefaultFieldOfInternalType : int }

   let errorPublicValueLaterInferredToInvolvePrivateUnionType = ref []  // escape check: escape via type inference
   let errorPublicValueLaterInferredToInvolveInternalUnionType = ref [] // escape check: escape via type inference

   let internal noErrorInternalValueLaterInferredToInvolveInternalUnionType = ref [] // no escape check: escape via type inference
   
   let private noErrorPrivateValueLaterInferredToInvolvePrivateUnionType = ref []  // escape check: escape via type inference
   let private noErrorPrivateValueLaterInferredToInvolveInternalUnionType = ref [] // no escape check: escape via type inference
   
   type Type() = 
       member        private x.PrivateProperty = 3
       static member private PrivateStaticProperty = 3
       member        private x.PrivateMethod() = 3
       static member private PrivateStaticMethod() = 3

       member        internal x.InternalProperty = 3
       static member internal   InternalStaticProperty = 3
       member        internal x.InternalMethod() = 3
       static member internal   InternalStaticMethod() = 3

       // Check we can access private things from this type
       member        x.NoError1 = x.PrivateProperty
       member        x.NoError2 = Type.PrivateStaticProperty
       static member NoError3 = Type.PrivateStaticProperty 
       static member NoError4 = Type.PrivateStaticMethod() 

       member        x.NoError1a = x.InternalProperty
       member        x.NoError2a = Type.InternalStaticProperty
       static member   NoError3a = Type.InternalStaticProperty 
       static member   NoError4a = Type.InternalStaticMethod() 

       static member Method30() = errorPublicValueLaterInferredToInvolvePrivateUnionType := [DefaultTagOfPrivateType(3)]
       static member Method31() = errorPublicValueLaterInferredToInvolveInternalUnionType := [DefaultTagOfInternalType(3)]
       static member Method33() = noErrorInternalValueLaterInferredToInvolveInternalUnionType := [DefaultTagOfInternalType(3)]
       static member Method34() = noErrorPrivateValueLaterInferredToInvolvePrivateUnionType := [DefaultTagOfPrivateType(3)]
       static member Method35() = noErrorPrivateValueLaterInferredToInvolveInternalUnionType := [DefaultTagOfInternalType(3)]

    type Type with 

         // Check we can access private things from an in-file augmentation
         member        x.NoError51 = x.PrivateProperty
         member        x.NoError52 = Type.PrivateStaticProperty
         static member NoError53 = Type.PrivateStaticProperty 
         static member NoError54 = Type.PrivateStaticMethod() 

    

module M2 =
   let private privateValue = 1
   let internal internalValue = 1
   
   type private PrivateUnionType = 
       |          DefaultTagOfPrivateType of int
       //| private  PrivateTagOfPrivateType of int
       //| internal InternalTagOfPrivateType of int
(*
   type private PrivateRecordType = 
       { DefaultFieldOfPrivateType : int }

   type internal InternalUnionType = 
       |          DefaultTagOfInternalType of int
       | private  PrivateTagOfInternalType of int
       | internal InternalTagOfInternalType of int

   type internal InternalRecordType = 
       { DefaultFieldOfInternalType : int }

   type RecordTypeWithPrivateField = 
       {          DefaultFieldOfPublicType : int; 
         private  PrivateFieldOfPublicType : int; 
         internal InternalFieldOfPublicType : int; 
         public   PublicFieldOfPublicType: int }

   let errorPublicValueLaterInferredToInvolvePrivateUnionType = ref []  // escape check: escape via type inference
   let errorPublicValueLaterInferredToInvolveInternalUnionType = ref [] // escape check: escape via type inference

   let internal noErrorInternalValueLaterInferredToInvolveInternalUnionType = ref [] // no escape check: escape via type inference
   
   let private noErrorPrivateValueLaterInferredToInvolvePrivateUnionType = ref []  // escape check: escape via type inference
   let private noErrorPrivateValueLaterInferredToInvolveInternalUnionType = ref [] // no escape check: escape via type inference
   
   type Type() = 
       member        private x.PrivateProperty = 3
       static member private PrivateStaticProperty = 3
       member        private x.PrivateMethod() = 3
       static member private PrivateStaticMethod() = 3

       member        internal x.InternalProperty = 3
       static member internal   InternalStaticProperty = 3
       member        internal x.InternalMethod() = 3
       static member internal   InternalStaticMethod() = 3

       // Check we can access private things from this type
       member        x.NoError1 = x.PrivateProperty
       member        x.NoError2 = Type.PrivateStaticProperty
       static member NoError3 = Type.PrivateStaticProperty 
       static member NoError4 = Type.PrivateStaticMethod() 

       member        x.NoError1a = x.InternalProperty
       member        x.NoError2a = Type.InternalStaticProperty
       static member   NoError3a = Type.InternalStaticProperty 
       static member   NoError4a = Type.InternalStaticMethod() 

       static member Method30() = errorPublicValueLaterInferredToInvolvePrivateUnionType := [DefaultTagOfPrivateType(3)]
       static member Method31() = errorPublicValueLaterInferredToInvolveInternalUnionType := [DefaultTagOfInternalType(3)]
       static member Method33() = noErrorInternalValueLaterInferredToInvolveInternalUnionType := [DefaultTagOfInternalType(3)]
       static member Method34() = noErrorPrivateValueLaterInferredToInvolveInternalUnionType := [DefaultTagOfPrivateType(3)]
       static member Method35() = noErrorPrivateValueLaterInferredToInvolvePrivateUnionType := [DefaultTagOfPrivateType(3)]
       
       *)
       
