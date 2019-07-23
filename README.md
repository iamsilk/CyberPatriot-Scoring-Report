# CyberPatriot Scoring Report
An unofficial practice scoring report for CyberPatriot competition practice.

Developed in C#, and WPF, the idea of this project is to provide the CyberPatriot community with an open source scoring program. Being open source, additions can be easily added and worked on by the community.

I, Stephen White, am also a competitor in CyberPatriot and CyberTitan. This is my first public project so any feedback is greatly appreciated.

This project is a work in progress and although has been tested within Windows 10 for its current features, many features are planned to be added in the near future. I have not tested the Scoring Report on any other operating systems. If you have issues on different operating systems, please open a ticket and I'll do my best to find a solution.

If you have any suggestions, open a ticket tagged as suggestion and it will be added to the todo list.

If anything is unclear and needs better explanation, feel free to ask. I hope the documentation here will be enough to explain. I have worked to comment nearly all the code to help explain aspects of this project to those just learning of this project.

## Installation:
To install this Scoring Report, download and run the installer. You will also need .NET Framework 4.6.1 or later.

In the installation, two executables will be unpacked, the Configuration Tool and the Scoring Report. Both of these tools will be explained in their own sections.

Along with the installation, a 'Scoring Report.html' web page will also be extracted. There is nothing except the basic format of the report within this webpage file. As the report is configured using the Configuration Tool, more items may be shown within the html file.

As of currently, the Scoring Report will not begin until you restart your machine or start the "Scoring Report" service manually with Window's built-in Service Manager, or some other service manager.

## Configuration Tool
The Configuration Tool is used to configure the aspects of the system which are scored.

Starting up the Configuration Tool will show you all the sections which can be configured. The current format tries to replicate what you may see within secpol.msc or gpedit.msc within Windows (atleast for Account/Local Policies).

The sections which are currently a part of this Scoring Report include:
- Account Policies
  - Password Policy
  - Account Lockout Policy
- Local Policies
  - Audit Policy
  - User Rights Assignment
  - Security Options
- Local Users
- Local Groups

For specifying users, you may specify either a username or security ID. This is a feature because accounts such as the default administrator and guest may be renamed, but you may still need to address details regarding their accounts.

All configurations can be saved by clicking the menu item File -> Save or Save As. Saving by default will save to the Configuration.dat file in the installation location.

Multiple output files can be specified within the Advanced tab. More details are provided within the **Scoring Report** section.

To prepare the system for practicing, you can remove the Configuration Tool by clicking the menu item `File -> Remove Configuration Tool`. This will remove the Configuration Tool.exe and shortcut for all on the system. Nothing else is affected by this removal. 

The Configuration file can still be copied to a system with the Configuration Tool or a copy of the Configuration Tool executable can be placed on the machine to read/modify the configuration again. Countermeasures for doing this so simply may be placed in the future to restrict such easy accessibility to answers on a system.

## Scoring Report
The Scoring Report is a Windows Service which scores the users progress based on comparing the configuration within the Configuration.dat file in the installation directory with the systems settings.

The output of the scoring report, by default, is saved in the 'Scoring Report.html' file. The format in which the report is outputted is manually configurable. This is explained partially later on.

The Scoring Report Windows Service starts automatically as the Local System user. This is because the user in which it runs as needs administrator privileges in order to get information about scored users and settings.

You can not run the Scoring Report as a regular user unless you are using some debugger. You may install/uninstall it as a service by passing the /i or /u argument respectively. Then you will need to start the service using some service manager.

The output of the scoring report is manually configurable and it is planned in the future to make this process simpler. Currently, you can specify different output files within the Configuration Tool in the Advanced tab; however, this is only the first step.

I will try to explain the formatting and process to creating your own customized output file; however, if you run into any issues, you may either ask or examine the [source code](https://github.com/Stephen3495/CyberPatriot-Scoring-Report/tree/master/Scoring%20Report/Scoring/Output) yourself to see the workings of the output management.

You must create a folder within the same directory as the specified output file. The name of this folder must be the name and extension of the output file, but with `_format` appended to the end.

For example, by default, the output file is `C:\CyberPatriot Scoring Report\Scoring Report.html`. The folder containing the formatting details has a total directory path of `C:\CyberPatriot Scoring Report\Scoring Report.html_format`.

Contained within this folder, currently, are the following files:
- format.txt - This is the main formatted file. Explained more later.
- section_nothing.txt - Text placed when nothing has been solved yet
- sectionhead_prefix.txt - Text preceding the header of a section
- sectionhead_suffix.txt - Text concluding the header of a section
- sectiontext_newline.txt - Text placed between each scored item of a section
- sectiontext_prefix.txt - Text preceding a scored item
- sectiontext_suffix.txt - Text concluding a scored item

### format.txt
This is the main formatted file, a.k.a. the basis of the entire output.

The formatting with this as the basis uses the .NET function [string.format](https://docs.microsoft.com/en-us/dotnet/api/system.string.format).

The parameters for this function are as follows in proper, incremental order:
- `{0}` - Integer value of total score for the competitor(s)
- `{1}` - Integer value of total score available on the system
- `{2:0.00}` - Percentage value of awarded score divided by total available. The addition of `:0.00` within the text forces two decimal places on the formatting. Details on formatting can be found within Microsoft's [documentation for the string.format function](https://docs.microsoft.com/en-us/dotnet/api/system.string.format)
- `{3}` - Date and time when the output file was generated based on score. Time is given in the system's time zone.
- `{4}` - String where all the sections are placed. The "meat" of the output. Takes into account the other formatting file specified earlier.
