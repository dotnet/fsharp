// #Warnings
//<Expects status="warning" id="FS0044">The type 'Hidden' is less accessible than the value, member or type 'Exported' it is used in</Expects>

module Library =
  type private Hidden = Hidden of unit
  type Exported = Hidden
    
exit 0