//<Expects status="Error" span="(5,6)" id="FS0039">The namespace or module 'XPlot' is not defined.</Expects>

#r "paket: "

open XPlot.Plotly

Chart.Line [ 1 .. 10 ]