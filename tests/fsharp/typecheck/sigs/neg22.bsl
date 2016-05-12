
neg22.fs(13,17,13,23): typecheck error FS0001: Type mismatch. Expecting a
    'float<kg>'    
but given a
    'float<m>'    
The unit of measure 'kg' does not match the unit of measure 'm'

neg22.fs(15,17,15,24): typecheck error FS0001: Type mismatch. Expecting a
    'float<m>'    
but given a
    'float<kg>'    
The unit of measure 'm' does not match the unit of measure 'kg'

neg22.fs(16,20,16,28): typecheck error FS0064: This construct causes code to be less generic than indicated by the type annotations. The unit-of-measure variable 'v has been constrained to be measure 'kg'.

neg22.fs(16,22,16,28): typecheck error FS0064: This construct causes code to be less generic than indicated by the type annotations. The unit-of-measure variable 'u has been constrained to be measure 'm'.

neg22.fs(17,20,17,29): typecheck error FS0001: Type mismatch. Expecting a
    'float<m>'    
but given a
    'float<kg>'    
The unit of measure 'm' does not match the unit of measure 'kg'

neg22.fs(28,12,28,18): typecheck error FS0957: The declared type parameters for this type extension do not match the declared type parameters on the original type 'LibGen<_>'

neg22.fs(40,12,40,18): typecheck error FS0957: The declared type parameters for this type extension do not match the declared type parameters on the original type 'LibGen<_>'

neg22.fs(33,17,33,19): typecheck error FS0341: The signature and implementation are not compatible because the type parameter 'T' has a constraint of the form 'T :> System.ValueType but the implementation does not. Either remove this constraint from the signature or add it to the implementation.
