{ new obj() with
    member x.ToString() = "INotifyEnumerableInternal"
  interface INotifyEnumerableInternal<'T>
  interface IEnumerable<_> with
    member x.GetEnumerator() = null }