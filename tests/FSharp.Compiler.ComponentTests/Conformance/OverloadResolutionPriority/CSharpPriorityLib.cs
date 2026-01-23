// C# Library for OverloadResolutionPriority Tests
// This file is compiled ONCE and used by all ORP tests

using System;
using System.Runtime.CompilerServices;

namespace PriorityTests
{
    /// Basic priority within same type - higher priority should win
    public static class BasicPriority
    {
        [OverloadResolutionPriority(1)]
        public static string HighPriority(object o) => "high";
        
        [OverloadResolutionPriority(0)]
        public static string LowPriority(object o) => "low";
        
        // Overloaded methods with same name but different priorities
        [OverloadResolutionPriority(2)]
        public static string Invoke(object o) => "priority-2";
        
        [OverloadResolutionPriority(1)]
        public static string Invoke(string s) => "priority-1-string";
        
        [OverloadResolutionPriority(0)]
        public static string Invoke(int i) => "priority-0-int";
    }
    
    /// Negative priority - should be deprioritized (used for backward compat scenarios)
    public static class NegativePriority
    {
        [OverloadResolutionPriority(-1)]
        public static string Legacy(object o) => "legacy";
        
        public static string Legacy(string s) => "current"; // default priority 0
        
        // Multiple negative levels
        [OverloadResolutionPriority(-2)]
        public static string Obsolete(object o) => "very-old";
        
        [OverloadResolutionPriority(-1)]
        public static string Obsolete(string s) => "old";
        
        public static string Obsolete(int i) => "new"; // default priority 0
    }
    
    /// Priority overrides type concreteness
    public static class PriorityVsConcreteness
    {
        // Less concrete but higher priority - should win
        [OverloadResolutionPriority(1)]
        public static string Process<T>(T value) => "generic-high-priority";
        
        // More concrete but lower priority - should lose
        [OverloadResolutionPriority(0)]
        public static string Process(int value) => "int-low-priority";
        
        // Another scenario: wrapped generic with priority beats concrete
        [OverloadResolutionPriority(1)]
        public static string Handle<T>(T[] arr) => "array-generic-high";
        
        public static string Handle(int[] arr) => "array-int-default";
    }
    
    /// Priority is scoped per-declaring-type for extension methods
    public static class ExtensionTypeA
    {
        [OverloadResolutionPriority(1)]
        public static string ExtMethod(this string s, int x) => "TypeA-priority1";
        
        public static string ExtMethod(this string s, object o) => "TypeA-priority0";
    }
    
    public static class ExtensionTypeB
    {
        // Different declaring type - priority is independent
        [OverloadResolutionPriority(2)]
        public static string ExtMethod(this string s, int x) => "TypeB-priority2";
        
        public static string ExtMethod(this string s, object o) => "TypeB-priority0";
    }
    
    /// Default priority is 0 when attribute is absent
    public static class DefaultPriority
    {
        // No attribute - implicit priority 0
        public static string NoAttr(object o) => "no-attr";
        
        [OverloadResolutionPriority(0)]
        public static string ExplicitZero(object o) => "explicit-zero";
        
        [OverloadResolutionPriority(1)]
        public static string PositiveOne(object o) => "positive-one";
        
        // Overloads where one has attribute and one doesn't
        public static string Mixed(string s) => "mixed-default";
        
        [OverloadResolutionPriority(1)]
        public static string Mixed(object o) => "mixed-priority";
    }
}

namespace ExtensionPriorityTests
{
    // ===== Per-declaring-type scoped priority for extensions =====
    
    /// Extension methods in Module A with varying priorities
    public static class ExtensionModuleA
    {
        [OverloadResolutionPriority(1)]
        public static string Transform<T>(this T value) => "ModuleA-generic-priority1";
        
        [OverloadResolutionPriority(0)]
        public static string Transform(this int value) => "ModuleA-int-priority0";
    }
    
    /// Extension methods in Module B with different priority assignments
    public static class ExtensionModuleB
    {
        [OverloadResolutionPriority(0)]
        public static string Transform<T>(this T value) => "ModuleB-generic-priority0";
        
        [OverloadResolutionPriority(2)]
        public static string Transform(this int value) => "ModuleB-int-priority2";
    }
    
    // ===== Same priority, normal tiebreakers apply =====
    
    /// Multiple overloads with same priority - concreteness should break tie
    public static class SamePriorityTiebreaker
    {
        [OverloadResolutionPriority(1)]
        public static string Process<T>(T value) => "generic";
        
        [OverloadResolutionPriority(1)]
        public static string Process(int value) => "int";
        
        [OverloadResolutionPriority(1)]
        public static string Process(string value) => "string";
    }
    
    /// Same priority with array types - concreteness on element type
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
        // Inherits priorities from base - no new attributes here
        public override string Method(object o) => "Derived-object";
        public override string Method(string s) => "Derived-string";
    }
    
    // New methods in derived with different priorities
    public class DerivedClassWithNewMethods : BaseClass
    {
        // New overloads with their own priorities
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
        // Extension method that adds new overload not conflicting with instance methods
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
    
    /// Mixed explicit zero and absent (implicit zero) 
    public static class ExplicitVsImplicitZero
    {
        [OverloadResolutionPriority(0)]
        public static string WithExplicitZero(object o) => "explicit-zero";
        
        public static string WithoutAttr(string s) => "no-attr";
        
        // These should compete equally, string should win by concreteness
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
}
