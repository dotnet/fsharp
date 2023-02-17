ImplFile
  (ParsedImplFileInput
     ("/root/Expression/SynExprRecordContainsTheRangeOfTheEqualsSignInSynExprRecordField.fs",
      false,
      QualifiedNameOfFile
        SynExprRecordContainsTheRangeOfTheEqualsSignInSynExprRecordField, [], [],
      [SynModuleOrNamespace
         ([SynExprRecordContainsTheRangeOfTheEqualsSignInSynExprRecordField],
          false, AnonModule,
          [Expr
             (Record
                (None, None,
                 [SynExprRecordField
                    ((SynLongIdent ([V], [], [None]), true), Some (2,4--2,5),
                     Some (Ident v), Some ((2,8--3,2), None));
                  SynExprRecordField
                    ((SynLongIdent ([X], [], [None]), true), Some (3,9--3,10),
                     Some
                       (App
                          (NonAtomic, false,
                           App
                             (NonAtomic, false,
                              App
                                (NonAtomic, false, Ident someLongFunctionCall,
                                 Ident a, (4,16--5,21)), Ident b, (4,16--6,21)),
                           Ident c, (4,16--7,21))), None)], (2,0--7,23)),
              (2,0--7,23))], PreXmlDocEmpty, [], None, (2,0--7,23),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [LineComment (3,13--3,28)] }, set []))
