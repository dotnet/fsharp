module TestModule

let objToString (o: obj) = o :?> string
let objnullToNullableString (o: objnull) = o :?> (string|null)
let objnullToOption (o:objnull) = o :?> Option<string>
let objNullTOInt(o:objnull) = o :?> int

let isObjnullAString(o:objnull) = o :? string
let isObjNullOption(o:objnull) = o :? Option<string>
let isObjNullObj(o:objnull) = o :? obj