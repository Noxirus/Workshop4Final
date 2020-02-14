using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrogrammersWorkshop
{
    //get all packages for a list and return the list
    public class PackagesDB
    {
        public static List<Packages> GetPackages()
        {
            List<Packages> packages = new List<Packages>(); //empty list
            Packages pkg; // aux for reading

            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string query = "SELECT * " +
                               "FROM Packages " +
                               "ORDER BY PackageId";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read())
                    {
                        pkg = new Packages();
                        pkg.PackageId = (int)reader["PackageId"];
                        pkg.PkgName = reader["PkgName"].ToString();
                        pkg.PkgStartDate = (DateTime)reader["PkgStartDate"];
                        pkg.PkgEndDate = (DateTime)reader["PkgEndDate"];
                        pkg.PkgDesc = reader["PkgDesc"].ToString();
                        pkg.PkgBasePrice = (decimal)reader["PkgBasePrice"];
                        pkg.PkgAgencyCommission = (decimal)reader["PkgAgencyCommission"];
                        packages.Add(pkg);
                    }
                }
            }
            return packages;
        }
// retrieve a single packages data
public static Packages GetPackage(int PackageID)
        {
            //creating the object to store the orders information
            Packages pkg = null;
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
                            pkg = new Packages();
                            pkg.PackageId = (int)reader["PackageId"];
                            pkg.PkgName = reader["PkgName"].ToString();
                            //taking in account potential null values for dates
                            if (reader["PkgStartDate"] == DBNull.Value)
                            {
                                pkg.PkgStartDate = null;
                            }
                            else
                            {
                                pkg.PkgStartDate = Convert.ToDateTime(reader["PkgStartDate"]);
                            }
                            if (reader["PkgEndDate"] == DBNull.Value)
                            {
                                pkg.PkgEndDate = null;
                            }
                            else
                            {
                                pkg.PkgEndDate = Convert.ToDateTime(reader["PkgEndDate"]);
                            }
                            pkg.PkgDesc = reader["PkgDesc"].ToString();
                            pkg.PkgBasePrice = Convert.ToDecimal(reader["PkgBasePrice"]);
                            pkg.PkgAgencyCommission = Convert.ToDecimal(reader["PkgAgencyCommission"]);
                        }
                    }
                }
            }
            return pkg;
        }// Get Package method completed
        //Adding package
        public static int AddPackage(Packages pkg)
        {
            int packageID = -1;
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string insertStatement =
                    "INSERT INTO Packages(PkgName, PkgStartDate, PkgEndDate, PkgDesc, PkgBasePrice, PkgAgencyCommission) " +
                    "OUTPUT inserted.PackageID " +
                    "VALUES(@PkgName, @PkgStartDate, @PkgEndDate, @PkgDesc, @PkgBasePrice, @PkgAgencyCommission)";
                using (SqlCommand cmd = new SqlCommand(insertStatement, connection))
                {
                    cmd.Parameters.AddWithValue("@PkgName", pkg.PkgName);
                    cmd.Parameters.AddWithValue("@PkgStartDate", pkg.PkgStartDate);
                    cmd.Parameters.AddWithValue("@PkgEndDate", pkg.PkgEndDate);
                    cmd.Parameters.AddWithValue("@PkgDesc", pkg.PkgDesc);
                    cmd.Parameters.AddWithValue("@PkgBasePrice", pkg.PkgBasePrice);
                    cmd.Parameters.AddWithValue("@PkgAgencyCommission", pkg.PkgAgencyCommission);
                    connection.Open();
                    //cmd.ExecuteNonQuery(); // INSERT statement
                    packageID = (int)cmd.ExecuteScalar(); // fixes problem of retrieving ID
                    //Thread.Sleep(10000);
                    // retrieve generated customerID
                    //string secondQuery = "SELECT IDENT_CURRENT('Customers')"; // most recent value  generated for identity column
                    //SqlCommand secondCmd = new SqlCommand(secondQuery, connection);
                    //customerID = Convert.ToInt32(secondCmd.ExecuteScalar());
                }
            }
            return packageID;
        }
        //delete a package
        public static bool DeletePackage(Packages pkg)
        {
            int count = 0; // how many rows deleted
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string deleteStatement =
                    "DELETE FROM Packages " +
                    "WHERE PackageId = @PackageId " + // to identify record
                    "AND PkgName = @PkgName " + // the remaining conditions - for optimistic concurrency
                    "AND PkgStartDate = @PkgStartDate " +
                    "AND PkgEndDate = @PkgEndDate " +
                    "AND PkgDesc = @PkgDesc " +
                    "AND PkgBasePrice = @PkgBasePrice " +
                    "AND PkgAgencyCommission = @PkgAgencyCommission";

                using (SqlCommand cmd = new SqlCommand(deleteStatement, connection))
                {
                    cmd.Parameters.AddWithValue("@PackageId", pkg.PackageId);
                    cmd.Parameters.AddWithValue("@PkgName", pkg.PkgName);
                    cmd.Parameters.AddWithValue("@PkgStartDate", pkg.PkgStartDate);
                    cmd.Parameters.AddWithValue("@PkgEndDate", pkg.PkgEndDate);
                    cmd.Parameters.AddWithValue("@PkgDesc", pkg.PkgDesc);
                    cmd.Parameters.AddWithValue("@PkgBasePrice", pkg.PkgBasePrice);
                    cmd.Parameters.AddWithValue("@PkgAgencyCommission", pkg.PkgAgencyCommission);
                    connection.Open();
                    count = cmd.ExecuteNonQuery(); // DELETE statement return # affected rows
                }
            }
            return (count > 0);
        }
        //updating a package
        public static bool UpdatePackage(Packages oldPkg, Packages newPkg)
        {
            int count; // how many rows updated
            using (SqlConnection connection = TravelExpertsDB.GetConnection())
            {
                string updateStatement =
                    "UPDATE Packages SET " +
                    "PkgName = @newPkgName, " +
                    "PkgStartDate = @newPkgStartDate, " +
                    "PkgEndDate = @newPkgEndDate, " +
                    "PkgDesc = @newPkgDesc, " +
                    "PkgBasePrice = @newPkgBasePrice, " +
                    "PkgAgencyCommission = @newPkgAgencyCommission " +
                    "WHERE PackageId = @oldPackageId " + // to identify record
                    "AND PkgName = @oldPkgName "+ // remainig conditions - otimistic concurrency
                    "AND PkgStartDate = @oldPkgStartDate " +
                    "AND PkgEndDate = @oldPkgEndDate " +
                    "AND PkgDesc = @oldPkgDesc " +
                    "AND PkgBasePrice = @oldPkgBasePrice " +
                    "AND PkgAgencyCommission = @oldPkgAgencyCommission";
                using (SqlCommand cmd = new SqlCommand(updateStatement, connection))
                {
                    cmd.Parameters.AddWithValue("@newPkgName", newPkg.PkgName);
                    cmd.Parameters.AddWithValue("@newPkgStartDate", newPkg.PkgStartDate);
                    cmd.Parameters.AddWithValue("@newPkgEndDate", newPkg.PkgEndDate);
                    cmd.Parameters.AddWithValue("@newPkgDesc", newPkg.PkgDesc);
                    cmd.Parameters.AddWithValue("@newPkgBasePrice", newPkg.PkgBasePrice);
                    cmd.Parameters.AddWithValue("@newPkgAgencyCommission", newPkg.PkgAgencyCommission);
                    cmd.Parameters.AddWithValue("@oldPackageId", oldPkg.PackageId);
                    cmd.Parameters.AddWithValue("@oldPkgName", oldPkg.PkgName);
                    cmd.Parameters.AddWithValue("@oldPkgStartDate", oldPkg.PkgStartDate);
                    cmd.Parameters.AddWithValue("@oldPkgEndDate", oldPkg.PkgEndDate);
                    cmd.Parameters.AddWithValue("@oldPkgDesc", oldPkg.PkgDesc);
                    cmd.Parameters.AddWithValue("@oldPkgBasePrice", oldPkg.PkgBasePrice);
                    cmd.Parameters.AddWithValue("@oldPkgAgencyCommission", oldPkg.PkgAgencyCommission);
                    connection.Open();
                    count = cmd.ExecuteNonQuery(); // returns how many rows updated
                }
            }
            return (count > 0);
        }
    }
}
