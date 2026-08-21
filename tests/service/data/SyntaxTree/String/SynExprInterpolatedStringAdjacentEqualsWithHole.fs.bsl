ImplFile
  (ParsedImplFileInput
     ("/root/String/SynExprInterpolatedStringAdjacentEqualsWithHole.fs", false,
      QualifiedNameOfFile SynExprInterpolatedStringAdjacentEqualsWithHole, [],
      [SynModuleOrNamespace
         ([SynExprInterpolatedStringAdjacentEqualsWithHole], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (n, None), false, None, (1,4--1,5)), None,
                  Const (Int32 42, (1,8--1,10)), (1,4--1,5), Yes (1,0--1,10),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,6--1,7) })], (1,0--1,10),
              { InKeyword = None });
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (2,4--2,5)), None,
                  InterpolatedString
                    ([String ("", (2,7--2,10)); FillExpr (Ident n, None);
                      String ("", (2,11--2,13))], Regular, (2,7--2,13)),
                  (2,4--2,5), Yes (2,0--2,13), { LeadingKeyword = Let (2,0--2,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (2,6--2,7) })],
              (2,0--2,13), { InKeyword = None })], PreXmlDocEmpty, [], None,
          (1,0--3,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
