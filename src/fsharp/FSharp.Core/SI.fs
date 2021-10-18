// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The International System of Units (SI)
namespace Microsoft.FSharp.Data.UnitSystems.SI.UnitNames
    open Microsoft.FSharp.Core

    /// The SI unit of length
    [<Measure>]
    type metre               

    /// The SI unit of length
    [<Measure>]
    type meter = metre               

    /// The SI unit of mass
    [<Measure>]
    type kilogram

    /// The SI unit of time
    [<Measure>]
    type second

    /// The SI unit of electric current
    [<Measure>]
    type ampere             

    /// The SI unit of thermodynamic temperature
    [<Measure>]
    type kelvin              

    /// The SI unit of amount of substance
    [<Measure>]
    type mole

    /// The SI unit of luminous intensity
    [<Measure>]
    type candela            

    /// The SI unit of frequency
    [<Measure>]
    type hertz = / second

    /// The SI unit of force
    [<Measure>]
    type newton = kilogram metre / second^2 

    /// The SI unit of pressure, stress
    [<Measure>]
    type pascal = newton / metre^2

    /// The SI unit of energy, work, amount of heat
    [<Measure>]
    type joule = newton metre

    /// The SI unit of power, radiant flux
    [<Measure>]
    type watt = joule / second       

    /// The SI unit of electric charge, amount of electricity
    [<Measure>]
    type coulomb = second ampere 

    /// The SI unit of electric potential difference, electromotive force
    [<Measure>]
    type volt = watt/ampere        

    /// The SI unit of capacitance
    [<Measure>]
    type farad = coulomb/volt

    /// The SI unit of electric resistance
    [<Measure>]
    type ohm = volt/ampere       

    /// The SI unit of electric conductance
    [<Measure>]
    type siemens = ampere/volt         

    /// The SI unit of magnetic flux
    [<Measure>]
    type weber = volt second        

    /// The SI unit of magnetic flux density
    [<Measure>]
    type tesla = weber/metre^2      

    /// The SI unit of inductance
    [<Measure>]
    type henry = weber/ampere        

    /// The SI unit of luminous flux
    [<Measure>]
    type lumen = candela        

    /// The SI unit of illuminance
    [<Measure>]
    type lux = lumen/metre^2 

    /// The SI unit of activity referred to a radionuclide
    [<Measure>]
    type becquerel = second^-1       

    /// The SI unit of absorbed dose
    [<Measure>]
    type gray = joule/kilogram       

    /// The SI unit of does equivalent
    [<Measure>]
    type sievert = joule/kilogram       

    /// The SI unit of catalytic activity
    [<Measure>]
    type katal = mole/second 


/// Common abbreviations for the International System of Units (SI)
namespace Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

    /// A synonym for Metre, the SI unit of length
    [<Measure>]
    type m = metre              

    /// A synonym for kilogram, the SI unit of mass
    [<Measure>]
    type kg = kilogram

    /// A synonym for second, the SI unit of time
    [<Measure>]
    type s = second

    /// A synonym for ampere, the SI unit of electric current
    [<Measure>]
    type A = ampere            

    /// A synonym for kelvin, the SI unit of thermodynamic temperature
    [<Measure>]
    type K = kelvin             

    /// A synonym for mole, the SI unit of amount of substance
    [<Measure>]
    type mol  = mole

    /// A synonym for candela, the SI unit of luminous intensity
    [<Measure>]
    type cd = candela            

    /// A synonym for hertz, the SI unit of frequency
    [<Measure>]
    type Hz = hertz

    /// A synonym for newton, the SI unit of force
    [<Measure>]
    type N = newton

    /// A synonym for pascal, the SI unit of pressure, stress
    [<Measure>]
    type Pa = pascal

    /// A synonym for joule, the SI unit of energy, work, amount of heat
    [<Measure>]
    type J = joule

    /// A synonym for watt, the SI unit of power, radiant flux
    [<Measure>]
    type W = watt     

    /// A synonym for coulomb, the SI unit of electric charge, amount of electricity
    [<Measure>]
    type C = coulomb 

    /// A synonym for volt, the SI unit of electric potential difference, electromotive force
    [<Measure>]
    type V = volt

    /// A synonym for farad, the SI unit of capacitance
    [<Measure>]
    type F = farad

    /// A synonym for siemens, the SI unit of electric conductance
    [<Measure>]
    type S = siemens         

    /// A synonym for UnitNames.ohm, the SI unit of electric resistance.
    [<Measure>]
    type ohm = Microsoft.FSharp.Data.UnitSystems.SI.UnitNames.ohm         

    /// A synonym for weber, the SI unit of magnetic flux
    [<Measure>]
    type Wb = weber

    /// A synonym for tesla, the SI unit of magnetic flux density
    [<Measure>]
    type T = tesla

    /// A synonym for lumen, the SI unit of luminous flux
    [<Measure>]
    type lm = lumen

    /// A synonym for lux, the SI unit of illuminance
    [<Measure>]
    type lx = lux

    /// A synonym for becquerel, the SI unit of activity referred to a radionuclide
    [<Measure>]
    type Bq = becquerel

    /// A synonym for gray, the SI unit of absorbed dose
    [<Measure>]
    type Gy = gray

    /// A synonym for sievert, the SI unit of does equivalent
    [<Measure>]
    type Sv = sievert

    /// A synonym for katal, the SI unit of catalytic activity
    [<Measure>]
    type kat = katal

    /// A synonym for henry, the SI unit of inductance
    [<Measure>]
    type H = henry
