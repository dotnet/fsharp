namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Objects that implement this interface know how to write their contents to an <see cref="ObjectWriter"/>
    /// </summary>
    internal interface IObjectWritable
    {
        void WriteTo(ObjectWriter writer);
    }
}
