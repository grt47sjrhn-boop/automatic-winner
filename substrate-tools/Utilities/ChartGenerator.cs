using System;
using System.Collections.Generic;
using System.Linq;
using ScottPlot;

namespace substrate_tools.Utilities
{
    public static class ChartGenerator
    {
        /// <summary>
        /// Generate a chart with just Persistence and Volatility series.
        /// </summary>
        public static void GeneratePersistenceVolatilityChart(
            IList<double> persistenceValues,
            IList<double> volatilityValues,
            string fileName = "persistence_volatility_chart.png")
        {
            var plot = new Plot();
            var xs = Enumerable.Range(1, persistenceValues.Count).Select(i => (double)i).ToArray();

            var persistenceSeries = plot.Add.Scatter(xs, persistenceValues.ToArray());
            persistenceSeries.LegendText = "Persistence";

            var volatilitySeries = plot.Add.Scatter(xs, volatilityValues.ToArray());
            volatilitySeries.LegendText = "Volatility";

            plot.Legend.IsVisible = true;
            plot.Title("Persistence & Volatility Across Ticks");
            plot.XLabel("Tick");
            plot.YLabel("Value");

            plot.SavePng(fileName, 800, 600);
            Console.WriteLine($"Chart saved as {fileName}");
        }

        /// <summary>
        /// Generate a chart with Persistence, Volatility, and multiple Trait weight series.
        /// </summary>
        public static void GenerateMultiSeriesChart(
            IList<double> persistenceValues,
            IList<double> volatilityValues,
            IList<IList<double>> traitWeightSeries,
            string fileName = "multi_series_chart.png")
        {
            var plot = new Plot();
            var xs = Enumerable.Range(1, persistenceValues.Count).Select(i => (double)i).ToArray();

            var persistenceSeries = plot.Add.Scatter(xs, persistenceValues.ToArray());
            persistenceSeries.LegendText = "Persistence";

            var volatilitySeries = plot.Add.Scatter(xs, volatilityValues.ToArray());
            volatilitySeries.LegendText = "Volatility";

            var traitIndex = 1;
            foreach (var traitWeights in traitWeightSeries)
            {
                var traitSeries = plot.Add.Scatter(xs, traitWeights.ToArray());
                traitSeries.LegendText = $"Trait {traitIndex}";
                traitIndex++;
            }

            plot.Legend.IsVisible = true;
            plot.Title("Persistence, Volatility, and Trait Weights Across Ticks");
            plot.XLabel("Tick");
            plot.YLabel("Value");

            plot.SavePng(fileName, 1000, 600);
            Console.WriteLine($"Chart saved as {fileName}");
        }

        /// <summary>
        /// Generate a stacked area chart of trait weights across ticks.
        /// Each trait is layered cumulatively to show constellation formation.
        /// </summary>
        public static void GenerateStackedTraitChart(
            IList<IList<double>> traitWeightSeries,
            string fileName = "stacked_traits_chart.png")
        {
            var plot = new Plot();

            if (traitWeightSeries == null || traitWeightSeries.Count == 0)
            {
                Console.WriteLine("No trait data provided. Skipping stacked chart.");
                return;
            }

            var tickCount = traitWeightSeries.First().Count;
            var xs = Enumerable.Range(1, tickCount).Select(i => (double)i).ToArray();

            var cumulative = new double[tickCount];
            var traitIndex = 1;

            foreach (var traitWeights in traitWeightSeries)
            {
                var ys = traitWeights.Zip(cumulative, (w, c) => w + c).ToArray();
                var areaSeries = plot.Add.FillY(xs, ys, cumulative);
                areaSeries.LegendText = $"Trait {traitIndex}";

                cumulative = ys;
                traitIndex++;
            }

            plot.Legend.IsVisible = true;
            plot.Title("Stacked Trait Weights Across Ticks");
            plot.XLabel("Tick");
            plot.YLabel("Cumulative Weight");

            plot.SavePng(fileName, 1000, 600);
            Console.WriteLine($"Stacked trait chart saved as {fileName}");
        }

        /// <summary>
        /// Generate all charts (line, multi-series, stacked) in one call.
        /// </summary>
        public static void GenerateAllCharts(
            IList<double> persistenceValues,
            IList<double> volatilityValues,
            IList<IList<double>> traitWeightSeries)
        {
            GeneratePersistenceVolatilityChart(persistenceValues, volatilityValues);
            GenerateMultiSeriesChart(persistenceValues, volatilityValues, traitWeightSeries);
            GenerateStackedTraitChart(traitWeightSeries);
        }
    }
}