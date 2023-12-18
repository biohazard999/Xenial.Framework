[![Commitizen friendly](https://img.shields.io/badge/commitizen-friendly-brightgreen.svg)](http://commitizen.github.io/cz-cli/) [![Join the chat at https://gitter.im/Xenial-Framework/community](https://badges.gitter.im/Xenial-Framework/community.svg)](https://gitter.im/Xenial-Framework/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) ![Xenial.Framework](https://github.com/xenial-io/Xenial.Framework/workflows/Xenial.Framework/badge.svg) ![Nuget](https://img.shields.io/nuget/v/Xenial.Framework)

<img src="img/logo.svg" width="100px" />

# Xenial.Framework

Extensions for the DevExpress ExpressApp Framework - Easy. Flexible. Focused.

Platform support: [.NET Standard 2.0 and upwards, including net462](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).  

- [Quick start](#quick-start)
- [Documentation](https://framework.xenial.io)
- [Nuget](https://www.nuget.org/packages?q=Xenial.Framework)
- [Online-Demo](https://framework.featurecenter.xenial.io/)

### Prerequisites

You need to have [node v12](https://nodejs.org/en/download/) and [dotnet sdk 3.1](https://dotnet.microsoft.com/download) installed on your local machine.

Set an environment variable to your [DevExpress Nuget Feed](https://nuget.devexpress.com/#feed-url)

```cmd
SET DEVEXPRESS_NUGET_FEED=https://nuget.devexpress.com/YOUR_API_KEY_HERE/api
```

For operating system different than Windows the steps may vary.

### Commitizen

We use node only for linting commit messages and pushes so make sure to install the proper hooks by running:

```cmd
npm install
```

Afterwards you are able to commit code either by hand using the [commitizen](https://www.npmjs.com/package/commitizen) rules or by running:

```cmd
git cz
//OR
npm run c
```  

### Building

You should be able to build the project by using the build scripts:

```cmd
#Windows
build.bat
#Or for short
b.bat
#Or for powershell
./build.ps1

#Linux & MacOS
. build.sh
```

> To prepare the project for local builds (remove potential warnings from VS) run `build local`

The project uses [bullseye](https://github.com/adamralph/bullseye) to list individual targets use `build -l`.

### Linting and Formatting

This project uses `dotnet format` to keep the code base consistent. If the build fails, you can use `build format` to automatically format code against the rules defined.

### Bypass checks

By default when you commit the message is linted and before you push changes a default integration build is started. To bypass this behavior you can add `--no-verify` to the `git commit` or `git push` commands.

### Writing docs

We use [Wyam](https://wyam.io/) to write the documentation. To serve up docs simply call `b docs.serve` and open `http://localhost:5080/`. All documentation is in the `docs` directory.

## Licensing

Xenial.Framework is dual-licensed under the [License Zero Prosperity Public License](https://licensezero.com/licenses/prosperity) and the [License Zero Private License](https://licensezero.com/licenses/private). The Prosperity License limits commercial use to a thirty-day trial period, after which a license fee must be paid to obtain a Private License.
