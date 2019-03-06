using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using GPSAPI.Models;
using Oracle.ManagedDataAccess.Client;
using Newtonsoft.Json;

namespace GPSAPI.Controllers
{
    public class ValuesController : ApiController
    {
        string connstr = "User Id=system;Password=jim930527;Data Source=localhost:1521/jimdb;";
        Configuration webconn = WebConfigurationManager.OpenWebConfiguration("/Web.config");
        ConnectionStringSettings connStringSetting;
        string connString;
        private List<Locations> locationList = new List<Locations>();
        // GET values/Locations
        [HttpGet]
        [ActionName("Locations")]
        public IEnumerable<Locations> Get()
        {
            return Select_Sales_Locate_List("");
        }

        [HttpGet]
        [ActionName("Id")]
        // GET values/Id
        public IEnumerable<Locations> Get(string id)
        {
            return Select_Sales_Locate_List(id);
        }

        // POST values/Update
        [HttpPost]
        [ActionName("Update")]
        public List<Locations> Post([FromBody]List<Locations> _locations)      //以json[] 接收
        //public Locations Post([FromBody]string _locations)    //以text 接收
        {
            Locations locations = new Locations();
            //locations = JsonConvert.DeserializeObject<Locations>(_locations); //以text 接收
            //locationList.Add(locations);  //以text 接收

            locationList = _locations ?? throw new HttpRequestException();
            Insert_Sales_Locate(locationList);
            return _locations;
        }

        [HttpPut]
        // PUT values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE values/Delete/5
        [HttpDelete]
        [ActionName("Delete")]
        public string Delete(string id)
        {
            Delete_Sales_Locate(id);
            return "Delete the Id : " + id;
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
        public IEnumerable<Locations> Select_Sales_Locate_List(string str_where)
        {
            string msql_init = "";
            if (str_where == "")
            {
                msql_init = "SELECT * FROM JIMDB.SALES_LOCATE ORDER BY serialNo DESC ";
            }
            else
            {
                msql_init = "SELECT * FROM JIMDB.SALES_LOCATE WHERE 1 = 1 AND deviceNo = '" + str_where + "' ORDER BY serialNo DESC";
            }
            connStringSetting = webconn.ConnectionStrings.ConnectionStrings["OracleDbContext"];
            //string connString = connStringSetting.ConnectionString;
            using (var conn_sel = new OracleConnection(connstr))
            {
                conn_sel.Open();
                var comm_sel = conn_sel.CreateCommand();
                comm_sel.CommandText = msql_init;
                using (var dr_sel = comm_sel.ExecuteReader())
                {
                    StringBuilder sb_sel = new StringBuilder();
                    while (dr_sel.Read())
                    {
                        locationList.Add(
                            new Locations
                            {
                                serialNo = dr_sel["serialNo"].ToString(),
                                deviceNo = dr_sel["deviceNo"].ToString(),
                                telNo = dr_sel["telNo"].ToString(),
                                imeiNo = dr_sel["imeiNo"].ToString(),
                                longitude = Double.Parse(dr_sel["longitude"].ToString()),
                                latitude = Double.Parse(dr_sel["latitude"].ToString()),
                                updateTime = dr_sel["updateTime"].ToString()
                            }
                        );
                    }
                }
                return locationList;
            }
        }

        public void Insert_Sales_Locate(List<Locations> locations)
        {
            if(locations.Count == 0)
            {
                Console.WriteLine("輸入失敗");
            }
            else
            {
                bool isInsert = false;
                //string msql_select = "SELECT DEVICENO FROM JIMDB.SALES_LOCATE WHERE deviceNo = '" + deviceNo + "'";
                using (var conn_ins = new OracleConnection(connstr))
                {
                    conn_ins.Open();
                    var comm_ins = conn_ins.CreateCommand();
                    //comm_ins.CommandText = msql_select;
                    //using (var dr_ins = comm_ins.ExecuteReader())
                    //{
                    //    StringBuilder sb = new StringBuilder();
                    //    if (!dr_ins.HasRows)
                    //    {
                    //        Console.WriteLine("查無此料號！！！");
                    //    }
                    //    else
                    //    {
                    //        isInsert = true;
                    //    }
                    //}
                    //if (!isInsert)//未寫入
                    //{
                    string msql_insert = "INSERT ALL ";
                    for (int i = 0; i <locations.Count; i++)
                    {
                        msql_insert +=
                            "INTO jimdb.sales_locate (serialNo,deviceNo,telNo,imeiNo,longitude,latitude,updatetime) VALUES ('" +
                            locations[i].serialNo + "','" +
                            locations[i].deviceNo + "','" +
                            locations[i].telNo + "','" +
                            locations[i].imeiNo + "'," +
                            locations[i].longitude + "," +
                            locations[i].latitude + "," +
                            "TO_DATE('" + locations[i].updateTime + "', 'YYYY-MM-DD hh24:mi:ss'))  ";
                    }
                    msql_insert += " SELECT * FROM dual";
                    comm_ins.CommandText = msql_insert;
                    try
                    {
                        comm_ins.ExecuteNonQuery();
                        Console.WriteLine("成功輸入");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("輸入失敗");
                    }
                    conn_ins.Close();

                }
            
            }
        }

        public void Delete_Sales_Locate(string deviceNo)
        {
            string msql_delete = "Delete jimdb.sales_locate WHERE deviceNo LIKE '%" + deviceNo + "%'";
            using (var conn_del = new OracleConnection(connstr))
            {
                conn_del.Open();
                var comm_del = conn_del.CreateCommand();
                comm_del.CommandText = msql_delete;
                comm_del.ExecuteNonQuery();
                Console.WriteLine("刪除成功");
                conn_del.Close();
            }
        }
    }
}
