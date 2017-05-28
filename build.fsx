#r "packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing.XUnit2

let buildDir = "./build"
let testDir = "./test"

let runnerPath = "./packages/xunit.runner.console/tools/xunit.console.exe"

Target "Clean" (fun _ -> CleanDirs [ buildDir; testDir ])

Target "BuildApp" (fun _ ->
    !! "src/**/*.fsproj"
    -- "src/**/*.Tests.fsproj"
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: ")

Target "BuildTests" (fun _ ->
    !! "src/**/*.Tests.fsproj"
    |> MSBuildDebug testDir "Build"
    |> Log "TestsBuild-Output: ")

Target "RunUnitTests" (fun _ ->
    !! (testDir @@ "*.Tests.dll")
    |> xUnit2 (fun p -> { p with ToolPath = runnerPath }))

"Clean"
    ==> "BuildApp"
    ==> "BuildTests"
    ==> "RunUnitTests"

RunTargetOrDefault "RunUnitTests"
