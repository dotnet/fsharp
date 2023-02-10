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
                  Named
                    (SynIdent (v, None), false, None,
                     /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (2,4--2,5)),
                  None,
                  ArbitraryAfterError
                    ("localBinding1",
                     /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (2,7--2,7)),
                  /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (2,4--2,5),
                  Yes
                    /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (2,4--2,7),
                  { LeadingKeyword =
                     Let
                       /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (2,6--2,7) })],
              /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (2,0--2,7))],
          PreXmlDocEmpty, [], None,
          /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (2,0--10,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives =
         [If
            (Not (Ident "DEBUG"),
             /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (3,4--3,14));
          If
            (And (Ident "FOO", Ident "BAR"),
             /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (4,8--4,22));
          If
            (Or (Ident "MEH", Ident "HMM"),
             /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (5,12--5,26));
          EndIf
            /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (7,12--7,18);
          EndIf
            /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (8,8--8,14);
          EndIf
            /root/ConditionalDirective/NestedIfEndifWithComplexExpressions.fs (9,4--9,10)]
        CodeComments = [] }, set []))
