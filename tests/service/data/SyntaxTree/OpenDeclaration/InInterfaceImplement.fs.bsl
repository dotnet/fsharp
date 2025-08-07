ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InInterfaceImplement.fs", false,
      QualifiedNameOfFile InInterfaceImplement, [],
      [SynModuleOrNamespace
         ([InInterfaceImplement], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (1,6--1,8)), None,
                         PreXmlDoc ((1,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (1,5--1,6), { AsKeyword = None });
                      Interface
                        (LongIdent
                           (SynLongIdent
                              ([System; IDisposable], [(2,20--2,21)],
                               [None; None])), Some (2,33--2,37),
                         Some
                           [Member
                              (SynBinding
                                 (None, Normal, false, false, [],
                                  PreXmlDoc ((4,8), FSharp.Compiler.Xml.XmlDocCollector),
                                  SynValData
                                    (Some
                                       { IsInstance = true
                                         IsDispatchSlot = false
                                         IsOverrideOrExplicitImpl = true
                                         IsFinal = false
                                         GetterOrSetterIsCompilerGenerated =
                                          false
                                         MemberKind = Member },
                                     SynValInfo
                                       ([[SynArgInfo ([], false, None)];
                                         [SynArgInfo ([], false, None)]],
                                        SynArgInfo ([], false, None)), None),
                                  LongIdent
                                    (SynLongIdent
                                       ([_; F], [(4,16--4,17)], [None; None]),
                                     None, None, Pats [Wild (4,19--4,20)], None,
                                     (4,15--4,20)), None,
                                  Const (Int32 3, (4,23--4,24)), (4,15--4,20),
                                  NoneAtInvisible,
                                  { LeadingKeyword = Member (4,8--4,14)
                                    InlineKeyword = None
                                    EqualsRange = Some (4,21--4,22) }),
                               (4,8--4,24))], (2,4--4,24))], (2,4--4,24)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (1,6--1,8)), None,
                        PreXmlDoc ((1,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (1,5--1,6), { AsKeyword = None })), (1,5--4,24),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,9--1,10)
                    WithKeyword = None })], (1,0--4,24))], PreXmlDocEmpty, [],
          None, (1,0--4,24), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,8)-(3,12) parse error Unexpected keyword 'open' in member definition. Expected 'member', 'override', 'static' or other token.
(3,20)-(4,8) parse error Expecting member body
