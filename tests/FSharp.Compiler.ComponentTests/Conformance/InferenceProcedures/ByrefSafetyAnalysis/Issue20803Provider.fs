module Provider

[<MeasureAnnotatedAbbreviation>]
type TaggedSpan<[<Measure>] 'u> = System.Span<int>

let make () : TaggedSpan<1> = System.Span<int>([| 1; 2; 3 |])
let add (s: System.Span<int>) x = s.Length + x
let f (s: System.Span<int>) = fun () -> s.Length
