// Consume a F# Enum from C#
namespace NC
{
    class T
    {
        static int Main ()
        {
            NF.M.SimpleEnum e = NF.M.SimpleEnum.A;
            return (int) e + (int) NF.M.SimpleEnum.B;
        }
    }
}
