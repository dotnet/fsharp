ImplFile
  (ParsedImplFileInput
     ("/root/Type/NamedParametersInDelegateType.fs", false,
      QualifiedNameOfFile NamedParametersInDelegateType, [], [],
      [SynModuleOrNamespace
         ([NamedParametersInDelegateType], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/Type/NamedParametersInDelegateType.fs (2,5--2,8)),
                  ObjectModel
                    (Delegate
                       (Fun
                          (Tuple
                             (false,
                              [Type
                                 (SignatureParameter
                                    ([], false, Some a,
                                     LongIdent (SynLongIdent ([A], [], [None])),
                                     /root/Type/NamedParametersInDelegateType.fs (2,23--2,27)));
                               Star
                                 /root/Type/NamedParametersInDelegateType.fs (2,28--2,29);
                               Type
                                 (SignatureParameter
                                    ([], false, Some b,
                                     LongIdent (SynLongIdent ([B], [], [None])),
                                     /root/Type/NamedParametersInDelegateType.fs (2,30--2,34)))],
                              /root/Type/NamedParametersInDelegateType.fs (2,23--2,34)),
                           Fun
                             (SignatureParameter
                                ([], false, Some c,
                                 LongIdent (SynLongIdent ([C], [], [None])),
                                 /root/Type/NamedParametersInDelegateType.fs (2,38--2,41)),
                              LongIdent (SynLongIdent ([D], [], [None])),
                              /root/Type/NamedParametersInDelegateType.fs (2,38--2,46),
                              { ArrowRange =
                                 /root/Type/NamedParametersInDelegateType.fs (2,42--2,44) }),
                           /root/Type/NamedParametersInDelegateType.fs (2,23--2,46),
                           { ArrowRange =
                              /root/Type/NamedParametersInDelegateType.fs (2,35--2,37) }),
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
                                         /root/Type/NamedParametersInDelegateType.fs (2,23--2,27)));
                                   Star
                                     /root/Type/NamedParametersInDelegateType.fs (2,28--2,29);
                                   Type
                                     (SignatureParameter
                                        ([], false, Some b,
                                         LongIdent
                                           (SynLongIdent ([B], [], [None])),
                                         /root/Type/NamedParametersInDelegateType.fs (2,30--2,34)))],
                                  /root/Type/NamedParametersInDelegateType.fs (2,23--2,34)),
                               Fun
                                 (SignatureParameter
                                    ([], false, Some c,
                                     LongIdent (SynLongIdent ([C], [], [None])),
                                     /root/Type/NamedParametersInDelegateType.fs (2,38--2,41)),
                                  LongIdent (SynLongIdent ([D], [], [None])),
                                  /root/Type/NamedParametersInDelegateType.fs (2,38--2,46),
                                  { ArrowRange =
                                     /root/Type/NamedParametersInDelegateType.fs (2,42--2,44) }),
                               /root/Type/NamedParametersInDelegateType.fs (2,23--2,46),
                               { ArrowRange =
                                  /root/Type/NamedParametersInDelegateType.fs (2,35--2,37) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, Some a);
                                 SynArgInfo ([], false, Some b)];
                                [SynArgInfo ([], false, Some c)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDocEmpty, None, None,
                            /root/Type/NamedParametersInDelegateType.fs (2,11--2,46),
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
                         /root/Type/NamedParametersInDelegateType.fs (2,11--2,46),
                         { GetSetKeywords = None })],
                     /root/Type/NamedParametersInDelegateType.fs (2,11--2,46)),
                  [], None,
                  /root/Type/NamedParametersInDelegateType.fs (2,5--2,46),
                  { LeadingKeyword =
                     Type /root/Type/NamedParametersInDelegateType.fs (2,0--2,4)
                    EqualsRange =
                     Some
                       /root/Type/NamedParametersInDelegateType.fs (2,9--2,10)
                    WithKeyword = None })],
              /root/Type/NamedParametersInDelegateType.fs (2,0--2,46))],
          PreXmlDocEmpty, [], None,
          /root/Type/NamedParametersInDelegateType.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))