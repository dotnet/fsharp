// #Conformance #LexFilter #Precedence 
#light

let mutable z=0;; // #light must not touch these. (non-#light will)
let mutable y=0;;
let mutable x=1;; // #light must reset these. (non-#light will not)
let mutable w=1;;


// In #light this means �if x then (y;z)�, in non-#light means �(if x then y); z�. 
if false then (); z<-z+1
;;


//In #light means �if x then (if y then z else w)�, in non-#light means �if x then (if y then z) else w�
if false then 
   if true then 
       ()
else 
   x<-0
;;

if false then 
   if false  then 
       ()
else 
   w<-0
;;

if true then 
   if false then 
       ()
else 
   y<-y+1
;;

exit ((1000*w)+(100*x)+(10*y)+z)
