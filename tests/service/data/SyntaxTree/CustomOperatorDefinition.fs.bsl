ImplFile
  (ParsedImplFileInput
     ("/root/CustomOperatorDefinition.fs", false,
      QualifiedNameOfFile CustomOperatorDefinition, [], [],
      [SynModuleOrNamespace
         ([CustomOperatorDefinition], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some a)];
                         [SynArgInfo ([], false, Some b)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent
                       ([op_Addition], [],
                        [Some
                           (OriginalNotationWithParen
                              (/root/CustomOperatorDefinition.fs (1,4--1,5), "+",
                               /root/CustomOperatorDefinition.fs (1,6--1,7)))]),
                     None, None,
                     Pats
                       [Named
                          (SynIdent (a, None), false, None,
                           /root/CustomOperatorDefinition.fs (1,8--1,9));
                        Named
                          (SynIdent (b, None), false, None,
                           /root/CustomOperatorDefinition.fs (1,10--1,11))],
                     None, /root/CustomOperatorDefinition.fs (1,4--1,11)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None, /root/CustomOperatorDefinition.fs (1,16--1,17)),
                        Ident a, /root/CustomOperatorDefinition.fs (1,14--1,17)),
                     Ident b, /root/CustomOperatorDefinition.fs (1,14--1,19)),
                  /root/CustomOperatorDefinition.fs (1,4--1,11), NoneAtLet,
                  { LeadingKeyword =
                     Let /root/CustomOperatorDefinition.fs (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some /root/CustomOperatorDefinition.fs (1,12--1,13) })],
              /root/CustomOperatorDefinition.fs (1,0--1,19))], PreXmlDocEmpty,
          [], None, /root/CustomOperatorDefinition.fs (1,0--1,19),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))