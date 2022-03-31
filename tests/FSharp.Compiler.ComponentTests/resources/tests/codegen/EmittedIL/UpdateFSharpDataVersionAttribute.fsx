let d = @"."

System.Environment.CurrentDirectory <- d

let sar (f : string) =
                use sr = new System.IO.StreamReader(f)
                let fc = sr.ReadToEnd()
                let fc' = fc.Replace("int32) = ( 01 00 01 00 00 00 09 00 00 00 07 00 00 00 00 00 )", "int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 )")
                fc'

System.IO.Directory.GetFiles(d,"*.bsl") 
|> Array.iter ( fun f -> let n = sar f
                         use sw = new System.IO.StreamWriter(f)
                         sw.Write(n))
