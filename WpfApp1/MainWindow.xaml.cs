using LiveCharts;

using System;
using System.Windows;
using System.Windows.Controls;

using WpfApp1;

namespace MzdovaKalkulacka
{
    public partial class MainWindow : Window
    {
        private bool isStudentTaxpayer;
        private bool isDisabilityTaxpayer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(GrossSalaryTextBox.Text, out decimal grossSalary) &&
                int.TryParse(ChildrenTextBox.Text, out int children) &&
                bool.TryParse(StudentTaxpayerCheckBox.IsChecked.ToString(), out isStudentTaxpayer) &&
                bool.TryParse(DisabilityTaxpayerCheckBox.IsChecked.ToString(), out isDisabilityTaxpayer) &&
                decimal.TryParse(NurseryFeeTextBox.Text, out decimal nurseryFee) &&
                decimal.TryParse(InterestOnHousingLoanTextBox.Text, out decimal interestOnHousingLoan) &&
                decimal.TryParse(DonationsTextBox.Text, out decimal donations) &&
                decimal.TryParse(PensionInsuranceContributionsTextBox.Text, out decimal pensionInsuranceContribution) &&
                decimal.TryParse(LifeInsuranceContributionTextBox.Text, out decimal lifeInsuranceContribution) &&
                decimal.TryParse(UnionDuesTextBox.Text, out decimal unionDues) &&
                decimal.TryParse(EducationContributionsTextBox.Text, out decimal educationContributions) &&
                decimal.TryParse(ResearchContributionsTextBox.Text, out decimal researchContributions))
            {
                string formattedGrossSalary = grossSalary.ToString("N0");
                string formattedLifeInsuranceContribution = lifeInsuranceContribution.ToString("N0");

                if (grossSalary >= 17300 && grossSalary <= 600000)
                {
                    decimal taxBase = CalculateTaxBase(grossSalary);
                    decimal tax = CalculateIncomeTax(taxBase);

                    // Výpočet sociálního pojištění
                    decimal socialSecurityContributionRate = 0.065m; // Sazba 6,5%
                    decimal socialSecurityContribution = grossSalary * socialSecurityContributionRate;

                    // Výpočet zdravotního pojištění
                    decimal healthInsuranceContributionRate = 0.045m; // Sazba 4,5%
                    decimal healthInsuranceContribution = grossSalary * healthInsuranceContributionRate;

                    // Výpočet čisté mzdy
                    decimal netSalary = CalculateNetSalary(grossSalary, tax, socialSecurityContribution, healthInsuranceContribution);

                    if (isStudentTaxpayer)
                    {
                        decimal studentAllowance = 335;
                        netSalary += studentAllowance;
                    }

                    if (isDisabilityTaxpayer)
                    {
                        decimal disabilityDeduction = 210;
                        netSalary += disabilityDeduction;
                    }


                    // Zvýhodnění pro děti
                    decimal childAllowance = CalculateChildAllowance(children);
                    netSalary += childAllowance;

                    // Přičtení daně na poplatníka
                    decimal taxpayerAllowance = 2570;
                    netSalary += taxpayerAllowance;

                    // Výpočet záloh z daní příjmů
                    decimal incomeTaxPrepayment = CalculateIncomeTaxPrepayment(netSalary, tax, children);

                    // Aktualizace hodnot v grafu
                    SocialSecuritySeries.Values = new ChartValues<decimal> { socialSecurityContribution };
                    HealthInsuranceSeries.Values = new ChartValues<decimal> { healthInsuranceContribution };
                    NetSalarySeries.Values = new ChartValues<decimal> { netSalary };
                    TaxPrepaymentSeries.Values = new ChartValues<decimal> { incomeTaxPrepayment };

                    NetSalaryTextBlock.Text = $"{netSalary.ToString("C1")}";

                    NetSalarySeries.LabelPoint = chartPoint => $"{chartPoint.Y} Kč";
                    HealthInsuranceSeries.LabelPoint = chartPoint => $"{chartPoint.Y} Kč";
                    SocialSecuritySeries.LabelPoint = chartPoint => $"{chartPoint.Y} Kč";
                    TaxPrepaymentSeries.LabelPoint = chartPoint => $"{chartPoint.Y} Kč";

                    Graf.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Minimální hrubá měsíční mzda je 17 300 Kč a maximální 600 000 Kč.");
                }
            }
            else
            {
                MessageBox.Show("Zadejte platovou měsíční mzdu a ostatní vstupní hodnoty ve správném formátu.");
            }
        }


        private decimal CalculateTaxBase(decimal grossSalary)
        {
            decimal taxBase = grossSalary;

            return taxBase;
        }

        private decimal CalculateIncomeTax(decimal taxBase)
        {
            decimal tax = 0;

            if (taxBase <= 161296)
            {
                tax = taxBase * 0.15m; // Daňová sazba 15% pro základ daně do 161 296 Kč
            }
            else
            {
                tax = taxBase * 0.23m; // Daňová sazba 23% pro základ daně nad 161 296 Kč
            }

            return tax;
        }

        private decimal CalculateIncomeTaxPrepayment(decimal netSalary, decimal tax, int children)
        {
            // Výpočet záloh z daní příjmů
            decimal incomeTaxPrepayment = tax - CalculateTaxAllowances(children);

            

            return incomeTaxPrepayment;
        }

        private decimal CalculateTaxAllowances(int children)
        {
            decimal taxAllowances = 0;

            // Přičtení daně na poplatníka
            decimal taxpayerAllowance = 2570;
            taxAllowances += taxpayerAllowance;


            // Sleva na studenta
            if (isStudentTaxpayer)
            {
                decimal studentAllowance = 335;
                taxAllowances += studentAllowance;
            }

            // Sleva na invaliditu
            if (isDisabilityTaxpayer)
            {
                decimal disabilityDeduction = 210; // Sleva ve výši 25% z čistého příjmu
                taxAllowances += disabilityDeduction;
            }

            // Slevy na děti
            decimal childAllowance = CalculateChildAllowance(children);
            taxAllowances += childAllowance;

            return taxAllowances;
        }

        private decimal CalculateChildAllowance(int children)
        {
            decimal childAllowance = children switch
            {
                1 => 1267,
                2 => 1860 + 1267,
                3 => 2320 + 1267 + 1860,
                4 => 2320 + 1267 + 1860 + 2320,
                5 => 2320 + 1267 + 1860 + 2320 + 2320,
                6 => 2320 + 1267 + 1860 + 2320 + 2320 + 2320,
                7 => 2320 + 1267 + 1860 + 2320 + 2320 + 2320 + 2320,
                8 => 2320 + 1267 + 1860 + 2320 + 2320 + 2320 + 2320 + 2320,
                9 => 2320 + 1267 + 1860 + 2320 + 2320 + 2320 + 2320 + 2320 + 2320,
                10 => 2320 + 1267 + 1860 + 2320 + 2320 + 2320 + 2320 + 2320 + 2320 + 2320,
                _ => 0
            };

            return childAllowance;
        }





        private decimal CalculateNetSalary(decimal grossSalary, decimal incomeTax, decimal socialSecurityContribution,
            decimal healthInsuranceContribution)
        {
            decimal deductions = incomeTax + socialSecurityContribution + healthInsuranceContribution;

            decimal netSalary = grossSalary - deductions;

            return netSalary;
            
        }





        private void AnnualCalculationButton_Click(object sender, RoutedEventArgs e)
        {
            decimal CalculateRefund(decimal nurseryFee,
            decimal interestOnHousingLoan, decimal donations, decimal lifeInsuranceContribution,
            decimal unionDues, decimal educationContributions, decimal researchContributions)
            {
                decimal deductibleExpenses = nurseryFee + (interestOnHousingLoan + lifeInsuranceContribution
                    + unionDues + educationContributions + researchContributions) * 0.15m;

                if (donations >= 1000)
                {
                    deductibleExpenses += donations * 0.15m;
                }

                return deductibleExpenses;
            }


            if (decimal.TryParse(GrossSalaryTextBox.Text, out decimal grossSalary) &&
                int.TryParse(ChildrenTextBox.Text, out int children) &&
                decimal.TryParse(NurseryFeeTextBox.Text, out decimal nurseryFee) &&
                decimal.TryParse(InterestOnHousingLoanTextBox.Text, out decimal interestOnHousingLoan) &&
                decimal.TryParse(DonationsTextBox.Text, out decimal donations) &&
                decimal.TryParse(PensionInsuranceContributionsTextBox.Text, out decimal pensionInsuranceContribution) &&
                decimal.TryParse(LifeInsuranceContributionTextBox.Text, out decimal lifeInsuranceContribution) &&
                decimal.TryParse(UnionDuesTextBox.Text, out decimal unionDues) &&
                decimal.TryParse(EducationContributionsTextBox.Text, out decimal educationContributions) &&
                decimal.TryParse(ResearchContributionsTextBox.Text, out decimal researchContributions))
            {

                if (grossSalary >= 17300 && grossSalary <= 600000)
                {
                    decimal annualNetSalary = 0;

                    // Provede výpočet ročního zúčtování na základě vložených hodnot
                    decimal taxBase = CalculateTaxBase(grossSalary);
                    decimal tax = CalculateIncomeTax(taxBase);
                    decimal incomeTaxPrepayment = CalculateIncomeTaxPrepayment(grossSalary, tax, children);
                    decimal refund = CalculateRefund(nurseryFee, interestOnHousingLoan, donations, lifeInsuranceContribution, unionDues, educationContributions, researchContributions);

                    // Provede výpočet ročního čístého příjmu
                    decimal annualSocialSecurityContribution = grossSalary * 0.065m * 12;
                    decimal annualHealthInsuranceContribution = grossSalary * 0.045m * 12;
                    decimal annualIncomeTaxPrepayment = incomeTaxPrepayment * 12;

                    decimal annualDeductions = annualSocialSecurityContribution + annualHealthInsuranceContribution + annualIncomeTaxPrepayment;

                    annualNetSalary = grossSalary * 12 - annualDeductions;

                    // Vytvoří novou instanci okna ročního zúčtování
                    var annualCalculationWindow = new AnnualCalculationWindow();

                    // Nastaví hodnoty v textblock, které jsou v okně s ročním zúčtováním
                    annualCalculationWindow.AnnualNetSalaryTextBlock.Text = $"{annualNetSalary:C1}";
                    annualCalculationWindow.AnnualGrossSalaryTextBlock.Text = $"{grossSalary * 12:C1}";
                    annualCalculationWindow.OverpaymentTextBlock.Text = $"{refund:C1}";

                    // Otevře okno s ročním zúčtováním
                    annualCalculationWindow.Show();
                }
                else
                {
                    MessageBox.Show("Minimální hrubá měsíční mzda je 17 300 Kč a maximální 600 000 Kč.");
                }
            }
            else
            {
                MessageBox.Show("Zadejte platovou měsíční mzdu a ostatní vstupní hodnoty ve správném formátu.");
            }
        }
    }
}
