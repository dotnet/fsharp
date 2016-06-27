// #Warnings
//<Expects status="Error" span="(6,8)" id="FS0952">The accessibility specified for the type abbreviation 'Exported' is more than that specified for the abbreviated type 'Hidden'.</Expects>

module Library =
  type internal Hidden = Hidden of unit
  type Exported = Hidden
    
exit 0