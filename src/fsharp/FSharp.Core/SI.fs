// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// The International System of Units (SI)
namespace Microsoft.FSharp.Data.UnitSystems.SI.UnitNames
    open Microsoft.FSharp.Core

    [<Measure>] 
    /// The SI unit of length
    type metre               

    [<Measure>] 
    /// The SI unit of length
    type meter = metre               

    [<Measure>] 
    /// The SI unit of mass
    type kilogram

    [<Measure>] 
    /// The SI unit of time
    type second

    [<Measure>] 
    /// The SI unit of electric current
    type ampere             

    [<Measure>] 
    /// The SI unit of thermodynamic temperature
    type kelvin              

    [<Measure>] 
    /// The SI unit of amount of substance
    type mole

    [<Measure>] 
    /// The SI unit of luminous intensity
    type candela            

    [<Measure>] 
    /// The SI unit of frequency
    type hertz = / second

    [<Measure>] 
    /// The SI unit of force
    type newton = kilogram metre / second^2 

    [<Measure>] 
    /// The SI unit of pressure, stress
    type pascal = newton / metre^2

    [<Measure>] 
    /// The SI unit of energy, work, amount of heat
    type joule = newton metre

    [<Measure>] 
    /// The SI unit of power, radiant flux
    type watt = joule / second       

    [<Measure>] 
    /// The SI unit of electric charge, amount of electricity
    type coulomb = second ampere 

    [<Measure>] 
    /// The SI unit of electric potential difference, electromotive force
    type volt = watt/ampere        

    [<Measure>] 
    /// The SI unit of capacitance
    type farad = coulomb/volt

    [<Measure>] 
    /// The SI unit of electric resistance
    type ohm = volt/ampere       

    [<Measure>] 
    /// The SI unit of electric conductance
    type siemens = ampere/volt         

    [<Measure>] 
    /// The SI unit of magnetic flux
    type weber = volt second        

    [<Measure>] 
    /// The SI unit of magnetic flux density
    type tesla = weber/metre^2      

    [<Measure>] 
    /// The SI unit of inductance
    type henry = weber/ampere        

    [<Measure>] 
    /// The SI unit of luminous flux
    type lumen = candela        

    [<Measure>] 
    /// The SI unit of illuminance
    type lux = lumen/metre^2 

    [<Measure>] 
    /// The SI unit of activity referred to a radionuclide
    type becquerel = second^-1       

    [<Measure>] 
    /// The SI unit of absorbed dose
    type gray = joule/kilogram       

    [<Measure>] 
    /// The SI unit of does equivalent
    type sievert = joule/kilogram       

    [<Measure>] 
    /// The SI unit of catalytic activity
    type katal = mole/second 


/// Common abbreviations for the International System of Units (SI)
namespace Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

    [<Measure>] 
    /// A synonym for Metre, the SI unit of length
    type m = metre              

    [<Measure>] 
    /// A synonym for kilogram, the SI unit of mass
    type kg = kilogram

    [<Measure>] 
    /// A synonym for second, the SI unit of time
    type s = second

    [<Measure>] 
    /// A synonym for ampere, the SI unit of electric current
    type A = ampere            

    [<Measure>] 
    /// A synonym for kelvin, the SI unit of thermodynamic temperature
    type K = kelvin             

    [<Measure>] 
    /// A synonym for mole, the SI unit of amount of substance
    type mol  = mole

    [<Measure>] 
    /// A synonym for candela, the SI unit of luminous intensity
    type cd = candela            

    [<Measure>] 
    /// A synonym for hertz, the SI unit of frequency
    type Hz = hertz

    [<Measure>] 
    /// A synonym for newton, the SI unit of force
    type N = newton

    [<Measure>] 
    /// A synonym for pascal, the SI unit of pressure, stress
    type Pa = pascal

    [<Measure>] 
    /// A synonym for joule, the SI unit of energy, work, amount of heat
    type J = joule

    [<Measure>] 
    /// A synonym for watt, the SI unit of power, radiant flux
    type W = watt     

    [<Measure>] 
    /// A synonym for coulomb, the SI unit of electric charge, amount of electricity
    type C = coulomb 

    [<Measure>] 
    /// A synonym for volt, the SI unit of electric potential difference, electromotive force
    type V = volt

    [<Measure>] 
    /// A synonym for farad, the SI unit of capacitance
    type F = farad

    [<Measure>] 
    /// A synonym for siemens, the SI unit of electric conductance
    type S = siemens         

    [<Measure>] 
    /// A synonym for UnitNames.ohm, the SI unit of electric resistance.
    type ohm = Microsoft.FSharp.Data.UnitSystems.SI.UnitNames.ohm         

    [<Measure>] 
    /// A synonym for weber, the SI unit of magnetic flux
    type Wb = weber

    [<Measure>] 
    /// A synonym for tesla, the SI unit of magnetic flux density
    type T = tesla

    [<Measure>] 
    /// A synonym for lumen, the SI unit of luminous flux
    type lm = lumen

    [<Measure>] 
    /// A synonym for lux, the SI unit of illuminance
    type lx = lux

    [<Measure>] 
    /// A synonym for becquerel, the SI unit of activity referred to a radionuclide
    type Bq = becquerel

    [<Measure>] 
    /// A synonym for gray, the SI unit of absorbed dose
    type Gy = gray

    [<Measure>] 
    /// A synonym for sievert, the SI unit of does equivalent
    type Sv = sievert

    [<Measure>] 
    /// A synonym for katal, the SI unit of catalytic activity
    type kat = katal

    [<Measure>] 
    /// A synonym for henry, the SI unit of inductance
    type H = henry
