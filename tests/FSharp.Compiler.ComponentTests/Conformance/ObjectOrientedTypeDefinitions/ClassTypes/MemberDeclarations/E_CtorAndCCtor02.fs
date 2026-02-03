// #Conformance #Regression
// It is just illegal to use '.ctor' or '.cctor' as member names
// The following code should NOT compile
//<Expects status="error" span="(9,21-9,30)" id="FS3066">Invalid member name\. Members may not have name '\.ctor' or '\.cctor'$</Expects>
//<Expects status="error" span="(12,21-12,31)" id="FS3066">Invalid member name\. Members may not have name '\.ctor' or '\.cctor'$</Expects>
namespace N

type I1 =
    abstract member ``.ctor`` : int -> int

type I2 =
    abstract member ``.cctor`` : int -> int
