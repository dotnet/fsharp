ImplFile
  (ParsedImplFileInput
     ("/root/Member/Auto property 15.fs", false, QualifiedNameOfFile A, [], [],
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
                         GetSet
                           (Some (Internal (4,15--4,23)),
                            Some (Public (4,40--4,46)),
                            Some (Private (4,52--4,59))),
                         Const (Int32 7, (4,33--4,34)), (4,4--4,63),
                         { LeadingKeyword =
                            MemberVal ((4,4--4,10), (4,11--4,14))
                           WithKeyword = Some (4,35--4,39)
                           EqualsRange = Some (4,31--4,32)
                           GetSetKeywords =
                            Some (GetSet ((4,47--4,50), (4,60--4,63))) })],
                     (4,4--4,63)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (3,6--3,8)), None,
                        PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,6), { AsKeyword = None })), (3,5--4,63),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,9--3,10)
                    WithKeyword = None })], (3,0--4,63))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,63), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
