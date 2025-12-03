ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Class 01.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [C],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (4,7--4,9)), None,
                         PreXmlDoc ((4,7), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,6), { AsKeyword = None });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([_; F], [(5,12--5,13)], [None; None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Const (Unit, (5,15--5,17)), (5,15--5,17))],
                               None, (5,11--5,17)), None,
                            Const (Int32 3, (5,20--5,21)), (5,11--5,17),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (5,4--5,10)
                              InlineKeyword = None
                              EqualsRange = Some (5,18--5,19) }), (5,4--5,21))],
                     (5,4--5,21)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,7--4,9)), None,
                        PreXmlDoc ((4,7), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,6), { AsKeyword = None })), (4,5--5,21),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,10--4,11)
                    WithKeyword = None })], (4,0--5,21));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M2],
                 PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (6,4--6,13)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((7,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([[]], SynArgInfo ([], false, None)),
                         None),
                      LongIdent
                        (SynLongIdent ([f], [], [None]), None, None,
                         Pats [Paren (Const (Unit, (7,14--7,16)), (7,14--7,16))],
                         None, (7,12--7,16)), None, Const (Unit, (7,19--7,21)),
                      (7,12--7,16), NoneAtLet,
                      { LeadingKeyword = Let (7,8--7,11)
                        InlineKeyword = None
                        EqualsRange = Some (7,17--7,18) })], (7,8--7,21),
                  { InKeyword = None })], false, (6,4--7,21),
              { ModuleKeyword = Some (6,4--6,10)
                EqualsRange = Some (6,14--6,15) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--7,21), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,44)] }, set []))

(6,4)-(6,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
