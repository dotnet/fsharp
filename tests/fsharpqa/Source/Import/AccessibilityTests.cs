using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FamAndAssembly")]
[assembly: InternalsVisibleTo("FamOrAssembly")]


public class Accessibility
{
    public int Public { get; set; }
    private int Private { get; set; }
    protected int Protected { get; set; }
    internal int Internal { get; set; }
    protected internal int FamOrAssembly { get; set; }
    private protected int FamAndAssembly { get; set; }
}

