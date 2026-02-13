using System;
using System.Runtime.CompilerServices;

namespace PriorityTests
{
    public static class BasicPriority
    {
        [OverloadResolutionPriority(1)]
        public static string HighPriority(object o) => "high";
        
        [OverloadResolutionPriority(0)]
        public static string LowPriority(object o) => "low";
        
        [OverloadResolutionPriority(2)]
        public static string Invoke(object o) => "priority-2";
        
        [OverloadResolutionPriority(1)]
        public static string Invoke(string s) => "priority-1-string";
        
        [OverloadResolutionPriority(0)]
        public static string Invoke(int i) => "priority-0-int";
    }
    
    public static class NegativePriority
    {
        [OverloadResolutionPriority(-1)]
        public static string Legacy(object o) => "legacy";
        
        public static string Legacy(string s) => "current"; // default priority 0
        
        [OverloadResolutionPriority(-2)]
        public static string Obsolete(object o) => "very-old";
        
        [OverloadResolutionPriority(-1)]
        public static string Obsolete(string s) => "old";
        
        public static string Obsolete(int i) => "new"; // default priority 0
    }
    
    public static class PriorityVsConcreteness
    {
        [OverloadResolutionPriority(1)]
        public static string Process<T>(T value) => "generic-high-priority";
        
        [OverloadResolutionPriority(0)]
        public static string Process(int value) => "int-low-priority";
        
        [OverloadResolutionPriority(1)]
        public static string Handle<T>(T[] arr) => "array-generic-high";
        
        public static string Handle(int[] arr) => "array-int-default";
    }
    
    public static class ExtensionTypeA
    {
        [OverloadResolutionPriority(1)]
        public static string ExtMethod(this string s, int x) => "TypeA-priority1";
        
        public static string ExtMethod(this string s, object o) => "TypeA-priority0";
    }
    
    public static class ExtensionTypeB
    {
        [OverloadResolutionPriority(2)]
        public static string ExtMethod(this string s, int x) => "TypeB-priority2";
        
        public static string ExtMethod(this string s, object o) => "TypeB-priority0";
    }
    
    public static class DefaultPriority
    {
        public static string NoAttr(object o) => "no-attr";
        
        [OverloadResolutionPriority(0)]
        public static string ExplicitZero(object o) => "explicit-zero";
        
        [OverloadResolutionPriority(1)]
        public static string PositiveOne(object o) => "positive-one";
        
        public static string Mixed(string s) => "mixed-default";
        
        [OverloadResolutionPriority(1)]
        public static string Mixed(object o) => "mixed-priority";
    }
}

namespace ExtensionPriorityTests
{
    // ===== Per-declaring-type scoped priority for extensions =====
    
    public static class ExtensionModuleA
    {
        [OverloadResolutionPriority(1)]
        public static string Transform<T>(this T value) => "ModuleA-generic-priority1";
        
        [OverloadResolutionPriority(0)]
        public static string Transform(this int value) => "ModuleA-int-priority0";
    }
    
    public static class ExtensionModuleB
    {
        [OverloadResolutionPriority(0)]
        public static string Transform<T>(this T value) => "ModuleB-generic-priority0";
        
        [OverloadResolutionPriority(2)]
        public static string Transform(this int value) => "ModuleB-int-priority2";
    }
    
    // ===== Same priority, normal tiebreakers apply =====
    
    public static class SamePriorityTiebreaker
    {
        [OverloadResolutionPriority(1)]
        public static string Process<T>(T value) => "generic";
        
        [OverloadResolutionPriority(1)]
        public static string Process(int value) => "int";
        
        [OverloadResolutionPriority(1)]
        public static string Process(string value) => "string";
    }
    
    public static class SamePriorityArrayTypes
    {
        [OverloadResolutionPriority(1)]
        public static string Handle<T>(T[] arr) => "generic-array";
        
        [OverloadResolutionPriority(1)]
        public static string Handle(int[] arr) => "int-array";
    }
    
    // ===== Inheritance hierarchy with mixed priorities =====
    
    public class BaseClass
    {
        [OverloadResolutionPriority(0)]
        public virtual string Method(object o) => "Base-object-priority0";
        
        [OverloadResolutionPriority(1)]
        public virtual string Method(string s) => "Base-string-priority1";
    }
    
    public class DerivedClass : BaseClass
    {
        public override string Method(object o) => "Derived-object";
        public override string Method(string s) => "Derived-string";
    }
    
    public class DerivedClassWithNewMethods : BaseClass
    {
        [OverloadResolutionPriority(2)]
        public string Method(int i) => "DerivedNew-int-priority2";
    }
    
    // ===== Extension methods vs instance methods priority =====
    
    public class TargetClass
    {
        [OverloadResolutionPriority(0)]
        public string DoWork(object o) => "Instance-object-priority0";
        
        [OverloadResolutionPriority(1)]
        public string DoWork(string s) => "Instance-string-priority1";
    }
    
    public static class TargetClassExtensions
    {
        [OverloadResolutionPriority(2)]
        public static string DoWork(this TargetClass tc, int i) => "Extension-int-priority2";
    }
    
    // ===== Instance-only class for priority testing =====
    
    public class InstanceOnlyClass
    {
        [OverloadResolutionPriority(2)]
        public string Call(object o) => "object-priority2";
        
        [OverloadResolutionPriority(0)]
        public string Call(string s) => "string-priority0";
    }
    
    // ===== Priority with zero vs absent attribute =====
    
    public static class ExplicitVsImplicitZero
    {
        [OverloadResolutionPriority(0)]
        public static string WithExplicitZero(object o) => "explicit-zero";
        
        public static string WithoutAttr(string s) => "no-attr";
    }
    
    // ===== Complex generic scenarios =====
    
    public static class ComplexGenerics
    {
        [OverloadResolutionPriority(2)]
        public static string Process<T, U>(T t, U u) => "fully-generic-priority2";
        
        [OverloadResolutionPriority(1)]
        public static string Process<T>(T t, int u) => "partial-concrete-priority1";
        
        [OverloadResolutionPriority(0)]
        public static string Process(int t, int u) => "fully-concrete-priority0";
    }

    // ===== Property / Indexer with ORP =====

    public class IndexerWithPriority
    {
        [OverloadResolutionPriority(1)]
        public string this[object key] => "object-indexer-priority1";

        [OverloadResolutionPriority(0)]
        public string this[string key] => "string-indexer-priority0";

        [OverloadResolutionPriority(2)]
        public string this[int index1, int index2] => "two-int-indexer-priority2";

        [OverloadResolutionPriority(0)]
        public string this[object index1, object index2] => "two-object-indexer-priority0";
    }
}
