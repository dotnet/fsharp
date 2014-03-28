public abstract class Base
{
    public abstract void PubF();
    internal abstract void IntG();
}

public abstract class Mid : Base
{
    internal override void IntG()
    {
    }
}


/////////////////////////////////////

// For regression of FSB 4205

public abstract class Bug4205_CSBase
{
    public virtual int Prop { get { return 42; } }
}

public abstract class Bug4205_CSDerived : Bug4205_CSBase
{
    // Demote virtual method to pure virtual / abstract
    public override abstract int Prop { get; }
}