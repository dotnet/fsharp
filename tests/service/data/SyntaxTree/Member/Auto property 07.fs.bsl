ImplFile
  (ParsedImplFileInput
     ("/root/Member/Auto property 07.fs", false, QualifiedNameOfFile A, [], [],
      [SynModuleOrNamespace
         ([A], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (3,6--3,8)), None,
                         PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,6), { AsKeyword = None });
                      AutoProperty
                        ([], false, Y,
                         Some (LongIdent (SynLongIdent ([int], [], [None]))),
                         PropertyGetSet,
                         { IsInstance = true
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
                         Const (Int32 7, (4,24--4,25)), (4,4--4,38),
                         { LeadingKeyword =
                            MemberVal ((4,4--4,10), (4,11--4,14))
                           WithKeyword = Some (4,26--4,30)
                           EqualsRange = Some (4,22--4,23)
                           GetSetKeywords =
                            Some (GetSet ((4,31--4,34), (4,35--4,38))) })],
                     (4,4--4,38)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (3,6--3,8)), None,
                        PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,6), { AsKeyword = None })), (3,5--4,38),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,9--3,10)
                    WithKeyword = None })], (3,0--4,38))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,38), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
