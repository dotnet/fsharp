// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regressiont test for TFS#712639
// Incorrect span or overlapping debugging spans
// The test will start failing once the bug is actually fixed
for wi = 1 to 3 do
    printfn "%A" wi
 
