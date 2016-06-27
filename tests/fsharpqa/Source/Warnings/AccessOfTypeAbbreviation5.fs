// #Warnings
//<Expects status="success"></Expects>

module Library =
  type private Hidden = Hidden of unit
  type private Exported = Hidden
    
exit 0