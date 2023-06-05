using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FreeNet
{
    public class CNetworkService
    {
		int connected_count;
		CListener client_listener;
		SocketAsyncEventArgsPool receive_event_args_pool;
		SocketAsyncEventArgsPool send_event_args_pool;
		BufferManager buffer_manager;

		public delegate void SessionHandler(CUserToken token);
		public SessionHandler session_created_callback { get; set; }

		// configs.
		int max_connections;
		int buffer_size;
		readonly int pre_alloc_count = 2;		// read, write

		public CNetworkService()
		{
			this.connected_count = 0;
			this.session_created_callback = null;
		}

		// Initializes the server by preallocating reusable buffers and 
		// context objects.  These objects do not need to be preallocated 
		// or reused, but it is done this way to illustrate how the API can 
		// easily be used to create reusable objects to increase server performance.
		//
		public void initialize()
		{
			this.max_connections = 10000;		// 최대 클라이언트 접속 허용 수치
			this.buffer_size = 1024;

			// 10000 * 1024 * 2, 1024
			// 전송/수신시 데이터를 저장할 공간을 미리 크게 확보
			this.buffer_manager = new BufferManager(this.max_connections * this.buffer_size * this.pre_alloc_count, this.buffer_size);
			// 비동기 통신에서는 언제 완료될지 모르므로
			// 콜백함수를 등록하거나 이벤트객체의 이벤트 알림을 통해 완료를 알고
			// 처리하게 된다.

			// 수신 관련 이벤트 Pool(비동기 통신에 필요한 SocketAsyncEventArgs객체 10000개 저장소풀 생성)
			this.receive_event_args_pool = new SocketAsyncEventArgsPool(this.max_connections);
            // 전송 관련 이벤트 Pool(비동기 통신에 필요한 SocketAsyncEventArgs객체 10000개 저장소풀 생성)
            this.send_event_args_pool = new SocketAsyncEventArgsPool(this.max_connections);

			// Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds 
			// against memory fragmentation
			this.buffer_manager.InitBuffer();

			// preallocate pool of SocketAsyncEventArgs objects
			SocketAsyncEventArgs arg;

            /*
			10000명의 클라이언트와의 비동기 통신을 위해서는 .NET에
            SocketAsyncEventArgs 객체를 사용해야 한다.
			SocketAsyncEventArgs객체에는 3가지 정보를 전달한다
			  1) 통신이 완료되었을 때 호출될 이벤트 핸들러 메서드(receive_completed/send_completed)
			  2) 누구와 통신하는지에 대한 클라이언트 연결 정보(CUserToken객체)
			  3) .NET에서 수신/송신시 저장할 byte 배열 공간(BufferManager의 공간의 시작/끝 위치)
			*/

            /* [비동기 통신을 위한 객체 준비 작업]
             * 10000개의 클라이언트와의 비동기 수신/송신작업을 하기 위해
			 * 각각 수신 args, 송신 args를 생성해서
			 * receive_event_args_pool에 수신 args객체 저장
			 * send_event_args_pool에 송신 args객체 저장
			 */
            for (int i = 0; i < this.max_connections; i++)
			{
				// 동일한 소켓에 대고 send, receive를 하므로
				// user token은 세션별로 하나씩만 만들어 놓고 
				// receive, send EventArgs에서 동일한 token을 참조하도록 구성한다.

				// 클라이언트 연결 정보를 의미
				CUserToken token = new CUserToken();

				// receive pool
				{
					// .NET의 비동기 통신에 사용하는 객체이다.
					//Pre-allocate a set of reusable SocketAsyncEventArgs
					arg = new SocketAsyncEventArgs();
					// 비동기 통신이 완료되면 호출되는 메서드를 등록
					arg.Completed += new EventHandler<SocketAsyncEventArgs>(receive_completed);
					// 비동기 통신이 완료되었을 때 전달할 객체를 등록
					arg.UserToken = token;

					// 비동기 통신에서 사용할, 즉 닷넷이 넘겨주는 데이터를 저장할
					// 공간을 BufferManager객체로부터 위치 영역 정해줌
					// assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
					this.buffer_manager.SetBuffer(arg);

					// args의 설정이 완료되면 일단 receive_event_args_pool에 저장해둔다.
					// add SocketAsyncEventArg to the pool
					this.receive_event_args_pool.Push(arg);
				}


				// send pool
				{
					//Pre-allocate a set of reusable SocketAsyncEventArgs
					arg = new SocketAsyncEventArgs();
					// 비동기 송신이 완료되면 호출된 콜백함수 등록
					arg.Completed += new EventHandler<SocketAsyncEventArgs>(send_completed);
					// 클라이언트 연결 정보(콜백함수 호출될 때 함께 전달됨)
					arg.UserToken = token;

					// assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
					// 비동기 통신에 사용될 버퍼 공간 영역 지정해주기
					this.buffer_manager.SetBuffer(arg);

					// add SocketAsyncEventArg to the pool
					// 생성된 비동기 송신 객체 pool에 저장
					this.send_event_args_pool.Push(arg);
				}
			}
		}

		public void listen(string host, int port, int backlog)
		{
			this.client_listener = new CListener();
			// 클라이언트 accept 완료 시 호출되는 콜백 메서드
			this.client_listener.callback_on_newclient += on_new_client;
			// 서버를 가동
			this.client_listener.start(host, port, backlog);
		}

		/// <summary>
		/// todo:검토중...
		/// 원격 서버에 접속 성공 했을 때 호출됩니다.
		/// </summary>
		/// <param name="socket"></param>
		public void on_connect_completed(Socket socket, CUserToken token)
		{
			// SocketAsyncEventArgsPool에서 빼오지 않고 그때 그때 할당해서 사용한다.
			// 풀은 서버에서 클라이언트와의 통신용으로만 쓰려고 만든것이기 때문이다.
			// 클라이언트 입장에서 서버와 통신을 할 때는 접속한 서버당 두개의 EventArgs만 있으면 되기 때문에 그냥 new해서 쓴다.
			// 서버간 연결에서도 마찬가지이다.
			// 풀링처리를 하려면 c->s로 가는 별도의 풀을 만들어서 써야 한다.
			SocketAsyncEventArgs receive_event_arg = new SocketAsyncEventArgs();
			receive_event_arg.Completed += new EventHandler<SocketAsyncEventArgs>(receive_completed);
			receive_event_arg.UserToken = token;
			receive_event_arg.SetBuffer(new byte[1024], 0, 1024);

			SocketAsyncEventArgs send_event_arg = new SocketAsyncEventArgs();
			send_event_arg.Completed += new EventHandler<SocketAsyncEventArgs>(send_completed);
			send_event_arg.UserToken = token;
			send_event_arg.SetBuffer(new byte[1024], 0, 1024);

			begin_receive(socket, receive_event_arg, send_event_arg);
		}

        /// <summary>
        /// 새로운 클라이언트가 접속 성공 했을 때 호출됩니다.
        /// AcceptAsync의 콜백 매소드에서 호출되며 여러 스레드에서 동시에 호출될 수 있기 때문에 공유자원에 접근할 때는 주의해야 합니다.
        /// </summary>
        /// <param name="client_socket"></param>
		void on_new_client(Socket client_socket, object token)
		{
            //todo:
            // peer list처리.

            // 멀티스레드에 안전하게 this.connected_count의 값을 1 증가
			// 연결 클라이언트 숫자 1 증가
            Interlocked.Increment(ref this.connected_count);

			Console.WriteLine(string.Format("[{0}] A client connected. handle {1},  count {2}",
				Thread.CurrentThread.ManagedThreadId, client_socket.Handle,
				this.connected_count));

			// 연결이 이루어진 클라이언트와 수신/송신 비동기 통신을 해야 한다.
			// 그러므로 Pool에서 각각 꺼낸다.
			// 플에서 하나 꺼내와 사용한다.
			SocketAsyncEventArgs receive_args = this.receive_event_args_pool.Pop();
			SocketAsyncEventArgs send_args = this.send_event_args_pool.Pop();

			CUserToken user_token = null;
			if (this.session_created_callback != null)
			{
                /*
                 * send_args나 receive_args는 동일한 user_token객체를 가지고 있으므로
                 * 어느 것을 사용해도 무방하다.
                 * user_token = send_args.UserToken as CUserToken;
                 */
                user_token = receive_args.UserToken as CUserToken;
				this.session_created_callback(user_token);
			}

            // 연결된 클라이언트에 대해서 비동기 수신/송신을 .NET에 등록한다.
            /* client_socket : 클라이언트와 연결된 통신 소켓
			 * receive_args : 클라이언트와 비동기 수신에 사용한 SocketAsyncEventArgs객체
			 * send_args : 클라이언트와 비동기 송신에 사용한 SocketAsyncEventArgs객체
			 */
            begin_receive(client_socket, receive_args, send_args);
			//user_token.start_keepalive();
		}

		void begin_receive(Socket socket, SocketAsyncEventArgs receive_args, SocketAsyncEventArgs send_args)
		{
			// receive_args, send_args 아무곳에서나 꺼내와도 된다. 둘다 동일한 CUserToken을 물고 있다.
			CUserToken token = receive_args.UserToken as CUserToken;

            /*
             * SocketAsyncEventArgs객체를 통해 CUserToken객체를 얻고 싶을 때
			 *   receive_args.UserToken
			 *   send_args.UserToken
			 * CUserToken객체를 통해 receive_args객체나 send_args객체를 얻고 싶을 때
			 *   token.receive_event_args
			 *   token.send_event_args
			 */
            token.set_event_args(receive_args, send_args);
			// 생성된 클라이언트 소켓을 보관해 놓고 통신할 때 사용한다.
			token.socket = socket;

			//.NET한테 이 클라이언트에서 수신이 발생하면 처리할 정보(receive_args)를 전달등록한다.
			bool pending = socket.ReceiveAsync(receive_args);

			/* pending
			 *  - true : .net에 의해 비동기 수신 진행중...(아직 수신 안됨)
			 *  - false : .net에 비동기 수신 등록중(즉, ReceiveAsync호출하는 중)에 
			 *             연결된 클라이언트에서 데이터가 수신되었다면
			 */

			// 직접 수신 처리
			if (!pending)
			{
				process_receive(receive_args);
			}
		}

		// This method is called whenever a receive or send operation is completed on a socket 
		//
		// <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
		void receive_completed(object sender, SocketAsyncEventArgs e)
		{
			if (e.LastOperation == SocketAsyncOperation.Receive)
			{
                /*
                SocketAsyncEventArgs e 객체는
				socket.ReceiveAsync(receive_args)에서 전달한 receive_args이다.
                e 객체에는 추가적으로  .NET에 의해 처리된 결과까지 포함되어 전달된다.
				그래서 이 e객체를 매개변수로 넘겨준다.
				*/
                process_receive(e);
				return;
			}

			throw new ArgumentException("The last operation completed on the socket was not a receive.");
		}

		// This method is called whenever a receive or send operation is completed on a socket 
		//
		// <param name="e">SocketAsyncEventArg associated with the completed send operation</param>
		void send_completed(object sender, SocketAsyncEventArgs e)
		{
			CUserToken token = e.UserToken as CUserToken;
			token.process_send(e);
		}

		// This method is invoked when an asynchronous receive operation completes. 
		// If the remote host closed the connection, then the socket is closed.  
		//
		private void process_receive(SocketAsyncEventArgs e)
		{
			// check if the remote host closed the connection
			CUserToken token = e.UserToken as CUserToken;

            // 0 byte이상 수신되었고, 소켓수신이 성공적이었다.
            /* 클라이언트와 접속종료되는 2가지 경우
			 * 1) 강제종료
			 *    클라이언트의 프로세스가 갑자기 종료 or
			 *    네트워크 망의 문제
			 *    e.SocketError == SocketError.SocketError
			 * 2) 정상종료
			 *    클라이언트의 소켓이 Close()를 호출하는 경우
			 *    e.BytesTransferred == 0 이 된다.
			 *    
			 * 다만, 위의 1), 2) 경우 모두 클라이언트와 통신이 종료된 경우라서
			 * 서버의 클라이언트 연결 소켓도 닫아주는 공통 처리를 하게 된다.			 *   
			 */
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
			{
				// 정상적으로 수신된 경우이므로 token객체의 on_receive에서 수신 처리를 한다.
				/* SocketAsyncEventArgs객체를 생성해서 전달할 때 아래처럼 
                 * 이전에 args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
                 * buffer의 사용 영역을 지정했는데 이 정보를 
                 * e 매개변수에서 아래처럼 사용할 수 있다
                 * 단, e.BytesTransferred는 클라이언트로부터 수신된 byte 크기를 의미한다.
                 * */
				token.on_receive(e.Buffer, e.Offset, e.BytesTransferred);

				// 다시 .NET에 클라이언트 연결소켓에 비동기 수신처리를 등록해 놓는다.
				bool pending = token.socket.ReceiveAsync(e);
				if (!pending)
				{
					// Oh! stack overflow??
					process_receive(e);
				}
			}
			else
			{
				Console.WriteLine(string.Format("error {0},  transferred {1}", e.SocketError, e.BytesTransferred));
				close_clientsocket(token);
			}
		}

		public void close_clientsocket(CUserToken token)
		{
			token.on_removed();

			// Free the SocketAsyncEventArg so they can be reused by another client
			// 버퍼는 반환할 필요가 없다. SocketAsyncEventArg가 버퍼를 물고 있기 때문에
			// 이것을 재사용 할 때 물고 있는 버퍼를 그대로 사용하면 되기 때문이다.
			if (this.receive_event_args_pool != null)
			{
				this.receive_event_args_pool.Push(token.receive_event_args);
			}

			if (this.send_event_args_pool != null)
			{
				this.send_event_args_pool.Push(token.send_event_args);
			}
		}
    }
}
