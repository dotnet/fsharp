ImplFile
  (ParsedImplFileInput
     ("/root/String/SynExprInterpolatedStringWithSynStringKindTripleQuote.fs",
      false,
      QualifiedNameOfFile SynExprInterpolatedStringWithSynStringKindTripleQuote,
      [], [],
      [SynModuleOrNamespace
         ([SynExprInterpolatedStringWithSynStringKindTripleQuote], false,
          AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None,
                     None), Named (SynIdent (s, None), false, None, (2,4--2,5)),
                  None,
                  InterpolatedString
                    ([String ("yo ", (2,8--2,16));
                      FillExpr (Const (Int32 42, (2,16--2,18)), None);
                      String ("", (2,18--2,22))], TripleQuote, (2,8--2,22)),
                  (2,4--2,5), Yes (2,0--2,22), { LeadingKeyword = Let (2,0--2,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (2,6--2,7) })],
              (2,0--2,22))], PreXmlDocEmpty, [], None, (2,0--3,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
