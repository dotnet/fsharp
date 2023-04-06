ImplFile
  (ParsedImplFileInput
     ("/root/Member/Auto property - Missing type 04.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], SimplePats ([], [], (3,6--3,8)), None,
                         PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,6), { AsKeyword = None });
                      AutoProperty
                        ([], false, P1, Some (FromParseError (4,18--4,18)),
                         Member, { IsInstance = true
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = false
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet },
                         PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                         None, ArbitraryAfterError ("autoProp1", (4,18--4,18)),
                         (4,4--4,18),
                         { LeadingKeyword =
                            MemberVal ((4,4--4,10), (4,11--4,14))
                           WithKeyword = None
                           EqualsRange = None
                           GetSetKeywords = None })], (4,4--4,18)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], [], (3,6--3,8)), None,
                        PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,6), { AsKeyword = None })), (3,5--4,18),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,9--3,10)
                    WithKeyword = None })], (3,0--4,18));
           Expr (Const (Unit, (6,0--6,2)), (6,0--6,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,0)-(6,1) parse error Incomplete structured construct at or before this point in member definition
