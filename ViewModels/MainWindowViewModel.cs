
namespace UnityCalculator.ViewModels
{
    using System;
    using System.IO;
    using CommunityToolkit.Mvvm.Input;
    using LiveChartsCore.SkiaSharpView.Painting;
    using LiveChartsCore.SkiaSharpView;
    using LiveChartsCore.SkiaSharpView.VisualElements;
    using LiveChartsCore;
    using SkiaSharp;
    using UnityCalculator.Models;

    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            this.PremiumCosts.PropertyChanged += (sender, args) => this.UpdateSeries();
            this.UpdateSeries();
        }

        public PremiumCosts PremiumCosts { get; set; } = new PremiumCosts();

        public ISeries[] Series { get; set; } =
        {
            new LineSeries<int> { Values = new int[0], Fill = null },
            new LineSeries<int> { Values = new int[0], Fill = null },
        };

        public Axis[] XAxes { get; set; } =
        {
            new Axis { Name = "Month" },
        };

        public Axis[] YAxes { get; set; } =
        {
            new Axis
            {
                Name = "Cost",
                Labeler = Labelers.Currency, // (value) => value.ToString("C")
            },
        };

        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "Unity vs. Unreal",
                TextSize = 20,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

        public string FinalUnityCost => this.GetFinalUnityCost().ToString("C0");

        public string FinalUnrealCost => this.GetFinalUnrealCost().ToString("C0");

        public string EffectiveUnityProfitSharing => Math.Max(0, this.GetFinalUnityCost() / ((this.PremiumCosts.TotalGrossRevenue ?? 0) - 1000000)).ToString("P2");

        private int GetFinalUnityCost() => (int)this.PremiumCosts.GetTotalUnityCosts(this.PremiumCosts.GetTotalMonths() - 1);

        private int GetFinalUnrealCost() => (int)this.PremiumCosts.GetTotalUnrealCosts(this.PremiumCosts.GetTotalMonths() - 1);

        private void UpdateSeries()
        {
            int monthsCount = this.PremiumCosts.GetTotalMonths();
            var unity = new int[monthsCount];
            var unreal = new int[monthsCount];

            for (int i = 0; i < monthsCount; i++)
            {
                unity[i] = (int)this.PremiumCosts.GetTotalUnityCosts(i);
                unreal[i] = (int)this.PremiumCosts.GetTotalUnrealCosts(i);
            }

            this.Series[0].Values = unity;
            this.Series[1].Values = unreal;

            // Notifying Properties Changed
            this.OnPropertyChanged(nameof(this.Series));
            this.OnPropertyChanged(nameof(this.FinalUnityCost));
            this.OnPropertyChanged(nameof(this.FinalUnrealCost));
            this.OnPropertyChanged(nameof(this.EffectiveUnityProfitSharing));
        }

        [RelayCommand]
        private void ExportToTSV()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            File.WriteAllText(Path.Combine(desktopPath, "UnityVsUnreal.tsv"), this.PremiumCosts.ToTSV());
        }
    }
}
