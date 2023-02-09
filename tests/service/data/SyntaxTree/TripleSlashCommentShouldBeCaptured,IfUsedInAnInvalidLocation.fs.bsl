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
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (x, None), false, None,
                     /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (2,4--2,5)),
                  None,
                  Sequential
                    (SuppressNeither, true,
                     While
                       (Yes
                          /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (7,4--7,14),
                        Const
                          (Bool true,
                           /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (7,10--7,14)),
                        Const
                          (Unit,
                           /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (7,18--7,20)),
                        /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (7,4--7,20)),
                     App
                       (NonAtomic, false,
                        App
                          (NonAtomic, true,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([op_Addition], [],
                                 [Some (OriginalNotation "+")]), None,
                              /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (8,6--8,7)),
                           Ident a,
                           /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (8,4--8,7)),
                        Const
                          (Int32 1,
                           /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (8,8--8,9)),
                        /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (8,4--8,9)),
                     /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (7,4--8,9)),
                  /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (1,0--2,5),
                  NoneAtLet,
                  { LeadingKeyword =
                     Let
                       /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (2,6--2,7) })],
              /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (1,0--8,9))],
          PreXmlDocEmpty, [], None,
          /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (2,0--8,9),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment
            /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (3,4--3,40);
          LineComment
            /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (5,4--5,36);
          LineComment
            /root/TripleSlashCommentShouldBeCaptured,IfUsedInAnInvalidLocation.fs (6,4--6,27)] },
      set []))