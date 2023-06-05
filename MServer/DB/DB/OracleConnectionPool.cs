using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

/*
Singleton Pattern을 사용하는 이유
=> Singleton Pattern이란? 단 1개의 객체만 생성되도록 유도하는 방식
OracleConnectionPool객체를 여러 개 만들지 않고
단 1개의 Pool객체를 통해 OracleConnection객체를 제공받기 위해
Pool객체가 여기 저기 만들어지면 Connection관리에 문제가 생길 수 있다.

외부에서 new를 통해 객체를 만들지 못하도록 
생성자를 private설정을 한다.
*/
namespace DB
{
    public class OracleConnectionPool
    {
        // 객체를 생성 저장할 static 변수
        private static OracleConnectionPool staticOracleConnectionPool = null;

        string ip;
        int port;
        string service_name;
        string id;
        string password;
        private string OracleConnInfo;
        object lo = new object();

        // 오라클 서버 연결 객체를 저장할 스택공간
        Stack<OracleConnection> connStack = new Stack<OracleConnection>();

        // 외부에서 이 객체를 얻을 수 있는 메서드
        public static OracleConnectionPool Instance(string ip = "localhost",
                                                int port = 1521, string service_name = "xe",
                                                string id = "mproject", string pass = "mproject")
        {
            // 처음 Instance가 호출되었을 때 객체를 생성
            if (staticOracleConnectionPool == null)
            {
                staticOracleConnectionPool = new OracleConnectionPool(ip, port, service_name, id, pass);
            }

            return staticOracleConnectionPool;
        }


        // 생성자는 private으로
        private OracleConnectionPool(string ip, int port, string service_name, string id, string password)
        {
            this.ip = ip;
            this.port = port;
            this.service_name = service_name;
            this.id = id;
            this.password = password;

            this.OracleConnInfo = $"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={ip})(PORT={port})))" +
                $"(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={service_name}))); User Id = {id}; Password = {password};";
        }

        // Pool로부터 OracleConnection객체를 요청할 때 사용하는 메서드
        public OracleConnection GetConnection()
        {
            OracleConnection conn = null;

            // 스택에 저장된 연결 객체를 꺼내서 제공한다.
            lock (lo)
            {
                if (connStack.Count > 0)
                {
                    conn = connStack.Pop();
                    if (conn.State == ConnectionState.Open)     // 오라클 서버와 연결이 되는지?
                        return conn;
                }
            }

            conn = new OracleConnection(this.OracleConnInfo);
            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    DBDebug.WriteLog("Oracle Server 연결 성공");
                }
            }
            catch (Exception ex)
            {
                conn = null;
                DBDebug.WriteLog($"DB Exception : {ex.Message}");
            }

            return conn;
        }

        // 사용한 OracleConnection을 Pool객체에 반환하는 메서드
        public void ReturnOracleConnection(OracleConnection conn)
        {
            lock (lo)
            {
                if (conn.State == ConnectionState.Open)
                    this.connStack.Push(conn);
            }
        }

        // 어플리케이션이 종료될 때 모든 OracleConnection을 닫아주는 메서드
        public void CloseAll()
        {
            foreach (var conn in connStack)
                conn.Close();
        }
    }
}
