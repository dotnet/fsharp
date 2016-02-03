using System;
using System.Net.Security;
using System.ServiceModel;

public class C {
    [FaultContract(typeof(Exception), ProtectionLevel = ProtectionLevel.Sign)]
    public static void Run() { Console.WriteLine("In run");}
}