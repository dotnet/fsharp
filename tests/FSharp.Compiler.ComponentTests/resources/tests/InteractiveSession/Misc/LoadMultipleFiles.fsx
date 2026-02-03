// #NoMT #FSI 
// Load multiple files in FSI
//<Expects status="success">\[.+LoadFile01\.fsx</Expects>
//<Expects status="success">.+LoadFile02\.fsx\]</Expects>

#load "LoadFile01.fsx" "LoadFile02.fsx";;

exit 0;;
