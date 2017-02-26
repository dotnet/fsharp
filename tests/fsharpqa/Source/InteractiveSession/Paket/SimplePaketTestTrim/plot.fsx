#r "paket:nuget XPlot.Plotly"

// note we don't have space at paket:nuget
// this should work

open XPlot.Plotly

Chart.Line [ 1 .. 10 ]