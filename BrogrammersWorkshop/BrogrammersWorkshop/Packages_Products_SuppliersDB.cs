using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrogrammersWorkshop
{
    public class Packages_Products_SuppliersDB
    {
        //retrieve a list of packages products and suppliers for the table
        public static List<PackagesProductInfo> GetPackProductsSuppliers()
        {
            List<PackagesProductInfo> packProdSup = new List<PackagesProductInfo>(); //empty list
            PackagesProductInfo pkgInfo; // aux for reading

            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string query = "SELECT PackageId, SupName, ProdName " +
                               "FROM Packages_Products_Suppliers AS pps JOIN Products_Suppliers AS ps " +
                               "ON pps.ProductSupplierId = ps.ProductSupplierId " +
                               "join Suppliers AS sup " +
                               "ON ps.SupplierId = sup.SupplierId " +
                               "join Products AS p " +
                               "ON ps.ProductId = p.ProductId " +
                               "ORDER BY PackageId";
                
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read())
                    {
                        pkgInfo = new PackagesProductInfo();
                        pkgInfo.PackageId = (int)reader["PackageId"];
                        pkgInfo.SupName = reader["SupName"].ToString();
                        pkgInfo.ProdName = reader["ProdName"].ToString();
                        packProdSup.Add(pkgInfo);
                    }
                }
            }
            return packProdSup;
        }
        // retrieve a single package product and supplier data





        public static Packages_Products_Suppliers GetPackProdSup(int PackageID)
        {
            //creating the object to store the orders information
            Packages_Products_Suppliers pkgProdSup = null;
            //opening a connection to SQL and inputting a query to access the specific orderIDs information
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string query = "SELECT * " +
                                "FROM Packages " +
                                "WHERE PackageID = @PackageID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PackageID", PackageID);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        //store values into the order object
                        if (reader.Read())
                        {
                            pkgProdSup = new Packages_Products_Suppliers();
                            pkgProdSup.PackageId = (int)reader["PackageId"];
                            pkgProdSup.ProductSupplierId = (int)reader["ProductSupplierId"];
                        }
                    }
                }
            }
            return pkgProdSup;
        }// Get Packages and suppliers method completed

        public static void  AddPackageProduct(Packages_Products_Suppliers pkgPrdSup)
        {
            
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string insertStatement =
                     "INSERT INTO Packages_Products_Suppliers(PackageId, ProductSupplierId) " +
                     "VALUES(@PackageId, @ProductSupplierId)";
                using (SqlCommand cmd = new SqlCommand(insertStatement, connection))
                {
                    cmd.Parameters.AddWithValue("@PackageId", pkgPrdSup.PackageId);
                    cmd.Parameters.AddWithValue("@ProductSupplierId", pkgPrdSup.ProductSupplierId);
                    connection.Open();
                    cmd.ExecuteScalar(); // fixes problem of retrieving ID
                }
            }
            
        }

        public static bool DeletePackagePro(Packages_Products_Suppliers pkg)
        {
            int count = 0; // how many rows deleted
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string deleteStatement =
                    "DELETE FROM Packages_Products_Suppliers " +
                    "WHERE PackageId = @PackageId "; 

                using (SqlCommand cmd = new SqlCommand(deleteStatement, connection))
                {
                    cmd.Parameters.AddWithValue("@PackageId", pkg.PackageId);
                    connection.Open();
                    count = cmd.ExecuteNonQuery(); // DELETE statement return # affected rows
                }
            }
            return (count > 0);
        }



    }
}