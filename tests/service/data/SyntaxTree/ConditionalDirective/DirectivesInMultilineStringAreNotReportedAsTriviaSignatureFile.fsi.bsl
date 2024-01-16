SigFile
  (ParsedSigFileInput
     ("/root/ConditionalDirective/DirectivesInMultilineStringAreNotReportedAsTriviaSignatureFile.fsi",
      QualifiedNameOfFile
        DirectivesInMultilineStringAreNotReportedAsTriviaSignatureFile, [], [],
      [SynModuleOrNamespaceSig
         ([Foobar], false, DeclaredNamespace,
          [Val
             (SynValSig
                ([], SynIdent (v, None), SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([string], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 Some
                   (Const
                      (String
                         ("
#if DEBUG
()
#endif
42
................",
                          TripleQuote, (4,17--9,19)), (4,17--9,19))),
                 (4,0--9,19), { LeadingKeyword = Val (4,0--4,3)
                                InlineKeyword = None
                                WithKeyword = None
                                EqualsRange = Some (4,15--4,16) }), (4,0--9,19))],
          PreXmlDocEmpty, [], None, (2,0--9,19),
          { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
