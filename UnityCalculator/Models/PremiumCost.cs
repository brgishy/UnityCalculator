
namespace UnityCalculator.Models
{
    using System;
    using System.Text;
    using UnityCalculator.ViewModels;

    public class PremiumCosts : ViewModelBase
    {
        private bool usedUnityProDuringDevelopment = true;
        private int monthsOfDevelopment = 24;
        private int monthsOfLive = 36;
        private int numberOfDevelopers = 6;
        private float gameCost = 4.99f;
        private float platformCut = 0.3f;
        private float totalGrossRevenue = 5000000.0f;
        private float yearlyUnityProCost = 2040f;

        public bool UsedUnityProDuringDevelopment
        {
            get => usedUnityProDuringDevelopment;
            set => SetProperty(ref usedUnityProDuringDevelopment, value);
        }

        public int? MonthsOfDevelopment
        {
            get => monthsOfDevelopment;
            set => SetProperty(ref monthsOfDevelopment, value ?? 0);
        }

        public int? MonthsOfLive
        {
            get => monthsOfLive;
            set => SetProperty(ref monthsOfLive, value ?? 0);
        }

        public int? NumberOfDevelopers
        {
            get => numberOfDevelopers;
            set => SetProperty(ref numberOfDevelopers, value ?? 0);
        }

        public float? GameCost
        {
            get => gameCost;
            set => SetProperty(ref gameCost, value ?? 0);
        }

        public float? PlatformCut
        {
            get => platformCut;
            set => SetProperty(ref platformCut, value ?? 0);
        }

        public float? TotalGrossRevenue
        {
            get => totalGrossRevenue;
            set => SetProperty(ref totalGrossRevenue, value ?? 0);
        }

        public float? YearlyUnityProCost
        {
            get => yearlyUnityProCost;
            set => SetProperty(ref yearlyUnityProCost, value ?? 0);
        }

        public float GetAverageGrossMonthlyRevenue() => this.totalGrossRevenue / this.monthsOfLive;

        public int GetAverageMonthlyInstalls() => (int)(this.GetAverageGrossMonthlyRevenue() / (gameCost * (1.0f - platformCut)));

        public string GetStage(int month) => month < this.monthsOfDevelopment ? "Dev" : "Live";

        public int GetMonthlyInstalls(int month) => month < this.monthsOfDevelopment ? 0 : this.GetAverageMonthlyInstalls();

        public float GetMonthlyGrossRevenue(int month) => month < this.monthsOfDevelopment ? 0 : this.GetAverageGrossMonthlyRevenue();

        public float GetMonthlyUnityProCost(int month)
        {
            float rolling12MonthsOfGrossRevenue = GetRollingMonthlyGrossRevenue(month);
            bool isUsingUnityPro = UsedUnityProDuringDevelopment || rolling12MonthsOfGrossRevenue > 200000.0f;

            return isUsingUnityPro ? yearlyUnityProCost / 12.0f * numberOfDevelopers : 0.0f;
        }

        public float GetMonthlyInstallRuntimeFee(int month)
        {
            int totalInstalls = GetTotalInstalls(month);
            float rollingRevenue = GetRollingMonthlyGrossRevenue(month);

            if (rollingRevenue < 1000000.0f || totalInstalls < 1000000)
            {
                return 0.0f;
            }

            CalculateInstallFee(month, out int tier1Count, out int tier2Count, out int tier3Count, out int tier4Count);

            return tier1Count * 0.15f +
                   tier2Count * 0.075f +
                   tier3Count * 0.03f +
                   tier4Count * 0.02f;
        }

        public float GetMonthlyPercentageRuntimeFee(int month)
        {
            int totalInstalls = GetTotalInstalls(month);
            float rollingRevenue = GetRollingMonthlyGrossRevenue(month);

            return rollingRevenue > 1000000.0f && totalInstalls > 1000000 ?
                GetMonthlyGrossRevenue(month) * 0.025f :
                0.0f;
        }

        public float GetMonthlyRuntimeFee(int month)
        {
            float install = GetMonthlyInstallRuntimeFee(month);
            float percent = GetMonthlyPercentageRuntimeFee(month);
            return install < percent ? install : percent;
        }

        public int GetTotalInstalls(int month) => this.SumPrevious(this.GetMonthlyInstalls, month);

        public float GetRollingMonthlyGrossRevenue(int month) => this.SumPrevious(this.GetMonthlyGrossRevenue, month, 12);

        public float GetTotalGrossRevenue(int month) => this.SumPrevious(this.GetMonthlyGrossRevenue, month);

        public float GetTotalUnityProCost(int month) => this.SumPrevious(this.GetMonthlyUnityProCost, month);

        public float GetTotalRuntimeFeeCosts(int month) => this.SumPrevious(this.GetMonthlyRuntimeFee, month);

        public float GetTotalUnityCosts(int month) => this.GetTotalUnityProCost(month) + this.GetTotalRuntimeFeeCosts(month);

        public float GetTotalUnrealCosts(int month) => Math.Max(0.0f, this.GetTotalGrossRevenue(month) - 1000000.0f) * 0.05f;

        public int GetTotalMonths() => this.monthsOfDevelopment + this.monthsOfLive;

        public string ToTSV()
        {
            var tsv = new StringBuilder();
            var separator = "\t";
            
            WriteHeader();

            for (int i = 0; i < this.GetTotalMonths(); i++)
            {
                WriteRow(i);
            }

            return tsv.ToString();

            void WriteHeader()
            {
                tsv.Append("Month");
                tsv.Append(separator);
                tsv.Append("Stage");
                tsv.Append(separator);
                tsv.Append("Monthly Installs");
                tsv.Append(separator);
                tsv.Append("Total Installs");
                tsv.Append(separator);
                tsv.Append("Monthly Revenue");
                tsv.Append(separator);
                tsv.Append("Rolling Revenue");
                tsv.Append(separator);
                tsv.Append("Total Revenue");
                tsv.Append(separator);
                tsv.Append("Monthly Pro");
                tsv.Append(separator);
                tsv.Append("Total Pro Costs");
                tsv.Append(separator);
                tsv.Append("Monthly Install Fee");
                tsv.Append(separator);
                tsv.Append("Monthly Percentage Fee");
                tsv.Append(separator);
                tsv.Append("Monthly Runtime Fee");
                tsv.Append(separator);
                tsv.Append("Total Runtime Fee");
                tsv.Append(separator);
                tsv.Append("Total Unity");
                tsv.Append(separator);
                tsv.Append("Total Unreal");
                tsv.Append(separator);
                tsv.AppendLine();
            }

            void WriteRow(int row)
            {
                tsv.Append((row + 1).ToString());
                tsv.Append(separator);
                tsv.Append(this.GetStage(row));
                tsv.Append(separator);
                tsv.Append(this.GetMonthlyInstalls(row));
                tsv.Append(separator);
                tsv.Append(this.GetTotalInstalls(row));
                tsv.Append(separator);
                tsv.Append(FormatMoney(this.GetMonthlyGrossRevenue(row)));
                tsv.Append(separator);
                tsv.Append(FormatMoney(this.GetRollingMonthlyGrossRevenue(row)));
                tsv.Append(separator);
                tsv.Append(FormatMoney(this.GetTotalGrossRevenue(row)));
                tsv.Append(separator);
                tsv.Append(FormatMoney(this.GetMonthlyUnityProCost(row)));
                tsv.Append(separator);
                tsv.Append(FormatMoney(this.GetTotalUnityProCost(row)));
                tsv.Append(separator);
                tsv.Append(FormatMoney(this.GetMonthlyInstallRuntimeFee(row)));
                tsv.Append(separator);
                tsv.Append(FormatMoney(this.GetMonthlyPercentageRuntimeFee(row)));
                tsv.Append(separator);
                tsv.Append(FormatMoney(this.GetMonthlyRuntimeFee(row)));
                tsv.Append(separator);
                tsv.Append(FormatMoney(this.GetTotalRuntimeFeeCosts(row)));
                tsv.Append(separator);
                tsv.Append(FormatMoney(this.GetTotalUnityCosts(row)));
                tsv.Append(separator);
                tsv.Append(FormatMoney(this.GetTotalUnrealCosts(row)));
                tsv.AppendLine();
            }

            string FormatMoney(float money) => money.ToString("C0");
        }

        private void CalculateInstallFee(int month, out int tier1Count, out int tier2Count, out int tier3Count, out int tier4Count)
        {
            int lastMonthTotalInstalls = this.GetTotalInstalls(month - 1);
            int thisMonthsInstalls = this.GetMonthlyInstalls(month);

            // We breached 1 MM, make sure we only count those
            if (lastMonthTotalInstalls < 1000000)
            {
                thisMonthsInstalls -= 1000000 - lastMonthTotalInstalls;
                lastMonthTotalInstalls = 1000000;
            }

            int currentInstallCount = lastMonthTotalInstalls - 1000000;

            tier1Count = GetBracketCount(0, 100000, currentInstallCount, thisMonthsInstalls);
            currentInstallCount += tier1Count;
            thisMonthsInstalls -= tier1Count;

            tier2Count = GetBracketCount(100000, 500000, currentInstallCount, thisMonthsInstalls);
            currentInstallCount += tier2Count;
            thisMonthsInstalls -= tier2Count;

            tier3Count = GetBracketCount(500000, 1000000, currentInstallCount, thisMonthsInstalls);
            currentInstallCount += tier3Count;
            thisMonthsInstalls -= tier3Count;

            tier4Count = GetBracketCount(1000001, int.MaxValue, currentInstallCount, thisMonthsInstalls);

            int GetBracketCount(int min, int max, int currentInstalls, int newInstalls)
            {
                if (newInstalls <= 0)
                {
                    return 0;
                }

                int installsLeftInBracket = currentInstalls > max ? 0 :
                                            currentInstalls > min ? max - currentInstalls :
                                            max - min;

                return newInstalls >= installsLeftInBracket ?
                       installsLeftInBracket :
                       newInstalls;
            }
        }

        private int SumPrevious(Func<int, int> Func, int month, int maxIterations = int.MaxValue)
        {
            int total = 0;
            int iteration = 0;

            while (month >= 0 && iteration < maxIterations)
            {
                total += Func(month);
                month--;
                iteration++;
            }

            return total;
        }

        private float SumPrevious(Func<int, float> Func, int month, int maxIterations = int.MaxValue)
        {
            float total = 0.0f;
            int iteration = 0;

            while (month >= 0 && iteration < maxIterations)
            {
                total += Func(month);
                month--;
                iteration++;
            }

            return total;
        }
    }
}
