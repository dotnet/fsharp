namespace NUnit.Framework

open Xunit

module Assert =

    let Fail(message) = Assert.True(false, message)
