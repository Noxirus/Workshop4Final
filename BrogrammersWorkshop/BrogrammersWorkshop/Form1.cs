using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
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
        private List<PackagesProductInfo> productSupplierList = Products_SuppliersDB.GetProductsSuppliers();
        private List<int> prod = ProductsDB.GetProductID();
        private List<int> supp = SuppliersDB.GetSupplierIDs();
        List<string> product = new List<string>();
        List<string> supplier = new List<string>();






        private void TravelExpert_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            foreach (var pkg in pack)
            {

                lstPkg.Items.Add(pkg.PkgName);
            }

            gridProductSuppliers.DataSource = productSupplierList;

            gridProductSuppliers.Columns[0].Visible = false;

            foreach (var prd in prod)
            {
                product.Add(ProductsDB.GetProduct(prd).ProdName);
            }

            foreach (var sup in supp)
            {
                supplier.Add(SuppliersDB.GetSupplier(sup).SupName);
            }


            var distinctPrd = productSupplierList.Select(o => o.ProdName).Distinct().ToList();
            comboPrdPack.DataSource = distinctPrd;
            comboProduct.DataSource = product;
            comboSupplier.DataSource = supplier;

            var lstsup = new List<string>();
            foreach (var supname in productSupplierList)
            {
                if (supname.ProdName == comboPrdPack.SelectedItem.ToString())
                {
                    listSuppPkg.Items.Add(supname.SupName);

                }
            }



            gridProductSuppliers.Columns[1].HeaderText = "Product Supplier ID";
            gridProductSuppliers.Columns[2].HeaderText = "Product Name";
            gridProductSuppliers.Columns[3].HeaderText = "Supplier Name";
            gridProductSuppliers.Columns[1].Width = 150;
            gridProductSuppliers.Columns[2].Width = 300;
            gridProductSuppliers.Columns[3].Width = 300;
            gridProductSuppliers.ClearSelection();

            ResetPrdSupplierPage();
            ResetProductList();
            ResetSupplierList();

            dtpPkgStrt.Enabled = false;
            dtpPkgEndDate.Enabled = false;

        }

        private void lstPkg_SelectedIndexChanged(object sender, EventArgs e)
        {
            PackageListLoad();



            PackProductUpdate();
            lblError.Text = "";
        }


        private void PackProductUpdate()
        {
            var prodpacklistupdate = Packages_Products_SuppliersDB.GetPackProductsSuppliers();

            var listProdPack = from prod in prodpacklistupdate
                               where prod.PackageId == Convert.ToInt32(txtpkgID.Text)
                               select new { prod.ProdName, prod.SupName };

            listProdPack.ToList();
            gridprdpkg.DataSource = listProdPack.ToList();
            gridprdpkg.Columns[0].HeaderText = "Product Name";
            gridprdpkg.Columns[1].HeaderText = "Supplier Name";
            gridprdpkg.Columns[0].Width = 100;
            gridprdpkg.Columns[1].Width = 200;

        }
        // Adding Packages  
        public void pkgADD_Click(object sender, EventArgs e)
        {

            txtPkgName.ReadOnly = false;
            txtPkgName.Text = "";
            dtpPkgStrt.Enabled = true;
            dtpPkgEndDate.Value.ToShortDateString();
            dtpPkgStrt.Value.ToShortDateString();
            dtpPkgEndDate.Enabled = true;
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
            lstPkg.Enabled = false;
            pkgADD.Enabled = false;
            pkgProductAdd.Enabled = false;
            pkgProductDelete.Enabled = false;
            lstPkg.Visible = false;
            lblavalpkg.Visible = false;
            lblError.Text = "";

        }
        // saving Added Packges **NEED VALIDATION OF DATA
        private void pkgSave_Click(object sender, EventArgs e)
        {
            if
                         (
                            Validation.IsPresent(txtPkgName, "Package Name", lblError) &&

                            Validation.IsPresent(txtBasePrice, "Base Price", lblError) &&
                            Validation.NotNegativeDeciaml(txtBasePrice, "Base Price", lblError) &&

                            Validation.IsPresent(txtCommission, "Commission", lblError) &&
                            Validation.NotNegativeDeciaml(txtCommission, "Commission", lblError) &&

                            Validation.IsPresent(txtDesc, "Description", lblError)
                          )

            {

                lblError.Text = "";
                //JARED - not really sure what this does

                DateTime? StartDate = string.IsNullOrWhiteSpace(dtpPkgStrt.Text)
                          ? (DateTime?)null
                          : DateTime.Parse(dtpPkgStrt.Text);

                DateTime? EndDate = string.IsNullOrWhiteSpace(dtpPkgEndDate.Text)
                  ? (DateTime?)null
                  : DateTime.Parse(dtpPkgEndDate.Text);

                var duration =( EndDate.Value - StartDate.Value).TotalDays;
               
               if (duration < 0 | duration == 0)
                {
                    MessageBox.Show("Package End Date must be greater than Package Start Date");
                    return;
                }


          
            
                if (Convert.ToDecimal(txtCommission.Text.Replace("$", "")) >= Convert.ToDecimal(txtBasePrice.Text.Replace("$", "")))
                {
                    lblError.Text = "Commission Should be less than Package Price";
                    return;
                }


                Packages pckAdd = new Packages();
                pckAdd.PkgName = txtPkgName.Text;
                pckAdd.PkgStartDate = StartDate;
                pckAdd.PkgEndDate = EndDate;
                pckAdd.PkgDesc = txtDesc.Text;
                pckAdd.PkgBasePrice = Convert.ToDecimal(txtBasePrice.Text.Replace("$", ""));
                pckAdd.PkgAgencyCommission = Convert.ToDecimal(txtCommission.Text.Replace("$", ""));


                PackagesDB.AddPackage(pckAdd);

                lstPkg.Items.Clear();

                List<Packages> packlstAdd = PackagesDB.GetPackages();

                //MessageBox.Show("Packages has been Added");

                foreach (var pkg in packlstAdd)
                {

                    lstPkg.Items.Add(pkg.PkgName);
                }

                lstPkg.SelectedIndex = 0;
                lstPkg.Enabled = true;

                lstPkg.Items.Clear();
                List<Packages> packupdated = PackagesDB.GetPackages();

                foreach (var pkg in packupdated)
                {

                    lstPkg.Items.Add(pkg.PkgName);
                }

                lstPkg.SelectedIndex = 0;

                ResetPackage();

            }
        }
        // EDIT Packages 
        private void pkgEdit_Click(object sender, EventArgs e)
        {
            if (
                Validation.IsListSelected(lstPkg, "Available Packages", lblError)
                )
            {
                txtPkgName.ReadOnly = false;
                dtpPkgStrt.Enabled = true;
                dtpPkgEndDate.Enabled = true;
                txtBasePrice.ReadOnly = false;
                txtBasePrice.Text= txtBasePrice.Text.Replace("$", "");
                txtCommission.ReadOnly = false;
                txtCommission.Text = txtCommission.Text.Replace("$", "");
                txtDesc.ReadOnly = false;


                txtCommission.ReadOnly = false;

                pkgADD.Enabled = false;
                pkgdelete.Enabled = false;
                pkgSave.Enabled = false;
                pkgCancel.Enabled = true;
                saveEdit.Visible = true;
                pkgProductAdd.Enabled = true;
                pkgProductDelete.Enabled = true;
                lblError.Text = "";
            }
        }
        // Save Edit Packages ** Need Validation of Data
        private void saveEdit_Click(object sender, EventArgs e)
        {
            if
             (
                Validation.IsPresent(txtPkgName, "Package Name", lblError) &&


                Validation.IsPresent(txtBasePrice, "Base Price", lblError) &&
                Validation.NotNegativeDeciaml(txtBasePrice, "Base Price", lblError) &&

                Validation.IsPresent(txtCommission, "Commission", lblError) &&
                Validation.NotNegativeDeciaml(txtCommission, "Commission", lblError) &&

                Validation.IsPresent(txtDesc, "Description", lblError)
              )

            {

                var pack = PackagesDB.GetPackages();
                Packages oldPck = new Packages();



                foreach (var item in pack)
                {
                    if (item.PackageId == Convert.ToInt32(txtpkgID.Text))
                    {

                        oldPck.PackageId = item.PackageId;
                        oldPck.PkgName = item.PkgName;
                        oldPck.PkgStartDate = item.PkgStartDate;
                        oldPck.PkgEndDate = item.PkgEndDate;
                        oldPck.PkgDesc = item.PkgDesc;
                        oldPck.PkgBasePrice = item.PkgBasePrice;
                        oldPck.PkgAgencyCommission = item.PkgAgencyCommission;



                    }


                }
                DateTime? StartDate = string.IsNullOrWhiteSpace(dtpPkgStrt.Text)
                        ? (DateTime?)null
                        : DateTime.Parse(dtpPkgStrt.Text);

                DateTime? EndDate = string.IsNullOrWhiteSpace(dtpPkgEndDate.Text)
                  ? (DateTime?)null
                  : DateTime.Parse(dtpPkgEndDate.Text);

                var duration = (EndDate.Value - StartDate.Value).TotalDays;

                if (duration < 0 | duration == 0)
                {
                    MessageBox.Show("Package End Date must be greater than Package Start Date");
                    return;
                }

                if (Convert.ToDecimal(txtCommission.Text.Replace("$", "")) >= Convert.ToDecimal(txtBasePrice.Text.Replace("$", "")))
                {
                    MessageBox.Show("Commission Should be less than Package Price");
                    return;
                }


                Packages updtPck = new Packages();
                updtPck.PackageId = Convert.ToInt32(txtpkgID.Text);
                updtPck.PkgName = txtPkgName.Text;
                updtPck.PkgStartDate = StartDate;
                updtPck.PkgEndDate = EndDate;
                updtPck.PkgDesc = txtDesc.Text;

                updtPck.PkgBasePrice = Convert.ToDecimal(txtBasePrice.Text.Replace("$", ""));
                updtPck.PkgAgencyCommission = Convert.ToDecimal(txtCommission.Text.Replace("$", ""));


                PackagesDB.UpdatePackage(oldPck, updtPck);
                //MessageBox.Show("Packages has been updated");
                lstPkg.Items.Clear();
                List<Packages> packupdated = PackagesDB.GetPackages();

                foreach (var pkg in packupdated)
                {

                    lstPkg.Items.Add(pkg.PkgName);
                }

                lstPkg.SelectedIndex = 0;


                ResetPackage();
            }
        }
        // Deleting Packages from Package ** Need Validation of Data
        private void pkgdelete_Click(object sender, EventArgs e)
        {
            Packages pkgDel = new Packages();
            Packages_Products_Suppliers pkgPrdDel = new Packages_Products_Suppliers();
            List<Packages> pack = PackagesDB.GetPackages();

            if (
                Validation.IsListSelected(lstPkg, "Available Packages", lblError)
                )

            {

                if (MessageBox.Show("Delete Package?", "Delete Confirmation", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    lblError.Text = "";
                    foreach (var item in pack)
                    {
                        if (item.PackageId == Convert.ToInt32(txtpkgID.Text)) // txt box is one number ahead of newly added items
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


                    var productSupplierid = from item in Products_SuppliersDB.GetProductsSuppliers()
                                            where item.PackageId == Convert.ToInt32(txtpkgID.Text)
                                            select new { item.ProductSupplierId };


                    Packages_Products_Suppliers pkgDelPro = new Packages_Products_Suppliers();

                    var id = productSupplierid.ToList();

                    foreach (var item in id)
                    {
                        pkgDelPro.ProductSupplierId = item.ProductSupplierId;
                    }
                    pkgDelPro.PackageId = Convert.ToInt32(txtpkgID.Text);

                    Packages_Products_SuppliersDB.DeletePackagePro(pkgDelPro);



                    PackagesDB.DeletePackage(pkgDel);
                    lstPkg.Items.Clear();




                    List<Packages> packupdatedel = PackagesDB.GetPackages();
                    foreach (var pkg in packupdatedel)
                    {
                        lstPkg.Items.Add(pkg.PkgName);
                    }



                    ResetPackage();
                }

            }
        }
        private void metroTabPage1_Click(object sender, EventArgs e)
        {
            lstPkg.Enabled = true;

        }

        private void lstProducts_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        // Adding Product to Product list 
        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            btnAddSaveProd.Visible = true;
            lblProdName.Visible = true;
            txtProductName.Visible = true;

            btnEditProducts.Enabled = false;
            txtProductName.Focus();

        }
        // adding Supplier in Supplier data table 
        private void btnAddSupplier_Click(object sender, EventArgs e)
        {

            lblSupplierName.Visible = true;
            txtSupplier.Visible = true;
            btnSaveAddSupp.Visible = true;
            btnEditSupplier.Enabled = false;
            txtSupplier.Focus();

        }
        // Adding Product to Product list ** Need Validation of Data
        private void btnAddSaveProd_Click(object sender, EventArgs e)
        {
            if (Validation.IsPresent(txtProductName, "Product Name", lblProdsError))
            {
                Products prodAdd = new Products();

                prodAdd.ProdName = txtProductName.Text;


          
            List<int> prod = ProductsDB.GetProductID();
            List<string> product = new List<string>();

            foreach (var prd in prod)
            {
                product.Add(ProductsDB.GetProduct(prd).ProdName);
            }
            comboProduct.DataSource = null;
            comboProduct.DataSource = product;


            List<int> suppupdated = SuppliersDB.GetSupplierIDs();
            List<string> supplier = new List<string>();

            foreach (var sup in suppupdated)
            {
                supplier.Add(SuppliersDB.GetSupplier(sup).SupName);
            }

            comboSupplier.DataSource = null;
            comboSupplier.DataSource = supplier;

                ProductsDB.AddProduct(prodAdd);


                gridProductSuppliers.DataSource = Products_SuppliersDB.GetProductsSuppliers();
                ResetProductList();
                ResetPrdSupplierPage();
                ResetProductSupllierList();

                txtProductName.Text = "";
                lblProdsError.Text = "";
            }

        }
        // Adding Supplier to supplier table  ** Need Validation of Data
        private void btnSaveAddSupp_Click(object sender, EventArgs e)
        {
            if (Validation.IsPresent(txtSupplier, "Supplier Name", lblSuppError))
            {
                Suppliers suppAdd = new Suppliers();


            List<int> supp = SuppliersDB.GetSupplierIDs();
            var newsuppindex = supp[supp.Count - 1] + 1;


                suppAdd.SupplierID = newsuppindex;

                suppAdd.SupName = txtSupplier.Text;

                SuppliersDB.AddSupplier(suppAdd);

                //MessageBox.Show("Supplier has been Added");

                gridProductSuppliers.DataSource = Products_SuppliersDB.GetProductsSuppliers();


            List<int> suppupdated = SuppliersDB.GetSupplierIDs();
            List<string> supplier = new List<string>();

            foreach (var sup in suppupdated)
            {
                supplier.Add(SuppliersDB.GetSupplier(sup).SupName);
            }

            comboSupplier.DataSource = null;
            comboSupplier.DataSource = supplier;


            List<int> prod = ProductsDB.GetProductID();
            List<string> product = new List<string>();

            foreach (var prd in prod)
            {
                product.Add(ProductsDB.GetProduct(prd).ProdName);
            }
            comboProduct.DataSource = null;
            comboProduct.DataSource = product;

                ResetSupplierList();
                ResetPrdSupplierPage();
                ResetProductSupllierList();

                txtSupplier.Text = "";
                lblSuppError.Text = "";
            }
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
            btnAddProduct.Visible = true;
            btnAddProduct.Visible = true;


        }


        public void ResetSupplierList()
        {
            lstSupplier.Items.Clear();

            List<int> supp1 = SuppliersDB.GetSupplierIDs();

            foreach (var supItem in supp1)
            {
                lstSupplier.Items.Add(SuppliersDB.GetSupplier(supItem).SupName);
            }

            btnAddSupplier.Visible = true;
            btnAddSupplier.Enabled = true;
            btnEditSupplier.Visible = true;
            btnEditSupplier.Enabled = true;

            lblSupplierName.Visible = false;
            txtSupplier.Visible = false;
            btnSaveAddSupp.Visible = false;
            btnUpdateSupplier.Visible = false;
            lstSupplier.Enabled = true;
        }

        private void btnResetSupplier_Click(object sender, EventArgs e)
        {
            ResetSupplierList();
            txtSupplier.Text = "";
            lblSuppError.Text = "";

        }


        // Editing Products in products Table

        private void btnResetProduct_Click(object sender, EventArgs e)
        {
            ResetProductList();
            ResetPrdSupplierPage();
            txtProductName.Text = "";
        }
        // Editing Products in Products table  ** Need Validation of Data
        private void btnEditProducts_Click(object sender, EventArgs e)
        {
            if (Validation.IsListSelected(lstProducts, "Products", lblProdsError))
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
        //  Editing Supplier in Supplier  Table ** Need Validation of Data
        private void btnEditSupplier_Click(object sender, EventArgs e)
        {
            if (Validation.IsListSelected(lstSupplier, "Suppliers", lblSuppError))

            {
                
                txtSupplier.Visible = true;
                btnUpdateSupplier.Visible = true;
                lblSupplierName.Visible = true;
                btnAddSupplier.Enabled = false;
                lstSupplier.Enabled = false;

                txtSupplier.Focus();
                txtSupplier.Text = lstSupplier.SelectedItem.ToString();




            }
        }

        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {

            if(Validation.IsPresent(txtProductName, "Product Name", lblProdsError))
            { 
                updateProductname(lstProducts.SelectedItem.ToString(), txtProductName.Text);

                gridProductSuppliers.DataSource = Products_SuppliersDB.GetProductsSuppliers();
                ResetPrdSupplierPage();
                ResetProductSupllierList();

                ResetProductList();

	            List<int> prod = ProductsDB.GetProductID();
	            List<string> product = new List<string>();

	            foreach (var prd in prod)
	            {
	                product.Add(ProductsDB.GetProduct(prd).ProdName);
	            }
	            comboProduct.DataSource = null;
	            comboProduct.DataSource = product;

	            List<int> suppupdated = SuppliersDB.GetSupplierIDs();
	            List<string> supplier = new List<string>();

	            foreach (var sup in suppupdated)
	            {
	                supplier.Add(SuppliersDB.GetSupplier(sup).SupName);
	            }

	            comboSupplier.DataSource = null;
	            comboSupplier.DataSource = supplier;

                txtProductName.Text = "";
                lblProdsError.Text = "";
            }

        }


        private void btnUpdateSupplier_Click(object sender, EventArgs e)
        {
          	
			if (Validation.IsPresent(txtSupplier, "Supplier Name", lblSuppError))
            {
	            updateSupplierName(lstSupplier.SelectedItem.ToString(), txtSupplier.Text);
	            gridProductSuppliers.DataSource = Products_SuppliersDB.GetProductsSuppliers();
	            ResetPrdSupplierPage();
	            ResetProductSupllierList();
	            ResetSupplierList();

	            List<int> suppupdated = SuppliersDB.GetSupplierIDs();
	            List<string> supplier = new List<string>();

	            foreach (var sup in suppupdated)
	            {
	                supplier.Add(SuppliersDB.GetSupplier(sup).SupName);
	            }
	            comboSupplier.DataSource = supplier;



	            List<int> prod = ProductsDB.GetProductID();
	            List<string> product = new List<string>();

	            foreach (var prd in prod)
	            {
	                product.Add(ProductsDB.GetProduct(prd).ProdName);
	            }
	            comboProduct.DataSource = null;
	            comboProduct.DataSource = product;

                txtSupplier.Text = "";
                lblSuppError.Text = "";
            }
        }



        private void saveProdSup_Click(object sender, EventArgs e)
        {
            ResetPrdSupplierPage();

            try
            {
                Products_Suppliers addPrdSupp = new Products_Suppliers();

                List<int> prod = ProductsDB.GetProductID();
                List<int> supp = SuppliersDB.GetSupplierIDs();

                foreach (var item in prod)
                {

                    //var newproductSupplierID = Convert.ToInt32(gridProductSuppliers.Rows[gridProductSuppliers.RowCount - 1]);
                    //newproductSupplierID = gridProductSuppliers.Rows[newproductSupplierID].Cells[0].Value;

                    if (comboProduct.SelectedItem.ToString() == ProductsDB.GetProduct(item).ProdName)
                    {
                        addPrdSupp.ProductId = item;
                    }

                }

                foreach (var item in supp)
                {


                    if (comboSupplier.SelectedItem.ToString() == SuppliersDB.GetSupplier(item).SupName)
                    {
                        addPrdSupp.SupplierId = item;
                    }

                }

                Products_SuppliersDB.AddProdSupplier(addPrdSupp);
                //MessageBox.Show("Product Supplier Added");
                gridProductSuppliers.DataSource = null;
                gridProductSuppliers.DataSource = Products_SuppliersDB.GetProductsSuppliers();
                gridProductSuppliers.Columns[0].Visible = false;
                gridProductSuppliers.Columns[1].HeaderText = "Product Supplier ID";
                gridProductSuppliers.Columns[2].HeaderText = "Product Name";
                gridProductSuppliers.Columns[3].HeaderText = "Supplier Name";
                gridProductSuppliers.Columns[1].Width = 150;
                gridProductSuppliers.Columns[2].Width = 300;
                gridProductSuppliers.Columns[3].Width = 300;
                ResetProductList();
                ResetSupplierList();
                ResetPrdSupplierPage();
                ResetProductSupllierList();

            }

            catch
            {
                MessageBox.Show("Same Product Supplier Already Exist");
            }




        }
        public void updateProductname(string prdName, string updatedprdName)
        {
            List<int> prod = ProductsDB.GetProductID();
            foreach (var item in prod)
            {

                if (prdName == ProductsDB.GetProduct(item).ProdName)
                {
                    Products updatedProdcut = new Products();

                    updatedProdcut.ProductID = item;
                    updatedProdcut.ProdName = updatedprdName;


                    ProductsDB.UpdateProduct(item, updatedProdcut);
                    //MessageBox.Show("Product Updated");



                }

            }
        }

        public void updateSupplierName(string supplierName,string updatedsupplierName)
        {   List<int> supp = SuppliersDB.GetSupplierIDs();

            foreach (var item in supp)
            {

                if (supplierName == SuppliersDB.GetSupplier(item).SupName)
                {
                    Suppliers updatedSupplier = new Suppliers();


                    updatedSupplier.SupplierID = item;
                    updatedSupplier.SupName = updatedsupplierName;

                    SuppliersDB.UpdateSupplier(item, updatedSupplier);


                    //MessageBox.Show("Supplier Updated");

                }


            }
        }

        // Editing Product_supplier table 

        private void btnEditAddProductSupplier_Click(object sender, EventArgs e)
        {

                try
            {


                var supplierproductID = gridProductSuppliers.CurrentRow.Cells[1].Value.ToString();
                txtPrdSupPrdName.Text = gridProductSuppliers.CurrentRow.Cells[2].Value.ToString();
                txtAddSupSupName.Text = gridProductSuppliers.CurrentRow.Cells[3].Value.ToString();

                addProdSupp.Visible = true;
                btnEditAddProductSupplier.Enabled = true;
                btnDeleteProdSupplier.Enabled = false;
                lblProductName.Visible = true;
                lblProductSupplierName.Visible = true;
                txtAddSupSupName.Visible = true;
                txtPrdSupPrdName.Visible = true;
                saveProdSup.Visible = false;
                btnAddPrdSaveEdit.Visible = true;
                comboProduct.Visible = false;
                comboSupplier.Visible = false;
                resetPrdSup.Enabled = true;



            }

            catch
            {
                MessageBox.Show("Please Select an item ");

                ResetPrdSupplierPage();
            }

        }
        // Editing Product_supplier table ** Need Validation of Data
        private void btnAddPrdSaveEdit_Click(object sender, EventArgs e)
        {

            updateProductname(gridProductSuppliers.CurrentRow.Cells[2].Value.ToString(), txtPrdSupPrdName.Text);
            updateSupplierName(gridProductSuppliers.CurrentRow.Cells[3].Value.ToString(), txtAddSupSupName.Text);
            //MessageBox.Show("Data Updated");

            gridProductSuppliers.DataSource = Products_SuppliersDB.GetProductsSuppliers();
            ResetProductList();
            ResetSupplierList();
            ResetProductSupllierList();
            ResetPrdSupplierPage();
        }

        private void comboPrdPack_SelectedIndexChanged(object sender, EventArgs e)
        {
            listSuppPkg.Items.Clear();


            foreach (var supname in productSupplierList)
            {
                if (supname.ProdName == comboPrdPack.SelectedItem.ToString())
                {
                    listSuppPkg.Items.Add(supname.SupName);

                }
            }
        }

        private void pkgProductAdd_Click(object sender, EventArgs e)

        {

            if (
                Validation.IsListSelected(lstPkg, "Available Packages", lblProdError) &&
                Validation.IsListSelected(listSuppPkg, "Suppliers", lblProdError)
                )
            {
                lblProdError.Text = "";
                PackProductUpdate();
                try
                {
                    var productSupplierid = from item in productSupplierList
                                            where item.ProdName == comboPrdPack.SelectedItem.ToString() && item.SupName == listSuppPkg.SelectedItem.ToString()
                                            select new { item.ProductSupplierId };


                        Packages_Products_Suppliers pkgAddPro = new Packages_Products_Suppliers();

                        var id = productSupplierid.ToList();

                        foreach (var item in id)
                        {
                            pkgAddPro.ProductSupplierId = item.ProductSupplierId;
                        }
                        pkgAddPro.PackageId = Convert.ToInt32(txtpkgID.Text);

                        Packages_Products_SuppliersDB.AddPackageProduct(pkgAddPro);

                        //MessageBox.Show("Proucts Added");

                        PackProductUpdate();

                    
                }

                catch
                {

                    MessageBox.Show("Product Already in the Packages OR Product/Suppleir not Selected for Add");
                }
            }
        }


        // Deleting Product From Packages
        private void pkgProductDelete_Click(object sender, EventArgs e)
        {
            if (
                Validation.IsListSelected(lstPkg, "Available Packages", lblProdError)
                )
            {
                if (MessageBox.Show("Delete Package?", "Delete Confirmation", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    lblProdError.Text = "";

                    var pkgProductName = gridprdpkg.CurrentRow.Cells[0].Value.ToString();
                    var pkgSupplierName = gridprdpkg.CurrentRow.Cells[1].Value.ToString();


                    var pkgProductSupplierId = from item in productSupplierList
                                               where item.ProdName == pkgProductName && item.SupName == pkgSupplierName
                                               select new { item.ProductSupplierId };

                    Packages_Products_Suppliers pkgDeletePro = new Packages_Products_Suppliers();

                    var id = pkgProductSupplierId.ToList();

                    foreach (var item in id)
                    {
                        pkgDeletePro.ProductSupplierId = item.ProductSupplierId;
                    }

                    pkgDeletePro.PackageId = Convert.ToInt32(txtpkgID.Text);

                    Packages_Products_SuppliersDB.DeletePackageProSupplier(pkgDeletePro);



                    PackProductUpdate();
                }
            }



            //  PackProductUpdate();


        }





        private void pkgCancel_Click(object sender, EventArgs e)
        {
            ResetPackage();
        }

        public void ResetPackage()
        {
            pkgADD.Enabled = true;
            pkgEdit.Enabled = true;
            pkgdelete.Enabled = true;
            pkgSave.Enabled = false;
            saveEdit.Visible = false;
            pkgCancel.Enabled = false;
            lstPkg.Enabled = true;
            lstPkg.SelectedIndex = 0;
            pkgProductAdd.Enabled = true;
            pkgProductDelete.Enabled = true;
            lstPkg.Visible = true;
            lblavalpkg.Visible = true;

            txtPkgName.ReadOnly = true;

            dtpPkgStrt.Enabled = true;


            dtpPkgEndDate.Enabled = true;
            txtBasePrice.ReadOnly = true;

            txtCommission.ReadOnly = true;
            txtDesc.ReadOnly = true;

            txtCommission.ReadOnly = true;

            lblError.Text = "";



            PackageListLoad();



        }

        public void PackageListLoad()
        {

            var pack = PackagesDB.GetPackages();



            foreach (var item in pack)
            {

                if (item.PkgName == lstPkg.SelectedItem.ToString())
                {

                    var pkgId = item.PackageId;
                    txtPkgName.Text = item.PkgName;
                    txtpkgID.Text = item.PackageId.ToString();
                    txtDesc.Text = item.PkgDesc;
                    txtBasePrice.Text = item.PkgBasePrice.ToString("c");
                    txtCommission.Text = item.PkgAgencyCommission.ToString("c");

                    if (item.PkgStartDate.HasValue)
                    {
                        dtpPkgStrt.Text = item.PkgStartDate.Value.ToShortDateString();
                    }
                    else
                    {
                        dtpPkgStrt.Format = DateTimePickerFormat.Short;
                    }


                    if (item.PkgEndDate.HasValue)
                    {
                        dtpPkgEndDate.Text = item.PkgEndDate.Value.ToShortDateString();
                    }
                    else
                    {
                        dtpPkgEndDate.Format = DateTimePickerFormat.Short;
                    }

                }




            }
        }

        private void resetPrdSup_Click(object sender, EventArgs e)
        {


            ResetPrdSupplierPage();


        }

        public void ResetPrdSupplierPage()
        {
            addProdSupp.Visible = true;
            btnEditAddProductSupplier.Visible = true;
            btnDeleteProdSupplier.Visible = true;

            btnEditAddProductSupplier.Enabled = true;
            btnDeleteProdSupplier.Enabled = true;
            lblProductName.Visible = false;
            lblProductSupplierName.Visible = false;
            txtAddSupSupName.Visible = false;
            txtPrdSupPrdName.Visible = false;
            saveProdSup.Visible = false;
            btnAddPrdSaveEdit.Visible = false;
            comboProduct.Visible = false;
            comboSupplier.Visible = false;
            resetPrdSup.Enabled = false;
        }

        private void addProdSupp_Click(object sender, EventArgs e)
        {
            addProdSupp.Visible = true;
            btnEditAddProductSupplier.Enabled = false;
            btnDeleteProdSupplier.Enabled = false;
            lblProductName.Visible = true;
            lblProductSupplierName.Visible = true;
            txtAddSupSupName.Visible = false;
            txtPrdSupPrdName.Visible = false;
            saveProdSup.Visible = true;
            btnAddPrdSaveEdit.Visible = false;
            comboProduct.Visible = true;
            comboSupplier.Visible = true;
            resetPrdSup.Enabled = true;
        }

        private void btnDeleteProdSupplier_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Delete Product?", "Delete Confirmation", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    var productSupplierID = gridProductSuppliers.CurrentRow.Cells[1].Value;
                Products_Suppliers itemDelete = new Products_Suppliers();
                Packages_Products_Suppliers pkgPrdDelete = new Packages_Products_Suppliers();

                pkgPrdDelete.ProductSupplierId = Convert.ToInt32(productSupplierID);
                itemDelete.ProductSupplierId = Convert.ToInt32(productSupplierID);

                Packages_Products_SuppliersDB.DeletePackageProSupplierByID(pkgPrdDelete);
                Products_SuppliersDB.DeleteProductsSuppliers(itemDelete);
                gridProductSuppliers.DataSource = Products_SuppliersDB.GetProductsSuppliers();
                ResetProductList();
                ResetSupplierList();
                ResetPrdSupplierPage();
                ResetProductSupllierList();


                }

            }
            catch
            {

                MessageBox.Show("Cannot Delete at this moment as this supplied id is linked with  booking details table.Plaese select another one to delete");
            }


        }

        public void ResetProductSupllierList()
        {
            lstPkg.Items.Clear();
            List<Packages> packupdated = PackagesDB.GetPackages();

            foreach (var pkg in packupdated)
            {

                lstPkg.Items.Add(pkg.PkgName);
            }

            lstPkg.SelectedIndex = 0;




            productSupplierList = Products_SuppliersDB.GetProductsSuppliers();
            var distinctPrd = productSupplierList.Select(o => o.ProdName).Distinct().ToList();
            comboPrdPack.DataSource = distinctPrd;
            comboProduct.DataSource = product;
            comboSupplier.DataSource = supplier;

            listSuppPkg.Items.Clear();
            foreach (var supname in productSupplierList)
            {
                if (supname.ProdName == comboPrdPack.SelectedItem.ToString())
                {
                    listSuppPkg.Items.Add(supname.SupName);

                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Close();

        }

    }
}
