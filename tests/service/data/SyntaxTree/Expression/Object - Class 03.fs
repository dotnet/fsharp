module Module

{ new T() with
      override _.P1 = 1

  interface I1 with
      member _.P2 = 2

  interface I2 with
      member _.P3 = 3 }
