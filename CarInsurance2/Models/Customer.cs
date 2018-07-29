using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace CarInsurance2.Models
{
    public class Customer
    {
        public string ID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
        public int carYear { get; set; }
        public string carMake { get; set; }
        public string carModel { get; set; }
        public bool DUI { get; set; }
        public int SpeedingTickets { get; set; }
        public bool FullOrLiability { get; set; } // full will be set to true, liability false
        public double price { get; set; }
        public Customer(string FName, string LName, string Email, DateTime DOB, int carYear, string carMake, string carModel, 
            bool DUI, int SpeedingTickets, bool FullOrLiability)
        {
            
            this.FName = FName;
            this.LName = LName;
            this.Email = Email;
            this.DOB = DOB;
            this.carYear = carYear;
            this.carMake = carMake.ToLower();
            this.carModel = carModel.ToLower();
            this.DUI = DUI;
            this.SpeedingTickets = SpeedingTickets;
            this.FullOrLiability = FullOrLiability;
        }

        public double GetQuote()
        {
            
            DateTime now = new DateTime();
            now = DateTime.Now;
            double basePrice = 50;
            if(DOB.Month >= now.Month)
            {
                if((now.Year + 1 - DOB.Year <= 25 && now.Year + 1 - DOB.Year >= 18) || now.Year + 1 - DOB.Year >= 100)
                {
                    basePrice += 25;
                }
                else if(now.Year + 1 - DOB.Year < 18)
                {
                    basePrice += 100;
                }
               
            }
            else
            {
                if ((now.Year + 1 - DOB.Year <= 25 && now.Year + 1 - DOB.Year >= 18) || now.Year + 1 - DOB.Year >= 100)
                {
                    basePrice += 25;
                }
                else if (now.Year - DOB.Year < 18)
                {
                    basePrice += 100;
                }
            }
            if(carYear < 2000 || carYear > 2015)
            {
                basePrice += 25;
            }
            if(carMake.Equals("porshe"))
            {
                if(carModel.Equals("911 carrera"))
                {
                    basePrice += 25;
                }
                basePrice += 25;
            }
            basePrice += SpeedingTickets * 10;
            basePrice = DUI ? basePrice *= 1.25 : basePrice;
            basePrice = FullOrLiability ? basePrice *= 1.5 : basePrice;
            basePrice = Math.Round(basePrice, 2);

            return basePrice;
        }
        public void SendEmail(double total)
        {
            MailMessage msg = new MailMessage();
            msg.From = new System.Net.Mail.MailAddress("kevingran7@gmail.com");
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 597;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network; 
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential();  // you would use the credentials for the email you want to send from here
            smtp.Host = "smtp.gmail.com";
            msg.To.Add(new MailAddress(Email));
            string message = String.Format("Hello {0} and thank you for submitting a request for a quote, " +
                "after thouroughly reviewing your qualifications we've found the following to be a fair market rate {1}", FName, total);
            msg.Body = message;
            smtp.Send(msg);

        }
    }
    
}