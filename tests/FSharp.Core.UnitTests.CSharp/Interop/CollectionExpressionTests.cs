using Microsoft.FSharp.Collections;

namespace FSharp.Core.UnitTests.CSharp.Interop;

// The CollectionBuilderAttribute type is only available in .NET 8 and up.
#if NET8_0_OR_GREATER
public class CollectionExpressionTests
{
    private sealed record RecordClass(int X) : IComparable
    {
        public int CompareTo(object? obj) => obj switch
        {
            null => 1,
            RecordClass(var otherX) => X.CompareTo(otherX),
            _ => throw new ArgumentException("Invalid comparison.", nameof(obj))
        };
    }

    private readonly record struct RecordStruct(int X) : IComparable
    {
        public int CompareTo(object? obj) => obj switch
        {
            null => 1,
            RecordStruct(var otherX) => X.CompareTo(otherX),
            _ => throw new ArgumentException("Invalid comparison.", nameof(obj))
        };
    }

    [Fact]
    public void FSharpList_Int_CanCreateUsingCollectionExpression()
    {
        var expected = ListModule.OfArray([1, 2, 3]);
        FSharpList<int> actual = [1, 2, 3];

        Assert.Equal(expected, actual);

        Assert.Collection(
            actual,
            item1 => Assert.Equal(1, item1),
            item2 => Assert.Equal(2, item2),
            item3 => Assert.Equal(3, item3));

        Assert.True(actual is [1, 2, 3]);
    }

    [Fact]
    public void FSharpList_RecordClass_CanCreateUsingCollectionExpression()
    {
        var expected = ListModule.OfArray([new RecordClass(1), new RecordClass(2), new RecordClass(3)]);
        FSharpList<RecordClass> actual = [new RecordClass(1), new RecordClass(2), new RecordClass(3)];

        Assert.Equal(expected, actual);

        Assert.Collection(
            actual,
            item1 => Assert.Equal(new RecordClass(1), item1),
            item2 => Assert.Equal(new RecordClass(2), item2),
            item3 => Assert.Equal(new RecordClass(3), item3));

        Assert.True(actual is [RecordClass(1), RecordClass(2), RecordClass(3)]);
    }

    [Fact]
    public void FSharpList_RecordStruct_CanCreateUsingCollectionExpression()
    {
        var expected = ListModule.OfArray([new RecordStruct(1), new RecordStruct(2), new RecordStruct(3)]);
        FSharpList<RecordStruct> actual = [new RecordStruct(1), new RecordStruct(2), new RecordStruct(3)];

        Assert.Equal(expected, actual);

        Assert.Collection(
            actual,
            item1 => Assert.Equal(new RecordStruct(1), item1),
            item2 => Assert.Equal(new RecordStruct(2), item2),
            item3 => Assert.Equal(new RecordStruct(3), item3));

        Assert.True(actual is [RecordStruct(1), RecordStruct(2), RecordStruct(3)]);
    }

    [Fact]
    public void FSharpSet_Int_CanCreateUsingCollectionExpression()
    {
        var expected = SetModule.OfArray([1, 2, 3]);
        FSharpSet<int> actual = [1, 2, 3];

        Assert.Equal(expected, actual);

        Assert.Collection(
            actual,
            item1 => Assert.Equal(1, item1),
            item2 => Assert.Equal(2, item2),
            item3 => Assert.Equal(3, item3));
    }

    [Fact]
    public void FSharpSet_RecordClass_CanCreateUsingCollectionExpression()
    {
        var expected = SetModule.OfArray([new RecordClass(1), new RecordClass(2), new RecordClass(3)]);
        FSharpSet<RecordClass> actual = [new RecordClass(1), new RecordClass(2), new RecordClass(3)];

        Assert.Equal(expected, actual);

        Assert.Collection(
            actual,
            item1 => Assert.Equal(new RecordClass(1), item1),
            item2 => Assert.Equal(new RecordClass(2), item2),
            item3 => Assert.Equal(new RecordClass(3), item3));
    }

    [Fact]
    public void FSharpSet_RecordStruct_CanCreateUsingCollectionExpression()
    {
        var expected = SetModule.OfArray([new RecordStruct(1), new RecordStruct(2), new RecordStruct(3)]);
        FSharpSet<RecordStruct> actual = [new RecordStruct(1), new RecordStruct(2), new RecordStruct(3)];

        Assert.Equal(expected, actual);

        Assert.Collection(
            actual,
            item1 => Assert.Equal(new RecordStruct(1), item1),
            item2 => Assert.Equal(new RecordStruct(2), item2),
            item3 => Assert.Equal(new RecordStruct(3), item3));
    }
}
#endif
