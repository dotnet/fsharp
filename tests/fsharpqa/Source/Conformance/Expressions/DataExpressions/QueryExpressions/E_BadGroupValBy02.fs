// #Conformance #DataExpressions #Query
// DevDiv:210830, groupValBy with poor diagnostics
//<Expects status="error" span="(9,6-9,32)" id="FS0001">This expression was expected to have type.+'System\.Linq\.IGrouping<'a,'b>'.+but here has type.+'unit'</Expects>

let words = ["blueberry"; "chimpanzee"; ]
let wordGroups =
query {
   for w in words do
     groupValBy w w.[0]. into g
     select (g.Key, g.ToArray())
}
