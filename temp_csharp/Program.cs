using System.Runtime.CompilerServices;

public class Test
{
    public async Task<int> AsyncMethod()
    {
        await Task.Delay(1000);
        
        return 42;
    }
}
