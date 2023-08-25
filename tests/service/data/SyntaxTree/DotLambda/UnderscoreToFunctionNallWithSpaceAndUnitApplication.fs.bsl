ImplFile
  (ParsedImplFileInput
     ("/root/DotLambda/UnderscoreToFunctionNallWithSpaceAndUnitApplication.fs",
      false,
      QualifiedNameOfFile UnderscoreToFunctionNallWithSpaceAndUnitApplication,
      [], [],
      [SynModuleOrNamespace
         ([UnderscoreToFunctionNallWithSpaceAndUnitApplication], false,
          AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None,
                     None),
                  Named (SynIdent (myFunc, None), false, None, (1,4--1,10)),
                  None,
                  DotLambda
                    (App
                       (NonAtomic, false, Ident MyMethodCall,
                        Const (Unit, (1,28--1,30)), (1,15--1,30)), (1,13--1,30),
                     { UnderscoreRange = (1,13--1,14)
                       DotRange = (1,14--1,15) }), (1,4--1,10), Yes (1,0--1,30),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,11--1,12) })], (1,0--1,30))],
          PreXmlDocEmpty, [], None, (1,0--1,30), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(1,30) parse error Incomplete structured construct at or before this point in expression
(1,13)-(1,14) parse error  _. shorthand syntax for lambda functions can only be used with atomic expressions. That means expressions with no whitespace unless enclosed in parentheses.
