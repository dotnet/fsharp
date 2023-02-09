ImplFile
  (ParsedImplFileInput
     ("/root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs",
      false,
      QualifiedNameOfFile
        TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation, [], [],
      [SynModuleOrNamespace
         ([TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation], false,
          AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (x, None), false, None,
                     /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (3,4--3,5)),
                  None,
                  Sequential
                    (SuppressNeither, true,
                     While
                       (Yes
                          /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (8,4--8,14),
                        Const
                          (Bool true,
                           /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (8,10--8,14)),
                        Const
                          (Unit,
                           /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (8,18--8,20)),
                        /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (8,4--8,20)),
                     App
                       (NonAtomic, false,
                        App
                          (NonAtomic, true,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([op_Addition], [],
                                 [Some (OriginalNotation "+")]), None,
                              /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (9,6--9,7)),
                           Ident a,
                           /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (9,4--9,7)),
                        Const
                          (Int32 1,
                           /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (9,8--9,9)),
                        /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (9,4--9,9)),
                     /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (8,4--9,9)),
                  /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (2,0--3,5),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let
                       /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (3,6--3,7) })],
              /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (2,0--9,9))],
          PreXmlDocEmpty, [], None,
          /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (3,0--10,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment
            /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (4,4--4,40);
          LineComment
            /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (6,4--6,36);
          LineComment
            /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (7,4--7,27)] },
      set []))