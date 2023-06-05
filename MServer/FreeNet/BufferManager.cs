using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace FreeNet
{
    /// <summary>
    /// This class creates a single large buffer which can be divided up and assigned to SocketAsyncEventArgs objects for use
    /// with each socket I/O operation.  This enables bufffers to be easily reused and gaurds against fragmenting heap memory.
    /// 
    /// The operations exposed on the BufferManager class are not thread safe.
    /// </summary>
    internal class BufferManager
    {

        int m_numBytes;                 // the total number of bytes controlled by the buffer pool
        byte[] m_buffer;                // the underlying byte array maintained by the Buffer Manager
        Stack<int> m_freeIndexPool;     // 
        int m_currentIndex;
        int m_bufferSize;

        public BufferManager(int totalBytes, int bufferSize)
        {
            m_numBytes = totalBytes;
            m_currentIndex = 0;
            m_bufferSize = bufferSize;
            m_freeIndexPool = new Stack<int>();
        }

        /// <summary>
        /// Allocates buffer space used by the buffer pool
        /// </summary>
        public void InitBuffer()
        {
            // create one big large buffer and divide that out to each SocketAsyncEventArg object
            m_buffer = new byte[m_numBytes];
        }

        /// <summary>
        /// Assigns a buffer from the buffer pool to the specified SocketAsyncEventArgs object
        /// </summary>
        /// <returns>true if the buffer was successfully set, else false</returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            // args가 사용하고 반납했을 때 진입하는 코드
            if (m_freeIndexPool.Count > 0)
            {
                args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
            }

            // 처음에 args를 할당할 때는 이곳으로 진입한다.
            else
            {
                if ((m_numBytes - m_bufferSize) < m_currentIndex)
                {
                    return false;
                }

                // m_buffer의 총 크기=  20,480,000 = byte 10000(최대 클라이언트 수) * 1024(args당 공간크기) * 2(수신/송신)
                // 비동기통신에 사용하는 args객체에서 
                // 버퍼의 일정 영역을 사용하도록 시작위치와 크기를 알려준다.
                // offset(args에서 사용할 버퍼의 시작위치) : m_currentIndex
                // m_bufferSize(1024) : 버퍼의 크기(즉 m_buffer의 끝 위치정보)
                /* 1번째 사용자의 수신args/송신 args
                 *   -1번째 receive args : m_buffer의 0 ~ 1023까지 사용(1024byte를 사용)
                 *   -2번째 send args : m_buffer의 1024 ~ 2047까지 사용(1024byte를 사용)
                 * 2번째 사용자의 수신args/송신 args
                 *   -3번째 receive args : m_buffer의 2048 ~ 3071까지 사용(1024byte를 사용)
                 *   -4번째 send args : m_buffer의 3072 ~ 4095까지 사용(1024byte를 사용)
                 * ...
                 * 10000번째 사용자의 수신args/송신 args 
                 *   -19999번째 receive args : m_buffer의 20,477,952 ~ 20,478,975까지 사용(1024byte를 사용)
                 *   -20000번째 send args : m_buffer의 20,478,976 ~ 20,479,999까지 사용(1024byte를 사용)
                 */
                args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
                // args에 버퍼 위치 할당할 때마다 시작위치를 m_bufferSize(1024)만큼 이동
                m_currentIndex += m_bufferSize;  
            }
            return true;
        }

        /// <summary>
        /// Removes the buffer from a SocketAsyncEventArg object.  This frees the buffer back to the 
        /// buffer pool
        /// </summary>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
}
