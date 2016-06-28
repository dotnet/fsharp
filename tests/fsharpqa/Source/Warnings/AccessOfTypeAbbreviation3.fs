// #Warnings
//<Expects status="Error" id="FS0410">The type 'Hidden' is less accessible than the value, member or type 'Exported' it is used in.</Expects>

module Library =
  type internal Hidden = Hidden of unit
  type Exported = Hidden
    
exit 0