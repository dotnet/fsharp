ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InInterfaceImplement2.fs", false,
      QualifiedNameOfFile InInterfaceImplement2, [],
      [SynModuleOrNamespace
         ([InInterfaceImplement2], false, AnonModule,
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
                                  PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
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
                                       ([_; F], [(3,16--3,17)], [None; None]),
                                     None, None, Pats [Wild (3,19--3,20)], None,
                                     (3,15--3,20)), None,
                                  Const (Int32 3, (3,23--3,24)), (3,15--3,20),
                                  NoneAtInvisible,
                                  { LeadingKeyword = Member (3,8--3,14)
                                    InlineKeyword = None
                                    EqualsRange = Some (3,21--3,22) }),
                               (3,8--3,24))], (2,4--3,24))], (2,4--3,24)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (1,6--1,8)), None,
                        PreXmlDoc ((1,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (1,5--1,6), { AsKeyword = None })), (1,5--3,24),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,9--1,10)
                    WithKeyword = None })], (1,0--3,24))], PreXmlDocEmpty, [],
          None, (1,0--4,19), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,8)-(4,12) parse error Unexpected keyword 'open' in object expression. Expected 'member', 'override', 'static' or other token.
(4,0)-(4,19) parse error Expecting member body
