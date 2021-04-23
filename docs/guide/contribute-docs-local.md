---
title: Contribute Documentation (locally)
---

# Contribute to Documentation locally

If you prefer to work in local environment with the benefits of syntax highlighting, spell checking, version control and visual feedback, it is possible to do so and the contributions that you make can then be submitted via the process know as 'Pull Requests'. 

This requires a little bit more setup, but if you are a developer most of the required tools are probably installed on your machine anyway.

## Technology stack

We use the following tools to write, build and deploy the documentation. 

* [GIT](https://git-scm.com/) (for version control)
* [Github-Actions](https://github.com/features/actions) (for Continuous Integration / Continuous Delivery)
* [.NET](http://dot.net/) (for the build pipeline)
  - [SimpleExec](https://github.com/adamralph/simple-exec)
  - [BullsEye](https://github.com/adamralph/bullseye)
* [NodeJS](https://nodejs.org/en/) (for writing the docs)
  - [VuePress](http://vuepress.vuejs.org/)

The documents are written in [markdown](https://en.wikipedia.org/wiki/Markdown) to allow for easy versioning and consistent styling.  

You can use a text-editor or IDE of your choice. Visual Studio Code [VSCode](https://code.visualstudio.com/) is recommended because it provides a couple of handy extensions that help with writing documentation.

## Prerequisites

* [Install VSCode](https://code.visualstudio.com/) (optional)
* [Install GIT](https://git-scm.com/download/)
  - Choose `Use Visual Studio Code as GIT's default editor` (optional)
* [Install .NET SDK](https://dotnet.microsoft.com/download)
  - We use the stable release, so choose the `recommended` version for your platform
* [Install NodeJS](https://nodejs.org/en/download/)
  - We use the LTS release, so choose the `LTS Recommended For Most Users` version for your platform


::: warning CAUTION
If the tools mentioned above are already installed please ensure that they are up to date with the latest releases.  Failure to do this may mean that it is impossible to build the documentation without encountering errors.
:::

::: tip
If you do not have a [Git Hub account](https://github.com/join) this would be the ideal time to set one up.

::: warning CAUTION
When setting up a new Git Hub account choose your username carefully. It is notoriously difficult to change at a later date.
:::
:::

## Clone the sources

First of all you need to [fork the repository](https://github.com/xenial-io/Xenial.Framework/fork) on [github](https://github.com/xenial-io/Xeniak.Framework).

![Fork the repository](/images/guide/contribute/fork-repo.png)

Afterwards you need to clone the repository from your fork. To do that you need to copy your url under the `code` button.

![Fork the repository](/images/guide/contribute/clone-fork.png)

```cmd
git clone https://github.com/YOUR_GITHUB_NAME/Xenial.Framework.git
```

<!--  my inclination would be to remove this line and the code block immediately beneath it for this reason:
If you know nothing about git and you enter a user name that's unavailable you will start getting very frustrated by the error messages
that come back.  Better to do it via the web where you get instant feedback if the user name you opt for has already been taken along with
usefull suggestions for alternatives -->
If you never worked with git before, its necessary to set your name and email address to start contributing:

```cmd
git config --global user.email "you@example.com"
git config --global user.name "Your Name"
```

In order to be able to keep your fork in sync with the `Xenial.Framework` main repository it is necessary to set an upstream:

```cmd
git remote add upstream https://github.com/xenial-io/Xenial.Framework.git
```


## Build the documentation

After cloning the repository you need to change the directory and call the `b.bat` file with the arguments `docs`. This will restore all the external dependencies and build the documentation.

```cmd
b.bat docs
```
<!--Might it be sensible to point out the need to set a System environment variable at this point -->
::: tip
You can list all available targets using the `b.bat -l` command.
:::

## Live preview

You can edit the documentation with help of live preview. This is useful because you get immediate feedback after hitting save, as it automatically refreshes the content without the need to refresh the browser manually.

```cmd
b.bat docs:serve
```

This will build the local documentation and launch a local server and browser window at the following address [http://127.0.0.1:8080/](http://127.0.0.1:8080/).

::: tip
You can edit all `*.md` **content**.  
If you edit `.vuepress/config.js` or any [frontmatter](https://v1.vuepress.vuejs.org/guide/frontmatter.html) you need to restart the server by hitting `ctrl+c` to stop the server and confirming the kill by hitting `[Y]es` twice.
:::

## Editing the documentation

To edit the documentation using VSCode open the subdirectory `./docs`:

```cmd
code docs
```

After you have made your changes to the source document, you can commit them using VSCode:

![Commit changes](/images/guide/contribute/commit-changes.png)

Or using the commandline:

```cmd
git add .
git commit -m "docs: your commit message"
```

::: tip
The `docs:` prefix in the commit message helps with generating a semantic changelog.
:::

After committing you need to 'push' your changes to your repository. This can be done in VSCode:

![Sync the repository](/images/guide/contribute/sync-repo.png)

Or using the commandline:

```cmd
git push
```

## Keep your fork in sync

Because GIT is a decentralized version control system, you have a complete historical copy of all of the changes you have made, hence the name `clone`.  
<!--The term original repository that I've used below may not be appropriate, feel free to suggest an alternative -->
This means you need to keep your GIT repository in sync with the original [`upstream`](https://github.com/xenial-io/Xenial.Framework.git) repository.

First you make sure that your repository has no uncommitted changes

```cmd
git status

# 
# On branch main
# Your branch is up to date with 'origin/main'.
# 
# nothing to commit, working tree clean
# 
```

```cmd
git fetch upstream
git checkout main
git merge upstream/main
```

::: tip
If you have already cloned the repository it is possible that you may have missed a step so, just as a gentle reminder: you must have configured your `upstream` at this stage:

```cmd
git remote add upstream https://github.com/xenial-io/Xenial.Framework.git
```

Learn more about [keeping repositories in sync over at github docs](https://docs.github.com/en/github/collaborating-with-issues-and-pull-requests/syncing-a-fork).
::: 

Afterwards you can [edit the documentation](#editing-the-documentation) as [you learned earlier](#editing-the-documentation).

::: tip PRO TIP
If you want to automate the process of automatically having your fork in sync , you may want to use [pull bot](https://github.com/apps/pull)
:::

## Creating a pull request

The last step to aligning the changes that you have made with the original repository is to [create a pull request](https://docs.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request-from-a-fork).

Navigate to your fork on github. For example `https://github.com/YOUR_GITHUB_NAME/Xenial.Framework`. Github will normally guide you through the process of creating a pull request once you have clicked on the `Pull request` link.

![Clicking the Pull request link](/images/guide/contribute/pull-request-1.png)

Make sure your source branch and the target branch are correct. By default this should be `main`. If you are a more advanced git user, you could of course create a separate branch. 

![Creating the pull request and making sure branches are set correctly](/images/guide/contribute/pull-request-2.png)

Afterwards you need to create the pull request:

![Create the pull request](/images/guide/contribute/pull-request-3.png)

