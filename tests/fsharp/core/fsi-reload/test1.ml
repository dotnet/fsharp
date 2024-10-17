// #Conformance #FSI 

#light

#load "test1a.ml";;

printf "test: %d\n" Test1.x;;
printf "test: %b\n" (Test1.X Test1.x =  Test1.X 3) ;;
printf "test: %d\n" (3 : Test1.x_t);;
printf "test: %d\n" Test1.Nested.x;;
printf "test: %b\n" (Test1.Nested.X Test1.Nested.x = Test1.Nested.X 3) ;;
printf "test: %d\n" (3 : Test1.Nested.x_t);;
open Test1
printf "test: %d\n" x;;
printf "test: %b\n" (X x = X 1) ;;
printf "test: %d\n" (1 : x_t);;
printf "x = %d\n" Nested.x;;
printf "x = %b\n" (Nested.X Nested.x = Nested.X 3) ;;
printf "x = %d\n" (3 : Nested.x_t);;
open Nested
printf "x = %d\n" x;;
printf "x = %b\n" (X x = X 3) ;;
printf "x = %d\n" (3 : x_t);;

#load "test1b.ml";;

printf "test: %d\n" Test1.y
printf "test: %d\n" (3 : Test1.y_t)
open Test1
printf "test: %d\n" y
printf "test: %d\n" (3 : y_t)

printf "test: %s\n" Test1.x;;
printf "test: %b\n" (Test1.X Test1.x = Test1.X "a") ;;
printf "test: %s\n" ("a\n" : Test1.x_t);;
printf "test: %s\n" Test1.Nested.x;;
printf "test: %b\n" (Test1.Nested.X Test1.x =  Test1.Nested.X "a") ;;
printf "test: %s\n" ("a\n" : Test1.Nested.x_t);;
open Test1
printf "test: %s\n" x;;
printf "test: %b\n" (X x = X "a") ;;
printf "test: %s\n" ("a\n" : x_t);;
printf "test: %s\n" Nested.x;;
printf "test: %b\n" (Nested.X Test1.x = Nested.X "a") ;;
printf "test: %s\n" ("a\n" : Nested.x_t);;
open Nested
printf "test: %s\n" x;;
printf "test: %b\n" (X x = X "a") ;;
printf "test: %s\n" ("a\n" : x_t);;


// Back to test1a.  Not this all happens in one interaction.
#load "test1a.ml"


printf "test: %d\n" Test1.x
printf "test: %b\n" (Test1.X Test1.x =  Test1.X 3) 
printf "test: %d\n" (3 : Test1.x_t)
printf "test: %d\n" Test1.Nested.x
printf "test: %b\n" (Test1.Nested.X Test1.Nested.x = Test1.Nested.X 3) 
printf "test: %d\n" (3 : Test1.Nested.x_t)
open Test1
printf "test: %d\n" x
printf "test: %b\n" (X x = X 1) 
printf "test: %d\n" (1 : x_t)
printf "x = %d\n" Nested.x
printf "x = %b\n" (Nested.X Nested.x = Nested.X 3) 
printf "x = %d\n" (3 : Nested.x_t)
open Nested
printf "x = %d\n" x
printf "x = %b\n" (X x = X 3) 
printf "x = %d\n" (3 : x_t)
;;

begin ignore (3 : x_t); ignore (3 : Nested.x_t); ignore (3 : Test1.Nested.x_t); ignore (3 : Test1.x_t); printf "TEST PASSED OK" end;;
#quit;; 

