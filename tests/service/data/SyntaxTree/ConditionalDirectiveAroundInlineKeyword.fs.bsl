ImplFile
  (ParsedImplFileInput
     ("/root/ConditionalDirectiveAroundInlineKeyword.fs", false,
      QualifiedNameOfFile ConditionalDirectiveAroundInlineKeyword, [], [],
      [SynModuleOrNamespace
         ([ConditionalDirectiveAroundInlineKeyword], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, true, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
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
                           /root/ConditionalDirectiveAroundInlineKeyword.fs (5,8--5,9));
                        Named
                          (SynIdent (ar, None), false, None,
                           /root/ConditionalDirectiveAroundInlineKeyword.fs (5,10--5,12))],
                     None,
                     /root/ConditionalDirectiveAroundInlineKeyword.fs (5,4--5,12)),
                  None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, false,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([Async; map],
                              [/root/ConditionalDirectiveAroundInlineKeyword.fs (5,20--5,21)],
                              [None; None]), None,
                           /root/ConditionalDirectiveAroundInlineKeyword.fs (5,15--5,24)),
                        Paren
                          (App
                             (NonAtomic, false,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([Result; map],
                                    [/root/ConditionalDirectiveAroundInlineKeyword.fs (5,32--5,33)],
                                    [None; None]), None,
                                 /root/ConditionalDirectiveAroundInlineKeyword.fs (5,26--5,36)),
                              Ident f,
                              /root/ConditionalDirectiveAroundInlineKeyword.fs (5,26--5,38)),
                           /root/ConditionalDirectiveAroundInlineKeyword.fs (5,25--5,26),
                           Some
                             /root/ConditionalDirectiveAroundInlineKeyword.fs (5,38--5,39),
                           /root/ConditionalDirectiveAroundInlineKeyword.fs (5,25--5,39)),
                        /root/ConditionalDirectiveAroundInlineKeyword.fs (5,15--5,39)),
                     Ident ar,
                     /root/ConditionalDirectiveAroundInlineKeyword.fs (5,15--5,42)),
                  /root/ConditionalDirectiveAroundInlineKeyword.fs (5,4--5,12),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let
                       /root/ConditionalDirectiveAroundInlineKeyword.fs (1,0--1,3)
                    InlineKeyword =
                     Some
                       /root/ConditionalDirectiveAroundInlineKeyword.fs (3,4--3,10)
                    EqualsRange =
                     Some
                       /root/ConditionalDirectiveAroundInlineKeyword.fs (5,13--5,14) })],
              /root/ConditionalDirectiveAroundInlineKeyword.fs (1,0--5,42))],
          PreXmlDocEmpty, [], None,
          /root/ConditionalDirectiveAroundInlineKeyword.fs (1,0--5,42),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives =
         [If
            (Not (Ident "FOO"),
             /root/ConditionalDirectiveAroundInlineKeyword.fs (2,0--2,8));
          EndIf /root/ConditionalDirectiveAroundInlineKeyword.fs (4,0--4,6)]
        CodeComments = [] }, set []))