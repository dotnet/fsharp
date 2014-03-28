
module Test

open Neg17
           
module N =
   let error1 = M.privateValue
   let error2a = M.DefaultTagOfPrivateType(3)
   //let error2b = M.PrivateTagOfPrivateType(3)
   //let noError2c = M.InternalTagOfPrivateType(3)
   let error3 = M.Type().PrivateProperty
   let error4 = M.Type.PrivateStaticProperty
   let error5 = M.Type().PrivateMethod()
   let error6 = M.Type.PrivateStaticMethod()
   let error7 = { M.DefaultFieldOfPrivateType = 3 }
   let error8 x = x.M.DefaultFieldOfPrivateType
   let error9 x = x.M.PrivateFieldOfPublicType

   let internal noError41 = M.internalValue
   let internal noError42a = M.DefaultTagOfInternalType(3)
   let internal error42a1 = M.DefaultTagOfUnionTypeWithPrivateRepresentation(3)
   //let internal error42b = M.PrivateTagOfInternalType(3)
   //let internal noError42c = M.InternalTagOfInternalType(3)
   let internal noError43 = M.Type().InternalProperty
   let internal noError44 = M.Type.InternalStaticProperty
   let internal noError45 = M.Type().InternalMethod()
   let internal noError46 = M.Type.InternalStaticMethod()
   let internal noError47 = { M.DefaultFieldOfInternalType = 3 }
   let internal noError48 x = x.M.DefaultFieldOfInternalType
   let internal error48a1 x = x.M.DefaultFieldOfRecordTypeWithPrivateRepresentation
   //let internal noError49 x = x.M.InternalFieldOfPublicType
   let noError410 (x:M.RecordTypeWithPrivateField) = ()

   let private noError71 = M.internalValue
   let private noError72a = M.DefaultTagOfInternalType(3)
   //let private error72b = M.PrivateTagOfInternalType(3)
   //let private noError72c = M.InternalTagOfInternalType(3)
   let private noError73 = M.Type().InternalProperty
   let private noError74 = M.Type.InternalStaticProperty
   let private noError75 = M.Type().InternalMethod()
   let private noError76 = M.Type.InternalStaticMethod()
   let private noError77 = { M.DefaultFieldOfInternalType = 3 }
   let private noError78 x = x.M.DefaultFieldOfInternalType
   //let private noError79 x = x.M.InternalFieldOfPublicType
   let noError10 (x:M.RecordTypeWithPrivateField) = ()

   let noError21 = M.internalValue  
   let error22   = M.DefaultTagOfInternalType(3) // returning internal type as public value
   let noError23 = M.Type().InternalProperty
   let noError24 = M.Type.InternalStaticProperty
   let noError25 = M.Type().InternalMethod()
   let noError26 = M.Type.InternalStaticMethod()
   let error27 = { M.DefaultFieldOfInternalType = 3 } // returning internal type as public value
   let error28 x = x.M.DefaultFieldOfInternalType      // accepting internal type as argument to public function
   //let error29 x = x.M.InternalFieldOfPublicType // returning internal type as argument to public function


       