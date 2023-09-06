open Prelude

module TestAssignToReturnByref = 
    type C() = 
        static let mutable v = System.DateTime.Now
        static member M() = &v
        static member P = &v
        member __.InstanceM() = &v
        member __.InstanceP with get() = &v
        static member Value = v

    let F1() = 
        let today = System.DateTime.Now.Date
        C.M() <-  today
        check "cwecjc" C.Value  today
        C.P <- C.M().AddDays(1.0)
        check "cwecjc1" C.Value (today.AddDays(1.0))
        let c = C() 
        c.InstanceM() <-  today.AddDays(2.0)
        check "cwecjc2" C.Value (today.AddDays(2.0))
        c.InstanceP <-  today.AddDays(3.0)
        check "cwecjc1" C.Value (today.AddDays(3.0))

    F1()