# VarianESAPI-ControlPointsPlot

This script can be used to plot the dose rate against gantry angle for VMAT plans created in the Eclipse treatment planning system.

![image](images/image2.png)

This derivative is plotted:

![image](images/image1.png)

## How to use the script

Create a new ESAPI project with ControlPointsPlot.cs as the source. Download NuGet package [OxyPlot](https://github.com/oxyplot/oxyplot) and add it to the project. Compile and that is all.


## How to change the script

If you wish to change the script, use the source file (ControlPointsPlot.cs), and add 

* OxyPlot.Core.1.0.0
* OxyPlot.Wpf.1.0.0

to your project. Then recompile. OxyPlot files are not included in the soruce, so you have to do things manually.


