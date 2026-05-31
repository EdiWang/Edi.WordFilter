# Project Guidelines

## Project Context

Edi.WordFilter is a small NuGet library for masking banned words in text. The main implementation is a trie-based `IMaskWordFilter` in `src/Edi.WordFilter/TrieTreeWordFilter.cs`, with word sources supplied through `IWordSource` implementations.

Treat public types and method signatures as package API. Avoid changing names, visibility, default split behavior, target frameworks, or masking semantics unless the task explicitly requires a breaking change.

## Code Style

- Use modern C# consistent with the existing code: file-scoped namespaces, primary constructors where useful, target-typed `new`, collection expressions, `var` for obvious local types, and early returns for guard clauses.
- Keep the library dependency-free unless there is a strong reason to add a package.
- Keep hot paths allocation-conscious. `TrieTreeWordFilter` currently uses `ReadOnlySpan<char>`, `Span<char>`, `stackalloc`, and `ArrayPool<char>`; preserve that style for filtering and search logic.
- Use `char.ToLowerInvariant` / invariant casing for word matching. The filter is case-insensitive and should not depend on current culture.
- Do not add generated output to source control. Ignore `bin/`, `obj/`, `TestResults/`, and NuGet package artifacts.

## Architecture

- `IWordSource` abstracts banned-word input. `StringWordSource` splits an in-memory string; `FileSystemWordSource` reads UTF-8 text from the first line of a file.
- `TrieTreeWordFilter` lowercases words when adding them to the trie, then scans content case-insensitively.
- `FilterContent` must preserve the original string length and replace matched characters with `*` only.
- When multiple words overlap, preserve the existing longest-match behavior.
- `ContainsAnyWord` should return as soon as any configured word is found.

## Build and Test

This repository requires a .NET SDK that can build `net10.0`; the package project also targets `net8.0`.

From the repository root, prefer these commands:

```powershell
dotnet restore src/Edi.WordFilter.slnx
dotnet build src/Edi.WordFilter.slnx -c Release
dotnet test src/Edi.WordFilter.Tests/Edi.WordFilter.Tests.csproj -c Release
```

For packaging checks, use:

```powershell
dotnet pack src/Edi.WordFilter/Edi.WordFilter.csproj -c Release
```

The VS Code `build` task builds the test project and is a good quick validation path.

## Testing Conventions

- Tests live in `src/Edi.WordFilter.Tests/TrieTreeWordFilterTests.cs` and use xUnit v3 `[Fact]` tests.
- Add or update tests for behavior changes, especially case-insensitive matching, empty input, duplicate or malformed word sources, overlapping words, single-character words, special characters, and long content over the stackalloc threshold.
- Keep assertions concrete and behavior-focused. Prefer testing through `IMaskWordFilter`/`TrieTreeWordFilter` public behavior over reflection.

## Documentation

Update `README.md` when usage, package behavior, target frameworks, or public API examples change. Keep README examples aligned with the actual constructors and default `|` split character.