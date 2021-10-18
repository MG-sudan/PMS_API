using CoursesOnline.Models;
using Newtonsoft.Json;
using PMS_API.Functions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PMS_API.Controllers
{
    public class PMSAPIController : ApiController
    {
        private bool LogIsActive = true;
        [Route("api/GetExecute")]
        [HttpPost]
        public ResponseObject GetExecuteAsync(RequsetObject obj)
        {
            var Temp = new ResponseObject();
            try
            {
                //==============================Log Section=======================
             
                if (LogIsActive)
                    InsertLog(JsonConvert.SerializeObject(obj), 1);

                if (obj.ServiceID.Equals("000"))//Login
                {
                    var ListInputObj = new List<InputClass> { 
                     new InputClass {Key ="UserNameOrUserEmail",Value=obj.RequestInput1 .ToString () },
                     new InputClass {Key ="UserPassword",Value=obj.RequestInput2 .ToString () },
                    };
                    var dt = sqlExcute("api_UserLogin", ListInputObj);
                    if (dt.Rows.Count > 0)
                    {
                        var FieldsObj = StandrdFunctions.GetConvertDatatableObject(dt);
                        Temp.ErrorNo = 0;
                        Temp.ErrorMassege = "";
                        Temp.Response1 = FieldsObj;
                    }
                    else
                    {
                        Temp.ErrorNo = -1;
                        Temp.ErrorMassege = "اسم المستخدم او كلمة المرور خطأ";
                        Temp.Response1 = null;
                    }

                }
                else if (obj.ServiceID.Equals("001"))//Get Project Details
                {
                    var ListInputObj = new List<InputClass> {
                     new InputClass {Key ="ContractNo",Value=obj.RequestInput1 .ToString () },
                    };
                    var dt = sqlExcute("api_GetProjectContractDetails", ListInputObj);
                    if (dt.Rows.Count > 0)
                    {
                        var FieldsObj = StandrdFunctions.GetConvertDatatableObject(dt);
                        Temp.ErrorNo = 0;
                        Temp.ErrorMassege = "";
                        Temp.Response1 = FieldsObj;
                    }
                    else
                    {
                        Temp.ErrorNo = -1;
                        Temp.ErrorMassege = "لا يوجد مشروع بهذا الرقم ";
                        Temp.Response1 = null;
                    }
                }
                else if (obj.ServiceID.Equals("002"))//Get Project Details GetBusinessContract
                {
                    var ListInputObj = new List<InputClass> {
                     new InputClass {Key ="ContractNo",Value=obj.RequestInput1 .ToString () },
                    };
                    var dt = sqlExcute("api_GetProjectContractDetails", ListInputObj);
                    if (dt.Rows.Count > 0)
                    {
                        var FieldsObj = StandrdFunctions.GetConvertDatatableObject(dt);
                        var ListOfBusinessContract = new List<FieldsClass>();
                        
                        var InputObj = new List<InputClass> {
                        new InputClass {Key ="ContractNo",Value=obj.RequestInput1 .ToString ()},
                        };
                        var dt2 = sqlExcute("api_GetBusinessAproval", InputObj);
                        if (dt2.Rows.Count >0)
                            ListOfBusinessContract= StandrdFunctions.GetConvertDatatableToList(dt2);
                        Temp.ErrorNo = 0;
                        Temp.ErrorMassege = "";
                        Temp.Response1 = FieldsObj;
                        Temp.Response2 = ListOfBusinessContract;
                    }
                    else
                    {
                        Temp.ErrorNo = -1;
                        Temp.ErrorMassege = "لا يوجد مشروع بهذا الرقم ";
                        Temp.Response1 = null;
                    }
                }
                else if (obj.ServiceID.Equals("003"))//Get Lookups Data
                {
                        Temp.ErrorNo = 0;
                        Temp.ErrorMassege = "";
                        Temp.Response1 = StandrdFunctions.GetConvertDatatableToList (sqlExcute("api_GetLookupData", new List<InputClass> { new InputClass { Key = "TypeID", Value = "1" } })); ;
                        Temp.Response2 = StandrdFunctions.GetConvertDatatableToList(sqlExcute("api_GetLookupData", new List<InputClass> { new InputClass { Key = "TypeID", Value = "2" } })); ;
                        Temp.Response3 = StandrdFunctions.GetConvertDatatableToList(sqlExcute("api_GetLookupData", new List<InputClass> { new InputClass { Key = "TypeID", Value = "3" } })); ;
                }
                else if (obj.ServiceID.Equals("004"))//Insert Visit 
                {
                    var inputobject = JsonConvert.DeserializeObject<FieldsClass>(obj.RequestInput1.ToString());
                    var ListInputObj = new List<InputClass> {
                     new InputClass {Key ="VisitReasonID",          Value=inputobject.Field1 },
                     new InputClass {Key ="VisitTypeID",            Value=inputobject.Field2 },
                     new InputClass {Key ="EngineerID",             Value=inputobject.Field3 },
                     new InputClass {Key ="SupervisorEngineerID",   Value=inputobject.Field4},
                     new InputClass {Key ="SafetySecurityStandards",Value=inputobject.Field5 },
                     new InputClass {Key ="Employment",             Value=inputobject.Field6 },
                     new InputClass {Key ="Workflow",               Value=inputobject.Field7 },
                     new InputClass {Key ="VisitResult",            Value=inputobject.Field8 },
                     new InputClass {Key ="Recommendations",        Value=inputobject.Field9},
                     new InputClass {Key ="ContractNo",        Value=inputobject.Field10},
                     new InputClass {Key ="ProjectNo",        Value=inputobject.Field11},
                    };
                    var ImgeList = JsonConvert.DeserializeObject<List<FieldsClass>>(obj.RequestInput2.ToString());
                   
                    var dt = sqlExcute("api_InsertProjectVisit", ListInputObj);
                    if (dt.Rows.Count > 0)
                    {
                        var VisitID = dt.Rows[0][0].ToString();
                        foreach (var item in ImgeList)
                        {
                          var imagepath=  SaveImage(item.Field1, item.Field2);
                            var dt2 = sqlExcute("api_InsertProjectVisitImage", new List<InputClass> { new InputClass { Key = "ProjectsVisitImagePath", Value = imagepath } , new InputClass { Key = "VisitID", Value = VisitID } });
                        }
                        Temp.ErrorNo = 0;
                        Temp.ErrorMassege = "تم ادخال البيانات بنجاح";
                        Temp.Response1 = null;
                    }
                    else
                    {
                        Temp.ErrorNo = -1;
                        Temp.ErrorMassege = "حدث خطأ اثناء ادخال البيانات ";
                        Temp.Response1 = null;
                    }
                }
                else if (obj.ServiceID.Equals("005"))//Update Contract 
                {
                    var inputobject = JsonConvert.DeserializeObject<FieldsClass>(obj.RequestInput1.ToString());
                    var ListInputObj = new List<InputClass> {
                     new InputClass {Key ="VisitIsDone",               Value=inputobject.Field1 },
                     new InputClass {Key ="IsReadyToStart",            Value=inputobject.Field2 },
                     new InputClass {Key ="NoContraindicationsToStart", Value=inputobject.Field3 },
                     new InputClass {Key ="Decision",                   Value=inputobject.Field4},
                     new InputClass {Key ="X",                          Value=inputobject.Field5 },
                     new InputClass {Key ="Y",                          Value=inputobject.Field6 },
                     new InputClass {Key ="ContractID",                 Value=inputobject.Field7 },
                     new InputClass {Key ="EngineerID1",                 Value=inputobject.Field8 },
                     new InputClass {Key ="EngineerID2",                 Value=inputobject.Field9 },
                     new InputClass {Key ="EngineerID3",                 Value=inputobject.Field10 },
                    };
                    var dt = sqlExcute("api_UpdateContract", ListInputObj);
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString().Equals("1"))
                        {
                            var ImgeList = JsonConvert.DeserializeObject<List<FieldsClass>>(obj.RequestInput2.ToString());
                            foreach (var item in ImgeList)
                            {
                                var imagepath = SaveImage(item.Field1, item.Field2);
                                var dt2 = sqlExcute("api_InsertProjectImage", new List<InputClass> { new InputClass { Key = "ProjectsImagePath", Value = imagepath }, new InputClass { Key = "ContractID", Value  = inputobject.Field7 } });
                            }
                            Temp.ErrorNo = 0;
                            Temp.ErrorMassege = "تم استلام الموقع بنجاح";
                            Temp.Response1 = null;
                        }
                        else
                        {
                            Temp.ErrorNo = -1;
                            Temp.ErrorMassege = "تم استلام الموقع مسبقاً";
                            Temp.Response1 = null;
                        }
                      
                    }
                    else
                    {
                        Temp.ErrorNo = -1;
                        Temp.ErrorMassege = "حدث خطأ في  النظام  ";
                        Temp.Response1 = null;
                    }
                }
                else if (obj.ServiceID.Equals("006"))//Update Contract 
                {
                   
                    var ListInputObj = new List<InputClass> {
                     new InputClass {Key ="BusinessID",               Value=obj.RequestInput1.ToString()},
                     new InputClass {Key ="BusinessApproveID",            Value=obj.RequestInput2.ToString() },
                    };
                    var dt = sqlExcute("api_UpdateBusinessAproval", ListInputObj);
                    if (dt.Rows.Count > 0)
                    {
                        Temp.ErrorNo = 0;
                        Temp.ErrorMassege = obj.RequestInput2.ToString() == "1"?"تم الاعتماد  بنجاح":"لم يتم الاعتماد";
                        Temp.Response1 = null;
                    }
                    else
                    {
                        Temp.ErrorNo = -1;
                        Temp.ErrorMassege = "حدث خطأ في الاعتماد   ";
                        Temp.Response1 = null;
                    }
                }
                else if (obj.ServiceID.Equals("007"))//Update Contract 
                {

                    var ListInputObj = new List<InputClass> {
                     new InputClass {Key ="Username",               Value=obj.RequestInput1.ToString()},
                    };
                    var dt = sqlExcute("api_PasswordSetting", ListInputObj);
                    if (dt.Rows.Count > 0)
                    {
                        var OutPut = StandrdFunctions.GetConvertDatatableObject(dt);
                        if (OutPut.Field1.Equals("1"))
                        {
                            var Msg = OutPut.Field3;
                            var PhoneNo = OutPut.Field4;
                            StandrdFunctions.SendSms(Msg, PhoneNo);
                            Temp.ErrorNo = 0;
                            Temp.ErrorMassege = OutPut.Field2;
                            Temp.Response1 = null;
                        }
                        else
                        {
                            Temp.ErrorNo = -1;
                            Temp.ErrorMassege = OutPut.Field2;
                            Temp.Response1 = null;
                        }
                    }
                    else
                    {
                        Temp.ErrorNo = -1;
                        Temp.ErrorMassege = "حدث خطأ في النظام   ";
                        Temp.Response1 = null;
                    }
                }
                else if (obj.ServiceID.Equals("100"))//Update Contract 
                {

                    var ListInputObj = new List<InputClass> {
                    };
                    var dt = sqlExcute("api_GetMobileMenu", ListInputObj);
                    if (dt.Rows.Count > 0)
                    {

                        var MenuList = new List<MenuClass>();
                        foreach (DataRow  item in dt.Rows)
                        {
                            MenuList.Add(new MenuClass { MenuTitle =item ["MenuTitleAr"].ToString (),MenuID  =int.Parse ( item["MenuID"].ToString() ), MenuImage = item["MenuImage"]!=null? item["MenuImage"].ToString():"" , GroupID =  int.Parse ( item["GroupID"].ToString()) , IsActive = bool .Parse ( item["IsActive"].ToString()) });
                        }
                        Temp.ErrorNo = 0;
                        Temp.ErrorMassege = "";
                        Temp.Response1 = MenuList;
                       
                    }
                    else
                    {
                        Temp.ErrorNo = -1;
                        Temp.ErrorMassege = "حدث خطأ في النظام   ";
                        Temp.Response1 = null;
                    }
                }
                //else if (obj.ServiceID.Equals("001"))
                //{
                //    var httpRequest = HttpContext.Current.Request;
                //    if ((httpRequest.Files.Count > 0))
                //    {
                //        foreach (string file in httpRequest.Files)
                //        {
                //            var postedFile = httpRequest.Files[file];
                //            var fileName = postedFile.FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault();
                //            var filePath = HttpContext.Current.Server.MapPath(("~/Uploads/" + fileName));
                //            postedFile.SaveAs(filePath);
                //        }

                //    }

                //}








            }
            catch (Exception ex)
            {
                Temp.ErrorNo = 50;
                Temp.ErrorMassege = " حدث خطأ  حاول مرة أخرى";// ex.Message ;
                if (LogIsActive)
                    InsertLog(ex.ToString(), 2);
            }

            //Return Output
            if (LogIsActive)
                InsertLog(JsonConvert.SerializeObject(Temp), 2);
            return Temp;
        }

        public string  SaveImage(string ImgStr, string ImgName)
        {
            var ProjectVistImage = "ProjectImages";
            String path = HttpContext.Current.Server.MapPath("~/"+ ProjectVistImage); //Path

            //Check if directory exist
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
            }

            string imageName = ImgName;

            //set the image path
            string imgPath = Path.Combine(path, imageName);

            byte[] imageBytes = Convert.FromBase64String(ImgStr);

            File.WriteAllBytes(imgPath, imageBytes);

            return ProjectVistImage+"/"+ ImgName;
        }
        DataTable  sqlExcute(string StoreName, List <InputClass> InputObject)
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            var dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand MyCommnd = new SqlCommand(StoreName, con);
                    MyCommnd.CommandType = CommandType.StoredProcedure;
                    foreach (var item in InputObject)
                    {
                        MyCommnd.Parameters.Add("@" + item.Key, SqlDbType.NVarChar).Value = item.Value;
                    }

                    SqlDataAdapter adp = new SqlDataAdapter(MyCommnd);
                    DataSet ds = new DataSet();
                    // string temp = "";
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    adp.Fill(dt);
                    //cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception)
            {

              
            }
            return dt;
        }

        public static void InsertLog(string Message, int LogType)
{
    string path = AppDomain.CurrentDomain.BaseDirectory;
    var floder = "API_Call_Logs";
    var logPath = path + floder;
    if (!Directory.Exists(logPath))
        Directory.CreateDirectory(logPath);
    string filename = logPath + "/API_Log_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
    var fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
    var s = new StreamWriter(fs);
    s.BaseStream.Seek(0, SeekOrigin.End);
    s.WriteLine("Time : " + DateTime.Now.ToString());
    var logTypeString = LogType == 1 ? "Json Request " : "Json Response :";
    s.WriteLine(logTypeString);
    s.WriteLine(Message);
    s.WriteLine("------------------------------------------------------------------------------------------------------------------------------------");
    s.WriteLine("");
    s.Close();
    }
}

public class ResponseObject
{
    public int ErrorNo { get; set; }
    public string ErrorMassege { get; set; }
    public object Response1 { get; set; }
    public object Response2 { get; set; }
    public object Response3 { get; set; }
}
public class InputClass
{
    public string  Value { get; set; }
    public string Key { get; set; }
}
 public class RequsetObject
{
    public string ServiceID { get; set; }
    public object RequestInput1 { get; set; }
    public object RequestInput2 { get; set; }
    public object RequestInput3 { get; set; }
}
    public class MenuClass
    {
        public int MenuID { get; set; }
        public string MenuTitle { get; set; }
        public string MenuImage { get; set; }
        public int GroupID { get; set; }
        public bool IsActive { get; set; }
    }
}