module MyTestModule

type Maybe<'T when 'T: not struct> = 'T | null
type MaybeString = string | null

let curried3Func (a:MaybeString) (b:string) (c:int) = (a,b,c)

let partiallyApplied (propperString:string) = curried3Func propperString

let secondOutOfTriple (a,b,c) d e : Maybe<_> = b