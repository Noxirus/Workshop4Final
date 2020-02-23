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
        public static bool IsPresent(MetroTextBox txtBox, string name, Label lblError)
        {
            bool isValid = true;
            if (txtBox.Text == "")
            {
                isValid = false;
                lblError.Text = name + " needs to be filled out";
                txtBox.Focus();
            }
            return isValid;
        }

            public static bool IsADecimal(MetroTextBox txtBox, string name, Label lblError)
        {
            bool isValid = true;
            decimal dec;
            if (!Decimal.TryParse(txtBox.Text, out dec)) 
            {

                isValid = false;
                lblError.Text = name + " must be a number. Type: Decimal";
                txtBox.SelectAll();
                txtBox.Focus();
            }
            return isValid;
        }
        public static bool IsNonNegativeInt32(MetroTextBox txtBox, string name, Label lblError)
        {
            bool isValid = true;
            int value;
            if (!Int32.TryParse(txtBox.Text, out value)) 
            {

                isValid = false;
                lblError.Text = name + " must be a number. Type: Int32";
                txtBox.SelectAll();
                txtBox.Focus();
            }
            else if (value < 0) // negetive
            {
                isValid = false;
                lblError.Text = name + " must be positive or zero";
                txtBox.SelectAll();
                txtBox.Focus();
            }
            return isValid;
        }
        public static bool IsCurrectDateTime(MetroTextBox txtBox, string name, Label lblError)
        {
            bool isValid = true;
            DateTime dt;
            if (!DateTime.TryParseExact(txtBox.Text, "MM/dd/yyyy", null, DateTimeStyles.None, out dt)) 
            {
                isValid = false;
                lblError.Text = "Please eneter " + name + " in format in MM/DD/YYYY";
            }
            return isValid;
        }

        public static bool NotNegativeDeciaml(MetroTextBox txtBox, string name, Label lblError)
        {
            bool isValid = true;
            decimal value;
            decimal notNegative;
            notNegative = Convert.ToDecimal(txtBox.Text);

            if (!Decimal.TryParse(txtBox.Text, out value))
            {
                isValid = false;
                IsADecimal(txtBox, name, lblError);

            }
            else if (notNegative < 0) // if negative
            {
                isValid = false;
                lblError.Text = name + " must be positive or zero";
                txtBox.SelectAll();
                txtBox.Focus();
            }
            return isValid;
        }
        public static bool IsListSelected(ListBox list, string name, Label lblError)
        {
            bool isValid = true;
            
            if (list.SelectedIndex == -1)
            {
                isValid = false;
                lblError.Text = "Please select a list item from " + name;
            }
            return isValid;
        }
    }// end of class
}// end of namespace
