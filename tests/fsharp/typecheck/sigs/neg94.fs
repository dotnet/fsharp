namespace Neg94

module Repro1 = 
  let v = Neg94Pre.Class1()

  let v2 = undefined  
  // We're expecting the compile of this file to fail, but the point is that the 
  // reference of neg94-pre.dll shouldn't give extra warnings about bad definitions.
  // See 
  //    https://github.com/Microsoft/visualfsharp/issues/631
  //    https://github.com/Microsoft/visualfsharp/issues/592

