// #Warnings
//<Expects status="success"></Expects>

module Library =
  type Hidden = Hidden of unit
  type Exported = Hidden
    
exit 0