SigFile
  (ParsedSigFileInput
     ("/root/ConditionalDirective/DirectivesInMultilineCommentAreNotReportedAsTriviaSignatureFile.fsi",
      QualifiedNameOfFile
        DirectivesInMultilineCommentAreNotReportedAsTriviaSignatureFile, [], [],
      [SynModuleOrNamespaceSig
         ([Foobar], false, DeclaredNamespace,
          [Val
             (SynValSig
                ([], SynIdent (v, None), SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([int], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 Some (Const (Int32 42, (10,4--10,6))), (4,0--10,6),
                 { LeadingKeyword = Val (4,0--4,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = Some (4,12--4,13) }), (4,0--10,6))],
          PreXmlDocEmpty, [], None, (2,0--10,6),
          { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [BlockComment (5,0--9,2)] }, set []))
