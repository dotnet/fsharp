// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
#light

// Regression test for FSharp1.0:4939
// Title: Make overload resolution in the presence of extension members work more like C#

module Test

type System.Object with 
    member x.Equals(a:int,b:int) = (a = b)
    member x.Equals(a:int) = true
    member x.Equals(a:obj) = false // NOTE : this one will always be ignored. It is uncallable

let mutable res = 
    not ("3".Equals(3,4)) &&  // expect: false  
    "3".Equals(3,3)       &&  // expect: true
    "3".Equals("3")       &&  // expect: true --> calls System.Object.Equals(obj)
    "3".Equals(2)         &&  // expect: true --> calls extension member override on "int"
    "3".Equals(box "3")       // expect: true --> since extension member is uncallable


type T = 
    class
        val mutable public X : int
        public new (i : int) = { X = i }
        
        member this.Incr (i : int) = this.X <- this.X + i
        static member CreateOne () = new T(0)
    end
    
type System.Convert with
    static member ToInt32(o : T) = o.X

res <- res && (System.Convert.ToInt32(new T(5)) = 5)

    
type T with
    member this.Incr(d : float) = this.Incr((int d))
    static member CreateOne (s : string) = new T(s.Length)

let tmp = new T(10)
res <- res
    && tmp.Incr(5) = tmp.Incr(5.2)
    && T.CreateOne().X = T.CreateOne("").X
    

if res <> true then exit 1
