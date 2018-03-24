// #ErrorMessages #NameResolution 
//<Expects status="error" span="(4,36)" id="FS1126">'global' may only be used as the first name in a qualified path</Expects>

let x = global.System.String.Empty.global.System.String.Empty

exit 0
