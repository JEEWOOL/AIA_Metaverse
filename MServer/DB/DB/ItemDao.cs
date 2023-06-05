using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class ItemDao
    {
        OracleConnectionPool connPool = OracleConnectionPool.Instance();

        // 전체 아이템 리스트
        public List<ItemVo> GetItemList()
        {
            List<ItemVo> voList = new List<ItemVo>();

            string sql = "SELECT * FROM item";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ItemVo itemVo = new ItemVo();
                    itemVo.ITEM_CODE = reader["item_code"].ToString();
                    itemVo.ITEM_NAME = reader["item_name"].ToString();


                    voList.Add(itemVo);
                }
            }
            catch (Exception ex)
            {
                DBDebug.WriteLog($"DB Exception : {ex.Message}");
            }
            finally
            {
                connPool.ReturnOracleConnection(conn);
            }

            return voList;
        }

        // 아이템 추가
        public int InsertItem(ItemVo vo, string initemcode, string initemname)
        {
            int nRow = -1;
            string sql = @"INSERT INTO item(item_code, item_name) " +
                        @"VALUES(:initemcode, :initemname)";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("initemcode", initemcode);
                cmd.Parameters.Add("initemname", initemname);                

                nRow = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                DBDebug.WriteLog($"DB Exception : {ex.Message}");
            }
            finally
            {
                connPool.ReturnOracleConnection(conn);
            }

            return nRow;
        }

        // 아이템 수정
        public int UpdateItem(ItemVo vo, string initemcode, string initemname)
        {
            int nRow = -1;
            string sql = @"UPDATE item SET item_name = :initemname " +
                        @"WHERE item_code = :initemcode";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("initemcode", initemcode);
                cmd.Parameters.Add("initemname", initemname);                              

                nRow = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                DBDebug.WriteLog($"DB Exception : {ex.Message}");
            }
            finally
            {
                connPool.ReturnOracleConnection(conn);
            }

            return nRow;
        }

        // 아이템 삭제
        public int DeleteItem(string initemname)
        {
            int nRow = -1;
            string sql = @"DELETE FROM item WHERE item_name = :initemname";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("initemname", initemname);

                nRow = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                DBDebug.WriteLog($"DB Exception : {ex.Message}");
            }
            finally
            {
                connPool.ReturnOracleConnection(conn);
            }

            return nRow;
        }
    }
}
