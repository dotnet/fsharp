type A() =
    static member A ((a,(b,(c:int)),d),e) = a,b,c,d,e
    static member A ((a,(b,(c:int)),d),(e,(f,g))) = a,b,c,d,e,f,g
    ;;
    
let a = A.A ((1,(2,("")),4),5);;
let a = A.A("a",("",1),1,1);;


type B =
    static member B (a,b,(c,(d,e,f,(g,h:string))),i,j)         = a,b,c,d,e,f,g,h,i,j
    static member B (a,b,(c,(d,e,f,(g,h:decimal))),i,j)         = a,b,c,d,e,f,g,h,i,j
;;
let b = B.B(1,2,(3,(4,5,6,(7,8))),9,10);;
