<div align=center>
<img src="https://github.com/Gaoyifei1011/PowerToolbox/assets/49179966/8d58b720-9e84-468a-a680-ac90e4f78ae4" width="140" height="140"/>
</div>

# <p align="center">Welcome to PowerToolbox</p>

### Application brief introduction

A toolbox that integrates multiple gadgets.

------

### The basic functionality of the app

> * Batch change file names
> * Batch change file extension names
> * Batch change the upper and lower case names of files
> * Batch modify file properties
> * Batch delete the certificates of signed files
> *	File unlock        
> *	Download manager     
> *	Extract file icons and Extract the package resource index(.pri) file content
> *	Theme switch
> *	Customize the right-click menu 
> *	Right-click menu items manager     
> *	Driver manager  
> *	Update manager 
> *	Loopback manager
> * Windows system Performance Evaluation

------

### Screenshot of the app

#### <p align="center">Loaf page</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/PowerToolbox/assets/49179966/ed2e50f8-c7cf-4c08-ba06-38b52d9dd4ad">
</div>

#### <p align="center">File tools page</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/PowerToolbox/assets/49179966/b8db2eab-7fd6-418b-918d-c1c30150072d">
</div>

#### <p align="center">Icon extract page</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/PowerToolbox/assets/49179966/dff09e6f-a7ba-4240-a234-27b97d235222">
</div>

#### <p align="center">Pri extract page</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/PowerToolbox/assets/49179966/c13681fe-17c2-4041-8d86-9d4ca78cc1bf">
</div>

#### <p align="center">更新管理页面</p>
<div align="center">
<img src="https://github.com/user-attachments/assets/994ec02e-8704-41df-a938-027e4dd0115e">
</div>

#### <p align="center">System assessment page</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/PowerToolbox/assets/49179966/072f3122-47d0-40b6-812e-6052a6e4416e">
</div>

------

### 项目开发进展

| Project content                                           | Development progress                                                 |
| --------------------------------------------------------- | -------------------------------------------------------------------- |
| Loaf                                                      | Finished                                                             |
| Batch change file names                                   | Finished                                                             |
| Batch change file extension names                         | Finished                                                             |
| Batch change the upper and lower case names of files      | Finished                                                             |
| Batch modify file properties                              | Finished                                                             |
| Batch delete the certificates of signed files             | Finished                                                             |
| File unlock                                               | Finished                                                             |
| Download manager                                          | Finished                                                             |
| Extract file icons                                        | Finished                                                             |
| Extract the package resource index(.pri) file conten      | Finished                                                             |
| Switch theme                                              | Finished                                                             |
| Customize the right-click menu                            | Finished                                                             |
| Right-click menu items manager                            | Finished                                                             |
| Driver manager                                            | Finished                                                             |
| Update manager                                            | Finished                                                             |
| Loopback manager                                          | Finished                                                             |
| Windows system assessment tool                            | Finished                                                             |
| To be continued                                           | ......                                                               |

------

### Project References (Sort by alphabetical order)

> * [Microsoft.UI.Xaml](https://github.com/microsoft/microsoft-ui-xaml)&emsp;
> * [Microsoft.Windows.SDK.BuildTools](https://aka.ms/WinSDKProjectURL)&emsp;
> * [Microsoft.Windows.SDK.BuildTools.MSIX](https://aka.ms/WinSDKProjectURL)&emsp;
> * [Microsoft.Windows.SDK.Contracts](https://aka.ms/WinSDKProjectURL)&emsp;
> * [Mile.Aria2](https://github.com/ProjectMile/Mile.Aria2)&emsp;
> * [System.Private.Uri](https://dot.net)&emsp;

[Code referenced or used during the learning process](https://github.com/Gaoyifei1011/PowerToolbox/blob/main/Description/StudyReferenceCode.md)&emsp;

------

### Download and installation considerations

> * The program is built using Xaml Islands, it is recommended that your system version is Windows 11 (codenamed 22H2 / internal version number 22621) or later, The minimum version is Windows 11 (codenamed 2004 / internal version number 19041) or later.
> * [Release](https://github.com/Gaoyifei1011/PowerToolbox/releases) The binary installation file for the page has been packaged into a compressed package. Unzip the package and run the install.ps1 file in Powershell admin mode (if necessary) for a quick installation.
> * Download and compile the project source code yourself. (Please read the project compilation steps below carefully)

------

### Project compilation steps and app localization

#### <p align="center">Tools that must be installed</p>

> * [Microsoft Visual Studio 2022](https://visualstudio.microsoft.com/) 
> * .NET desktop development (install according to Visual Studio tooltip content after opening the solution, .NET Framework SDK version 4.8.1)

#### <p align="center">Compilation steps</p>

> * Clone the project and download the code locally
> * Open the PowerToolbox.sln file using Visual Studio 2022, and if the solution prompts that some tools are not installed, open the solution again after completing the installation tool steps.
> * Restore the project's Nuget package.
> * After the restore is complete, right-click PowerToolboxPackage, set the project as the startup project, and click Deploy.
> * After the deployment is complete, open the Start menu to run the app.

#### <p align="center">App localization</p>
##### The project was initially available in both Chinese Simplified and English formats, and if you want to translate your app into a language you are familiar with or correct errors in content that has been translated, please refer to the steps below.

> * Look for readme template files in the DeScription folder, for example, the English version is a README_EN-US.md file, rename it to README_(corresponding language).md file.
> * Open the renamed file, translate all the statements and save them. Please check it carefully after the translation is completed.
> * Open the README.md of the project's home page and add your language in the language selection at the top. For example, "English", note that the text is accompanied by a hyperlink.
> * README_ (corresponding language).The language screenshot added in the md file is replaced with the app screenshot in the language you are familiar with.
> * Complete the translation steps described above to ensure that all steps run smoothly.
> * Open the PowerToolboxPackage packaging project, find the Package.appxmanifest file, right-click the file, click View Code, find the Sources tab, and add the corresponding language according to the template, such as "<Resource Language="EN-US"/>".
> * Open the Strings folder of the PowerToolbox project and create the language you are using, for example ( English (United States) file name is en-us , you can refer to the Table of Indicating Language (Culture) Codes and Countries and Regions)
> * Open the resw file under the subfolder and translate each name.
> * Compile and run the code and test your language, when the application is first opened if there is no language you use to display English (United States) by default, it needs to be dynamically adjusted in the settings.
> * Create a PR after completing the above steps, then submit the modified content to this project and wait for the merge.

------

### Thanks (Sort by alphabetical order)

> * [AndromedaMelody](https://github.com/AndromedaMelody)&emsp;
> * [Blinue](https://github.com/Blinue)&emsp;
> * [cnbluefire](https://github.com/cnbluefire)&emsp;
> * [MicaApps](https://github.com/MicaApps)&emsp;
> * [MouriNaruto](https://github.com/MouriNaruto)&emsp;
> * [Osirisoo0O](https://github.com/Osirisoo0O)&emsp;

------

### Other content

> * This project is an open source project licensed under the MIT license, and you may modify, distribute, or merge copies with new copies. If you use the project, please do not use it for illegal purposes, and the developer will not be held responsible.
> * [Part tool prototype](https://github.com/Gaoyifei1011/PowerToolbox/blob/main/Description/RawApplicationDescription.md)&emsp;
