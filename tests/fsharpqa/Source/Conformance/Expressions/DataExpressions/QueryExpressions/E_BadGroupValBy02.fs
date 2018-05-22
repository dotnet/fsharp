// #Conformance #DataExpressions #Query
// DevDiv:210830, groupValBy with poor diagnostics
//<Expects status="error" span="(9,6-9,32)" id="FS0501">The member or object constructor 'GroupValBy' takes 3 argument\(s\) but is here given 4\. The required signature is 'member Linq\.QueryBuilder\.GroupValBy \: source\:Linq\.QuerySource\<'T,'Q\> \* resultSelector\:\('T \-\> 'Value\) \* keySelector\:\('T \-\> 'Key\) \-\> Linq\.QuerySource\<System\.Linq\.IGrouping\<'Key,'Value\>,'Q\> when 'Key \: equality'\.</Expects>

let words = ["blueberry"; "chimpanzee"; ]
let wordGroups =
query {
   for w in words do
     groupValBy w w.[0]. into g
     select (g.Key, g.ToArray())
}
