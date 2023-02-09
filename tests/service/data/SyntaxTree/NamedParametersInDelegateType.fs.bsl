ImplFile
  (ParsedImplFileInput
     ("/root/NamedParametersInDelegateType.fs", false,
      QualifiedNameOfFile NamedParametersInDelegateType, [], [],
      [SynModuleOrNamespace
         ([NamedParametersInDelegateType], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/NamedParametersInDelegateType.fs (1,5--1,8)),
                  ObjectModel
                    (Delegate
                       (Fun
                          (Tuple
                             (false,
                              [Type
                                 (SignatureParameter
                                    ([], false, Some a,
                                     LongIdent (SynLongIdent ([A], [], [None])),
                                     /root/NamedParametersInDelegateType.fs (1,23--1,27)));
                               Star
                                 /root/NamedParametersInDelegateType.fs (1,28--1,29);
                               Type
                                 (SignatureParameter
                                    ([], false, Some b,
                                     LongIdent (SynLongIdent ([B], [], [None])),
                                     /root/NamedParametersInDelegateType.fs (1,30--1,34)))],
                              /root/NamedParametersInDelegateType.fs (1,23--1,34)),
                           Fun
                             (SignatureParameter
                                ([], false, Some c,
                                 LongIdent (SynLongIdent ([C], [], [None])),
                                 /root/NamedParametersInDelegateType.fs (1,38--1,41)),
                              LongIdent (SynLongIdent ([D], [], [None])),
                              /root/NamedParametersInDelegateType.fs (1,38--1,46),
                              { ArrowRange =
                                 /root/NamedParametersInDelegateType.fs (1,42--1,44) }),
                           /root/NamedParametersInDelegateType.fs (1,23--1,46),
                           { ArrowRange =
                              /root/NamedParametersInDelegateType.fs (1,35--1,37) }),
                        SynValInfo
                          ([[SynArgInfo ([], false, Some a);
                             SynArgInfo ([], false, Some b)];
                            [SynArgInfo ([], false, Some c)]],
                           SynArgInfo ([], false, None))),
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (Invoke, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (Tuple
                                 (false,
                                  [Type
                                     (SignatureParameter
                                        ([], false, Some a,
                                         LongIdent
                                           (SynLongIdent ([A], [], [None])),
                                         /root/NamedParametersInDelegateType.fs (1,23--1,27)));
                                   Star
                                     /root/NamedParametersInDelegateType.fs (1,28--1,29);
                                   Type
                                     (SignatureParameter
                                        ([], false, Some b,
                                         LongIdent
                                           (SynLongIdent ([B], [], [None])),
                                         /root/NamedParametersInDelegateType.fs (1,30--1,34)))],
                                  /root/NamedParametersInDelegateType.fs (1,23--1,34)),
                               Fun
                                 (SignatureParameter
                                    ([], false, Some c,
                                     LongIdent (SynLongIdent ([C], [], [None])),
                                     /root/NamedParametersInDelegateType.fs (1,38--1,41)),
                                  LongIdent (SynLongIdent ([D], [], [None])),
                                  /root/NamedParametersInDelegateType.fs (1,38--1,46),
                                  { ArrowRange =
                                     /root/NamedParametersInDelegateType.fs (1,42--1,44) }),
                               /root/NamedParametersInDelegateType.fs (1,23--1,46),
                               { ArrowRange =
                                  /root/NamedParametersInDelegateType.fs (1,35--1,37) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, Some a);
                                 SynArgInfo ([], false, Some b)];
                                [SynArgInfo ([], false, Some c)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDocEmpty, None, None,
                            /root/NamedParametersInDelegateType.fs (1,11--1,46),
                            { LeadingKeyword = Synthetic
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member },
                         /root/NamedParametersInDelegateType.fs (1,11--1,46),
                         { GetSetKeywords = None })],
                     /root/NamedParametersInDelegateType.fs (1,11--1,46)), [],
                  None, /root/NamedParametersInDelegateType.fs (1,5--1,46),
                  { LeadingKeyword =
                     Type /root/NamedParametersInDelegateType.fs (1,0--1,4)
                    EqualsRange =
                     Some /root/NamedParametersInDelegateType.fs (1,9--1,10)
                    WithKeyword = None })],
              /root/NamedParametersInDelegateType.fs (1,0--1,46))],
          PreXmlDocEmpty, [], None,
          /root/NamedParametersInDelegateType.fs (1,0--1,46),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))