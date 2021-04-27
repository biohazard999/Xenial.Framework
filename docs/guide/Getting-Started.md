---
title: Getting Started
---

Xenial.Framework supports DevExpress expressApplicationFramework (XAF) solutions that target both WinForms and Blazor and is available as a Nuget package. At the time of writing this documentation the minimum supported version of the XAF framework is Version 20.2.6 with the latest version of the Xenial.Framework being 0.0.54.

Details of the latest version of Xenial.Framework and the XAF framework versions that it supports can be found [here]

<!--Insert suitable link to a page on the Xenial site in the sentence above.  Alternatively if there is no plan to keep a changelog delete the sentence above -->

# Creating a new Winforms XAF project

Create a new project in Visual Studio selecting the DevExpress V20.2 XAF Solution Wizard as the project type. Xenial.Framework can be used in the Community Edition of Visual Studio, there is no requirement for it to be used in either the professional or Enterprise versions.  The illustrations below were taken from Visual Studio 2019 Enterprise.

![New Project 1 ](/images/guide/overview/newProject1.png)


Select Winforms as the project type.


![New Project 2 ](/images/guide/overview/newProject2.png)


Select XPO for the ORM and then click finish.


![New Project 3 ](/images/guide/overview/newProject3.png)


The wizard will then create the sution which, when finished,  will contain three projects.


![New Project 4 ](/images/guide/overview/newProject4.png)




# Adding Xenial.Framework to the project

Xenial.Framework can be added to the XAF solution created by the wizard using either the visual tools that Visual Studio provides or by using the command line.



# Visual approach

From the Solution Explorer select the XAF solution and invoke the right click contextual menu, and select Manage NuGet Packages for Solution.


![New Project 5 ](/images/guide/overview/newProject5.png)


The NuGet tab will open.  Within that;

    1) Select Browse
    2) Type Xenial in the search box
    3) Select Xenial.Framework



![New Project 6 ](/images/guide/overview/newProject6.png)


Having selected Xenial.Framework ensure that the solution's agnostic module has been checked and click Install.


![New Project 8 ](/images/guide/overview/newProject8.png)


Xenial.Framework has now been installed into the solution.

# Commandline approach


From Visual Studio's View menu select Other Windows and then Package Manager Console.

At the PM> prompt type `dotnet add package Xenial.Framework --version 0.0.54`


![New Project 7 ](/images/guide/overview/newProject7.png)


Xenial.Framework has now been installed into the solution.


