---
title: SourceGenerators - ImageNamesGenerator
sidebarDepth: 5
---

# ImageNamesGenerator - Introduction

A generator that helps you avoid mistakes when dealing with ImageNames's.

## Intent

When writing XAF code we often need to deal with ImageNames. Most of the time those are `DefaultDetailView` `DefaultListView` and `DefaultLookupListView` of a particular business class. With the help of `ModelNodeIdHelper` we can eliminate most string magic in regular code (like when defining an `Controller` with a `TargetViewId`). But a lot of code in XAF needs to be defined in `Attributes` hence we can not use `ModelNodeIdHelper` in that case. Another case that can't be solved are [custom views](TODO: Define Custom Views). To avoid this you need to define constants that need a lot of maintenance. This source generator tries to minimize this weakness.

## Usage
