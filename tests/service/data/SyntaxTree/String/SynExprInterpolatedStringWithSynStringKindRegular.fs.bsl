ImplFile
  (ParsedImplFileInput
     ("/root/String/SynExprInterpolatedStringWithSynStringKindRegular.fs", false,
      QualifiedNameOfFile SynExprInterpolatedStringWithSynStringKindRegular, [],
      [],
      [SynModuleOrNamespace
         ([SynExprInterpolatedStringWithSynStringKindRegular], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (s, None), false, None, (2,4--2,5)), None,
                  InterpolatedString
                    ([String ("yo ", (2,8--2,14));
                      FillExpr (Const (Int32 42, (2,14--2,16)), None);
                      String ("", (2,16--2,18))], Regular, (2,8--2,18)),
                  (2,4--2,5), Yes (2,0--2,18), { LeadingKeyword = Let (2,0--2,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (2,6--2,7) })],
              (2,0--2,18))], PreXmlDocEmpty, [], None, (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
