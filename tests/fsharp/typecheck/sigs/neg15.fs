module Test


// TODO: private and internal module checking
// TODO: implement escape checks


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

   type PublicRecordType = 
       {          DefaultFieldOfPublicType : int; 
         //private  PrivateFieldOfPublicType : int; 
         //internal InternalFieldOfPublicType : int; 
                  PublicFieldOfPublicType: int }

   let errorPublicValueLaterInferredToInvolvePrivateUnionType = ref []  // escape via type inference
   let errorPublicValueLaterInferredToInvolveInternalUnionType = ref [] // escape via type inference

   let internal errorInternalValueLaterInferredToInvolvePrivateUnionType = ref []  // escape via type inference
   let internal noErrorInternalValueLaterInferredToInvolveInternalUnionType = ref [] // escape via type inference
   
   let private noErrorPrivateValueLaterInferredToInvolvePrivateUnionType = ref []  // escape via type inference
   let private noErrorPrivateValueLaterInferredToInvolveInternalUnionType = ref [] // escape via type inference
   
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

       member          x.Error1 = { DefaultFieldOfPrivateType=3 } // returning a private type from a public method
       member          x.Error2 = DefaultTagOfPrivateType(3) // returning a private type from a public method
       member          x.Error3 = { DefaultFieldOfInternalType=3 } // returning a internal type from a public method
       member          x.Error4 = DefaultTagOfPrivateType(3) // returning a internal type from a public method
       member internal x.Error5 = { DefaultFieldOfPrivateType=3 } // returning a private type from a internal method
       member internal x.Error6 = DefaultTagOfPrivateType(3) // returning a private type from a internal method
       member          x.Error7(y:PrivateUnionType) = () // accepting private type as argument to an public method
       member          x.Error8(y:PrivateRecordType) = () // accepting private type as argument to an public method
       member          x.Error9(y:InternalUnionType) = () // accepting internal type as argument to an public method
       member          x.Error10(y:InternalRecordType) = () // accepting internal type as argument to an public method
       member internal x.Error11(y:PrivateUnionType) = () // accepting private type as argument to an internal method
       member internal x.Error12(y:PrivateRecordType) = () // accepting private type as argument to an internal method


       static member Method30() = errorPublicValueLaterInferredToInvolvePrivateUnionType := [DefaultTagOfPrivateType(3)]
       static member Method31() = errorPublicValueLaterInferredToInvolveInternalUnionType := [DefaultTagOfInternalType(3)]
       static member Method32() = errorInternalValueLaterInferredToInvolvePrivateUnionType := [DefaultTagOfPrivateType(3)]
       static member Method33() = noErrorInternalValueLaterInferredToInvolveInternalUnionType := [DefaultTagOfInternalType(3)]
       static member Method34() = noErrorPrivateValueLaterInferredToInvolvePrivateUnionType := [DefaultTagOfPrivateType(3)]
       static member Method35() = noErrorPrivateValueLaterInferredToInvolveInternalUnionType := [DefaultTagOfInternalType(3)]

    type Type with 

         // Check we can access private things from an in-file augmentation
         member        x.NoError51 = x.PrivateProperty
         member        x.NoError52 = Type.PrivateStaticProperty
         static member NoError53 = Type.PrivateStaticProperty 
         static member NoError54 = Type.PrivateStaticMethod() 

           
module N =
   let error1 = M.privateValue
   let error2a = M.DefaultTagOfPrivateType(3)
   //let error2b = M.PrivateTagOfPrivateType(3)
   //let noError2c = M.InternalTagOfPrivateType(3)
   let error2d = M.DefaultTagOfUnionTypeWithPrivateRepresentation(3)
   let error3 = M.Type().PrivateProperty
   let error4 = M.Type.PrivateStaticProperty
   let error5 = M.Type().PrivateMethod()
   let error6 = M.Type.PrivateStaticMethod()
   let error7 = { M.DefaultFieldOfPrivateType = 3 }
   let error7a = { M.DefaultFieldOfRecordTypeWithPrivateRepresentation = 3  }
   let error7a1 { M.DefaultFieldOfRecordTypeWithPrivateRepresentation = x } = x
   let error7a2 { M.DefaultFieldOfPrivateType = x } = x
   let error8 x = x.M.DefaultFieldOfPrivateType
   let error8a x = x.M.DefaultFieldOfRecordTypeWithPrivateRepresentation 
   //let error9 x = x.M.PrivateFieldOfPublicType

   let internal noError41 = M.internalValue
   let internal noError42a = M.DefaultTagOfInternalType(3)
   //let internal error42b = M.PrivateTagOfInternalType(3)
   let internal noError42c = M.InternalTagOfInternalType(3)
   let internal noError43 = M.Type().InternalProperty
   let internal noError44 = M.Type.InternalStaticProperty
   let internal noError45 = M.Type().InternalMethod()
   let internal noError46 = M.Type.InternalStaticMethod()
   let internal noError47 = { M.DefaultFieldOfInternalType = 3 }
   let internal noError48 x = x.M.DefaultFieldOfInternalType
   //let internal noError49 x = x.M.InternalFieldOfPublicType
   let noError410 (x:M.PublicRecordType) = ()

   let private noError71 = M.internalValue
   let private noError72a = M.DefaultTagOfInternalType(3)
   //let private error72b = M.PrivateTagOfInternalType(3)
   let private noError72c = M.InternalTagOfInternalType(3)
   let private noError73 = M.Type().InternalProperty
   let private noError74 = M.Type.InternalStaticProperty
   let private noError75 = M.Type().InternalMethod()
   let private noError76 = M.Type.InternalStaticMethod()
   let private noError77 = { M.DefaultFieldOfInternalType = 3 }
   let private noError78 x = x.M.DefaultFieldOfInternalType
   //let private noError79 x = x.M.InternalFieldOfPublicType
   let noError10 (x:M.PublicRecordType) = ()

   let noError21 = M.internalValue  
   let error22   = M.DefaultTagOfInternalType(3) // returning internal type as public value
   let noError23 = M.Type().InternalProperty
   let noError24 = M.Type.InternalStaticProperty
   let noError25 = M.Type().InternalMethod()
   let noError26 = M.Type.InternalStaticMethod()
   let error27 = { M.DefaultFieldOfInternalType = 3 } // returning internal type as public value
   let error28 x = x.M.DefaultFieldOfInternalType      // accepting internal type as argument to public function
   //let error29 x = x.M.InternalFieldOfPublicType // returning internal type as argument to public function

type X() = 
    member private x.P = 1
    member private x.M() = 1
    static member private SP = 1
    static member private SM() = 1
    member internal x.IP = 1
    member internal x.IM() = 1
    static member internal SIP = 1
    static member internal SIM() = 1

type X2 = 
    new() = { f = 1; f2 = 1 }
    private new (dummy:int) = { f = 1; f2 = 1 }
    val private f : int
    member private x.P = 1
    member private x.M() = 1
    static member private SP = 1
    static member private SM() = 1

    val internal f2 : int
    member internal x.IP = 1
    member internal x.IM() = 1
    static member internal SIP = 1
    static member internal SIM() = 1


let xx = X() 
let xx2 = X2()
xx.P   // This now gives access error
xx.M()   // This now gives access error
X.SP   // This now gives access error
X.SM()   // This now gives access error
xx2.f   // This now gives access error
xx2.P   // This now gives access error
xx2.M()   // This now gives access error
X2.SP   // This now gives access error
X2.SM()   // This now gives access error

xx.IP   // no access error
xx.IM()   // no access error
X.SIP   // no access error
X.SIM()   // no access error
xx2.f2   // no access error
xx2.IP   // no access error
xx2.IM()   // no access error
X2.SIP   // no access error
X2.SIM()   // no access error


let xx3 = X2(2) // should give an access error 
