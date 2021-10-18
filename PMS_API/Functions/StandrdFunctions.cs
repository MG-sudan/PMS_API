
using CoursesOnline.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PMS_API.Functions
{
    public class StandrdFunctions
    {

       
        public static string SendSms(string text, string MobileNo)
        {
            string msg = text;
            // string tel = "966569076447";
            string tel = MobileNo;
            // string sendermsg = "Khamees";
            string url = "http://api-server3.com/api/send.aspx?username=ASEERIT&password=generation123&language=2&sender=ASEERIT&mobile=" + MobileNo + "&message=" + text;
            //DownloadString(url);

            var r = string.Empty;
            using (var web = new System.Net.WebClient())
                r = web.DownloadString(url);
            return r;
        }
        public static void SendEmail(string txtTo, string subj, string msg)
        {
            using (MailMessage mm = new MailMessage("wasel.traveling@gmail.com", txtTo))
            {
                mm.Subject = subj;
                mm.Body = msg;

                mm.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("wasel.traveling@gmail.com", "wasel1234!@#");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
                //ClientScript.RegisterStartupScript(GetType(), "alert", "alert('Email sent.');", true);
            }
        }

        public static void SendEmail(string txtTo, string msg)
        {
            using (MailMessage mm = new MailMessage("wasel.traveling@gmail.com", txtTo))
            {
                mm.Subject = "الرد على استفسار اونلاين مزاد";
                mm.Body = msg;

                mm.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("wasel.traveling@gmail.com", "wasel1234!@#");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
                //ClientScript.RegisterStartupScript(GetType(), "alert", "alert('Email sent.');", true);
            }
        }



        public string SendNotificationFromFirebaseCloud(string Msg, string Title)
        {//test new Z_ERP.Functions.StandrdFunctions().SendNotificationFromFirebaseCloud("test", "test");
            var result = "-1";
            var webAddr = "https://fcm.googleapis.com/fcm/send";
            // string AppSenderID = "";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "key=AAAAOr6bRUY:APA91bGY7CHnl8e9fJ4q1QdLW05hHeIu_IVo85BzGkFLI6w2gaKS8k-nGcDeZX2HO7W-b8fvHQC4FFGJulBJbk4IKcNWr9mq0Q2i8ZzALwyEhATORDA1g9jGf9QTEA0H6MSbvFtn-mHu");
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string UserTopicID = "";

                //UserTopicID = "ERPToMoawad";
                UserTopicID = "ERPToMohammed";
                string ttt = "{\"to\": \"/topics/" + UserTopicID + "\",\"data\": {\"ShortDesc\": \"" + Title + "\",\"IncidentNo\": \"123\",\"Description\":\"detail desc\"},\"notification\":{\"title\": \"" + Title + "\",\"text\": \"" + Msg + "\",\"sound\":\"default\"}}";
                streamWriter.Write(ttt);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }

        public  string Encrypt(string clearText)
        {
            string EncryptionKey = "Booking!@#";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public  string Decrypt(string cipherText)
        {
            string EncryptionKey = "Booking!@#";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static  List<FieldsClass> GetConvertDatatableToList(DataTable datatable )
        {
            var temp = new List<FieldsClass>();

            foreach (DataRow  item in datatable.Rows)
            {
                var newItem = new FieldsClass();
                for (int i = 0; i < datatable.Columns.Count ; i++)
                {
                    switch (i)
                    {
                        case 0:
                            newItem.Field1  = item[i].ToString ();
                            break;
                        case 1:
                            newItem.Field2 = item[i].ToString();
                            break;
                        case 2:
                            newItem.Field3 = item[i].ToString();
                            break;
                        case 3:
                            newItem.Field4 = item[i].ToString();
                            break;
                        case 4:
                            newItem.Field5 = item[i].ToString();
                            break;
                        case 5:
                            newItem.Field6 = item[i].ToString();
                            break;
                        case 6:
                            newItem.Field7 = item[i].ToString();
                            break;
                        case 7:
                            newItem.Field8 = item[i].ToString();
                            break;
                        case 8:
                            newItem.Field9 = item[i].ToString();
                            break;
                        case 9:
                            newItem.Field10 = item[i].ToString();
                            break;
                        case 10:
                            newItem.Field11 = item[i].ToString();
                            break;
                        case 11:
                            newItem.Field12 = item[i].ToString();
                            break;
                        case 12:
                            newItem.Field13 = item[i].ToString();
                            break;
                        case 13:
                            newItem.Field14 = item[i].ToString();
                            break;
                        case 14:
                            newItem.Field15 = item[i].ToString();
                            break;
                        case 15:
                            newItem.Field16 = item[i].ToString();
                            break;
                        case 16:
                            newItem.Field17 = item[i].ToString();
                            break;
                        case 17:
                            newItem.Field18 = item[i].ToString();
                            break;
                        case 18:
                            newItem.Field19 = item[i].ToString();
                            break;
                        case 19:
                            newItem.Field20 = item[i].ToString();
                            break;
                        case 20:
                            newItem.Field21 = item[i].ToString();
                            break;
                        case 21:
                            newItem.Field22 = item[i].ToString();
                            break;
                        case 22:
                            newItem.Field23 = item[i].ToString();
                            break;
                    }
                }
                temp.Add(newItem);
            }



            return temp;
        }
        public static  FieldsClass GetConvertDatatableObject(DataTable datatable)
        {
            var Temp = new FieldsClass();
            if (datatable.Rows.Count > 0)
            {
                var item = datatable.Rows[0];
                for (int i = 0; i < datatable.Columns.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            Temp.Field1 = item[i].ToString();
                            break;
                        case 1:
                            Temp.Field2 = item[i].ToString();
                            break;
                        case 2:
                            Temp.Field3 = item[i].ToString();
                            break;
                        case 3:
                            Temp.Field4 = item[i].ToString();
                            break;
                        case 4:
                            Temp.Field5 = item[i].ToString();
                            break;
                        case 5:
                            Temp.Field6 = item[i].ToString();
                            break;
                        case 6:
                            Temp.Field7 = item[i].ToString();
                            break;
                        case 7:
                            Temp.Field8 = item[i].ToString();
                            break;
                        case 8:
                            Temp.Field9 = item[i].ToString();
                            break;
                        case 9:
                            Temp.Field10 = item[i].ToString();
                            break;
                        case 10:
                            Temp.Field11 = item[i].ToString();
                            break;
                        case 11:
                            Temp.Field12 = item[i].ToString();
                            break;
                        case 12:
                            Temp.Field13 = item[i].ToString();
                            break;
                        case 13:
                            Temp.Field14 = item[i].ToString();
                            break;
                        case 14:
                            Temp.Field15 = item[i].ToString();
                            break;
                        case 15:
                            Temp.Field16 = item[i].ToString();
                            break;
                        case 16:
                            Temp.Field17 = item[i].ToString();
                            break;
                        case 17:
                            Temp.Field18 = item[i].ToString();
                            break;
                        case 18:
                            Temp.Field19 = item[i].ToString();
                            break;
                        case 19:
                            Temp.Field20 = item[i].ToString();
                            break;

                        case 20:
                            Temp.Field21 = item[i].ToString();
                            break;
                        case 21:
                            Temp.Field22 = item[i].ToString();
                            break;
                        case 22:
                            Temp.Field23 = item[i].ToString();
                            break;
                    }
                }
            }
            else
            {
                Temp = null;
            }
            return Temp;
        }

    }



}