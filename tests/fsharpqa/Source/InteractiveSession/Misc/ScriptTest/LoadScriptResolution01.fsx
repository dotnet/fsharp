// Regression for Dev11:14444
// Previously fsc would fail to load script1.fsx because it wasn't loading relative to the directory where the script is

#load "script1.fsx"

exit 0