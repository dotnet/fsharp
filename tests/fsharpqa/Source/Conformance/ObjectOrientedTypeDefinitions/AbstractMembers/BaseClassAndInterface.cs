namespace CSLibrary
{
    public interface I
    {
        void M();
    }

    public class C : I
    {
        public virtual void M() { System.Console.WriteLine("I am my method"); }

        void I.M()
        {
            System.Console.WriteLine("I am going via the interface");
            M();
        }
    }

}
