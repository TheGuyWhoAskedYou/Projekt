using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                decimal taxBase = CalculateTaxBase(grossSalary, children, nurseryFee, serviceCarPrice, interestOnHousingLoan);
                decimal tax = CalculateIncomeTax(taxBase);
                decimal socialSecurityContribution = CalculateSocialSecurityContribution(grossSalary);
                decimal healthInsuranceContribution = CalculateHealthInsuranceContribution(grossSalary);
                decimal netSalary = CalculateNetSalary(grossSalary, tax, socialSecurityContribution, healthInsuranceContribution,
                    pensionInsuranceContribution, lifeInsuranceContribution, unionDues, educationContributions, researchContributions);
            }
            else
            {
                MessageBox.Show("Zadejte platovou měsíční mzdu a ostatní vstupní hodnoty ve správném formátu.");
            }
        }

        private decimal CalculateTaxBase(decimal grossSalary, int children, decimal nurseryFee, decimal serviceCarPrice, decimal interestOnHousingLoan)
        {
            decimal taxBase = grossSalary;

            // Odpočítávání nezdanitelných částí základu daně
            taxBase -= nurseryFee;
            taxBase -= interestOnHousingLoan;

            // Zvýhodnění pro děti
            decimal childAllowance = children switch
            {
                1 => 1520,
                2 => 2690,
                3 => 4020,
                _ => 0
            };
            taxBase -= childAllowance;

            // Zvýhodnění pro manželku/manažela s nulovými příjmy
            if (hasZeroIncomeSpouse)
            {
                decimal zeroIncomeSpouseAllowance = 2400;
                taxBase -= zeroIncomeSpouseAllowance;
            }

            // Omezení daňového základu pro služební auto
            decimal carLimit = 3000;
            if (serviceCarPrice > carLimit)
            {
                decimal carExcess = serviceCarPrice - carLimit;
                taxBase -= carExcess;
            }

            return taxBase;
        }

        private decimal CalculateIncomeTax(decimal taxBase)
        {
            decimal tax = 0;

            if (isDisabilityTaxpayer)
            {
                if (taxBase <= 15100)
                {
                    tax = taxBase * 0.15m;
                }
                else if (taxBase <= 100700)
                {
                    tax = 2265 + (taxBase - 15100) * 0.23m;
                }
                else
                {
                    tax = 23114 + (taxBase - 100700) * 0.33m;
                }
            }
            else
            {
                if (taxBase <= 10600)
                {
                    tax = taxBase * 0.15m;
                }
                else if (taxBase <= 85528)
                {
                    tax = 1590 + (taxBase - 10600) * 0.23m;
                }
                else
                {
                    tax = 13830 + (taxBase - 85528) * 0.33m;
                }
            }

            return tax;
        }

        private decimal CalculateSocialSecurityContribution(decimal grossSalary)
        {
            decimal socialSecurityContribution = grossSalary * 0.0654m;
            return socialSecurityContribution;
        }

        private decimal CalculateHealthInsuranceContribution(decimal grossSalary)
        {
            decimal healthInsuranceContribution = grossSalary * 0.045m;
            return healthInsuranceContribution;
        }

        private decimal CalculateNetSalary(decimal grossSalary, decimal incomeTax, decimal socialSecurityContribution,
            decimal healthInsuranceContribution, decimal pensionInsuranceContribution, decimal lifeInsuranceContribution,
            decimal unionDues, decimal educationContributions, decimal researchContributions)
        {
            decimal deductions = incomeTax + socialSecurityContribution + healthInsuranceContribution +
                pensionInsuranceContribution + lifeInsuranceContribution + unionDues +
                educationContributions + researchContributions;

            decimal netSalary = grossSalary - deductions;
            return netSalary;
        }
    }
}
