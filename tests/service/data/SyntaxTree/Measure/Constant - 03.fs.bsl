ImplFile
  (ParsedImplFileInput
     ("/root/Measure/Constant - 03.fs", false, QualifiedNameOfFile Constant - 03,
      [], [],
      [SynModuleOrNamespace
         ([Constant - 03], false, AnonModule,
          [Expr
             (Const
                (Measure
                   (Int32 23, (1,0--1,2), Seq ([One (1,3--1,4)], (1,3--1,4)),
                    { LessRange = (1,2--1,3)
                      GreaterRange = (1,4--1,5) }), (1,0--1,5)), (1,0--1,5))],
          PreXmlDocEmpty, [], None, (1,0--1,5), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(1,5) parse warning The declarations in this file will be placed in an implicit module 'Constant - 03' based on the file name 'Constant - 03.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
