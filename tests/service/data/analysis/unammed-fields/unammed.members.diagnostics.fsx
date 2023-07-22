type TopLevelDU =
| TopLevelCase1 of int
| TopLevelCase2 of ok: int * okok: string
| TopLevelCase3 of int * noOkCase: string

module Outer =
  type DU =
  | Case1 of int
  | Case2 of ok: int * okok: string
  | Case3 of int * noOkCase: string

  exception Exception1 of int
  exception Exception2 of ok: int * okok: string
  exception Exception3 of int * noOkCase: string  
  
  module Inner =
    type InnerDU =
    | InnerCase1 of int
    | InnerCase2 of ok: int * okok: string
    | InnerCase3 of int * noOkCase: string
    
    exception InnerException1 of int
    exception InnerException2 of ok: int * okok: string
    exception InnerException3 of int * noOkCase: string

    type PrivateDU = private PrivateCase1 of int * string
    exception private PrivateException of int * string