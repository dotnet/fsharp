ImplFile
  (ParsedImplFileInput
     ("/root/CodeComment/TripleSlashCommentShouldBeCapturedIfUsedInAnInvalidLocation.fs",
      false,
      QualifiedNameOfFile
        TripleSlashCommentShouldBeCapturedIfUsedInAnInvalidLocation, [], [],
      [SynModuleOrNamespace
         ([TripleSlashCommentShouldBeCapturedIfUsedInAnInvalidLocation], false,
          AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (3,4--3,5)), None,
                  Sequential
                    (SuppressNeither, true,
                     While
                       (Yes (8,4--8,14), Const (Bool true, (8,10--8,14)),
                        Const (Unit, (8,18--8,20)), (8,4--8,20)),
                     App
                       (NonAtomic, false,
                        App
                          (NonAtomic, true,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([op_Addition], [],
                                 [Some (OriginalNotation "+")]), None,
                              (9,6--9,7)), Ident a, (9,4--9,7)),
                        Const (Int32 1, (9,8--9,9)), (9,4--9,9)), (8,4--9,9),
                     { SeparatorRange = None }), (2,0--3,5), NoneAtLet,
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,6--3,7) })], (2,0--9,9))],
          PreXmlDocEmpty, [], None, (3,0--10,0), { LeadingKeyword = None })],
      (true, true),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment (4,4--4,40); LineComment (6,4--6,36);
          LineComment (7,4--7,27)] }, set []))

(4,4)-(7,27) parse info XML comment is not placed on a valid language element.
