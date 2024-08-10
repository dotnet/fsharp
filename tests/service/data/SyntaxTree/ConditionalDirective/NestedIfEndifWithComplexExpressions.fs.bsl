ImplFile
  (ParsedImplFileInput
     ("/root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs", false,
      QualifiedNameOfFile NestedIfEndifWithComplexExpressions, [], [],
      [SynModuleOrNamespace
         ([NestedIfEndifWithComplexExpressions], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (v, None), false, None, (2,4--2,5)), None,
                  Const (Unit, (11,4--11,6)), (2,4--2,5), Yes (2,0--11,6),
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,6--2,7) })], (2,0--11,6))],
          PreXmlDocEmpty, [], None, (2,0--12,0), { LeadingKeyword = None })],
      (true, true),
      { ConditionalDirectives =
         [If (Not (Ident "DEBUG"), (3,4--3,14));
          If (And (Ident "FOO", Ident "BAR"), (4,8--4,22));
          If (Or (Ident "MEH", Ident "HMM"), (5,12--5,26)); EndIf (7,12--7,18);
          EndIf (8,8--8,14); EndIf (9,4--9,10)]
        CodeComments = [] }, set []))
