using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class GameSetDao
    {
        OracleConnectionPool connPool = OracleConnectionPool.Instance();

        public GameSetVo GetGameSetData(string ingamename)
        {
            GameSetVo vo = new GameSetVo();

            string sql = @"SELECT * FROM game_set WHERE game_name = :ingamename";

            // 커넥션풀에서 오라클연결 객체를 대여
            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("ingamename", ingamename);

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    vo.GAME_NAME = reader["game_name"].ToString();
                    vo.USER_MIN = reader["user_min"].ToString();
                    vo.USER_MAX = reader["user_max"].ToString();
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
        public List<GameSetVo> GetGameSetList()
        {
            List<GameSetVo> voList = new List<GameSetVo>();

            string sql = "SELECT * FROM game_set";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GameSetVo vo = new GameSetVo();
                    vo.GAME_NAME = reader["game_name"].ToString();
                    vo.USER_MIN = reader["user_min"].ToString();
                    vo.USER_MAX = reader["user_max"].ToString();

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
        public int InsertGameSet(GameSetVo vo, string ingamename, string inusermin, string inusermax)
        {
            int nRow = -1;
            string sql = @"INSERT INTO game_set(game_name, user_min, user_max) " +
                        @"VALUES(:ingamename, :inusermin, :inusermax)";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("ingamename", ingamename);
                cmd.Parameters.Add("inusermin", inusermin);
                cmd.Parameters.Add("inusermax", inusermax);

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
        public int UpdateGameSet(GameSetVo vo, string ingamename, string inusermin, string inusermax)
        {
            int nRow = -1;
            string sql = @"UPDATE game_set SET user_min = :inusermin, user_max = :inusermax " +
                        @"WHERE game_name = :ingamename";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("ingamename", ingamename);
                cmd.Parameters.Add("inusermin", inusermin);
                cmd.Parameters.Add("inusermax", inusermax);                

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
        public int DeleteGameSet(string ingamename)
        {
            int nRow = -1;
            string sql = @"DELETE FROM game_set WHERE game_name = :ingamename";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
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
    }
}
