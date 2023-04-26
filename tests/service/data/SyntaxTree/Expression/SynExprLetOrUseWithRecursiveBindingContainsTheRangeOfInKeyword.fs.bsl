ImplFile
  (ParsedImplFileInput
     ("/root/Expression/SynExprLetOrUseWithRecursiveBindingContainsTheRangeOfInKeyword.fs",
      false,
      QualifiedNameOfFile
        SynExprLetOrUseWithRecursiveBindingContainsTheRangeOfInKeyword, [], [],
      [SynModuleOrNamespace
         ([SynExprLetOrUseWithRecursiveBindingContainsTheRangeOfInKeyword],
          false, AnonModule,
          [Expr
             (Do
                (LetOrUse
                   (true, false,
                    [SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named (SynIdent (f, None), false, None, (3,12--3,13)),
                        None, Const (Unit, (3,16--3,18)), (3,12--3,13),
                        Yes (3,4--3,18),
                        { LeadingKeyword = LetRec ((3,4--3,7), (3,8--3,11))
                          InlineKeyword = None
                          EqualsRange = Some (3,14--3,15) });
                     SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((4,8), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named (SynIdent (g, None), false, None, (4,8--4,9)),
                        None, Const (Unit, (4,12--4,14)), (4,8--4,9),
                        Yes (4,4--4,14), { LeadingKeyword = And (4,4--4,7)
                                           InlineKeyword = None
                                           EqualsRange = Some (4,10--4,11) })],
                    Const (Unit, (5,4--5,6)), (3,4--5,6),
                    { InKeyword = Some (4,15--4,17) }), (2,0--5,6)), (2,0--5,6))],
          PreXmlDocEmpty, [], None, (2,0--6,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
