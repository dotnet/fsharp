ImplFile
  (ParsedImplFileInput
     ("/root/String/SynExprInterpolatedStringWithTripleQuoteMultipleDollars.fs",
      false,
      QualifiedNameOfFile
        SynExprInterpolatedStringWithTripleQuoteMultipleDollars, [], [],
      [SynModuleOrNamespace
         ([SynExprInterpolatedStringWithTripleQuoteMultipleDollars], false,
          AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (s, None), false, None, (2,4--2,5)), None,
                  InterpolatedString
                    ([String ("1 + ", (2,8--2,21));
                      FillExpr (Const (Int32 41, (2,21--2,23)), None);
                      String (" = ", (2,25--2,32));
                      FillExpr (Const (Int32 6, (2,32--2,33)), None);
                      String (" * 7", (2,35--2,43))], TripleQuote, (2,8--2,43)),
                  (2,4--2,5), Yes (2,0--2,43), { LeadingKeyword = Let (2,0--2,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (2,6--2,7) })],
              (2,0--2,43))], PreXmlDocEmpty, [], None, (2,0--2,43),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
