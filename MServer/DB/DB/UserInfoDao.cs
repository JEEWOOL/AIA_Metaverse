using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class UserInfoDao
    {
        OracleConnectionPool connPool = OracleConnectionPool.Instance();

        // id로 해당 유저가 존재하는지 여부
        public UserInfoVo GetUserData(string inuserid, string inuserpass)
        {
            UserInfoVo userInfoVo = new UserInfoVo();
            string sql = @"SELECT * FROM userinfo WHERE user_id=:inuserid AND user_pass=:inuserpass";

            // 커넥션풀에서 오라클연결 객체를 대여
            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("inuserid", inuserid);
                cmd.Parameters.Add("inuserpass", inuserpass);                

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    userInfoVo.USER_ID = reader["user_id"].ToString();
                    userInfoVo.USER_PASS = reader["user_pass"].ToString();
                    userInfoVo.GAME_NO = reader["game_no"].ToString();
                    userInfoVo.COIN = reader["coin"].ToString();
                    userInfoVo.INVENT = reader["invent"].ToString();
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
            
            return userInfoVo;
        }

        // 전체 회원 리스트 
        public List<UserInfoVo> GetUserList()
        {
            List<UserInfoVo> voList = new List<UserInfoVo>();

            string sql = "SELECT * FROM userinfo";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    UserInfoVo userInfoVo = new UserInfoVo();
                    userInfoVo.USER_ID = reader["user_id"].ToString();
                    userInfoVo.USER_PASS = reader["user_pass"].ToString();
                    userInfoVo.GAME_NO = reader["game_no"].ToString();

                    voList.Add(userInfoVo);
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

        // 특정 유저가 게임룸에 진입
        public int UpdateUserJoin(string inuserid, string ingameno)
        {
            int nRow = -1;

            string sql = @"UPDATE userinfo SET game_no = :ingameno " +                          
                        @"WHERE user_id = :inuserid";


            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("inuserid", inuserid);
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

        // 특정 유저가 게임룸에서 나감
        public int UpdateUserOut(string inuserid)
        {
            int nRow = -1;
            string defgameno = " ";
            string sql = @"UPDATE userinfo SET game_no = :defgameno " +
                        @"WHERE user_id = :inuserid";


            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("inuserid", inuserid);
                cmd.Parameters.Add("defgameno", defgameno);

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

        // 회원정보 저장
        public int InsertUser(UserInfoVo vo)
        {
            int nRow = -1;
            string sql = @"INSERT INTO userinfo(user_id, user_pass, coin) " +
                         @"VALUES(:userid, :userpass, '5000')";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("userid", vo.USER_ID);
                cmd.Parameters.Add("userpass", vo.USER_PASS);

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

        // 특정 멤버 데이터 삭제
        public int DeleteUser(string inuserid, string inuserpass)
        {
            int nRow = -1;
            string sql = @"DELETE FROM userinfo WHERE user_id = :inuserid AND user_pass=:inuserpass";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("inuserid", inuserid);
                cmd.Parameters.Add("inuserpass", inuserpass);

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

        // coin 업데이트
        public int AddCoin(string inuserid, int incoin)
        {
            int nRow = -1;           
            string sql = @"UPDATE userinfo SET coin = coin + :incoin " +
                        @"WHERE user_id = :inuserid";


            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("incoin", incoin);
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
        public int RemoveCoin(string inuserid, int incoin)
        {
            int nRow = -1;
            string sql = @"UPDATE userinfo SET coin = coin - :incoin " +
                        @"WHERE user_id = :inuserid";


            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("incoin", incoin);
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
        // coin 불러오기
        public int LoadCoin(string inuserid)
        {
            int result = -1;
            string sql = @"SELECT coin FROM userinfo WHERE user_id = :inuserid";

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
                    result = int.Parse(reader["coin"].ToString());
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

            return result;
        }
    }
}
