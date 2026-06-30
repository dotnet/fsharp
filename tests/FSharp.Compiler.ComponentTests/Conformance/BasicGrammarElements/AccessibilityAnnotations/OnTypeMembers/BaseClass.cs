namespace TestBaseClass
{
	public class BaseClass
	{
		protected static int ProtectedStatic() { return 3; }
		protected int ProtectedInstance() { return 4; }
		protected string ProtectedField = "protected-field";
		protected static string ProtectedStaticField = "protected-static-field";
	}
}