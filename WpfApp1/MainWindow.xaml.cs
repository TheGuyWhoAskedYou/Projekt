using LiveCharts;

using System;
using System.Windows;
using System.Windows.Controls;

using WpfApp1;

namespace MzdovaKalkulacka
{
    public partial class MainWindow : Window
    {
        private bool hasZeroIncomeSpouse;
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
                bool.TryParse(ZeroIncomeSpouseCheckBox.IsChecked.ToString(), out hasZeroIncomeSpouse) &&
                decimal.TryParse(ServiceCarPriceTextBox.Text, out decimal serviceCarPrice) &&
                decimal.TryParse(InterestOnHousingLoanTextBox.Text, out decimal interestOnHousingLoan) &&
                decimal.TryParse(DonationsTextBox.Text, out decimal donations) &&
                decimal.TryParse(PensionInsuranceContributionsTextBox.Text, out decimal pensionInsuranceContribution) &&
                decimal.TryParse(LifeInsuranceContributionTextBox.Text, out decimal lifeInsuranceContribution) &&
                decimal.TryParse(UnionDuesTextBox.Text, out decimal unionDues) &&
                decimal.TryParse(EducationContributionsTextBox.Text, out decimal educationContributions) &&
                decimal.TryParse(ResearchContributionsTextBox.Text, out decimal researchContributions))
            {
                if (grossSalary >= 17300)
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

                    // Apply additional tax deductions
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
                    MessageBox.Show("Minimální měsíční mzda je 17 300 Kč.");
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

            if (taxBase <= 1000000)
            {
                tax = taxBase * 0.15m; // Daňová sazba 15% pro základ daně do 1 000 000 Kč
            }
            else
            {
                tax = 150000 + (taxBase - 1000000) * 0.23m; // Daňová sazba 23% pro základ daně nad 1 000 000 Kč
            }

            return tax;
        }

        private decimal CalculateIncomeTaxPrepayment(decimal netSalary, decimal tax, int children)
        {
            // Výpočet záloh z daní příjmů
            decimal incomeTaxPrepayment = tax - CalculateTaxAllowances(children);

            // Omezení záloh z daní příjmů na čistou mzdu
            if (incomeTaxPrepayment > netSalary)
            {
                incomeTaxPrepayment = netSalary;
            }

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

            // Sleva na manžela/manaželku bez příjmů
            if (hasZeroIncomeSpouse)
            {
                decimal zeroIncomeSpouseAllowance = 2070;
                taxAllowances += zeroIncomeSpouseAllowance;
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
            if (decimal.TryParse(GrossSalaryTextBox.Text, out decimal grossSalary) &&
                int.TryParse(ChildrenTextBox.Text, out int children) &&
                decimal.TryParse(NurseryFeeTextBox.Text, out decimal nurseryFee) &&
                decimal.TryParse(ServiceCarPriceTextBox.Text, out decimal serviceCarPrice) &&
                decimal.TryParse(InterestOnHousingLoanTextBox.Text, out decimal interestOnHousingLoan) &&
                decimal.TryParse(DonationsTextBox.Text, out decimal donations) &&
                decimal.TryParse(PensionInsuranceContributionsTextBox.Text, out decimal pensionInsuranceContribution) &&
                decimal.TryParse(LifeInsuranceContributionTextBox.Text, out decimal lifeInsuranceContribution) &&
                decimal.TryParse(UnionDuesTextBox.Text, out decimal unionDues) &&
                decimal.TryParse(EducationContributionsTextBox.Text, out decimal educationContributions) &&
                decimal.TryParse(ResearchContributionsTextBox.Text, out decimal researchContributions))
            {
                if (grossSalary >= 17300)
                {
                    decimal annualNetSalary = 0;

                    // Perform the annual calculation based on the provided input values
                    decimal taxBase = CalculateTaxBase(grossSalary);
                    decimal tax = CalculateIncomeTax(taxBase);
                    decimal incomeTaxPrepayment = CalculateIncomeTaxPrepayment(grossSalary, tax, children);

                    // Perform the calculation for annual net salary
                    decimal annualSocialSecurityContribution = grossSalary * 0.065m * 12; // Assuming constant throughout the year
                    decimal annualHealthInsuranceContribution = grossSalary * 0.045m * 12; // Assuming constant throughout the year
                    decimal annualPensionInsuranceContribution = pensionInsuranceContribution * 12;
                    decimal annualLifeInsuranceContribution = lifeInsuranceContribution * 12;
                    decimal annualUnionDues = unionDues * 12;
                    decimal annualEducationContributions = educationContributions * 12;
                    decimal annualResearchContributions = researchContributions * 12;
                    decimal annualIncomeTaxPrepayment = incomeTaxPrepayment * 12;

                    decimal annualDeductions = annualSocialSecurityContribution + annualHealthInsuranceContribution +
                        annualPensionInsuranceContribution + annualLifeInsuranceContribution + annualUnionDues +
                        annualEducationContributions + annualResearchContributions + annualIncomeTaxPrepayment;

                    annualNetSalary = grossSalary * 12 - annualDeductions;

                    // Create a new instance of the AnnualCalculationWindow
                    var annualCalculationWindow = new AnnualCalculationWindow();

                    // Set the values in the AnnualCalculationWindow
                    annualCalculationWindow.AnnualNetSalaryTextBlock.Text = $"{annualNetSalary:C1}";
                    annualCalculationWindow.AnnualGrossSalaryTextBlock.Text = $"{grossSalary * 12:C1}";

                    // Show the annual calculation window
                    annualCalculationWindow.Show();
                }
                else
                {
                    MessageBox.Show("Minimální měsíční mzda je 17 300 Kč.");
                }
            }
            else
            {
                MessageBox.Show("Zadejte platovou měsíční mzdu a ostatní vstupní hodnoty ve správném formátu.");
            }
        }
    }
}
