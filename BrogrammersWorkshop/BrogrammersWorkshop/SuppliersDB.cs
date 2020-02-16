using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrogrammersWorkshop
{
    public class SuppliersDB
    {
        //retrieving a single supplier data
        public static Suppliers GetSupplier(int SupplierID)
        {
            //creating an object to store the suppliers information
            Suppliers sup = null;
            //opening a connection to SQL and inputting a query to access the specific ProductIDs information
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string query = "SELECT SupplierID, SupName " +
                                "FROM Suppliers " +
                                "WHERE SupplierID = @SupplierID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SupplierID", SupplierID);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        //store values into the supplier object
                        if (reader.Read())
                        {
                            sup = new Suppliers();
                            sup.SupplierID = (int)reader["SupplierID"];
                            sup.SupName = reader["SupName"].ToString();
                        }

                    }
                }
            }
            return sup;
        }// Get Supplier method ends.

        // getting all supplier IDs for the combo box
        public static List<int> GetSupplierIDs()
        {
            List<int> supplierIDs = new List<int>(); // empty list of IDs
            int id; // for reading
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string query = "SELECT SupplierID FROM Suppliers";
                // any exception not handled here is automatically thrown to the form
                // where the method was called
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    // close connection as soon as done with reading
                    while (reader.Read())
                    {
                        id = (int)reader["SupplierID"];
                        supplierIDs.Add(id);
                    }
                }// command objectrecycled
            }// connection object recycled

            return supplierIDs;
        }
        //accessing SQL and updating product
        public static bool UpdateSupplier(int supplierID, Suppliers newSup)
        {
            int count; // how many rows updated
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string updateStatement =
                    "UPDATE Suppliers SET " +
                    "SupName = @newSupName " +
                    "WHERE supplierID = @supplierID";

                using (SqlCommand cmd = new SqlCommand(updateStatement, connection))
                {
                    if (newSup != null)
                    {
                        cmd.Parameters.AddWithValue("@newSupName", newSup.SupName);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@newSupName", DBNull.Value);
                    }
                    cmd.Parameters.AddWithValue("@supplierID", supplierID);

                    connection.Open();
                    count = cmd.ExecuteNonQuery(); // returns how many rows updated
                }
            }
            return (count > 0);
        }

        // inserts row into Suppliers table and returns generated SupplierID value
        public static int AddSupplier(Suppliers sup)
        {
            int supplierID = -1;
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string insertStatement =
                     "INSERT INTO Suppliers(SupplierId, SupName) " +
                     "OUTPUT inserted.SupplierID " +
                     "VALUES(@SupplierId, @SupName)";
                using (SqlCommand cmd = new SqlCommand(insertStatement, connection))
                {
                    cmd.Parameters.AddWithValue("@SupplierId", sup.SupplierID);
                    cmd.Parameters.AddWithValue("@SupName", sup.SupName);
                    connection.Open();
                    supplierID = (int)cmd.ExecuteScalar(); // fixes problem of retrieving ID
                }
            }
            return supplierID;
        }

        //public static bool DeleteSupplier(Suppliers sup)
        //{
        //    int count = 0; // how many rows deleted
        //    using (SqlConnection connection = TravelExpertsDB.GetConnection())
        //    {
        //        string deleteStatement =
        //            "DELETE FROM Suppliers " +
        //            "WHERE SupplierID = @SupplierID " + // to identify record
        //            "AND SupName = @SupName "; // the remaining conditions - for optimistic concurrency
        //        using (SqlCommand cmd = new SqlCommand(deleteStatement, connection))
        //        {
        //            cmd.Parameters.AddWithValue("@SupplierID", sup.SupplierID);
        //            cmd.Parameters.AddWithValue("@SupName", sup.SupName);
        //            connection.Open();
        //            count = cmd.ExecuteNonQuery(); // DELETE statement return # affected rows
        //        }

        //    }
        //    return (count > 0);
        //}

    }
}

