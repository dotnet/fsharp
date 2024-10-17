ImplFile
  (ParsedImplFileInput
     ("/root/ConditionalDirective/NestedIfElseEndif.fs", false,
      QualifiedNameOfFile NestedIfElseEndif, [], [],
      [SynModuleOrNamespace
         ([NestedIfElseEndif], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (v, None), false, None, (2,4--2,5)), None,
                  Const (Int32 3, (10,8--10,9)), (2,4--2,5), Yes (2,0--10,9),
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,6--2,7) })], (2,0--10,9))],
          PreXmlDocEmpty, [], None, (2,0--12,0), { LeadingKeyword = None })],
      (true, true),
      { ConditionalDirectives =
         [If (Ident "FOO", (3,4--3,11)); If (Ident "MEH", (4,8--4,15));
          Else (6,8--6,13); EndIf (8,8--8,14); Else (9,4--9,9);
          EndIf (11,4--11,10)]
        CodeComments = [] }, set []))
