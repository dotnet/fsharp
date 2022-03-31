// #NoMono #NoMT #CodeGen #EmittedIL #Tuples   
type A() = class end

// A code+optimization pattern, see https://github.com/Microsoft/visualfsharp/issues/6532
type C() = 
    static member inline F (?x1: A, ?x2: A) = 
        let count = 0
        let count = match x1 with None -> count | Some _ -> count + 1
        let count = match x2 with None -> count | Some _ -> count + 1
        let attribs = ResizeArray<_>(count)
        match x1 with None -> () | Some v1 -> attribs.Add(v1)
        match x2 with None -> () | Some v2 -> attribs.Add(v2)
        attribs

//Expect rough equivalent of:
//        let d = ResizeArray<_>(0)
//        d
let test() = 
    C.F ()

//Expect rough equivalent of:
//        let x1 = A()
//        let d = ResizeArray<_>(1)
//        d.Add(x1)
//        d
let test2() = 
    C.F (x1=A())

//Expect rough equivalent of:
//        let x2 = A()
//        let d = ResizeArray<_>(1)
//        d.Add(x2)
//        d
let test3() = 
    C.F (x2=A())

//Expect rough equivalent of:
//        let x1 = A()
//        let x2 = A()
//        let d = ResizeArray<_>(2)
//        d.Add(x1)
//        d.Add(x2)
//        d
let test4() = 
    C.F (x1=A(), x2=A())

