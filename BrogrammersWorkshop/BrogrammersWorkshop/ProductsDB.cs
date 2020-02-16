using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrogrammersWorkshop
{
    public static class ProductsDB
    {
        //retrieving a single product data
        public static Products GetProduct(int ProductID)
        {
            //creating an object to store the orders information
            Products prd = null;
            //opening a connection to SQL and inputting a query to access the specific ProductIDs information
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string query = "SELECT ProductID, ProdName " +
                                "FROM Products " +
                                "WHERE ProductID = @ProductID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", ProductID);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        //store values into the product object
                        if (reader.Read())
                        {
                            prd = new Products();
                            prd.ProductID = (int)reader["ProductID"];
                            prd.ProdName = reader["ProdName"].ToString();
                        }

                    }
                }  
            }
            return prd;


        } // Get Product method ends.

        // getting all product IDs for the combo box
        public static List<int> GetProductID()
        {
            List<int> productIDs = new List<int>(); // empty list of IDs
            int id; // for reading
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string query = "SELECT ProductID FROM Products";
                // any exception not handled here is automatically thrown to the form
                // where the method was called
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    // close connection as soon as done with reading
                    while (reader.Read())
                    {
                        id = (int)reader["ProductID"];
                        productIDs.Add(id);
                    }
                }// command objectrecycled
            }// connection object recycled

            return productIDs;
        }

        //accessing SQL and updating product
        public static bool UpdateProduct(int productID, Products newPrd)
        {
            int count; // how many rows updated
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string updateStatement =
                    "UPDATE Products SET " +
                    " ProdName = @newProdName " +
                    "WHERE productID = @productID";

                using (SqlCommand cmd = new SqlCommand(updateStatement, connection))
                {
                    if (newPrd != null)
                    {
                        cmd.Parameters.AddWithValue("@newProdName", newPrd.ProdName);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@newProdName", DBNull.Value);
                    }
                    cmd.Parameters.AddWithValue("@productID", productID);

                    connection.Open();
                    count = cmd.ExecuteNonQuery(); // returns how many rows updated
                }
            }
            return (count > 0);
        }

        // inserts row into Products table and returns generated ProductID value
        public static int AddProduct(Products prd)
        {
            int productID = -1;
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string insertStatement =
                     "INSERT INTO Products(ProdName) " +
                     "OUTPUT inserted.ProductID " +
                     "VALUES(@ProdName)";
                using (SqlCommand cmd = new SqlCommand(insertStatement, connection))
                {
                    cmd.Parameters.AddWithValue("@ProdName", prd.ProdName);
                    connection.Open();
                    productID = (int)cmd.ExecuteScalar(); // fixes problem of retrieving ID
                }
            }
            return productID;
        }

        //public static bool DeleteProduct(Products prd)
        //{
        //    int count = 0; // how many rows deleted
        //    using (SqlConnection connection  = TravelExpertsDB.GetConnection())
        //    {
        //        string deleteStatement =
        //            "DELETE FROM Products " +
        //            "WHERE ProductID = @ProductID " + // to identify record
        //            "AND ProdName = @ProdName "; // the remaining conditions - for optimistic concurrency

        //        using (SqlCommand cmd = new SqlCommand(deleteStatement, connection))
        //        {
        //            cmd.Parameters.AddWithValue("@ProductID", prd.ProductID);
        //            cmd.Parameters.AddWithValue("@ProdName", prd.ProdName);
        //            connection.Open();
        //            count = cmd.ExecuteNonQuery(); // DELETE statement return # affected rows
        //        }

        //    }
        //    return (count > 0);
        //}



    }
       
}
