ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/NewKeyword.fs", false,
      QualifiedNameOfFile NewKeyword, [], [],
      [SynModuleOrNamespace
         ([NewKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Y],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], SimplePats ([], [], (2,6--2,8)), None,
                         PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (2,5--2,6), { AsKeyword = None });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Constructor },
                               SynValInfo
                                 ([[SynArgInfo ([], false, Some message)]],
                                  SynArgInfo ([], false, None)), None, None),
                            LongIdent
                              (SynLongIdent ([new], [], [None]), None,
                               Some (SynValTyparDecls (None, false)),
                               Pats
                                 [Paren
                                    (Typed
                                       (Named
                                          (SynIdent (message, None), false, None,
                                           (3,9--3,16)),
                                        LongIdent
                                          (SynLongIdent ([string], [], [None])),
                                        (3,9--3,23)), (3,8--3,24))], None,
                               (3,4--3,7)), None,
                            App
                              (Atomic, false, Ident Y,
                               Const (Unit, (3,28--3,30)), (3,27--3,30)),
                            (3,4--3,24), NoneAtInvisible,
                            { LeadingKeyword = New (3,4--3,7)
                              InlineKeyword = None
                              EqualsRange = Some (3,25--3,26) }), (3,4--3,30))],
                     (3,4--3,30)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], [], (2,6--2,8)), None,
                        PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,5--2,6), { AsKeyword = None })), (2,5--3,30),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--3,30))], PreXmlDocEmpty, [],
          None, (2,0--4,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
