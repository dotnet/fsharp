namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Objects that implement this interface know how to write their contents to an <see cref="ObjectWriter"/>,
    /// so they can be reconstructed later by an <see cref="ObjectReader"/>.
    /// </summary>
    internal interface IObjectWritable
    {
        void WriteTo(ObjectWriter writer);
    }
}
