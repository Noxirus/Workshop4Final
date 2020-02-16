using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrogrammersWorkshop
{
    public partial class TravelExpert : MetroFramework.Forms.MetroForm
    {
        public TravelExpert()
        {
            InitializeComponent();
        }
        
        private List<Packages> pack = PackagesDB.GetPackages();
        private List<PackagesProductInfo> packProdSupp = Packages_Products_SuppliersDB.GetPackProductsSuppliers();
        private List<int> prod = ProductsDB.GetProductID();
        private List<int> supp = SuppliersDB.GetSupplierIDs();
        
        


    private void TravelExpert_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            
            foreach (var pkg in pack)
            {

                lstPkg.Items.Add(pkg.PkgName);
            }

           

            foreach (var supItem in supp)
            {
                lstSupplier.Items.Add(SuppliersDB.GetSupplier(supItem).SupName);
            }

            ResetProductList();



        }

        private void lstPkg_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var item in pack)
            {

                if(item.PkgName==lstPkg.SelectedItem.ToString())
                {
                    
                    var  pkgId = item.PackageId;
                    txtPkgName.Text = item.PkgName;
                    txtpkgID.Text = item.PackageId.ToString();
                    txtDesc.Text = item.PkgDesc;
                    txtBasePrice.Text = item.PkgBasePrice.ToString("c");
                    txtCommission.Text = item.PkgAgencyCommission.ToString("c");

                    if(item.PkgStartDate.HasValue)
                    {
                        txtPkgStrt.Text = item.PkgStartDate.Value.ToShortDateString();
                    }
                    else
                    {
                        txtPkgStrt.Text = "";
                    }


                    if (item.PkgEndDate.HasValue)
                    {
                        txtPkgEndDate.Text = item.PkgEndDate.Value.ToShortDateString();
                    }
                    else
                    {
                        txtPkgEndDate.Text = "";
                    }
                    
                }

          


            }

            List<object> pkgPrd = new List<object>();

            foreach (var pkg in pack)
            {
                if (pkg.PkgName == lstPkg.SelectedItem.ToString())
                {
                    gridprdpkg.Visible = true;
                    foreach ( var prdSup in packProdSupp)
                    {
                        if (prdSup.PackageId==pkg.PackageId)
                        {
                            pkgPrd.Add(prdSup);
                        }
                    }


                 }
            }

            foreach (var prdSup in packProdSupp)
            {
                if (prdSup.PackageId == Convert.ToInt32(txtpkgID.Text))
                {
                    gridprdpkg.DataSource = pkgPrd;
                    gridprdpkg.Columns[0].Visible = false;
                    gridprdpkg.Columns[1].Visible = false;
                    gridprdpkg.Columns[2].HeaderText = "Product Name";
                    gridprdpkg.Columns[3].HeaderText = "Supplier Name";
                    gridprdpkg.Columns[2].Width = 200;
                    gridprdpkg.Columns[3].Width = 200;
               
                }

                else
                {

                    gridprdpkg.Visible = true;
                }
               
            }

           


        

        }

        public void pkgADD_Click(object sender, EventArgs e)
        {

            txtPkgName.ReadOnly = false;
            txtPkgName.Text = "";
            txtPkgStrt.ReadOnly = false;
            txtPkgEndDate.Text = "";
            txtPkgStrt.Text = "";
            txtPkgEndDate.ReadOnly = false;
            txtBasePrice.ReadOnly = false;
            txtBasePrice.Text = "";
            txtCommission.ReadOnly = false;
            txtDesc.ReadOnly = false;
            txtDesc.Text = "";
            txtCommission.ReadOnly = false;
            txtCommission.Text = "";
            pkgEdit.Enabled = false;
            pkgdelete.Enabled = false;
            pkgSave.Enabled = true;
            pkgCancel.Enabled = true;
            gridprdpkg.DataSource = null;
            gridprdpkg.Rows.Clear();
            lstPkg.Enabled=false;
        }

        private void pkgSave_Click(object sender, EventArgs e)
        {
            DateTime? StartDate = string.IsNullOrWhiteSpace(txtPkgStrt.Text)
                ? (DateTime?)null
                : DateTime.Parse(txtPkgStrt.Text);

            DateTime? EndDate = string.IsNullOrWhiteSpace(txtPkgEndDate.Text)
              ? (DateTime?)null
              : DateTime.Parse(txtPkgEndDate.Text);


            Packages pckAdd = new Packages();
            pckAdd.PkgName = txtPkgName.Text;
            pckAdd.PkgStartDate = StartDate;
            pckAdd.PkgEndDate = EndDate;
            pckAdd.PkgDesc = txtDesc.Text;
            pckAdd.PkgBasePrice =Convert.ToDecimal( txtBasePrice.Text.Replace("$", ""));
            pckAdd.PkgAgencyCommission = Convert.ToDecimal(txtCommission.Text.Replace("$", ""));


            PackagesDB.AddPackage(pckAdd);

            lstPkg.Items.Clear();

            List<Packages> packlstAdd = PackagesDB.GetPackages();

            MessageBox.Show("Packages has been Added");

            foreach (var pkg in packlstAdd)
            {

                lstPkg.Items.Add(pkg.PkgName);
            }

            



        }

        private void pkgEdit_Click(object sender, EventArgs e)
        {
            txtPkgName.ReadOnly = false;
            txtPkgStrt.ReadOnly = false;
            txtPkgEndDate.ReadOnly = false;
            txtBasePrice.ReadOnly = false;
           
            txtCommission.ReadOnly = false;
            txtDesc.ReadOnly = false;
         
            txtCommission.ReadOnly = false;

            pkgADD.Enabled = false;
            pkgdelete.Enabled = false;
            pkgSave.Enabled = false;
            pkgCancel.Enabled = true;
            saveEdit.Visible = true;

        }

        private void saveEdit_Click(object sender, EventArgs e)


        {


            Packages oldPck = new Packages();
            foreach (var item in pack)
            {
                if(item.PackageId == Convert.ToInt32(txtpkgID.Text))
                    {
            
                        oldPck.PackageId = item.PackageId;
                        oldPck.PkgName = item.PkgName;
                        oldPck.PkgStartDate = item.PkgStartDate;
                        oldPck.PkgEndDate = item.PkgEndDate;
                        oldPck.PkgDesc =item.PkgDesc;
                        oldPck.PkgBasePrice = item.PkgBasePrice;
                        oldPck.PkgAgencyCommission = item.PkgAgencyCommission;

                    }
            }
            DateTime? StartDate = string.IsNullOrWhiteSpace(txtPkgStrt.Text)
                    ? (DateTime?)null
                    :  DateTime.Parse(txtPkgStrt.Text);

            DateTime? EndDate = string.IsNullOrWhiteSpace(txtPkgEndDate.Text)
              ? (DateTime?)null
              : DateTime.Parse(txtPkgEndDate.Text);


            Packages updtPck = new Packages();
            updtPck.PackageId = Convert.ToInt32(txtpkgID.Text);
            updtPck.PkgName = txtPkgName.Text;
            updtPck.PkgStartDate = StartDate;
            updtPck.PkgEndDate = EndDate;
            updtPck.PkgDesc = txtDesc.Text;

            updtPck.PkgBasePrice = Convert.ToDecimal(txtBasePrice.Text.Replace("$",""));
            updtPck.PkgAgencyCommission = Convert.ToDecimal(txtCommission.Text.Replace("$", ""));


            PackagesDB.UpdatePackage(oldPck, updtPck);
            MessageBox.Show("Packages has been updated");
        }

        private void pkgdelete_Click(object sender, EventArgs e)
        {
            Packages pkgDel = new Packages();
            Packages_Products_Suppliers pkgPrdDel = new Packages_Products_Suppliers();


       



            foreach (var item in pack)
            {
                if (item.PackageId == Convert.ToInt32(txtpkgID.Text))
                {

                    pkgDel.PackageId = item.PackageId;
                    pkgDel.PkgName = item.PkgName;
                    pkgDel.PkgStartDate = item.PkgStartDate;
                    pkgDel.PkgEndDate = item.PkgEndDate;
                    pkgDel.PkgDesc = item.PkgDesc;
                    pkgDel.PkgBasePrice = item.PkgBasePrice;
                    pkgDel.PkgAgencyCommission = item.PkgAgencyCommission;


                }
            }
         

            PackagesDB.DeletePackage(pkgDel);
            lstPkg.Items.Clear();
            MessageBox.Show("Packages has been Deleted");
            List<Packages> packupdatedel = PackagesDB.GetPackages();
            foreach (var pkg in packupdatedel)
            {
                lstPkg.Items.Add(pkg.PkgName);
            }






        }

        private void metroTabPage1_Click(object sender, EventArgs e)
        {
            lstPkg.Enabled = true;
        }

        private void lstProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            btnAddSaveProd.Visible = true;
            lblProdName.Visible = true;
            txtProductName.Visible = true;

            btnEditProducts.Enabled = false;
            txtProductName.Focus();

        }

        private void btnAddSaveProd_Click(object sender, EventArgs e)
        {
            Products prodAdd = new Products();

            prodAdd.ProdName = txtProductName.Text;

            ProductsDB.AddProduct(prodAdd);

            ResetProductList();

        }




        public void ResetProductList()
        {
            lstProducts.Items.Clear();
             List<int> prod1 = ProductsDB.GetProductID();
            foreach (var prdItem in prod1)
            {
                lstProducts.Items.Add(ProductsDB.GetProduct(prdItem).ProdName);
            }

            btnAddSaveProd.Visible = false;
            lblProdName.Visible = false;
            txtProductName.Visible = false;
            btnUpdateProduct.Visible = false;
            btnEditProducts.Enabled = true;
            btnAddProduct.Enabled = true;
            lstProducts.Enabled = true;


            btnSaveAddSupp.Visible = false;
            lblSupplierName.Visible = false;
            txtSupplier.Visible = false;
            btnUpdateSupplier.Visible = false;
        }

        private void btnResetProduct_Click(object sender, EventArgs e)
        {
            ResetProductList();
        }

        private void btnEditProducts_Click(object sender, EventArgs e)
        {   
            if (lstProducts.SelectedItem == null)
            {
                MessageBox.Show("Please Select a Product to edit");

            }
            else
            {
                txtProductName.Visible = true;
                btnUpdateProduct.Visible = true;
                lblProdName.Visible = true;
                btnAddProduct.Enabled = false;
                lstProducts.Enabled = false;

                txtProductName.Focus();
                txtProductName.Text = lstProducts.SelectedItem.ToString();

                


            }
           
        }

        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            foreach (var item in prod)
            {

                if (lstProducts.SelectedItem.ToString() == ProductsDB.GetProduct(item).ProdName)
                {
                    Products updatedProdcut = new Products();

                    updatedProdcut.ProductID = item;
                    updatedProdcut.ProdName = txtProductName.Text;


                    ProductsDB.UpdateProduct(item, updatedProdcut);
                    MessageBox.Show("Product Updated");

                    ResetProductList();

                }
            }
        }
    }
}
