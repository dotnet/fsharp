module Provider

[<MeasureAnnotatedAbbreviation>]
type TaggedSpan<[<Measure>] 'u> = System.Span<int>

val make: unit -> TaggedSpan<1>
val add: s: System.Span<int> -> x: int -> int
val f: s: System.Span<int> -> unit -> int
