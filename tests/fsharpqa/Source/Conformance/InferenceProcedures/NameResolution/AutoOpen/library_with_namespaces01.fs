// #Conformance #TypeInference #Attributes #ReqNOMT 
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
