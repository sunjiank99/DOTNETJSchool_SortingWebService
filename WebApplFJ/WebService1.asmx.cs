using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.Data.SqlClient;
using System.Configuration;

namespace WebApplFJ
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://scyueve.f3322.net/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {




        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        
        public class Goods
        {
            public string DrugCode;
            public string DrugName;
            public Decimal DrugPrice;
            public string Untis;  //物品单位
            public Decimal PuzzlePrice;//拼团单价

        }
        
        public class GoodsView
        {
            public string UserTel;
            public string UserName;
            public string DrugCode;
            public string DrugName;
            public Decimal DrugWeight;
            public string FJ_id;
            public DateTime FJ_time;
            public int packed;
            public Decimal DrugPrice;
            public Decimal TotalPrice;
            public Decimal PuzzlePrice;   //拼团单价
            public Decimal PuzzleTotlePrice; //拼团总价
            public string Units;//单位
        }
        [WebMethod]
        /// <summary>
        /// 验证用户名密码
        /// </summary>
        /// <param name="id">用户名</param>
        /// <param name="ps">密码</param>
        /// <returns>true正确 false 错误</returns>
        public bool verifyPassWord(string id, string ps)
        {
            
            string ConnString = ConfigurationManager.AppSettings["SCYueve"];
            string inputPs, readPS;
            bool returnVal;
            string ConnQuery = "select " + "UserPwd" + " from " + "ES_User" + " where " + "UserLogin" + "='" + id + "'";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();
            reader.Read();
            if (!reader.HasRows) //读取的行数
            {
                returnVal = false;
            }
            else
            {
                readPS = reader[0].ToString();
                inputPs = FormsAuthentication.HashPasswordForStoringInConfigFile(ps, "MD5");
                //string strMd5 = HashPasswordForStoringInConfigFile("123", "md5"); 


                if (readPS == inputPs)
                {
                    returnVal = true;

                }
                else
                {
                    returnVal = false;
                }



            }

            reader.Close();
            connection.Close();
            connection.Dispose();

            //ConnString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=sngoo712;Data Source=(local)";
            return returnVal;



        }

        [WebMethod]
        /// <summary>
        /// 验证是否有登入权限
        /// </summary>
        /// <param name="id">登入账号</param>
        /// <returns></returns>
        public bool verifyUserRole(string id)
        {

            string ConnString = ConfigurationManager.AppSettings["SCYueve"];
            //string inputPs, readPS;
            bool returnVal=false;
            string ConnQuery = "select " + "RoleNames" + " from " + "ES_User" + " where " + "UserLogin" + "='" + id + "'";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();
            reader.Read();
            string RoleRead = reader["RoleNames"].ToString();
            string[] RoleReadArray = RoleRead.Split(',');
            foreach (string i in RoleReadArray)
            {
                if (i == "配送经理" || i == "分拣员")
                { 
                    returnVal=true;
                    break;
                }
            }

          

            reader.Close();
            connection.Close();
            connection.Dispose();

            //ConnString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=sngoo712;Data Source=(local)";
            return returnVal;
        
        
        
        }


        [WebMethod]
        /// <summary>
        /// 读取数据库数据
        /// </summary>
        /// <param name="classname">要读取得列名</param>
        /// <param name="indexclass">索引列名</param>
        /// <param name="indexcontent">索引内容</param>
        /// <param name="tablename">要读取得表名</param>
        /// <returns></returns>
        public string searchContent(string classname, string indexclass, string indexcontent, string tablename)
        {
            string ConnString = ConfigurationManager.AppSettings["SCYueve"];
            string consequence;
            string ConnQuery = "select " + classname + " from " + tablename + " where " + indexclass + "='" + indexcontent + "'";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();
            reader.Read();

            consequence = reader[0].ToString();
            if (consequence == null)
            {
                consequence = "-1";
            }
            reader.Close();
            connection.Close();
            connection.Dispose();

            //ConnString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=sngoo712;Data Source=(local)";
            return consequence;




        }



        [WebMethod]
        /// <summary>
        /// 读取货物信息
        /// </summary>
        /// <returns></returns>
        public List<Goods> ReadGoods(string fruitClass)
        {
            string ConnString = ConfigurationManager.AppSettings["SCYueve"];
           
            List<Goods> cacheList = new List<Goods>();

            string ConnQuery = "select " + "*" + " from " + "物品信息_主表 where 物品分类='"+fruitClass+"'";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader["物品类型"].ToString() == "商品")
                {
                    Goods cache = new Goods();
                    cache.DrugCode = reader["物品编号"].ToString();
                    cache.DrugName = reader["物品名称"].ToString();
                    cache.DrugPrice = Convert.ToDecimal(reader["物品单价"]);
                    if (reader["拼团单价"] == DBNull.Value)
                    {
                        cache.PuzzlePrice = 0;
                    }
                    else
                    {
                        cache.PuzzlePrice = Convert.ToDecimal(reader["拼团单价"]);
                    }
                    cache.Untis = reader["物品单位"].ToString();

                    cacheList.Add(cache);

                }


            }



            reader.Close();
            connection.Close();
            connection.Dispose();

            return cacheList;






        }

        [WebMethod]
        /// <summary>
        /// 写入日分拣数据表
        /// </summary>
        /// <param name="cache"> 写入的分拣信息</param>
        public void WriteIntoFJ(GoodsView info)
        {

            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            string ConnQuery = " insert into fj_GoodsInfo (UserTel,UserName,DrugCode,DrugName,DrugWeight,FJ_id,FJ_time,Packed,DrugPrice,TotalPrice,PuzzlePrice,PuzzleTotlePrice,Units)values('" + info.UserTel + "','" + info.UserName + "','" + info.DrugCode + "','" + info.DrugName + "','" + info.DrugWeight + "','" + info.FJ_id + "','" + info.FJ_time + "','" + info.packed + "','" + info.DrugPrice + "','" + info.TotalPrice + "','" + info.PuzzlePrice + "','"+info.PuzzleTotlePrice + "','"+info.Units+ "')";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            lo_cmd.ExecuteNonQuery();




            connection.Close();
            connection.Dispose();




        }



        [WebMethod]
        /// <summary>
        /// 读取分拣订单信息
        /// </summary>
        /// <param name="StarTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <returns></returns>

        public List<GoodsView> readFJINFO(string StarTime, string EndTime,string Username)
        {

            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            List<GoodsView> cacheRead = new List<GoodsView>();


            string ConnQuery = "select * from fj_GoodsInfo where FJ_time between'" + StarTime + "' and'" + EndTime + "'" + "and  UserName='"+Username+"'";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();

            if (!reader.HasRows) //读取的行数
            {
                cacheRead = null;
            }
            else
            {

                while (reader.Read())
                {
                    GoodsView cacheItem = new GoodsView();
                    cacheItem.DrugCode = reader["DrugCode"].ToString();
                    cacheItem.DrugName = reader["DrugName"].ToString();
                    cacheItem.DrugWeight = Convert.ToDecimal(reader["DrugWeight"]);
                    cacheItem.FJ_id = reader["FJ_id"].ToString();
                    cacheItem.FJ_time = Convert.ToDateTime(reader["FJ_time"]);
                    cacheItem.packed = Convert.ToInt32(reader["Packed"]);
                    cacheItem.UserName = reader["UserName"].ToString();
                    cacheItem.UserTel = reader["UserTel"].ToString();

                    if (reader["DrugPrice"].ToString() != "")
                    {

                        cacheItem.DrugPrice = Convert.ToDecimal(reader["DrugPrice"]);

                    }
                    else
                    {
                        cacheItem.DrugPrice = 0;
                    }

                    if (reader["TotalPrice"].ToString() != "")
                    {
                        cacheItem.TotalPrice = Convert.ToDecimal(reader["TotalPrice"]);
                    }
                    else
                    {
                        cacheItem.TotalPrice = 0;
                    }
                    if (reader["PuzzlePrice"].ToString() != "")
                    {
                        cacheItem.PuzzlePrice = 0;
                    }
                    else
                    {
                        cacheItem.PuzzlePrice = Convert.ToDecimal(reader["PuzzlePrice"]);
                    }

                    if (reader["PuzzleTotlePrice"].ToString() != "")
                    {
                        cacheItem.PuzzleTotlePrice = 0;
                    }
                    else
                    {
                        cacheItem.PuzzleTotlePrice = Convert.ToDecimal(reader["PuzzleTotlePrice"]);
                    }

                    if (reader["Units"] == DBNull.Value)
                    {

                        cacheItem.Units = "斤";
                    }
                    else
                    {
                        cacheItem.Units = reader["Units"].ToString();
                    }
                    cacheRead.Add(cacheItem);

                }





            }

            reader.Close();
            connection.Close();
            connection.Dispose();
            return cacheRead;

        }
        [WebMethod]
        /// <summary>
        /// 从数据库删除指定的分拣信息
        /// </summary>
        /// <param name="FJ_id"></param>
        public void Delete(string FJ_id)
        {

            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            string ConnQuery = " delete  fj_GoodsInfo where FJ_id='" + FJ_id + "'";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            lo_cmd.ExecuteNonQuery();




            connection.Close();
            connection.Dispose();



        }
        [WebMethod]
        /// <summary>
        /// 修改重量
        /// </summary>
        /// <param name="FJ_id">分拣号码</param>
        /// <param name="weight">重量</param>

        public void ALterWeight(string FJ_id, double weight,double TotalPrice,double PuzzleTotlePrice)
        {
            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            string ConnQuery = "  update fj_GoodsInfo set DrugWeight='" + weight + "' where FJ_id='" + FJ_id + "' \n";
            ConnQuery += "update fj_GoodsInfo set TotalPrice='" + TotalPrice + "'where FJ_id='" + FJ_id + "' \n";
            ConnQuery += "update fj_GoodsInfo set PuzzleTotlePrice='" + PuzzleTotlePrice + "'where FJ_id='" + FJ_id + "' ";
          
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            lo_cmd.ExecuteNonQuery();




            connection.Close();
            connection.Dispose();


        }
        [WebMethod]
        /// <summary>
        /// 根据分拣账号查询信息
        /// </summary>
        /// <param name="FJ_id">分拣账号</param>
        /// <param name="UserName">分拣人姓名</param>
        /// <returns></returns>
        public GoodsView searcheGoods(string FJ_id, string UserName)
        {


            string ConnString = ConfigurationManager.AppSettings["Sngoo"];
            GoodsView consequence=new GoodsView();
            string ConnQuery = "select " + "*" + " from " + "fj_GoodsInfo" + " where " + "FJ_id" + "='" + FJ_id + "'"+"and UserName='"+UserName+"'";
            SqlConnection connection = new SqlConnection(ConnString);
            connection.Open();
            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);
            SqlDataReader reader = lo_cmd.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {

                consequence.DrugCode = reader["DrugCode"].ToString();
                consequence.DrugName = reader["DrugName"].ToString();
                consequence.DrugPrice = Convert.ToDecimal(reader["DrugPrice"]);
                consequence.DrugWeight = Convert.ToDecimal(reader["DrugWeight"]);
                consequence.FJ_id = reader["FJ_id"].ToString();
                consequence.FJ_time = Convert.ToDateTime(reader["FJ_time"]);
                consequence.packed = Convert.ToInt32(reader["Packed"]);
                consequence.TotalPrice = Convert.ToDecimal(reader["TotalPrice"]);
                consequence.UserName = reader["UserName"].ToString();
                consequence.UserTel = reader["UserTel"].ToString();
                consequence.PuzzlePrice = Convert.ToDecimal(reader["PuzzlePrice"]);
                consequence.PuzzleTotlePrice = Convert.ToDecimal(reader["PuzzleTotlePrice"]);
                consequence.Units = reader["Units"].ToString();

            }
            else
            {
                consequence = null;
            }
            reader.Close();
            connection.Close();
            connection.Dispose();
            return consequence;
        
        
        }
        [WebMethod]

        /// <summary>
        /// 返回水果分类表
        /// </summary>
        /// <returns></returns>
        public List<string> GetGoodsClass()
        {


            List<string> returnVal = new List<string>();
            List<string> readGoodsClass = new List<string>();

            string ConnString = ConfigurationManager.AppSettings["SCYueve"];  //设置数据库连接字符

            SqlConnection connection = new SqlConnection(ConnString);       //启动连接
            connection.Open();
            //string inputPs, readPS;
            string ConnQuery = "  select 物品分类 from 物品信息_主表 ";  //设置查询语句

            SqlCommand lo_cmd = new SqlCommand(ConnQuery, connection);

            SqlDataReader reader = lo_cmd.ExecuteReader();
            //reader.Read();
            //returnVal.Add(reader[0].ToString());



            while (reader.Read())
            {

                readGoodsClass.Add(reader[0].ToString());
            }

            var result1 = from s in readGoodsClass group s by s;


            foreach (var s in result1)
            {
                returnVal.Add(s.Key);
            }

             
            






                reader.Close();
            lo_cmd.Dispose();

            return returnVal;


        }
    }
}
