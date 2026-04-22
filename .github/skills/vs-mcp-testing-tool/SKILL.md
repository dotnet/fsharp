---
name: vs-mcp-testing-tools
description: Guide for using Visual Studio MCP tools for Visual Studio automation, troubleshooting, and debugging test generation workflows. Use this when users want to open their solution in Visual Studio and build, run tests or use the Copilot Chat window.
license: MIT
---

# Visual Studio MCP Tools

This skill provides guidance on using the Visual Studio MCP (Model Context Protocol) server tools for automating Visual Studio workflows, test generation, and troubleshooting.

## Quick Start

1. Verify `vs-open-solution` tool is available (MCP server is set up).
2. Open a solution: `vs-open-solution path="C:\path\to\MySolution.sln"`
4. Open a source file with `vs-open-file`
3. Wait 10 seconds for the errors to populate, then use `vs-get-error-list` tool to get the list of errors.

## Features

| Document | Contents |
|---|---|
| [REFERENCE.md](REFERENCE.md) | Complete MCP tools API reference — all tool names, parameters, setup, and configuration |

## When to Use This Skill

Use this skill when users need help with:

- Setting up or using VS MCP testing tools
- Integration testing VS F# features
- Debugging F# LSP server at the VS UI level

## Key Rules

- Use **full absolute paths in single quotes** for file-based annotations via MCP (e.g., `#file:'C:\path\to\File.cs'`).
- Don't mix different annotation scope types in a single request.
