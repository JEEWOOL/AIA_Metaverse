using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FreeNet
{
	class CListener
	{
        // 비동기 Accept를 위한 EventArgs.
		SocketAsyncEventArgs accept_args;

		Socket listen_socket;

        // Accept처리의 순서를 제어하기 위한 이벤트 변수.
		AutoResetEvent flow_control_event;

        // 새로운 클라이언트가 접속했을 때 호출되는 콜백.
		public delegate void NewclientHandler(Socket client_socket, object token);
		public NewclientHandler callback_on_newclient;

        public CListener()
        {
			this.callback_on_newclient = null;
        }

		public void start(string host, int port, int backlog)
		{
			// 소켓 객체 생성
			// IPv4, TCP
			// 소켓은 Application계층과 .NET내부의 Transport 계층을 연결해주는 역할
			this.listen_socket = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);

			IPAddress address;
			if (host == "0.0.0.0")
			{
				//서버 프로세스가 돌아가는 Host의 IP주소를 부여
				address = IPAddress.Any;
			}
			else
			{
				// 정해준 서버 IP주소를 부여
				address = IPAddress.Parse(host);
			}

			// 주소 구조체(IP, Port)
			IPEndPoint endpoint = new IPEndPoint(address, port);

			try
			{
				// 소켓에 주소를 부여(휴대폰에 전화번호 부여)
				listen_socket.Bind(endpoint);
				// 서버가 접속 대기가능 상태로 진입
				listen_socket.Listen(backlog);

				// 클라이언트 비동기 연결에 필요한 객체 생성
				this.accept_args = new SocketAsyncEventArgs();
				// 비동기 연결이 완료되었을 때 호출된 콜백함수 등록
				this.accept_args.Completed += new EventHandler<SocketAsyncEventArgs>(on_accept_completed);

				// 클라이언트의 연결을 담당하는 스레드 생성/시작
				Thread listen_thread = new Thread(do_listen);
				listen_thread.Start();
			}
			catch (Exception e)
			{
				//Console.WriteLine(e.Message);
			}
		}

        /// <summary>
        /// 루프를 돌며 클라이언트를 받아들입니다.
        /// 하나의 접속 처리가 완료된 후 다음 accept를 수행하기 위해서 event객체를 통해 흐름을 제어하도록 구현되어 있습니다.
        /// </summary>
		void do_listen()
		{
			// 신호가 없는 상태로 신호기 객체 생성(이벤트 발생시 대기에서 풀림)
			// 대기에서 풀리는 순간 다시 신호대기 상태로 돌입
            this.flow_control_event = new AutoResetEvent(false);

			while (true)
			{
				// SocketAsyncEventArgs를 재사용 하기 위해서 null로 만들어 준다.
				this.accept_args.AcceptSocket = null;

				bool pending = true;
				try
				{
                    // 비동기 accept를 호출하여 클라이언트의 접속을 받아들입니다.
                    // 비동기 메소드 이지만 동기적으로 수행이 완료될 경우도 있으니
                    // 리턴값을 확인하여 분기시켜야 합니다.

                    /* listen_socket.Accept (동기 연결 대기 함수)
                     *   => 클라이언트의 연결이 되기 전까지 리턴하지 않고 멈춰있다가
                     *      연결이 이루어지면 리턴하여 동작
					 * listen_socket.AcceptAsync (비동기 연결 대기 함수)
					 *   => 클라이언트의 연결과 관계없이 무조건 리턴하여 다음 동작
					 */
					/*
					.NET에 나중에 클라이언트가 연결을 하면 accept_args에 있는
					콜백함수를 호출해주면 된다고 .NET한테 미리 등록해줌
					*/
					/*
					pending == true는 현재 비동기 통신이 진행중이라는 뜻
						       아직 클라이언트가 접속하지 않았다.
					           거의 대부분의 경우
					pending == false는 현재 비동기 통신을 진행하는 중에
						       클라이언트가 접속했다는 뜻
						       이 경우는 바로 Accept처리를 해야함.
					*/
                    pending = listen_socket.AcceptAsync(this.accept_args);
				}
				catch (Exception e)
				{
					//Console.WriteLine(e.Message);
					continue;
				}

				// 즉시 완료 되면 이벤트가 발생하지 않으므로 리턴값이 false일 경우 콜백 매소드를 직접 호출해 줍니다.
				// pending상태라면 비동기 요청이 들어간 상태이므로 콜백 매소드를 기다리면 됩니다.
				// http://msdn.microsoft.com/ko-kr/library/system.net.sockets.socket.acceptasync%28v=vs.110%29.aspx

				// 비동기 수신등록 중에 클라이언트가 연결 요청한 상황이므로
				// 바로 accept처리를 해야 하므로 아래처럼 직접 on_accept_completed를 호출해줌.
				if (!pending)
				{
					on_accept_completed(null, this.accept_args);
				}

				// 클라이언트 접속 처리가 완료되면 이벤트 객체의 신호를 전달받아 다시 루프를 수행하도록 합니다.

				// 신호가 발생하지 않으면 여기서 계속 리턴하지 않고 대기
				this.flow_control_event.WaitOne();

				// *팁 : 반드시 WaitOne -> Set 순서로 호출 되야 하는 것은 아닙니다.
				//      Accept작업이 굉장히 빨리 끝나서 Set -> WaitOne 순서로 호출된다고 하더라도 
				//      다음 Accept 호출 까지 문제 없이 이루어 집니다.
				//      WaitOne 메소드가 호출될 때 이벤트 객체가 이미 signalled 상태라면 스레드를 대기 하지 않고 계속 진행하기 때문입니다.
			}
		}

        /// <summary>
        /// AcceptAsync의 콜백 매소드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">AcceptAsync 매소드 호출시 사용된 EventArgs</param>
		void on_accept_completed(object sender, SocketAsyncEventArgs e)
		{
			// 연결 성공일 때
            if (e.SocketError == SocketError.Success)
            {
				// e매개변수를 통해 클라이언트와 연결되는 소켓 객체 정보를 얻을 수 있다.
                // 새로 생긴 소켓을 보관해 놓은뒤~
                Socket client_socket = e.AcceptSocket;

                // 다음 연결을 받아들인다.
                // 신호기에 신호발생(즉, this.flow_control_event.WaitOne() 이 리턴된다.)
                // flow_control_event는 AutoResetEvent객체이므로 리턴 후 다시 자동 신호 꺼짐
                this.flow_control_event.Set();

                // 이 클래스에서는 accept까지의 역할만 수행하고 클라이언트의 접속 이후의 처리는
                // 외부로 넘기기 위해서 콜백 매소드를 호출해 주도록 합니다.
                // 이유는 소켓 처리부와 컨텐츠 구현부를 분리하기 위함입니다.
                // 컨텐츠 구현부분은 자주 바뀔 가능성이 있지만, 소켓 Accept부분은 상대적으로 변경이 적은 부분이기 때문에
                // 양쪽을 분리시켜주는것이 좋습니다.
                // 또한 클래스 설계 방침에 따라 Listen에 관련된 코드만 존재하도록 하기 위한 이유도 있습니다.
                if (this.callback_on_newclient != null)
                {
                    this.callback_on_newclient(client_socket, e.UserToken);
                }

				return;
            }
            else
            {
                //todo:Accept 실패 처리.
				//Console.WriteLine("Failed to accept client.");
            }

			// 다음 연결을 받아들인다.
            this.flow_control_event.Set();
		}

        //int connected_count = 0;
        //void on_new_client(Socket client_socket)
        //{
        //    //Interlocked.Increment(ref this.connected_count);

        //    //Console.WriteLine(string.Format("[{0}] A client connected. handle {1},  count {2}", 
        //    //	Thread.CurrentThread.ManagedThreadId, client_socket.Handle,
        //    //	this.connected_count));
        //    //return;

        //    // 연결이 성립되면 패킷 헤더 읽을 준비를 한다.
        //    //StateObject state = new StateObject(client_socket);
        //    //state.remain_size_to_read = CPacket.HEADER_SIZE;
        //    //client_socket.BeginReceive(state.buffer, 0, CPacket.HEADER_SIZE, 0,
        //    //	new AsyncCallback(on_recv_header), state);
        //}

		//void on_recv_header(IAsyncResult iar)
		//{
			//StateObject state = (StateObject)iar.AsyncState;

			//int read_size = state.workSocket.EndReceive(iar);

			//// 헤더가 짤려서 올 경우 못읽은 만큼 다시 읽어온다.
			//if (state.remain_size_to_read > read_size)
			//{
			//	state.remain_size_to_read -= read_size;
			//	state.workSocket.BeginReceive(state.buffer, read_size, state.remain_size_to_read, SocketFlags.None,
			//		new AsyncCallback(on_recv_header), state);
			//	return;
			//}

			////Console.WriteLine("read size " + remain_size);
			//if (read_size <= 0)
			//{
			//	Console.WriteLine(string.Format("[{0}] [on_disconnect]  client socket {1}",
			//		System.Threading.Thread.CurrentThread.ManagedThreadId,
			//		state.workSocket.Handle));
			//	state.workSocket.Close();
			//	return;
			//}
			//else
			//{
			//	// 바디 사이즈 파싱.
			//	Int16 body_size = BitConverter.ToInt16(state.buffer, 0);

			//	if (body_size <= 0 || body_size > 10240)
			//	{
			//		state.workSocket.Close();
			//		return;
			//	}

			//	// 프로토콜 id 파싱.
			//	short protocol_id = BitConverter.ToInt16(state.buffer, 2);
			//	state.set_protocol(protocol_id);

			//	state.body_size = body_size;
			//	state.remain_size_to_read = state.body_size;

			//	// 바디 읽기.
			//	Array.Clear(state.buffer, 0, state.buffer.Length);
			//	state.workSocket.BeginReceive(state.buffer, 0, body_size, 0,
			//		new AsyncCallback(on_recv), state);
			//}
		//}

		//void on_recv(IAsyncResult iar)
		//{
			//StateObject state = (StateObject)iar.AsyncState;

			//int read_size = state.workSocket.EndReceive(iar);
			//if (state.remain_size_to_read > read_size)
			//{
			//	state.remain_size_to_read -= read_size;
			//	state.workSocket.BeginReceive(state.buffer, read_size, state.remain_size_to_read, SocketFlags.None,
			//		new AsyncCallback(on_recv), state);
			//	return;
			//}

			//if (read_size <= 0)
			//{
			//	Console.WriteLine(string.Format("[{0}] [on_disconnect]  client socket {1}",
			//		System.Threading.Thread.CurrentThread.ManagedThreadId,
			//		state.workSocket.Handle));
			//	state.workSocket.Close();
			//}
			//else
			//{
			//	try
			//	{
			//		Interlocked.Increment(ref recv_body_count);

			//		// 하나의 패킷 길이만큼만 잘라서 peer에 전달.
			//		byte[] clone_buffer = new byte[state.body_size];
			//		System.Buffer.BlockCopy(state.buffer, 0, clone_buffer, 0, state.body_size);

			//		// Peer객체에 전달하고 다음 메시지를 받을 수 있도록 다시 헤더를 읽을 준비를 한다.
			//		CPacket msg = new CPacket(state.protocol_id, clone_buffer);
			//		state.peer.on_recv(msg);

			//		// 이 버퍼는 다음 패킷을 받을 때 사용할 것이기 때문에 깔끔하게 클리어 해준다.
			//		Array.Clear(state.buffer, 0, state.buffer.Length);

			//		// 헤더 읽기.
			//		state.remain_size_to_read = CPacket.HEADER_SIZE;
			//		state.workSocket.BeginReceive(state.buffer, 0, CPacket.HEADER_SIZE, 0,
			//			new AsyncCallback(on_recv_header), state);
			//	}
			//	catch (Exception e)
			//	{
			//		Console.WriteLine(e.Message + "\n" + e.StackTrace);

			//		//byte[] clone_buffer = new byte[state.body_size];
			//		//System.Buffer.BlockCopy(state.buffer, 0, clone_buffer, 0, state.body_size);

			//		//CPacket msg = new CPacket(clone_buffer);
			//		//Int32 data = msg.pop_int32();

			//		//Console.WriteLine("int32 " + data + ",   buffer : " + clone_buffer.Length);
			//	}
			//}
		//}
	}
}
