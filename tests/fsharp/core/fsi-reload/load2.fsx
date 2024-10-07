// Test the case where b2.fsi/fs is in the same namespace as b1.fs, and so the namespace is implicitly opened
#load "b1.fs"
#load "b2.fsi" "b2.fs" 

printf "TEST PASSED OK" ; 
