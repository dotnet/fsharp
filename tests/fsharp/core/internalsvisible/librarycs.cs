[assembly:System.Runtime.CompilerServices.InternalsVisibleTo("main, PublicKey=0024000004800000940000000602000000240000525341310004000001000100DDA0D353F027AB6ADC878BFBFE4A07FB00FFD2EDD5F255A14C6474B7F4E561796822B6B3CF83D81716C6AFE9BE5D343D7F99EF98252EAD91E7C3C4DF043FDD71FD3130F6611C3C0F7D4F3E698491E9B74D4DE456042A737F4FC5443A98BF989B7377BEE0969C58B85C26B48EF94FFBC95E68E10545FB573243E249204921AFB8")]
namespace LibraryCS
{
        public class APublicClass
        {
                private  static int PrivateProperty    { get { return 2; } }
                internal static int InternalProperty   { get { return 2; } }
        }
        internal class AInternalClass
        {
                private  static int PrivateProperty    { get { return 2; } }
                internal static int InternalProperty   { get { return 2; } }
        }
        internal class APrivateClass
        {
                private  static int PrivateProperty    { get { return 2; } }
                internal static int InternalProperty   { get { return 2; } }
        }
    public class Class1
    {
        public Class1() { }
        protected static int ProtectedStatic;
        internal static int InternalStatic;
        protected internal static int ProtectedInternalStatic;
        public static int PublicStatic;
        protected int Protected;
        internal int Internal;
        protected internal int ProtectedInternal;
        public int Public;
    }
}
namespace Repro
{
    internal delegate string D(int x);
    internal class C { public C(D d) { } }
}