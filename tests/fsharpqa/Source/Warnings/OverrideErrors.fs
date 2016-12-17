// #Warnings
//<Expects status="Error" span="(12,16)" id="FS0856">This override takes a different number of arguments to the corresponding abstract member. The following abstract members were found:</Expects>
//<Expects>abstract member Base.Member : int * string -> string</Expects>
//<Expects status="Error" span="(20,24)" id="FS0001">This expression was expected to have type</Expects>

type Base() =
    abstract member Member: int * string -> string
    default x.Member (i, s) = s

type Derived1() =
    inherit Base()
    override x.Member() = 5

type Derived2() =
    inherit Base()
    override x.Member (i : int) = "Hello"

type Derived3() =
    inherit Base()
    override x.Member (s : string, i : int) = sprintf "Hello %s" s
    
exit 0