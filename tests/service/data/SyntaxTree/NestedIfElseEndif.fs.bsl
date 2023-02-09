ImplFile
  (ParsedImplFileInput
     ("/root/NestedIfElseEndif.fs", false, QualifiedNameOfFile NestedIfElseEndif,
      [], [],
      [SynModuleOrNamespace
         ([NestedIfElseEndif], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (v, None), false, None,
                     /root/NestedIfElseEndif.fs (2,4--2,5)), None,
                  Const (Int32 3, /root/NestedIfElseEndif.fs (10,8--10,9)),
                  /root/NestedIfElseEndif.fs (2,4--2,5),
                  Yes /root/NestedIfElseEndif.fs (2,0--10,9),
                  { LeadingKeyword = Let /root/NestedIfElseEndif.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some /root/NestedIfElseEndif.fs (2,6--2,7) })],
              /root/NestedIfElseEndif.fs (2,0--10,9))], PreXmlDocEmpty, [], None,
          /root/NestedIfElseEndif.fs (2,0--12,0), { LeadingKeyword = None })],
      (true, false),
      { ConditionalDirectives =
         [If (Ident "FOO", /root/NestedIfElseEndif.fs (3,4--3,11));
          If (Ident "MEH", /root/NestedIfElseEndif.fs (4,8--4,15));
          Else /root/NestedIfElseEndif.fs (6,8--6,13);
          EndIf /root/NestedIfElseEndif.fs (8,8--8,14);
          Else /root/NestedIfElseEndif.fs (9,4--9,9);
          EndIf /root/NestedIfElseEndif.fs (11,4--11,10)]
        CodeComments = [] }, set []))