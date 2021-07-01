using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using DSIAppMaintenance.DataBase;
using DSIAppMaintenance.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;

namespace DSIAppMaintenance.Controllers
{
    public class SettingController : Controller
    {
        SqlConnection connSource;
        ///SqlConnection conn = DBUtils.GetDBConnection();


        // GET: SettingController
        public ActionResult Index()
        {
            return View();
        }

        // GET: SettingController
        public ActionResult Maj()
        {
            return View();
        }

        // POST: SettingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Maj(Maj collection)
        {

            //connSource = DBUtils.GetDBConnection();

            //SqlCommand cmd = new SqlCommand(collection.Script, connSource);
            //cmd.CommandType = CommandType.StoredProcedure;


            //connSource.Open();
            //cmd.ExecuteNonQuery();
            //connSource.Close();
            //connSource.Dispose();
            //cmd.Dispose();

            
            ////
            //using (SqlCommand command = connSource.CreateCommand())
            //{
            //    FileInfo file = new FileInfo(@"C:\Users\direct11\Documents\Visual Studio 2017\website\AGRIX_EN_COURS\agrice_agromex\DSIAppMaintenance\DataBase\Script\SCRIPMAJ.sql");
            //    string script = file.OpenText().ReadToEnd();
            //    command.CommandText = script.Replace("GO","");
            //    connSource.Open();
            //    int affectedRows = command.ExecuteNonQuery();
            //}



            try
            {

                return View(collection);
            }
            catch
            {
                return View();
            }
        }

        private string path = "";

        private string fileName = "";


        private string pathDir = "";
        private string pathDirTmp = "";

        public ActionResult GenCodeEtat()
        {
            ViewBag.Message = "Your contact page.";



            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenCodeEtat(string txtCode, string txtNameFile, string txtNameReport, string txtNameTableAd)
        {
            path = @"D:\igor_save\";
            pathDir = @"D:\SAVE_FILE_TMP\";
            fileName = txtNameFile;
            string file = path + "" + fileName + ".txt";
            string[] TtxtCode;

            TtxtCode = txtCode.Split(",".ToCharArray());


            FileInfo fichier = new FileInfo(file);
            StreamWriter sw = new StreamWriter(file, true, System.Text.Encoding.ASCII);



            //****initialisation de la procedure de l'état****//
            sw.WriteLine("Protected Sub " + txtNameFile + "()\n");

            //****Si la tableAdapteur a été renseigner on ajout la condition****//
            if (txtNameTableAd.Trim() != "")
                sw.WriteLine("If " + txtNameTableAd + ".Rows.Count <> 0 Then\n");

            //****initialisation des variables****//
            foreach (var variable in TtxtCode)
                sw.WriteLine("Dim v" + variable.Trim() + " As String = \"\" \n");

            //****Déclaration et inialisation des parametres de l'etat****//
            foreach (var variable in TtxtCode)
            {
                if (variable.Trim().IndexOf("pied_page1") > -1 || variable.Trim().IndexOf("pied_page2") > -1)
                {
                    if (variable.Trim().IndexOf("pied_page1") > -1)
                    {
                        sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vpied_page1\").ToString)\n");
                    }

                    if (variable.Trim().IndexOf("pied_page2") > -1)
                    {
                        sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vpied_page2\").ToString)\n");
                    }
                }
                else
                {
                    sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",v" + variable.Trim() + ")\n");
                }

            }


            foreach (var variable in TtxtCode)
            {
                sw.WriteLine(txtNameReport + ".LocalReport.SetParameters(rpt_" + variable.Trim() + ")\n");

            }

            //
            sw.WriteLine("rptv_et_hand.LocalReport.Refresh()\n");

            //****Si la tableAdapteur a été renseigner on ajout la fin de la condition****//
            if (txtNameTableAd.Trim() != "")
                sw.WriteLine("End If \n");


            //****Din de la procedure de l'état****//
            sw.WriteLine("End Sub \n");


            sw.Close();

            return View();

        }


        public ActionResult GenCodeEtatAuto()
        {
            ViewBag.Message = "Your contact page.";


            return View();
        }

        private void initValue()
        {
            path = @"D:\igor_save\";
            pathDir = @"D:\SAVE_FILE_TMP\";

            //using (var database = new DatabaseEntities())
            //{
            //    foreach (var param in database.Parametrages)
            //    {
            //        path = @"" + param.cheminsave;
            //    }
            //}
        }


        [HttpPost]
        public ActionResult GenCodeEtatAuto(string txtNameTableAd, string txtNameReport, List<IFormFile> textGetFile)
        {



            initValue();

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            if (!System.IO.Directory.Exists(pathDir))
                System.IO.Directory.CreateDirectory(pathDir);


            List<string> LTtxtCode = new List<string>();

            foreach (var filexml in textGetFile)
            {


                LTtxtCode.Clear();

                fileName = filexml.FileName.Replace(".rdlc", "");
                string file = path + "" + fileName + ".txt";

                if (System.IO.File.Exists(file))
                    System.IO.File.Delete(file);



                pathDirTmp = filexml.FileName;

                if (System.IO.File.Exists(pathDir + "" + pathDirTmp))
                    System.IO.File.Delete(pathDir + "" + pathDirTmp);

                using (var stream = new FileStream(pathDir + "" + pathDirTmp, FileMode.Create))
                {
                    filexml.CopyTo(stream);
                }

                ///filexml.CopyTo(pathDir + "" + pathDirTmp);


                XmlDocument docxml = new XmlDocument();
                docxml.Load(pathDir + "" + pathDirTmp);



                foreach (XmlElement element in docxml.GetElementsByTagName("ReportParameter"))
                {
                    LTtxtCode.Add(element.GetAttribute("Name"));

                }



                FileInfo fichier = new FileInfo(file);

                StreamWriter sw = new StreamWriter(file, true, System.Text.Encoding.ASCII);


                //****initialisation de la procedure de l'état****//
                sw.WriteLine("Protected Sub " + fileName + "()\n");

                //****Si la tableAdapteur a été renseigner on ajout la condition****//
                if (txtNameTableAd.Trim() != "")
                    sw.WriteLine("If " + txtNameTableAd + ".Rows.Count <> 0 Then\n");

                //****initialisation des variables****//
                foreach (var variable in LTtxtCode)
                {

                    if (variable.Trim().IndexOf("activite") > -1 || variable.Trim().IndexOf("capital") > -1 || variable.Trim().IndexOf("rc") > -1 || variable.Trim().IndexOf("cc") > -1 || variable.Trim().IndexOf("email") > -1 || variable.Trim().IndexOf("pays") > -1 || variable.Trim().Replace("_", "").IndexOf("libcamp") > -1 || variable.Trim().IndexOf("lib_sit") > -1 || variable.Trim().IndexOf("tel") > -1 || variable.Trim().IndexOf("sigle") > -1 || variable.Trim().IndexOf("entete2") > -1 || variable.Trim().IndexOf("entete1") > -1 || variable.Trim().IndexOf("fax") > -1 || variable.Trim().IndexOf("pied_page1") > -1 || variable.Trim().IndexOf("pied_page2") > -1 || variable.Trim().IndexOf("ville") > -1 || variable.ToLower().Trim().IndexOf("adresse") > -1) //adresse
                    {

                    }
                    else
                    {
                        sw.WriteLine("Dim v" + variable.Trim() + " As String = \" \"");
                    }

                }

                sw.WriteLine("\n\n" + txtNameReport + ".LocalReport.DataSources.Clear()\n\n");


                //****Déclaration et inialisation des parametres de l'etat****//
                foreach (var variable in LTtxtCode)
                {
                    //
                    if (variable.Trim().IndexOf("activite") > -1 || variable.Trim().IndexOf("capital") > -1 || variable.Trim().IndexOf("rc") > -1 || variable.Trim().IndexOf("cc") > -1 || variable.Trim().IndexOf("email") > -1 || variable.Trim().IndexOf("pays") > -1 || variable.Trim().Replace("_", "").IndexOf("libcamp") > -1 || variable.Trim().IndexOf("lib_sit") > -1 || variable.Trim().IndexOf("tel") > -1 || variable.Trim().IndexOf("sigle") > -1 || variable.Trim().IndexOf("entete2") > -1 || variable.Trim().IndexOf("entete1") > -1 || variable.Trim().IndexOf("fax") > -1 || variable.Trim().IndexOf("pied_page1") > -1 || variable.Trim().IndexOf("pied_page2") > -1 || variable.Trim().IndexOf("ville") > -1 || variable.ToLower().Trim().IndexOf("adresse") > -1) //adresse
                    {


                        if (variable.Trim().IndexOf("activite") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vactivite\").ToString)");
                        }

                        if (variable.Trim().IndexOf("capital") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vcapital\").ToString)");
                        }

                        if (variable.Trim().IndexOf("rc") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vrc\").ToString)");
                        }

                        if (variable.Trim().IndexOf("cc") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vcc\").ToString)");
                        }

                        if (variable.Trim().IndexOf("email") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vemail\").ToString)");
                        }

                        if (variable.Trim().IndexOf("pays") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vlib_pays\").ToString)");
                        }

                        if (variable.Trim().Replace("_", "").IndexOf("libcamp") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vlibcamp\").ToString)");
                        }

                        if (variable.Trim().IndexOf("lib_sit") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vlib_sit\").ToString)");
                        }

                        if (variable.Trim().IndexOf("sigle") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vsigle\").ToString)");
                        }

                        if (variable.Trim().IndexOf("tel") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vtel\").ToString)");
                        }

                        if (variable.Trim().IndexOf("entete1") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"ventete1\").ToString)");
                        }

                        if (variable.Trim().IndexOf("entete2") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"ventete2\").ToString)");
                        }

                        if (variable.Trim().IndexOf("fax") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vfax\").ToString)");
                        }

                        if (variable.Trim().IndexOf("pied_page1") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vpied_page1\").ToString)");
                        }

                        if (variable.Trim().IndexOf("pied_page2") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vpied_page2\").ToString)");
                        }

                        if (variable.Trim().IndexOf("ville") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vville\").ToString())");
                        }

                        if (variable.ToLower().Trim().IndexOf("adresse") > -1)
                        {
                            sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",Session(\"vadresse\").ToString())");
                        }
                    }
                    else
                    {
                        sw.WriteLine("Dim rpt_" + variable.Trim() + " As New ReportParameter(\"" + variable.Trim() + "\",v" + variable.Trim() + ")");
                    }

                }

                sw.WriteLine("\n");

                foreach (var variable in LTtxtCode)
                {
                    sw.WriteLine(txtNameReport + ".LocalReport.SetParameters(rpt_" + variable.Trim() + ")");

                }

                //
                sw.WriteLine("\n" + txtNameReport + ".LocalReport.Refresh()\n\n");

                //****Si la tableAdapteur a été renseigner on ajout la fin de la condition****//
                if (txtNameTableAd.Trim() != "")
                    sw.WriteLine("End If \n");


                //****Din de la procedure de l'état****//
                sw.WriteLine("End Sub \n");


                sw.Close();

                if (System.IO.File.Exists(pathDir + "" + pathDirTmp))
                    System.IO.File.Delete(pathDir + "" + pathDirTmp);

            }



            if (System.IO.Directory.Exists(pathDir))
                System.IO.Directory.Delete(pathDir);

            return View();

        }

        private void initValueHtml()
        {
            path = @"D:\Agrix_projet_devpt_light\";
            pathDir = @"D:\SAVE_FILE_TMP_R\";

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            path = @"D:\Agrix_projet_devpt_light\agrice\";

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            if (!System.IO.Directory.Exists(pathDir))
                System.IO.Directory.CreateDirectory(pathDir);

        }


        public ActionResult ReplaceHhml()
        {
            ViewBag.Message = "Your contact page.";


            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ReplaceHhml(List<IFormFile> textGetFile)
        {

            CodeForm codeForm = new CodeForm();
            initValueHtml();

            List<string> LTtxtCode = new List<string>();

            DBUtils cnn1 = new DBUtils();

            connSource = cnn1.GetDBConnection();
            await connSource.OpenAsync();

            string sql = "SELECT type_desc,OBJECT_NAME([sm].[object_id]) AS [ObjectName],[sm].[definition] AS [ObjectDefinition],sm.object_id,ROW_NUMBER() OVER (PARTITION BY 1  ORDER BY sm.object_id) as id	FROM sys.sql_modules sm INNER JOIN sys.objects o ON sm.object_id=o.object_id WHERE 	is_ms_shipped = 0 ";
            SqlCommand cmd = new SqlCommand(sql, connSource);

            SqlDataAdapter adSql = new SqlDataAdapter(cmd);
            DataTable mondataset = new DataTable();
            adSql.Fill(mondataset);
            adSql.Dispose();
            connSource.Close();
            var tableName ="";



            StreamReader fileAspx;

            foreach (var filexml in textGetFile)
            {

                LTtxtCode.Clear();

                fileName = filexml.FileName;

                string file = path + "" + fileName ;

                if (System.IO.File.Exists(file))
                    System.IO.File.Delete(file);



                pathDirTmp = filexml.FileName;

                if (System.IO.File.Exists(pathDir + "" + pathDirTmp))
                    System.IO.File.Delete(pathDir + "" + pathDirTmp);

                FileInfo fichier = new FileInfo(file);

                StreamWriter sw = new StreamWriter(file, true, System.Text.Encoding.UTF8);

                using (var stream = new FileStream(pathDir + "" + pathDirTmp, FileMode.Create))
                {

                    await filexml.CopyToAsync(stream);
                }
                
                var text = "" ;
                fileAspx = new StreamReader(pathDir + "" + pathDirTmp);



                 while ((file = fileAspx.ReadLine()) != null)
                 {
                //file = fileAspx.ReadLine();

                    file = file.Replace("@cod_sit,", "@cod_sit,@franchise,");
                   
                    //file = file.Replace("wplantfou(@cod_sit", "WPLANTFOU(@nocamp,@cod_sit");

                    //file = file.Replace("wplantfou_attente(@cod_sit", "WPLANTFOU_ATTENTE(@nocamp,@cod_sit");
                    //file = file.Replace("WPLANTFOU_attente(@cod_sit", "WPLANTFOU_ATTENTE(@nocamp,@cod_sit");
                    //file = file.Replace("WPLANTFOU_ATTENTE(@cod_sit", "WPLANTFOU_ATTENTE(@nocamp,@cod_sit");

                    //file = file.Replace("WGPS((@cod_sit", "WGPS(@nocamp,@cod_sit");
                    //file = file.Replace("wgps(@cod_sit", "WGPS(@nocamp,@cod_sit");

                    //file = file.Replace("WEPOUSE(@cod_sit", "WEPOUSE(@nocamp,@cod_sit");
                    //file = file.Replace("wepouse(@cod_sit", "WEPOUSE(@nocamp,@cod_sit");

                    //file = file.Replace("WENFANT((@cod_sit", "WENFANT(@nocamp,@cod_sit");
                    //file = file.Replace("wenfant(@cod_sit", "WENFANT(@nocamp,@cod_sit");

                    

                    //file = file.Replace("WORIG(@nocamp,@cod_sit,@cod_pays", "WORIG(@nocamp,@cod_pays");
                    //file = file.Replace("worig(@nocamp,@cod_sit,@cod_pays", "WORIG(@nocamp,@cod_pays");

                    //file = file.Replace("WREGION(@nocamp,@cod_sit,@cod_pays", "WREGION(@nocamp,@cod_pays");
                    //file = file.Replace("wregion(@nocamp,@cod_sit,@cod_pays", "WREGION(@nocamp,@cod_pays");

                    //file = file.Replace("WSOUSPREF(@nocamp,@cod_sit,@cod_pays", "WSOUSPREF(@nocamp,@cod_pays");
                    //file = file.Replace("wsouspref(@nocamp,@cod_sit,@cod_pays", "WSOUSPREF(@nocamp,@cod_pays");

                    //file = file.Replace("WDEPARTEM(@nocamp,@cod_sit,@cod_pays", "WDEPARTEM(@nocamp,@cod_pays");
                    //file = file.Replace("wdepartem(@nocamp,@cod_sit,@cod_pays", "WDEPARTEM(@nocamp,@cod_pays");

                    //file = file.Replace("WORIG(@cod_pays", "WORIG(@nocamp,@cod_pays");
                    //file = file.Replace("worig(@cod_pays", "WORIG(@nocamp,@cod_pays");

                    //file = file.Replace("WVILLAGE(@cod_pays", "WVILLAGE(@nocamp,@cod_pays");
                    //file = file.Replace("wvillage(@cod_pays", "WVILLAGE(@nocamp,@cod_pays");

                    //file = file.Replace("wregion(@cod_pays", "WREGION(@nocamp,@cod_pays");
                    //file = file.Replace("WREGION(@cod_pays", "WREGION(@nocamp,@cod_pays");

                    //file = file.Replace("WSOUSPREF(@cod_pays", "WSOUSPREF(@nocamp,@cod_pays");
                    //file = file.Replace("wsouspref(@cod_pays", "WSOUSPREF(@nocamp,@cod_pays");

                    //file = file.Replace("WDEPARTEM(@cod_pays", "WDEPARTEM(@nocamp,@cod_pays");
                    //file = file.Replace("wdepartem(@cod_pays", "WDEPARTEM(@nocamp,@cod_pays");

                    //file = file.Replace("WENCLAVE(@cod_pays", "WENCLAVE(@nocamp,@cod_pays");
                    //file = file.Replace("wenclave(@cod_pays", "WENCLAVE(@nocamp,@cod_pays");

                    //file = file.Replace("WGPS_enclave(@cod_pays", "WGPS_ENCLAVE(@nocamp,@cod_pays");
                    //file = file.Replace("wgps_enclave(@cod_pays", "WGPS_ENCLAVE(@nocamp,@cod_pays");
                    //file = file.Replace("WGPS_ENCLAVE(@cod_pays", "WGPS_ENCLAVE(@nocamp,@cod_pays");



                    //foreach (DataRow row in mondataset.Rows)
                    //{
                    //    tableName = row["ObjectName"].ToString();

                        


                    //    if (fileName.IndexOf(".vb") > -1)
                    //    {
                    //        //if (vtableNameTrouve)
                    //        //{
                                
                    //            if(file.IndexOf(".CommandType") > -1)
                    //            {
                    //                await sw.WriteLineAsync(file);
                    //                await sw.WriteLineAsync(file.Substring(0, file.IndexOf(".CommandType")) + ".Parameters.AddWithValue(\"@franchise\", Session(\"vfranchise\"))");

                                    
                    //                file = "";
                    //                vtableNameTrouve = false;

                    //            }
                               

                    //        //}

                           
                               if (file.IndexOf(".Parameters.AddWithValue(\"@cod_sit\", Session(\"vcod_sit\"))") > -1)
                              {
                                await sw.WriteLineAsync(file.Substring(0, file.IndexOf(".Parameters")) + ".Parameters.AddWithValue(\"@franchise\", Session(\"vfranchise\"))");
                               // await sw.WriteLineAsync(file);
                                  

                    //                file = "";
                    //                vOrigTrouve = false;

                                }
                            
                            
                    //       // if(file.IndexOf(tableName) > -1)
                    //        //{
                    //            vtableNameTrouve = true;
                    //        //}

                    //       //if (file.IndexOf("WORIG(@nocamp") > -1)
                    //        //{
                    //        //    vOrigTrouve = true;
                    //        //}

                    //    }
                    //    //else
                    //    //{
                    //    //    vOrigTrouve = false;
                    //    //    vtableNameTrouve = false;
                    //        //file = file.Replace(tableName + "(@cod_sit", tableName + "(@nocamp,@cod_sit");
                    //    //}
                        
                      
                    //}

                    await sw.WriteLineAsync(file);
                }

                fileAspx.Close();

           

                sw.Close();

               

                if (System.IO.File.Exists(pathDir + "" + pathDirTmp))
                    System.IO.File.Delete(pathDir + "" + pathDirTmp);
            }

            if (System.IO.Directory.Exists(pathDir))
                System.IO.Directory.Delete(pathDir);


            return View(codeForm);
        }

        
        public ActionResult DupliqueData()
        {
            ViewBag.Message = "Duplique data";


            return View();
        }

        [HttpPost]
        public async Task<ActionResult> DupliqueData(String textGetFile)
        {
            var traitementOk = false;

            DBUtils cnn1 = new DBUtils();
            SqlDataAdapter adSql;
            DataTable mondataset;
            SqlCommand cmd;

            connSource = cnn1.GetDBConnection();
            await connSource.OpenAsync();

            var sql = "select * from traitement_camp";
            
            try
            {

                cmd = new SqlCommand(sql, connSource);
                adSql = new SqlDataAdapter(cmd);
                mondataset = new DataTable();
                adSql.Fill(mondataset);
                adSql.Dispose();
                if (mondataset.Rows.Count > 0)
                {
                    traitementOk = true;
                }
               
            }
            catch
            {
                traitementOk = false;
            }
            finally
            {
               
                connSource.Close();
            }


            if (traitementOk)
            {
                ViewBag.Message = "Traitement déjà executé";
            }
            else
            {
                string file = Path.GetFullPath("./DataBase/Script/"); // chemin du script traitement_camp
                StreamReader fileAspx;

                fileAspx = new StreamReader(file + "traitement_camp.sql");

                sql = "";
                while ((file = fileAspx.ReadLine()) != null)
                {

                    sql += '\n' + file.Replace("BDD_DSI", cnn1.database);
                }

                fileAspx.Close();


                await connSource.OpenAsync();

                try
                {
                    cmd = new SqlCommand(sql, connSource);
                    cmd.ExecuteNonQuery();

                }
                catch(Exception e)
                {
                    ViewBag.Message = "Erreur lors du traitement : " + e.Message;
                }
                finally
                {

                    connSource.Close();
                }
            }



            return View();
        }

    }
}
