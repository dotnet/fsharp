SigFile
  (ParsedSigFileInput
     ("/root/OperatorName/OperatorNameInSynValSig.fsi",
      QualifiedNameOfFile IntrinsicOperators, [], [],
      [SynModuleOrNamespaceSig
         ([IntrinsicOperators], false, NamedModule,
          [Val
             (SynValSig
                ([],
                 SynIdent
                   (op_Amp,
                    Some
                      (OriginalNotationWithParen ((3,4--3,5), "&", (3,6--3,7)))),
                 SynValTyparDecls (None, true),
                 Fun
                   (SignatureParameter
                      ([], false, Some e1,
                       LongIdent (SynLongIdent ([bool], [], [None])),
                       (3,9--3,17)),
                    Fun
                      (SignatureParameter
                         ([], false, Some e2,
                          LongIdent (SynLongIdent ([bool], [], [None])),
                          (3,21--3,29)),
                       LongIdent (SynLongIdent ([bool], [], [None])),
                       (3,21--3,37), { ArrowRange = (3,30--3,32) }), (3,9--3,37),
                    { ArrowRange = (3,18--3,20) }),
                 SynValInfo
                   ([[SynArgInfo ([], false, Some e1)];
                     [SynArgInfo ([], false, Some e2)]],
                    SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 None, (3,0--3,37), { LeadingKeyword = Val (3,0--3,3)
                                      InlineKeyword = None
                                      WithKeyword = None
                                      EqualsRange = None }), (3,0--3,37))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--3,37), { LeadingKeyword = Module (2,0--2,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
