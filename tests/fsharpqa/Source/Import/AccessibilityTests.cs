using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FamAndAssembly")]
[assembly: InternalsVisibleTo("FamOrAssembly")]


public class Accessibility
{
    public int Public { get; set; }
    private int Private { get; set; }
    protected int Protected { get; set; }
    internal int Internal { get; set; }
    // Note: accessibility was modified using dnspy
    public int FamOrAssembly { get; set; }
    // Note: accessibility was modified using dnspy
    public int FamAndAssembly { get; set; }
}

