namespace FSharp.Tools.Visualizer.WPF

open System.Windows
open System

type FSharpVisualizerApplication () =
    inherit Application()

    do
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        AppDomain.CurrentDomain.UnhandledException.Add (fun _ -> ())
