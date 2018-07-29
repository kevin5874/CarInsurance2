using CarInsurance2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarInsurance2.Controllers
{
    public class HomeController : Controller
    {
        string connectionString = "Data Source=DESKTOP-1N6P132;Initial Catalog=Quotes;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Submit(string firstName, string lastName, string emailAddress, DateTime DOB, string carYear, string carMake,
            string carModel, bool coverage, bool dui, int tickets)
        {
            var customer = new Customer(firstName, lastName, emailAddress, DOB, Int32.Parse(carYear), carMake, carModel, dui, tickets, coverage);
            double price = customer.GetQuote();
            //customer.SendEmail(price); Spent a while trying to figure out what's going on here, eventually realized that the reason
            //I couldn't send the email was due to using IIS express which doesn't allow for SMTP email, oh well was a good learning experience
            


            string queryString = @"INSERT INTO CustomerQuotes (FName, LName, Email, DOB, CarYear, CarMake, CarModel, DUI, SpeedingTickets, Coverage, Price) VALUES
                                      (@FName, @LName, @Email, @DOB, @CarYear, @CarMake, @CarModel, @DUI, @SpeedingTickets, @Coverage, @Price)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.Add("@FName", SqlDbType.NVarChar);
                cmd.Parameters.Add("@LName", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar);
                cmd.Parameters.Add("@DOB", SqlDbType.Date);
                cmd.Parameters.Add("@CarYear", SqlDbType.NChar);
                cmd.Parameters.Add("@CarMake", SqlDbType.NVarChar);
                cmd.Parameters.Add("@CarModel", SqlDbType.NVarChar);
                cmd.Parameters.Add("@DUI", SqlDbType.NChar);
                cmd.Parameters.Add("@SpeedingTickets", SqlDbType.Int);
                cmd.Parameters.Add("@Coverage", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Price", SqlDbType.NVarChar);
                cmd.Parameters["@FName"].Value = firstName;
                cmd.Parameters["@LName"].Value = lastName;
                cmd.Parameters["@Email"].Value = emailAddress;
                cmd.Parameters["@DOB"].Value = DOB;
                cmd.Parameters["@CarYear"].Value = carYear;
                cmd.Parameters["@CarMake"].Value = carMake;
                cmd.Parameters["@CarModel"].Value = carModel;
                cmd.Parameters["@DUI"].Value = dui.ToString();
                cmd.Parameters["@SpeedingTickets"].Value = tickets;
                cmd.Parameters["@Coverage"].Value = coverage.ToString();
                cmd.Parameters["@Price"].Value = price.ToString();
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            return View("Success");
        }
        public ActionResult Admin()
        {
            string queryString = @"SELECT FName, LName, Email, Price FROM CustomerQuotes";
            List<CustomerVM> Quotes = new List<CustomerVM>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(queryString, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var quote = new CustomerVM();
                    
                    quote.FName = reader["FName"].ToString();
                    quote.LName = reader["LName"].ToString();
                    quote.Email = reader["Email"].ToString();
                    quote.price = Double.Parse(reader["Price"].ToString());
                    Quotes.Add(quote);
                }
            }
            return View(Quotes);
        }
    }
}