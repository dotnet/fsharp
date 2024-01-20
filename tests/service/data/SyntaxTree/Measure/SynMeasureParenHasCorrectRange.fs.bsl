ImplFile
  (ParsedImplFileInput
     ("/root/Measure/SynMeasureParenHasCorrectRange.fs", false,
      QualifiedNameOfFile SynMeasureParenHasCorrectRange, [], [],
      [SynModuleOrNamespace
         ([SynMeasureParenHasCorrectRange], false, AnonModule,
          [Expr
             (Const
                (Measure
                   (UInt32 40u, (2,0--2,3),
                    Divide
                      (Some (Seq ([Named ([hr], (2,4--2,6))], (2,4--2,6))),
                       (2,7--2,8),
                       Seq
                         ([Paren
                             (Seq
                                ([Named ([staff], (2,10--2,15));
                                  Named ([weeks], (2,16--2,21))], (2,10--2,21)),
                              (2,9--2,22))], (2,9--2,22)), (2,4--2,22)),
                    { LessRange = (2,3--2,4)
                      GreaterRange = (2,22--2,23) }), (2,0--2,23)), (2,0--2,23))],
          PreXmlDocEmpty, [], None, (2,0--2,23), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
