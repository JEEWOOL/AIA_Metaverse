using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class RoomDao
    {
        OracleConnectionPool connPool = OracleConnectionPool.Instance();

        public RoomVo GetRoomData(string inuserid)
        {
            RoomVo vo = new RoomVo();

            string sql = @"SELECT * FROM room WHERE user_id = :inuserid";

            // 커넥션풀에서 오라클연결 객체를 대여
            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("inuserid", inuserid);

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    vo.USER_ID = reader["user_id"].ToString();
                    vo.ROOM_DATA = reader["room_data"].ToString();                    
                }
            }
            catch (Exception ex)
            {
                DBDebug.WriteLog($"DB Exception : {ex.Message}");
            }
            finally
            {
                // 커넥션풀에 오라클연결 객체를 반환
                connPool.ReturnOracleConnection(conn);
            }

            return vo;
        }
        public List<RoomVo> GetRoomList()
        {
            List<RoomVo> voList = new List<RoomVo>();

            string sql = "SELECT * FROM room";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RoomVo vo = new RoomVo();
                    vo.USER_ID = reader["user_id"].ToString();
                    vo.ROOM_DATA = reader["room_data"].ToString();                    

                    voList.Add(vo);
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
        public int InsertRoom(RoomVo vo, string inuserid, string inroomdata)
        {
            int nRow = -1;
            string sql = @"INSERT INTO room(user_id, room_data) " +
                        @"VALUES(:inuserid, :inroomdata)";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("inuserid", vo.USER_ID);
                cmd.Parameters.Add("inroomdata", vo.ROOM_DATA);                                

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
        public int UpdateRoom(string inuserid, string inroomdata)
        {
            int nRow = -1;
            string sql = @"INSERT INTO room(user_id, room_data) " +
                        @"VALUES(:inuserid, :inroomdata)";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("inuserid", inuserid);
                cmd.Parameters.Add("inroomdata", inroomdata);

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
            if(nRow == 1)
                return 1;

            nRow = -1;
            sql = @"UPDATE room SET room_data = :inroomdata " +
                        @"WHERE user_id = :inuserid";

            conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("inuserid", inuserid);
                cmd.Parameters.Add("inroomdata", inroomdata);

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
        public int DeleteRoom(string inuserid)
        {
            int nRow = -1;
            string sql = @"DELETE FROM room WHERE user_id = :inuserid";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("inuserid", inuserid);

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
