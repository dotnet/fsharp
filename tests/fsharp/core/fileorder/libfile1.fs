
// Repro for https://github.com/Microsoft/visualfsharp/issues/1298
namespace Ploeh.Weird.Repro

type Foo = {
    Value : string
    Text : string }

type Bar = {
    Value : string
    Number : int }

type Baz = {
    Value : int
    Text : string }