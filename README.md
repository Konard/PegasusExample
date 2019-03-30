# Pegasus Example

Master branch contains proof of concept example of using [Pegasus](https://github.com/otac0n/Pegasus) PEG generator configured as a [MSBuild](https://github.com/Microsoft/msbuild) Task.

If you require all in code dynamic compilation, here an example at [dynamic-compile branch](https://github.com/Konard/PegasusExample/tree/dynamic-compile)

### Setup
Requires .NET Core 2.2 SDK and Runtime.

https://dotnet.microsoft.com/download

### Run
```
dotnet run
```

### CompilePegGrammar Task

If you need to have more control over parsers' compilation than you might need to use CompilePegGrammar:

Replace:
```XML
  <ItemGroup>
    <PegGrammar Include="Test.peg" />
  </ItemGroup>
```

With:
```XML
  <Target Name="CompilePegGrammar" BeforeTargets="BeforeBuild"  >
    <CompilePegGrammar InputFiles="./Test.peg" OutputFiles="./Test.peg.g.cs" ></CompilePegGrammar>
    <ItemGroup>
      <Compile Remove="Test.peg.g.cs" />
      <Compile Include="Test.peg.g.cs" />
    </ItemGroup>
  </Target>
```

CompilePegGrammar generates files in your working directory, so don't forget to add .gitignore rule for that.
```
# Generated code
*.g.cs
```

The full example of using CompilePegGrammar can be found here: https://github.com/Konard/PegasusExample/tree/9b0d0abf0fb7cfc8ec14f01eded55a8cd62c08b8

## Development process videos
https://www.youtube.com/playlist?list=PLeYxH0Vd8lotSflSnuA0TlaFUWQ7-E2af
