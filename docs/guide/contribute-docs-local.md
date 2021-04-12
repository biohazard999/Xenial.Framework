---
title: Contribute Documentation (locally)
---

# Contribute to Documentation locally

If you prefer a local environment with syntax highlighting, spell checking, version control and visual feedback, you can contribute via submitting pull-requests.  

This requires a little bit more setup, but if you are a developer most of the tools are probably installed on your machine anyway.

## Technology stack

We use a couple of tools to write, build and deploy our docs. Currently we are using the following ones:

* [GIT](https://git-scm.com/) (for version control)
* [Github-Actions](https://github.com/features/actions) (for Continuous Integration / Continuous Delivery)
* [.NET](http://dot.net/) (for the build pipeline)
  - [SimpleExec](https://github.com/adamralph/simple-exec)
  - [BullsEye](https://github.com/adamralph/bullseye)
* [NodeJS](https://nodejs.org/en/) (for writing the docs)
  - [VuePress](http://vuepress.vuejs.org/)

We write docs in [markdown](https://en.wikipedia.org/wiki/Markdown) to allow for easy versioning and consistent styling.  

You can use your text-editor or IDE of your choice, but we would recommend to use [VSCode](https://code.visualstudio.com/) because it provides a couple of handy extensions to help writing documentation.

## Prerequisites

* [Install VSCode](https://code.visualstudio.com/) (optional)
* [Install GIT](https://git-scm.com/download/)
  - Choose `Use Visual Studio Code as GIT's default editor` (optional)
* [Install .NET SDK](https://dotnet.microsoft.com/download)
  - We use the stable release, so choose the `recommended` version for your platform
* [Install NodeJS](https://nodejs.org/en/download/)
  - We use the LTS release, so choose the `LTS Recommended For Most Users` version for your platform
