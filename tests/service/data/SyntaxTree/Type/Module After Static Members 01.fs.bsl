ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module After Static Members 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MyType],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,11)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo ([[]], SynArgInfo ([], false, None)),
                               None),
                            LongIdent
                              (SynLongIdent ([StaticMethod], [], [None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Const (Unit, (5,30--5,32)), (5,30--5,32))],
                               None, (5,18--5,32)), None,
                            Const (Int32 42, (5,35--5,37)), (5,18--5,32),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember ((5,4--5,10), (5,11--5,17))
                              InlineKeyword = None
                              EqualsRange = Some (5,33--5,34) }), (5,4--5,37))],
                     (5,4--5,37)), [], None, (4,5--5,37),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,12--4,13)
                    WithKeyword = None })], (4,0--5,37));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InvalidModule],
                 PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (7,4--7,24)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((8,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (x, None), false, None, (8,12--8,13)),
                      None, Const (Int32 1, (8,16--8,17)), (8,12--8,13),
                      Yes (8,8--8,17), { LeadingKeyword = Let (8,8--8,11)
                                         InlineKeyword = None
                                         EqualsRange = Some (8,14--8,15) })],
                  (8,8--8,17), { InKeyword = None })], false, (7,4--8,17),
              { ModuleKeyword = Some (7,4--7,10)
                EqualsRange = Some (7,25--7,26) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--8,17), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,39)] }, set []))

(7,4)-(7,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
