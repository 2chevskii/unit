<script setup>
import packageJson from '../package.json';

const packageVersion = packageJson['latestReleaseVersion'];

</script>

# Installation

Package is [provided on NuGet][nuget-pkg], listed as `Dvchevskii.Unit`

Latest package version is: <code>{{ packageVersion }}</code>

You can use various methods to install it in your project:

## .NET CLI

<div class="language-sh vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">sh</span><pre class="shiki shiki-themes github-light github-dark vp-code"><code><span class="line"><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">dotnet</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> add path/to/project.csproj package Dvchevskii.Unit --version </span><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;">{{ packageVersion }}</span></span></code></pre></div>

## Visual Studio package manager console

<div class="language-powershell vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">powershell</span><pre class="shiki shiki-themes github-light github-dark vp-code"><code><span class="line"><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;">NuGet\Install-Package</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> Dvchevskii.Unit </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">-</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">Version </span><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;">{{ packageVersion }}</span></span></code></pre></div>

## PackageReference tag

<div class="language-xml vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">xml</span><pre class="shiki shiki-themes github-light github-dark vp-code"><code><span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#22863A;--shiki-dark:#85E89D;">PackageReference</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Include</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">=</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;">"Dvchevskii.Unit"</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Version</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">=</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;">"{{ packageVersion }}"</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> /&gt;</span></span></code></pre></div>

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
