# Installation

Package is [provided on NuGet][nuget-pkg], listed as `Dvchevskii.Unit`

You can use various methods to install it in your project:

## .NET CLI

```sh
dotnet add path/to/project.csproj package Dvchevskii.Unit
```

## Visual Studio package manager console

```ps1
NuGet\Install-Package Dvchevskii.Unit
```

## PackageReference tag

```xml
<PackageReference Include="Dvchevskii.Unit" Version="1.0.3" />
```

## Alternative installation options

### Installing packages provided from branch builds

You can use packages which are built on every commit by GitHub actions

Builds are performed by the `main` workflow, and can be found on [this page][main-workflow]

The name of the artifact is `packages`

After downloading it, you have to extract the archive containing packages inside your local folder, which
serves as a NuGet source, which has to be pre-configured.
The final path would look like something along the lines of `/your/local/nuget/source/Dvchevskii.Optional/<version>/Dvchevskii.Optional.nupkg`.
If all of the above was done, you will be able to install it with
any method listed before

### Referencing libraries directly

The aforementioned [workflow][main-workflow] also provides an artifact with built libraries
not packed inside the `.nupkg` file, but inside an archived directory

The name of the artifact is `libraries`

You can use this to reference libraries directly in your project

[nuget-pkg]: https://nuget.org/packages/Dvchevskii.Unit
[main-workflow]: https://github.com/2chevskii/unit/actions/workflows/main.yml
