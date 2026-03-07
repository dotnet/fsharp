// #NoMT #FSI 
// Verify FSI bails after the first error it finds

let x = 1
this is not valid code

exit 1;;


#load "File does not exist"

exit 1;;

exit 0;;
