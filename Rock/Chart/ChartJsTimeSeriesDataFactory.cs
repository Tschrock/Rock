// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Rock.Chart
{
    /// <summary>
    /// Creates a ChartJs data structure suitable for plotting a value-over-time data series on a Cartesian grid.
    /// </summary>
    public class ChartJsTimeSeriesDataFactory<TDataPoint>
        where TDataPoint : IChartJsTimeSeriesDataPoint
    {
        // Default color palette
        private const string ColorGray = "#4D4D4D";
        private const string ColorBlue = "#5DA5DA";
        private const string ColorOrange = "#FAA43A";
        private const string ColorGreen = "#60BD68";
        private const string ColorPink = "#F17CB0";
        private const string ColorBrown = "#B2912F";
        private const string ColorPurple = "#B276B2";
        private const string ColorYellow = "#DECF3F";
        private const string ColorRed = "#F15854";

        private List<ChartJsTimeSeriesDataset> _Datasets = new List<ChartJsTimeSeriesDataset>();

        public ChartJsTimeSeriesDataFactory()
        {
            this.Datasets = new List<ChartJsTimeSeriesDataset>();

            this.ChartColors = GetDefaultColorPalette();
        }

        #region Properties

        /// <summary>
        /// The style of chart to display.
        /// </summary>
        public ChartJsTimeSeriesChartStyleSpecifier ChartStyle { get; set; } = ChartJsTimeSeriesChartStyleSpecifier.Line;

        /// <summary>
        /// The start date for the time series.
        /// If not specified, the date of the earliest data point will be used.
        /// </summary>
        public DateTime? StartDateTime { get; set; } = null;

        /// <summary>
        /// The end date for the time series.
        /// If not specified, the date of the latest data point will be used.
        /// </summary>
        public DateTime? EndDateTime { get; set; } = null;

        /// <summary>
        /// A collection of HTML Colors that represent the default palette for datasets in the chart.
        /// Colors are selected in order from this list for each dataset that does not have a specified color.
        /// </summary>
        public List<string> ChartColors { get; set; }

        /// <summary>
        /// A collection of data points that are displayed on the chart as one or more series of data.
        /// </summary>
        public List<ChartJsTimeSeriesDataset> Datasets
        {
            get { return _Datasets; }
            set
            {
                _Datasets = value ?? new List<ChartJsTimeSeriesDataset>();
            }
        }

        /// <summary>
        /// Does the data set contain any data points?
        /// </summary>
        public bool HasData
        {
            get
            {
                return _Datasets != null && _Datasets.Any();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get a data structure in JSON format that is compatible for use with the Chart.js component.
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            var jsonSetting = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            // Create the data structure for Chartjs parameter "data".
            var datasets = new List<object>();

            var availableColors = GetDatasetColors();

            foreach ( var dataset in this.Datasets )
            {
                //var jsDataset = new ChartJsChartDataset();
                var dataPoints = dataset.DataPoints.Select( dp => new { x = dp.DateTime.ToISO8601DateString(), y = dp.Value } ).OrderBy( dp => dp.x ).ToList();

                // Use the color specifically assigned to this dataset, or get the next color from the queue.
                string borderColor = dataset.BorderColor;

                if ( string.IsNullOrWhiteSpace( borderColor ) )
                {
                    borderColor = availableColors.Dequeue();

                    // Recycle the color in case we have more datasets than colors in the palette.
                    availableColors.Enqueue( borderColor );
                }

                dynamic jsDataset = new { label = dataset.Name, borderColor = borderColor, data = dataPoints };

                datasets.Add( jsDataset );
            }

            dynamic chartData = new { datasets = datasets };

            // Create the data structure for Chartjs parameter "options".
            long? minDate = null;
            long? maxDate = null;

            if ( this.StartDateTime != null )
            {
                minDate = this.StartDateTime.Value.ToJavascriptMilliseconds();
            }

            if ( this.EndDateTime != null )
            {
                maxDate = this.EndDateTime.Value.ToJavascriptMilliseconds();
            }

            // Prevent Chart.js from displaying decimal values in the y-axis by forcing the step size to 1 if the value range is below 10.
            var maxValue = GetMaximumDataValue();

            decimal? stepSize = null;

            if ( maxValue < 10 )
            {
                stepSize = 1;
            }

            var optionsXaxes = new List<object>() { new { type = "time", time = new { displayFormats = new { month = "MMM-YYYY" }, tooltipFormat = "MMM-YYYY", minUnit = "month", min = minDate, max = maxDate } } };

            var optionsYaxes = new List<object>() { new { ticks = new { suggestedMin = 0, stepSize = stepSize } } };

            dynamic optionsData = new { maintainAspectRatio = false, scales = new { xAxes = optionsXaxes, yAxes = optionsYaxes } };

            // Create the data structure for Chartjs parameter "chart".
            string chartStyle = GetChartJsStyleParameterValue( this.ChartStyle );

            dynamic chartStructure = new { type = chartStyle, data = chartData, options = optionsData };

            // Return the JSON representation of the Chartjs data structure.
            string chartParametersJson = JsonConvert.SerializeObject( chartStructure, Formatting.None, jsonSetting );

            return chartParametersJson;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Get the default color palette.
        /// </summary>
        /// <returns></returns>
        private List<string> GetDefaultColorPalette()
        {
            return new List<string> { ColorBlue, ColorGreen, ColorPink, ColorBrown, ColorPurple, ColorYellow, ColorRed, ColorOrange, ColorGray };
        }

        private string GetChartJsStyleParameterValue( ChartJsTimeSeriesChartStyleSpecifier chartStyle )
        {
            if ( chartStyle == ChartJsTimeSeriesChartStyleSpecifier.Bar )
            {
                return "bar";
            }
            else if ( chartStyle == ChartJsTimeSeriesChartStyleSpecifier.Bubble )
            {
                return "bubble";
            }
            else
            {
                return "line";
            }
        }

        /// <summary>
        /// Get the maximum data value of all data points.
        /// </summary>
        /// <returns></returns>
        private decimal GetMaximumDataValue()
        {
            decimal maxDataset = 0;

            foreach ( var dataset in this.Datasets )
            {
                var maxValue = dataset.DataPoints.Max( x => x.Value );

                if ( maxValue > maxDataset )
                {
                    maxDataset = maxValue;
                }
            }

            return maxDataset;
        }

        /// <summary>
        /// Creates a queue of colors to be used as the palette for the chart datasets.
        /// </summary>
        /// <returns></returns>
        private Queue<string> GetDatasetColors()
        {
            var availableColors = this.ChartColors ?? this.GetDefaultColorPalette();

            var colorQueue = new Queue<string>( availableColors );

            return colorQueue;
        }

        #endregion
    }

    /// <summary>
    /// A set of data points and configuration options for a dataset that can be plotted on a ChartJs chart.
    /// </summary>
    public class ChartJsTimeSeriesDataset
    {
        public string Name { get; set; }

        /// <summary>
        /// The color of the border or outline of the region included in this dataset.
        /// </summary>
        public string BorderColor { get; set; }

        public List<IChartJsTimeSeriesDataPoint> DataPoints { get; set; }
    }

    /// <summary>
    /// Specifies the chart style for a value-over-time data series in Chart.js
    /// </summary>
    public enum ChartJsTimeSeriesChartStyleSpecifier
    {
        Line = 0,
        Bar = 1,
        Bubble = 2
    }
}