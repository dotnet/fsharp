// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// The QuickParse unit tests that used to live here (the CheckGetPartialLongName member and the
// CheckIsland0..CheckIsland50 members) were direct public-API tests of
// FSharp.Compiler.EditorServices.QuickParse with no Salsa harness. They were migrated to the
// cross-platform corpus at tests/FSharp.Compiler.Service.Tests/QuickParseTests.fs (M2
// quickparse batch 1), where the CheckIsland family is a single parametrized Theory, the
// GetPartialLongNameEx checks are a second parametrized Theory, and the commented-out
// CheckIsland25 is a skipped Fact. This file is intentionally left as an empty namespace.

namespace Tests.LanguageService
