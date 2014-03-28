module Neg11
// check that a decent error is given when a constructor is too polymorphic
type Gaussian1D =
   class 
      val p : float;
      new (precisionMean:'a) = { p = 0.0 }
  end

// note this test shouldn't contain any other code
