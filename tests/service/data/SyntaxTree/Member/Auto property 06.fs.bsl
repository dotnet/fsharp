ImplFile
  (ParsedImplFileInput
     ("/root/Member/Auto property 06.fs", false, QualifiedNameOfFile Module, [],
      [],
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
                     [AutoProperty
                        ([], false, P, Some (FromParseError (4,17--4,17)),
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
                         GetSet (None, None, None),
                         Const (Int32 1, (4,20--4,21)), (4,4--4,21),
                         { LeadingKeyword =
                            MemberVal ((4,4--4,10), (4,11--4,14))
                           WithKeyword = None
                           EqualsRange = Some (4,18--4,19)
                           GetSetKeywords = None })], (4,4--4,21)), [], None,
                  (3,5--4,21), { LeadingKeyword = Type (3,0--3,4)
                                 EqualsRange = Some (3,7--3,8)
                                 WithKeyword = None })], (3,0--4,21));
           Expr (Const (Unit, (6,0--6,2)), (6,0--6,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,18)-(4,19) parse error Unexpected symbol '=' in member definition
