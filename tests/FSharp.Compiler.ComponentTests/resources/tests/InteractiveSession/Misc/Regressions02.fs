// #Regression #NoMT #FSI 
// Regression for FSB 3739
// ICE in fsi with interface constraint on type generic parameter (for interface defined in the same interaction) [was:  ICE when feeding code to fsi.exe]

type IA = 
    abstract AbstractMember : int -> int

type IB = 
    abstract AbstractMember : int -> int

type C<'a when 'a :> IB>() = 
    static member StaticMember(x:'a) = x.AbstractMember(1)

;;

type Tester() =
    interface IB with
        override this.AbstractMember x = -x


if C<Tester>.StaticMember( new Tester() ) <> -1 then
    exit 1


exit 0;;
    
