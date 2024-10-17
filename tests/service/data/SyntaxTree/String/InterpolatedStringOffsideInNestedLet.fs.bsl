ImplFile
  (ParsedImplFileInput
     ("/root/String/InterpolatedStringOffsideInNestedLet.fs", false,
      QualifiedNameOfFile InterpolatedStringOffsideInNestedLet, [], [],
      [SynModuleOrNamespace
         ([InterpolatedStringOffsideInNestedLet], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (a, None), false, None, (1,4--1,5)), None,
                  LetOrUse
                    (false, false,
                     [SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (None, SynValInfo ([], SynArgInfo ([], false, None)),
                            None),
                         Named (SynIdent (b, None), false, None, (2,8--2,9)),
                         None,
                         InterpolatedString
                           ([String ("
", (3,8--4,1));
                             FillExpr (Const (Int32 0, (4,1--4,2)), None);
                             String ("", (4,2--4,4))], Regular, (3,8--4,4)),
                         (2,8--2,9), Yes (2,4--4,4),
                         { LeadingKeyword = Let (2,4--2,7)
                           InlineKeyword = None
                           EqualsRange = Some (2,10--2,11) })], Ident b,
                     (2,4--5,5), { LetOrUseKeyword = (2,4--2,7)
                                   InKeyword = None }), (1,4--1,5), NoneAtLet,
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,6--1,7) })], (1,0--5,5))],
          PreXmlDocEmpty, [], None, (1,0--5,5), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
