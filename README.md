# Problem to solve

In case when you got a big, enterprise level software with more than dozen solutions involved it is extremely problematic to keep all Nuget packages synchronized across whole software especially if solution written by different groups of people. To make it more complicated software might use some special version of some nuget package which make approach "update all to latest" not an option. Also self generated Nuget packages might be used which referencing each other. This is add even more problems to find out "Why do I have three different versions of the same package in my install and where it all came from?" 

# How To Use: 
1. Setup proper app config providing TFS server address and Nuget repository. It is might be your personal repository or public. Also because TFS hierarchy loading is expensive it is might be simplified by providing proper initial folder for it. Set location for the DGML file to be stored. 
2. Run App and expand TFS hierarchy on a left. Find folders solutions from which will be included in analysis and check it.
3. Click "Build DGML File" button. This will build dgml file at the path provided from app.config. This file might be used to open with visual studio and see all dependencies. Source code for that part might be found [here](https://github.com/ThomasArdal/NuGetPackageVisualizer) . 
4. When file built successfully it might be analyzed to see which project not used proper version of the package which will cause more than one version of the Nuget packages will be in use. First in the right window all not latest package will be listed in yellow color e.g."> jQuery 2.1.1 -> jQuery 3.2.1" Which is mean that used jQuery library is not updated. After that information which project need to be updated. E.g. 

"> angularjs 1.2.18 --> 1.6.6"
">  --> Angular.UI 0.4"
"> Project ------------> ACME.SomeExplorer"
">  --> Angular.UI.Bootstrap 0.6.0"
"> Project ------------> ACME.SomeExplorer"

Which is mean that latest angularjs is 1.6.6 but 1.2.18 is used. On this package two other packages depend on. It is Angular.UI and Angular.UI.Bootstrap which probably need an update two. All this packages need to be updated in  ACME.SomeExplorer project. 

5. Button "Reveal Version Duplicates" will generate short report which package among analyzed solutions have different versions of the same package. E.g. 

"> jQuery"
">       1.10.2"
">       2.1.1"

Means that among analyzed solutions jQuery package used in two different versions 1.10.2 and 2.1.1

