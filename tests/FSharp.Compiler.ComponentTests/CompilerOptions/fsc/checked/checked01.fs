// #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:891
// --checked[+|-] option
// This file is supposed to be compiled with checked ON
//<Expects status="success"></Expects>
module M

let t1 = 
            try
                let a = System.Int32.MaxValue + 1       // will throw!
                false
            with 
                | _ -> true

let t2 = 
            try
                let a = System.Int32.MinValue - 1       // will throw!
                false
            with 
                | _ -> true


let t3 = 
            try
                let a = System.Int64.MaxValue + 1L       // will throw!
                false
            with 
                | _ -> true

let t4 = 
            try
                let a = System.Int64.MinValue - 1L       // will throw!
                false
            with 
                | _ -> true

let t5 = 
            try
                let a = System.Int16.MaxValue + 1s       // will throw!
                false
            with 
                | _ -> true

let t6 = 
            try
                let a = System.Int16.MinValue - 1s       // will throw!
                false
            with 
                | _ -> true

let t7 = 
            try
                let a = System.SByte.MaxValue + 1y       // will throw!
                false
            with 
                | _ -> true

let t8 = 
            try
                let a = System.SByte.MinValue - 1y       // will throw!
                false
            with 
                | _ -> true

let t1u = 
            try
                let a = System.UInt32.MaxValue + 1u       // will throw!
                false
            with 
                | _ -> true

let t2u = 
            try
                let a = System.UInt32.MinValue - 1u       // will throw!
                false
            with 
                | _ -> true


let t3u = 
            try
                let a = System.UInt64.MaxValue + 1UL       // will throw!
                false
            with 
                | _ -> true

let t4u = 
            try
                let a = System.UInt64.MinValue - 1UL       // will throw!
                false
            with 
                | _ -> true

let t5u = 
            try
                let a = System.UInt16.MaxValue + 1us       // will throw!
                false
            with 
                | _ -> true

let t6u = 
            try
                let a = System.UInt16.MinValue - 1us       // will throw!
                false
            with 
                | _ -> true

let t7u = 
            try
                let a = System.Byte.MaxValue + 1uy       // will throw!
                false
            with 
                | _ -> true

let t8u = 
            try
                let a = System.Byte.MinValue - 1uy       // will throw!
                false
            with 
                | _ -> true


(if (t1 && t2 && t3 && t4 && t5 && t6 && t7 && t8) &&
   (t1u && t2u && t3u && t4u && t5u && t6u && t7u && t8u)
    then 0 else 1) |> exit
