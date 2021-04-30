---
title: Getting Started
---
# Getting Started


Xenial.Framework supports DevExpress expressApplicationFramework (XAF) solutions that target both WinForms and Blazor and is available as a Nuget package. At the time of writing this documentation the minimum supported version of the XAF framework is Version 20.2.6 with the latest version of the Xenial.Framework being 0.0.54.

Details of the latest version of Xenial.Framework and the XAF framework versions that it supports can be found [here]

<!--Insert suitable link to a page on the Xenial site in the sentence above.  Alternatively if there is no plan to keep a changelog delete the sentence above -->

## Creating a new WinForms XAF project

Create a new project in Visual Studio selecting the DevExpress V20.2 XAF Solution Wizard as the project type. Xenial.Framework can be used in the Community Edition of Visual Studio, there is no requirement for it to be used in either the professional or Enterprise versions.  The illustrations below were taken from Visual Studio 2019 Enterprise.



![New Project 1 ](/images/guide/overview/newProject1.png)



Select WinForms as the project type.



![New Project 2 ](/images/guide/overview/newProject2.png)



Select XPO for the ORM and then click finish.



![New Project 3 ](/images/guide/overview/newProject3.png)



The wizard will then create the solution which, when finished, will contain three projects.




![New Project 4 ](/images/guide/overview/newProject4.png)




## Adding Xenial.Framework to the WinForms project


Xenial.Framework can be added to the XAF solution created by the wizard using either the visual tools that Visual Studio provides or by using the command line.



## Visual approach

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


## Commandline approach


From Visual Studio's View menu select Other Windows and then Package Manager Console.

At the PM> prompt type `dotnet add package Xenial.Framework --version 0.0.54`



![New Project 7 ](/images/guide/overview/newProject7.png)



Xenial.Framework has now been installed into the solution.





## Creating a new Blazor XAF project


As before create a new project in Visual Studio, this time selecting the DevExpress V20.2 XAF.Blazor Solution Wizard as the project type.

![New Project 11  ](/images/guide/overview/newProject11.png)


Select XPO as the ORM to use and click Install.


![New Project 10  ](/images/guide/overview/newProject10.png)


Upon completion of the wizard the created solution will contain three projects.


![New Project 12  ](/images/guide/overview/newProject12.png)



## Adding Xenial.Framework to the Blazor project


Follow the same steps as outlined above for adding the Xenial.Framework to a WinForms Project taking care to ensure that the correct agnostic project is selected.


![New Project 13  ](/images/guide/overview/newProject13.png)