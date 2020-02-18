using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrogrammersWorkshop
{
    class Validation
    {
        public static bool IsPresent(MetroTextBox txtBox, string name)
        {
            bool isValid = true;
            if (txtBox.Text == "")
            {
                isValid = false;
                MessageBox.Show(name + " needs to be filled out", "Missing Information");
                txtBox.Focus();
            }
            return isValid;
        }
        public static bool IsADecimal(MetroTextBox txtBox, string name)
        {
            bool isValid = true;
            decimal dec;
            if (!Decimal.TryParse(txtBox.Text, out dec)) 
            {

                isValid = false;
                MessageBox.Show(name + " must be a number \nType: Int32", "Input Error");
                txtBox.SelectAll();
                txtBox.Focus();
            }
            return isValid;
        }
        public static bool IsAInt32(MetroTextBox txtBox, string name)
        {
            bool isValid = true;
            int value;
            if (!Int32.TryParse(txtBox.Text, out value)) 
            {

                isValid = false;
                MessageBox.Show(name + " must be a number \nType: Int32", "Input Error");
                txtBox.SelectAll();
                txtBox.Focus();
            }
            return isValid;
        }
        public static bool IsCurrectDateTime(MetroTextBox txtBox, string name)
        {
            bool isValid = true;
            DateTime dt;
            if (!DateTime.TryParseExact(txtBox.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out dt)) 
            {
                isValid = false;
                MessageBox.Show("Please Eneter start Date in format in MM/DD/YYYY", "Input Error");
            }
            return isValid;
        }

        public static bool NotNegativeDeciaml(MetroTextBox txtBox, string name)
        {
            bool isValid = true;
            decimal value;
            decimal notNegative;
            notNegative = Convert.ToDecimal(txtBox.Text);

            if (!Decimal.TryParse(txtBox.Text, out value))
            {
                isValid = false;
                IsADecimal(txtBox, name);

            }
            else if (notNegative < 0) // if negative
            {
                isValid = false;
                MessageBox.Show(name + "must be positive or zero", "Input Error");
                txtBox.SelectAll();
                txtBox.Focus();
            }
            return isValid;
        }
    }// end of class
}// end of namespace
