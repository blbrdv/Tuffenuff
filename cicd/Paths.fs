module CICD.Paths

open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

[<Literal>]
let private projExtension = "fsproj"

[<Literal>]
let private projDir = "src"

[<Literal>]
let private projName = "Tuffenuff"

[<Literal>]
let private testsProjDir = "tests"

[<Literal>]
let private integrationTestsProjName = "Tuffenuff.IntegrationTests"

[<Literal>]
let private unitTestsProjName = "Tuffenuff.UnitTests"

[<Literal>]
let private scriptsDir = "scripts"

let inline private proj (name : string) = $"%s{name}.%s{projExtension}"

let inline private projFile (name : string) (subDir : string) =
    subDir @@ name @@ proj name

let inline private genFile (name : string) = $"%s{name}.gen.fs"

let private dslPath = projDir @@ projName @@ "DSL"

let srcProjFile = projFile projName projDir
let packageFile = projDir @@ projName @@ "**/*.nupkg"

let integrationTestsProjFile = projFile integrationTestsProjName testsProjDir

let unitTestsProjFile = projFile unitTestsProjName testsProjDir

let syntaxVersionsGeneratorScript = scriptsDir @@ "SyntaxVersionsGenerator.fsx"

let syntaxVersionFile = dslPath @@ genFile "SyntaxVersions"
let syntaxUpstreamVersionFile = dslPath @@ genFile "UpstreamSyntaxVersions"

let buildDirs = !! "**/bin" ++ "**/obj" -- "cicd/**"
