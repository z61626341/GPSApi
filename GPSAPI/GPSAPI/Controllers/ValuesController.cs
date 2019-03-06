using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using Oracle.ManagedDataAccess.Client;

namespace GPSAPI.Controllers
{
    public class ValuesController : ApiController
    {
        Configuration webconn = WebConfigurationManager.OpenWebConfiguration("/Web.config");
        ConnectionStringSettings connStringSetting;
        string connString;
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            Init_Sales_Locate();
            return "ID : " + id + " my ID : " + Select_Sales_Locate();
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public string Delete(int id)
        {
            return "Delete";
        }

        public void Init_Sales_Locate()//查詢有無此料號
        {
            if (0 < webconn.ConnectionStrings.ConnectionStrings.Count)
            {
                if (connString != null)
                    Console.WriteLine("已取到連線字串。");
                else
                    Console.WriteLine("無法取得連線字串。");
            }
        }
        public string Select_Sales_Locate()
        {
            connStringSetting = webconn.ConnectionStrings.ConnectionStrings["OracleDbContext"];
            string connstr = "User Id=system;Password=password;Data Source=localhost:1521/jimdb;";
            string msql_init = "SELECT DEVICENO FROM JIMDB.SALES_LOCATE WHERE rownum = 1";
            string connString = connStringSetting.ConnectionString;
            using (var conn = new OracleConnection(connString))
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandText = msql_init;
                OracleDataAdapter da = new OracleDataAdapter(comm);
                using (var dr = comm.ExecuteReader())
                {
                    StringBuilder sb = new StringBuilder();
                    while (dr.Read())
                    {
                        //sb.AppendLine(dr["單號"].ToString());
                        return dr["DEVICENO"].ToString();
                    }

                    //Response.Write(sb.ToString());
                }
                return "Error Find";
            }
        }

        public void Insert_Sales_Locate(string deviceNo, string imeiNo, int latitude, int longitude, string updateTime)
        {
            bool isInsert = false;
            string connstr = "User Id=system;Password=sys_dsc;Data Source=10.1.1.70/topprod;";
            //string msql = "select tc_barcode01 單號,tc_barcode02 項次,tc_barcode03 料號,tc_barcode04 客戶產品編號,tc_barcode05 倉位,tc_barcode06 儲位,tc_barcode07 數量,tc_barcode08 單位,tc_barcode09 摘要,tc_barcode10 目前已出數量,tc_barcode11 備註批號 from e01.tc_barcode_file where tc_barcode01 like '%" + txtNo + "%'";
            //string msql = "select * from e01.tc_barcode_file where tc_barcode01 like '%" + txtNo + "%'";
            using (var conn2 = new OracleConnection(connstr))
            {
                conn2.Open();
                var comm = conn2.CreateCommand();
                //comm.CommandText = msql;
                using (var dr2 = comm.ExecuteReader())
                {
                    StringBuilder sb = new StringBuilder();
                    if (!dr2.HasRows)
                    {
                        //Response.Write("<Script language='JavaScript'>alert('查無此料號！！！');</Script>");
                        //txt_Msg.Text = "尚未寫入此單號！！！";
                    }
                    else
                    {
                        isInsert = true;
                        //while (dr.Read())
                        //{
                        //    sb.AppendLine(dr["料號"].ToString());

                        //    Console.WriteLine(dr["料號"].ToString());
                        //}
                        //Response.Write(sb.ToString());
                    }

                }
                if (!isInsert)//未寫入
                {
                    //msql = "INSERT INTO e01.tc_barcode_file SELECT ogb01,ogb03,ogb04,ogb11,ogb09,ogb091,ogb092,ogb12,ogb05,ogbud05,0,wmsys.wm_concat(oao06) FROM e01.ogb_file LEFT JOIN e01.oao_file ON oao01 = ogb01 AND oao03 = ogb03 WHERE ogb01 LIKE '%" + txtNo + "%' GROUP BY ogb01,ogb03,ogb04,ogb11,ogb09,ogb091,ogb092,ogb12,ogb05,ogbud05";
                    //comm.CommandText = msql;
                    try
                    {
                        comm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //txt_Msg.Text = "錯誤訊息 : " + ex.Message;
                    }
                    //txt_Msg.Text = "寫入此資料！！！";

                }
                else
                {
                    //msql = "Delete e01.tc_barcode_file WHERE tc_barcode01 LIKE '%" + txtNo + "%'";
                    //comm.CommandText = msql;
                    //comm.ExecuteNonQuery();
                    //msql = "INSERT INTO e01.tc_barcode_file SELECT ogb01,ogb03,ogb04,ogb11,ogb09,ogb091,ogb092,ogb12,ogb05,ogbud05,0,wmsys.wm_concat(oao06) FROM e01.ogb_file LEFT JOIN e01.oao_file ON oao01 = ogb01 AND oao03 = ogb03 WHERE ogb01 LIKE '%" + txtNo + "%' GROUP BY ogb01,ogb03,ogb04,ogb11,ogb09,ogb091,ogb092,ogb12,ogb05,ogbud05";
                    //comm.CommandText = msql;
                    //comm.ExecuteNonQuery();
                    //txt_Msg.Text = "更新此資料！！！";

                }
                conn2.Close();

            }

        }
    }
}
