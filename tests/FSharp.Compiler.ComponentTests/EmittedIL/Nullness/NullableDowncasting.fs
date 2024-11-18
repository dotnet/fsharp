module TestModule

let objToString (o: obj) = o :?> string
let objnullToNullableString (o: objnull) = o :?> (string|null)
let objnullToOption (o:objnull) = o :?> Option<string>
let objNullTOInt(o:objnull) = o :?> int
let castToA<'a>(o:objnull) = o :?> 'a


let isObjnullAString(o:objnull) = o :? string
let isObjNullOption(o:objnull) = o :? Option<string>
let isOfType<'a>(o:objnull) = o :? 'a

let castToNullableB<'b when 'b:not null and 'b:not struct>(a: objnull) = a :?> ('b|null)
let downcastIplicitGeneric (o:objnull) : (_|null) = downcast o