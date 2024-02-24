module Module

type INumericNorm<'T,'Measure>  = interface INumeric<'T>
                                   member Norm : 'T -> 'Measure
