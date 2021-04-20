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

## Clone the sources

First of all you need to [fork the repository](https://github.com/xenial-io/Xenial.Framework/fork) on [github](https://github.com/xenial-io/Xeniak.Framework).

![Fork the repository](/images/guide/contribute/fork-repo.png)

Afterwards you need to clone the repository from your fork. To do that you need to copy your url under the `code` button.

![Fork the repository](/images/guide/contribute/clone-fork.png)

```cmd
git clone https://github.com/YOUR_GITHUB_NAME/Xenial.Framework.git
```

If you never worked with git before, its necessary to set your name and email address to start contributing:

```cmd
git config --global user.email "you@example.com"
git config --global user.name "Your Name"
```

In order to be able to keep your fork in sync with the `Xenial.Framework` main repository, we need to set an upstream:

```cmd
git remote add upstream https://github.com/xenial-io/Xenial.Framework.git
```


## Build the documentation

After cloning the repository you need to change the directory and call the `b.bat` file with the arguments `docs`. This will restore all the external dependencies and build the documentation.

```cmd
b.bat docs
```

::: tip
You can list all available targets using the `b.bat -l` command.
:::

## Live preview

You can edit the documentation with help of live preview. This is useful because you get immediate feedback after hitting save. This will automatically refresh the content without the need to refresh the browser manually.

```cmd
b.bat docs:serve
```

This will build the local documentation and launch a local server and browser window at address [http://127.0.0.1:8080/](http://127.0.0.1:8080/).

::: tip
You can edit all `*.md` **content**.  
If you edit `.vuepress/config.js` or any [frontmatter](https://v1.vuepress.vuejs.org/guide/frontmatter.html) you need to restart the server by hitting `ctrl+c` to stop the server and confirming the kill by hitting `[Y]es` twice.
:::

## Editing the documentation

To edit code using VSCode we need to open the subdirectory `./docs`:

```cmd
code docs
```

After changing sources, you can commit your changes using VSCode:

![Commit changes](/images/guide/contribute/commit-changes.png)

Or using commandline:

```cmd
git add .
git commit -m "docs: your commit message"
```

::: tip
The `docs:` prefix in the commit message helps with generating a semantic changelog.
:::

After commiting you need to push your changes to your repository. This can be done also via VSCode:

![Sync the repository](/images/guide/contribute/sync-repo.png)

Or using commandline:

```cmd
git push
```

## Keep your fork in sync

Because GIT is a decentralized version control system, you have a complete copy of all history, hence the name `clone`.  
That means you need to keep your GIT repository in sync with the [`upstream`](https://github.com/xenial-io/Xenial.Framework.git).

First you make sure that your repository has no uncommitted changes

```cmd
git status


```

```cmd
git fetch upstream
git checkout main
git merge upstream/main
```