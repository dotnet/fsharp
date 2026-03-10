// #Conformance #DataExpressions #Query
// Dev11: 184318, this used to throw a NullRefException at runtime instead of giving a compilation error
//<Expects status="error" id="FS0043" span="(14,18-14,22)">The type 'Product' does not have 'null' as a proper value</Expects>

// FirstOrDefault - Condition
let products = getProductList()

let product789 =
    query{
        for p in products do
        where (p.ProductID = 789)
        headOrDefault
    }
if product789 <> null then printfn "product789 failed"; exit 1
