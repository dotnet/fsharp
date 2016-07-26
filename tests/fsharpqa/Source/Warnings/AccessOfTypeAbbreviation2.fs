// #Warnings
//<Expects status="success"></Expects>

module Library =
  type internal Hidden = Hidden of unit
  type internal Exported = Hidden
    
exit 0