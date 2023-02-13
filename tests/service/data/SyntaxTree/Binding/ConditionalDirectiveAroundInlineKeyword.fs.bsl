ImplFile
  (ParsedImplFileInput
     ("/root/Binding/ConditionalDirectiveAroundInlineKeyword.fs", false,
      QualifiedNameOfFile ConditionalDirectiveAroundInlineKeyword, [], [],
      [SynModuleOrNamespace
         ([ConditionalDirectiveAroundInlineKeyword], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, true, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some f)];
                         [SynArgInfo ([], false, Some ar)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([map], [], [None]), None, None,
                     Pats
                       [Named
                          (SynIdent (f, None), false, None,
                           /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,8--6,9));
                        Named
                          (SynIdent (ar, None), false, None,
                           /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,10--6,12))],
                     None,
                     /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,4--6,12)),
                  None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, false,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([Async; map],
                              [/root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,20--6,21)],
                              [None; None]), None,
                           /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,15--6,24)),
                        Paren
                          (App
                             (NonAtomic, false,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([Result; map],
                                    [/root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,32--6,33)],
                                    [None; None]), None,
                                 /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,26--6,36)),
                              Ident f,
                              /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,26--6,38)),
                           /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,25--6,26),
                           Some
                             /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,38--6,39),
                           /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,25--6,39)),
                        /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,15--6,39)),
                     Ident ar,
                     /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,15--6,42)),
                  /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,4--6,12),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let
                       /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (2,0--2,3)
                    InlineKeyword =
                     Some
                       /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (4,4--4,10)
                    EqualsRange =
                     Some
                       /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (6,13--6,14) })],
              /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (2,0--6,42))],
          PreXmlDocEmpty, [], None,
          /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (2,0--7,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives =
         [If
            (Not (Ident "FOO"),
             /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (3,0--3,8));
          EndIf
            /root/Binding/ConditionalDirectiveAroundInlineKeyword.fs (5,0--5,6)]
        CodeComments = [] }, set []))
