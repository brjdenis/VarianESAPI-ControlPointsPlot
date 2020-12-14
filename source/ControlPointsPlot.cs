using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Wpf;
using OxyPlot.Axes;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.3")]

// TODO: Uncomment the following line if the script requires write access.
// [assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
  public class Script
  {
    public Script()
    {
    }
   [MethodImpl(MethodImplOptions.NoInlining)]
    public void Execute(ScriptContext context,  System.Windows.Window window /*, ScriptEnvironment environment*/)
    {
            if (context.PlanSetup == null)
            {
                MessageBox.Show("No plan is active.", "Error message");
                return;
            }
            if (context.StructureSet == null)
            {
                MessageBox.Show("No structure set is assigned.", "Error message");
                return;
            }

            // Load plan
            ExternalPlanSetup plan = context.ExternalPlanSetup;

            window.Title = "\u0394 MU/\u0394 \u00B0";
            window.Content = CreatePlotView(plan);
        }

        private PlotView CreatePlotView(ExternalPlanSetup plan)
        {
            return new PlotView { Model = CreatePlotModel(plan) }; 
        }

        private PlotModel CreatePlotModel(ExternalPlanSetup plan)
        {
            var myModel = new PlotModel { Title = "\u0394 MU/\u0394 \u00B0" };
            List<double> maxy = new List<double>();

            foreach (var beam in plan.Beams)
            {
                var line = new OxyPlot.Series.LineSeries
                {
                    Title = beam.Id,
                    Tag = beam.Id
                };

                if (beam.MLCPlanType == VMS.TPS.Common.Model.Types.MLCPlanType.VMAT || beam.MLCPlanType == VMS.TPS.Common.Model.Types.MLCPlanType.ArcDynamic)
                {
                    // Get from plan:
                    var metersetWeights = beam.ControlPoints.Select(cp => cp.MetersetWeight).ToList();
                    var gantryAngles = beam.ControlPoints.Select(ga => ga.GantryAngle).ToList();
                    var monitorUnit = beam.Meterset.Value;

                    List<double> dMW = new List<double>();
                    List<double> dangle = new List<double>();

                    for (int k=0; k< metersetWeights.Count()-1; k++)
                    {
                        dMW.Add((metersetWeights[k + 1] - metersetWeights[k]) * monitorUnit);
                        dangle.Add((180.0 - Math.Abs((Math.Abs(gantryAngles[k + 1] - gantryAngles[k]) - 180.0))));
                    }
                    maxy.Add(dMW.Max()); 
                    var points = new List<DataPoint>();

                    for (int i=0; i < dMW.Count(); i++)
                    {
                        points.Add(new DataPoint(i+1, dMW[i]/dangle[i]));
                    }
                    line.Points.AddRange(points);
                }
                myModel.Series.Add(line);
            }

            // Add zero point
            var line0 = new OxyPlot.Series.LineSeries();
            line0.Points.Add(new DataPoint(0, 0));

            myModel.Series.Add(line0);

            // Add x- axis
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Title = "Control point interval",
                Position = AxisPosition.Bottom
            });

            // Add y- axis
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Title = "\u0394 MU/\u0394 \u00B0",
                Position = AxisPosition.Left
            });

            myModel.LegendTitle = "Legend";
            myModel.LegendPosition = LegendPosition.RightTop;

            return myModel;
        }

    }
}
