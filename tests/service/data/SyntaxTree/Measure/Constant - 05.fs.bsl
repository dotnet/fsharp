ImplFile
  (ParsedImplFileInput
     ("/root/Measure/Constant - 05.fs", false, QualifiedNameOfFile Constant - 05,
      [], [],
      [SynModuleOrNamespace
         ([Constant - 05], false, AnonModule,
          [Expr
             (Const
                (Measure
                   (Int32 42, (1,0--1,2),
                    Divide
                      (Some (Seq ([One (1,4--1,7)], (1,4--1,7))),
                       Seq ([Named ([m], (1,10--1,11))], (1,10--1,11)),
                       (1,4--1,11)), { LessRange = (1,2--1,3)
                                       GreaterRange = (1,11--1,12) }),
                 (1,0--1,12)), (1,0--1,12))], PreXmlDocEmpty, [], None,
          (1,0--1,12), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(1,0)-(1,12) parse warning The declarations in this file will be placed in an implicit module 'Constant - 05' based on the file name 'Constant - 05.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
