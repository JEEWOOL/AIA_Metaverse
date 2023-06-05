using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class CharacterDao
    {
        OracleConnectionPool connPool = OracleConnectionPool.Instance();

        // id로 해당 캐릭터가 존재하는지 여부
        public CharacterVo GetCharacterData(string inuserid)
        {
            CharacterVo characterVo = new CharacterVo();

            string sql = @"SELECT * FROM character WHERE user_id = :inuserid";

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
                    characterVo.CHAR_NO = reader["char_no"].ToString();
                    characterVo.COIN = reader["coin"].ToString();
                    characterVo.USER_ID = reader["user_id"].ToString();
                    characterVo.INVENT = reader["invent"].ToString();
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

            return characterVo;
        }
        // 전체 캐릭터 리스트 
        public List<CharacterVo> GetCharacterList()
        {
            List<CharacterVo> voList = new List<CharacterVo>();

            string sql = "SELECT * FROM character";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CharacterVo characterVo = new CharacterVo();
                    characterVo.CHAR_NO = reader["char_no"].ToString();
                    characterVo.COIN = reader["coin"].ToString();
                    characterVo.USER_ID = reader["user_id"].ToString();
                    characterVo.INVENT = reader["invent"].ToString();

                    voList.Add(characterVo);
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
        // 캐릭터정보 저장
        public int InsertCharacter(CharacterVo vo)
        {
            int nRow = -1;
            string sql = @"INSERT INTO character(char_no, coin, user_id, invent) " +
                        @"VALUES(char_no_sequence.next_val, :user_coin, :userid, :user_invent)";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("user_coin", vo.COIN);
                cmd.Parameters.Add("userid", vo.USER_ID);
                cmd.Parameters.Add("user_invent", vo.INVENT);

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

        // 특정 캐릭터 데이터 수정
        public int UpdateCharacter(CharacterVo vo, string inusercoin, string inuserinvent, string inuserid)
        {
            int nRow = -1;
            string sql = @"UPDATE character SET coin=:inusercoin, invent=:inuserinvent " +
                        @"WHERE user_id=:inuserid";

            OracleConnection conn = connPool.GetConnection();

            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add("inusercoin", inusercoin);                
                cmd.Parameters.Add("inuserinvent", inuserinvent);
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
        // 특정 캐릭터 데이터 삭제
        public int DeleteCharacter(string inuserid)
        {
            int nRow = -1;
            string sql = @"DELETE FROM character WHERE user_id = :inuserid";

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
