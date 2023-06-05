using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class GameRoomDao
    {
        OracleConnectionPool connPool = OracleConnectionPool.Instance();
        public GameRoomVo GetGameRoomData(string ingameno)
        {
            GameRoomVo vo = new GameRoomVo();
            
            string sql = @"SELECT * FROM game_room WHERE game_no = ':ingameno'";
                   
            // 커넥션풀에서 오라클연결 객체를 대여
            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("ingameno", ingameno);

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    vo.GAME_NO = (short)reader["game_no"];
                    vo.GAME_NAME = reader["game_name"].ToString();                    
                    vo.USER_COUNT = reader["user_count"].ToString();
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
        public List<GameRoomVo> GetGameRoomList()
        {
            List<GameRoomVo> voList = new List<GameRoomVo>();

            string sql = "SELECT * FROM game_room";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GameRoomVo vo = new GameRoomVo();
                    vo.GAME_NO = (short)reader["game_no"];
                    vo.GAME_NAME = reader["game_name"].ToString();                    
                    vo.USER_COUNT = reader["user_count"].ToString();

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
        public int InsertGameRoom(string ingamename)
        {
            int nRow = -1;
            char defcount = '1';
            string sql = @"INSERT INTO game_room(game_no, game_name, user_count) " +
                        @"VALUES(game_no_sequence.next_val, :ingamename, :defcount)";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("ingamename", ingamename);
                cmd.Parameters.Add("defcount", defcount);

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
        public int InsertGameRoom(GameRoomVo vo)
        {
            int nRow = -1;
            char defcount = '1';
            string sql = @"INSERT INTO game_room(game_no, game_name, user_count) " +
                        @"VALUES(:gameNum, :ingamename, :defcount)";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("gameNum", vo.GAME_NO);
                cmd.Parameters.Add("ingamename", vo.GAME_NAME);
                cmd.Parameters.Add("defcount", defcount);

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
        public int UpdateGameRoom(GameRoomVo vo, short ingameno, string ingamename)
        {
            int nRow = -1;

            string sql = @"UPDATE game_room SET user_count = (SELECT COUNT(user_id) FROM userinfo WHERE game_no = :ingameno) " +
                         @"WHERE game_no = :ingameno AND game_name = :ingamename";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("ingameno", ingameno);
                cmd.Parameters.Add("ingamename", ingamename);

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
        public int DeleteGameRoom(string ingameno)
        {
            int nRow = -1;
            string sql = @"DELETE FROM game_room WHERE game_no = :ingameno";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("ingameno", ingameno);

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
