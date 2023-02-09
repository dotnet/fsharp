ImplFile
  (ParsedImplFileInput
     ("/root/NewKeyword.fs", false, QualifiedNameOfFile NewKeyword, [], [],
      [SynModuleOrNamespace
         ([NewKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Y],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/NewKeyword.fs (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [],
                         SimplePats ([], /root/NewKeyword.fs (1,6--1,8)), None,
                         PreXmlDoc ((1,6), FSharp.Compiler.Xml.XmlDocCollector),
                         /root/NewKeyword.fs (1,5--1,6), { AsKeyword = None });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Constructor },
                               SynValInfo
                                 ([[SynArgInfo ([], false, Some message)]],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent ([new], [], [None]), None,
                               Some (SynValTyparDecls (None, false)),
                               Pats
                                 [Paren
                                    (Typed
                                       (Named
                                          (SynIdent (message, None), false, None,
                                           /root/NewKeyword.fs (2,9--2,16)),
                                        LongIdent
                                          (SynLongIdent ([string], [], [None])),
                                        /root/NewKeyword.fs (2,9--2,23)),
                                     /root/NewKeyword.fs (2,8--2,24))], None,
                               /root/NewKeyword.fs (2,4--2,7)), None,
                            App
                              (Atomic, false, Ident Y,
                               Const (Unit, /root/NewKeyword.fs (2,28--2,30)),
                               /root/NewKeyword.fs (2,27--2,30)),
                            /root/NewKeyword.fs (2,4--2,24), NoneAtInvisible,
                            { LeadingKeyword =
                               New /root/NewKeyword.fs (2,4--2,7)
                              InlineKeyword = None
                              EqualsRange =
                               Some /root/NewKeyword.fs (2,25--2,26) }),
                         /root/NewKeyword.fs (2,4--2,30))],
                     /root/NewKeyword.fs (2,4--2,30)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats ([], /root/NewKeyword.fs (1,6--1,8)), None,
                        PreXmlDoc ((1,6), FSharp.Compiler.Xml.XmlDocCollector),
                        /root/NewKeyword.fs (1,5--1,6), { AsKeyword = None })),
                  /root/NewKeyword.fs (1,5--2,30),
                  { LeadingKeyword = Type /root/NewKeyword.fs (1,0--1,4)
                    EqualsRange = Some /root/NewKeyword.fs (1,9--1,10)
                    WithKeyword = None })], /root/NewKeyword.fs (1,0--2,30))],
          PreXmlDocEmpty, [], None, /root/NewKeyword.fs (1,0--2,30),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))