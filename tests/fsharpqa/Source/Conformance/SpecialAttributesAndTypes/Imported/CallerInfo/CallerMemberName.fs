namespace Test

open System
open System.Runtime.CompilerServices
open System.Reflection
open CSharpLib

[<assembly:MyCallerMemberNameAttribute>]
do
    ()

[<MyCallerMemberName()>]
type MyTy() =
    let functionVal = MyTy.GetCallerMemberName
    let typeLetValue = MyTy.GetCallerMemberName()
    let typeLetFunc (i:int) = i, MyTy.GetCallerMemberName()
    let typeLetFuncNested () =
        let nestedFunc () = MyTy.GetCallerMemberName()
        nestedFunc ()
    do
        MyTy.Check(MyTy.GetCallerMemberName(), Some(".ctor"), "primary ctor")
    static do
        MyTy.Check(MyTy.GetCallerMemberName(), Some(".cctor"), "static ctor")

    new(i : int) =
        MyTy.Check(MyTy.GetCallerMemberName(), Some(".ctor"), ".NET ctor")
        MyTy()
    
    member __.Item
        with get(i:int) = MyTy.GetCallerMemberName()
        and set(i:int) (v:string option) =
            MyTy.Check(MyTy.GetCallerMemberName(), Some("Item"), "index setter")

    member __.CheckMembers() =
        MyTy.Check(MyTy.GetCallerMemberName(), Some("CheckMembers"), ".NET method")
        MyTy.Check(typeLetValue, Some("typeLetValue"), "type let value")
        MyTy.Check(typeLetFunc 2 |> snd, Some("typeLetFunc"), "type let func")
        MyTy.Check((typeLetFuncNested ()) , Some("typeLetFuncNested"), "type let func nested")
        MyTy.Check(__.GetCallerMemberNameProperty1, Some("GetCallerMemberNameProperty1@"), "auto property getter")
        MyTy.Check(MyTy.GetCallerMemberNameProperty, Some("GetCallerMemberNameProperty"), "property getter")
        MyTy.GetCallerMemberNameProperty <- Some("test")
        MyTy.Check(__.[10], Some("Item"), "indexer getter")
        __.[10] <- Some("test")

        let result =
            [1..10]
            |> List.map (fun i -> MyTy.GetCallerMemberName())
            |> List.head
        MyTy.Check(result, Some("CheckMembers"), "lambda")
        MyTy.Check(functionVal (), Some("functionVal"), "functionVal")
        ()

    static member GetCallerMemberName([<CallerMemberName>] ?memberName : string) =
        memberName

    static member Check(actual : string option, expected : string option, message) =
        printfn "%A" actual
        if actual <> expected then
            failwith message
    
    static member GetCallerMemberNameProperty
        with get () = MyTy.GetCallerMemberName()
        and set (v : string option) =
            MyTy.Check(MyTy.GetCallerMemberName(), Some("GetCallerMemberNameProperty"), "property setter")
        
    member val GetCallerMemberNameProperty1 = MyTy.GetCallerMemberName() with get, set

[<Struct>]
type MyStruct =
    val A : int
    new(a : int) =
        { A = a }
        then
            MyTy.Check(MyTy.GetCallerMemberName(), Some(".ctor"), "struct ctor")

[<Extension>]
type Extensions =
    [<Extension>]
    static member DotNetExtensionMeth(instance : System.DateTime) =
        MyTy.GetCallerMemberName()

type IMyInterface =
    abstract member MyInterfaceMethod : unit -> string option

[<AbstractClass>]
type MyAbstractTy() =
    abstract MyAbstractMethod : unit -> string option

module Program =
    type System.String with
        member __.StringExtensionMeth() =
            MyTy.Check(MyTy.GetCallerMemberName(),Some("StringExtensionMeth"), "extension method")
            1
        member __.StringExtensionProp =
            MyTy.Check(MyTy.GetCallerMemberName(), Some("StringExtensionProp"), "extension property")
            2

    let callerInfoAsFunc = MyTy.GetCallerMemberName
    let rebindFunc = callerInfoAsFunc
    let moduleLetVal = MyTy.GetCallerMemberName()
    let moduleFunc (i : int) = i, MyTy.GetCallerMemberName()
    let moduleFuncNested i =
        let nestedFunc j =
            (j + 1),MyTy.GetCallerMemberName()
        nestedFunc i
    let ``backtick value name`` =  MyTy.GetCallerMemberName()
    let (+++) a b =
        (a+b, MyTy.GetCallerMemberName())

    MyTy.Check(MyTy.GetCallerMemberName(), Some(".cctor"), "module cctor")

    [<EntryPoint>]
    let main (_:string[]) =
        MyTy.Check(MyTy.GetCallerMemberName(), Some("main"), "main")
        
        MyTy.Check(MyTy.GetCallerMemberName("foo"), Some("foo"), "passed value")
        
        MyTy.Check(moduleLetVal, Some("moduleLetVal"), "module let value")

        MyTy.Check(``backtick value name``, Some("backtick value name"), "backtick identifier")

        MyTy.Check(moduleFunc 3 |> snd, Some("moduleFunc"), "module func")

        MyTy.Check(moduleFuncNested 10 |> snd, Some("moduleFuncNested"), "module func nested")

        let inst = MyTy()
        inst.CheckMembers()
        let inst2 = MyTy(2)
        inst2.CheckMembers()

        let v = CallerInfoTest.MemberName()
        MyTy.Check(Some(v), Some("main"), "C# main")

        MyTy.Check(Some(CallerInfoTest.MemberName("foo")), Some("foo"), "C# passed value")
            
        match CallerInfoTest.AllInfo(21) with
        | (_, _, "main") -> ()
        | x -> failwithf "Unexpected C# result with multiple parameter types: %A" x

        MyTy.Check(() |> callerInfoAsFunc, Some("callerInfoAsFunc"), "method as function value 1")
        MyTy.Check(() |> rebindFunc, Some("callerInfoAsFunc"), "method as function value 2")

        let typeAttr = typeof<MyTy>.GetCustomAttributes(typeof<MyCallerMemberNameAttribute>, false).[0] :?> MyCallerMemberNameAttribute
        MyTy.Check(Some(typeAttr.MemberName), Some("dflt"), "attribute on type")

        let asmAttr = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof<MyCallerMemberNameAttribute>, false).[0] :?> MyCallerMemberNameAttribute
        MyTy.Check(Some(asmAttr.MemberName), Some("dflt"), "attribute on asm")

        let s = "123"
        let s1 = s.StringExtensionMeth()
        let s2 = s.StringExtensionProp

        let dt = System.DateTime.Now
        MyTy.Check(dt.DotNetExtensionMeth(), Some("DotNetExtensionMeth"), ".NET extension method")

        let strct = MyStruct(10)

        MyTy.Check(1 +++ 2 |> snd, Some("op_PlusPlusPlus"), "operator")

        let obj = { new IMyInterface with
            member this.MyInterfaceMethod() = MyTy.GetCallerMemberName() }
        MyTy.Check(obj.MyInterfaceMethod(), Some("MyInterfaceMethod"), "Object expression from interface")

        let obj1 = { new MyAbstractTy() with member x.MyAbstractMethod() = MyTy.GetCallerMemberName() }
        MyTy.Check(obj1.MyAbstractMethod(), Some("MyAbstractMethod"), "Object expression from abstract type")

        let asyncVal = async { return MyTy.GetCallerMemberName() } |> Async.RunSynchronously
        MyTy.Check(asyncVal, Some("main"), "Async computation expression value")

        let anonymousLambda = fun () -> MyTy.GetCallerMemberName()
        MyTy.Check(anonymousLambda(), Some("main"), "Anonymous lambda")

        let delegateVal = new Func<string option>(fun () -> MyTy.GetCallerMemberName())
        MyTy.Check(delegateVal.Invoke(), Some("main"), "Delegate value")
        0