using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeNet
{
	public class CPacketBufferManager
	{
        // 멀티스레드 안전(동기화)하게 1개 스레드만 진입하는 용도의 자물쇠
        static object cs_buffer = new object();  
		static Stack<CPacket> pool;		// 통신에 사용할 CPacket객체 저장소
		static int pool_capacity;		// pool에 저장될 CPacket객체 용량


		public static void initialize(int capacity)
		{
			pool = new Stack<CPacket>();
			pool_capacity = capacity;
			allocate();
		}

		static void allocate()
		{
			// 용량만큼 Stack에 CPacket객체를 생성해서 미리 저장해 놓는다.
			for (int i = 0; i < pool_capacity; ++i)
			{
				pool.Push(new CPacket());
			}
		}

		public static CPacket pop()
		{
			// 1개 스레드만 진입 가능
			lock (cs_buffer)
			{
				// 이미 할당한 모든 CPacket객체를 꺼내어 썼을 때
				// 추가 할당한다.
				if (pool.Count <= 0)
				{
					Console.WriteLine("reallocate.");
					allocate();
				}

				// 현재 pool에 저장된 CPacket객체를 꺼내어 준다.
				// pool.Count가 1개씩 감소한다.
				return pool.Pop();
			}
		}

		// 다 사용한 CPacket객체를 다시 pool에 반납
		public static void push(CPacket packet)
		{
			lock(cs_buffer)
			{
				// pool에 다시 저장
				// pool.Count가 1 증가한다.
				pool.Push(packet);
			}
		}
	}
}
