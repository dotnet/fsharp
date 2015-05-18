module Test


module Test1 = 
    type SomeClass(x : 'T) = class end
        //member this.P = 1
        //member this.X = x

    let SomeFunc<'U> (x : 'U) =
        SomeClass(x)

module Test2 = 
    open System

    type SomeClass(updater : #IComparable->unit) =
        let onLoaded () = 
            let adorner : #IComparable = failwith ""
            updater adorner
        member this.OnLoaded = onLoaded

    let UpdateAdorner (updater : #IComparable->unit)=
        let updater = SomeClass (updater)
        updater.OnLoaded
