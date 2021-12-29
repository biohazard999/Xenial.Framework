---
title: SourceGenerators - LayoutBuilderGenerator
sidebarDepth: 5
demoName: Acme.LayoutBuilderDemo
---

# LayoutBuilderGenerator - Introduction

A generator that helps you write strongly typed layouts using [DetailViewLayoutBuilders](layout-builders.md) and reduce the overhead (and syntax noise) of lambda expressions.

## Intent

When writing [DetailViewLayoutBuilders](layout-builders.md) code we write them in a strongly typed fashion. Because we know the target type upfront, we can use source generators to reduce syntax noise and help avoid mistakes that can occur when using the traditional lambda syntax. This source generator tries to minimize this weakness.

::: tip TIP
This generator also has benefits to force a cleaner structure when defining layouts, as well as helping with [Edit & Continue](https://docs.microsoft.com/visualstudio/debugger/how-to-use-edit-and-continue-csharp) and [HotReload](https://docs.microsoft.com/visualstudio/debugger/hot-reload) support.
:::

::: warning CAUTION
When updating from an older Xenial to a newer Xenial version, it's necessary to restart VisualStudio/VSCode after the upgrade, so Intellisense can reload the new SourceGenerator. So it may come to false positive warnings if they don't match.
:::

## Diagnostics

|ID            | Severity | Message                                                                     | Reason                                                                                 |
|:------------:|:--------:|-----------------------------------------------------------------------------|----------------------------------------------------------------------------------------|
|XENGEN0010    | Error    | Could not parse boolean MSBUILD variable `<GenerateXenialViewIdsAttribute>` | MsBuild variable needs to be in boolean parsable format: `true`/`false`/`True`/`False` |
|XENGEN0100    | Error    | The class using the `[XenialViewIdsAttribute]` needs to be partial          | We can not generate code for non partial classes                                       |
|XENGEN0101    | Error    | The class using the `[XenialViewIdsAttribute]` needs to be in a namespace   | We can not generate code in the global namespace                                       |

## Demo-Source

<!-- markdownlint-disable MD033 -->
You can find demo sources in the <a target="_blank" :href=" $var['gitHubUrl'] + '/tree/' + $var['gitBranch'] + '/demos/SourceGenerators/' + $frontmatter.demoName">Xenial.Framework repository</a> for in depth usage information.
<!-- markdownlint-enable MD033 -->
