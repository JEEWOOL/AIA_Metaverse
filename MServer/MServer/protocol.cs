using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer
{
    public enum PROTOCOL : short
    {
        BEGIN = 0,

        REG_MEMBER_REQ = 1,     // 회원가입 요청
        REG_MEMBER_ACK = 2,     // 회원가입 응답

        LOGIN_REQ = 3,     // 로그인 요청
        LOGIN_ACK = 4,     // 로그인 응답

        JOIN_ROOM_REQ = 5,    // 방 참여 요청
        JOIN_ROOM_ACK = 6,   // 방 참여 응답

        LEAVE_GAME_ROOM_REQ = 7,        // 방 떠나기 요청
        LEAVE_GAME_ROOM_ACK = 8,        // 방 떠나기 응답

        SPAWN_PLAYER_REQ = 9,      // 게임룸에 플레이어 생성
        SPAWN_PLAYER_ACK = 10,      // 게임룸에 플레이어 생성 응답

        TRANSFORM_PLAYER_REQ = 11,  // 플레이어 위치/회전 정보 전달
        TRANSFORM_PLAYER_ACK = 12,  // 플레이어 위치/회전 정보 응답

        SHOT_PLAYER_REQ = 13,       // 플레이어 총 발사 전달
        SHOT_PLAYER_ACK = 14,       // 플레이어 총 발사 응답

        ICE_BROKE_REQ = 15,
        ICE_BROKE_ACK = 16,

        GAME_END_REQ = 17,
        GAME_END_ACK = 18,

        GAME_START_REQ = 19,
        GAME_START_ACK = 20,

        MYROOM_SAVE_REQ = 21,
        MYROOM_SAVE_ACK = 22,

        MYROOM_LOAD_REQ = 23,
        MYROOM_LOAD_ACK = 24,

        GET_MONEY_REQ = 25,
        GET_MONEY_ACK = 26,

        UPDATE_MONEY_REQ = 27,
        UPDATE_MONEY_ACK = 28,

        END
    }
}
