open Prelude

module TestAssignToReturnByref2 = 
    let mutable v = System.DateTime.Now
    let M() = &v

    let F1() = 
        let today = System.DateTime.Now.Date
        M() <-  today
        check "cwecjc" v  today

    F1()