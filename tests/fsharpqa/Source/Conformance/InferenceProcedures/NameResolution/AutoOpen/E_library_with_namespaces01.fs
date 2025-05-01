// #Regression #Conformance #TypeInference #Attributes #ReqNOMT 
// The Expects here is an artifact to make our automation happy.
// It actually refers to E_Module02.fs
//<Expects id="FS0039" span="(10,22-10,24)" status="error">The type 'C2' is not defined</Expects>
#light

namespace XX.YY.ZZ
   type T3 = decimal []
   type C3() = class
               end
   type G3<'a> = class
                 end
   type G4<'a> = class
                 end

namespace XX.YY
      type T2 = decimal []
      type C2() = class
                  end
      type G2<'a> = class
                    end
 
namespace XX
      type T1 = decimal []
      type C1() = class
                  end
      type G1<'a> = class
                    end
