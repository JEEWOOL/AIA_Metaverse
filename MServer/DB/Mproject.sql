DROP USER mproject CASCADE;

CREATE USER mproject IDENTIFIED BY mproject DEFAULT TABLESPACE users TEMPORARY TABLESPACE temp PROFILE DEFAULT;

GRANT CONNECT, RESOURCE TO mproject ;
GRANT CREATE VIEW, CREATE SYNONYM TO mproject;

ALTER USER mproject ACCOUNT UNLOCK;

conn mproject/mproject;

DROP SEQUENCE char_no_sequence;
DROP SEQUENCE game_no_sequence;
DROP SEQUENCE log_sequence;


--------------------- Sequence ----------------------------

CREATE SEQUENCE char_no_sequence
  START WITH 1
  INCREMENT BY 1
  NOMAXVALUE
  NOMINVALUE
  NOCYCLE
  CACHE 20;

CREATE SEQUENCE game_no_sequence
  START WITH 1
  INCREMENT BY 1
  NOMAXVALUE
  NOMINVALUE
  NOCYCLE
  CACHE 20;


CREATE SEQUENCE log_sequence
  START WITH 1
  INCREMENT BY 1
  NOMAXVALUE
  NOMINVALUE
  NOCYCLE
  CACHE 20;
  
  
---------------------------- Table --------------------

DROP TABLE userinfo;
DROP TABLE character;
DROP TABLE item;
DROP TABLE room;
DROP TABLE game_room;
DROP TABLE game_set;
DROP TABLE log;

CREATE TABLE userinfo (
  user_id     VARCHAR2(32) NOT NULL,
  user_pass   VARCHAR2(32) NOT NULL,
  game_no     NUMBER,
  coin        VARCHAR2(32),
  invent      VARCHAR(4000)
);

CREATE TABLE character (
  char_no      NUMBER NOT NULL,
  coin         VARCHAR(45) NULL,
  user_id      VARCHAR(32) NOT NULL,
  invent       VARCHAR(4000)
);

CREATE TABLE item (
  item_code    VARCHAR(45) NOT NULL,
  item_name    VARCHAR(45) NOT NULL
);

CREATE TABLE room (
  user_id      VARCHAR(32) NOT NULL,
  room_data    VARCHAR(4000)
);

CREATE TABLE game_room (
  game_no      NUMBER NOT NULL,
  game_name    VARCHAR(45) NOT NULL,  
  user_count   VARCHAR(45) NOT NULL    
);

CREATE TABLE game_set (
  game_name VARCHAR(45) NOT NULL,  
  user_min  VARCHAR(45) NOT NULL,
  user_max  VARCHAR(45) NOT NULL
);

CREATE TABLE log (
  log_seq   NUMBER,
  log_time  DATE default SYSDATE,
  log_ip    VARCHAR2(20),
  log_port  VARCHAR2(5),
  log_info  VARCHAR2(1000),
  user_id VARCHAR2(32)
);

-- PRIMARY KEY 설정

ALTER TABLE userinfo
 ADD CONSTRAINT user_id_pk PRIMARY KEY(user_id);

ALTER TABLE character
 ADD CONSTRAINT char_no_pk PRIMARY KEY(char_no);
 
ALTER TABLE item
 ADD CONSTRAINT item_code_pk PRIMARY KEY(item_code);

ALTER TABLE room
 ADD CONSTRAINT room_user_id_pk PRIMARY KEY(user_id);

ALTER TABLE game_room
 ADD CONSTRAINT game_no_pk PRIMARY KEY(game_no);
 
ALTER TABLE game_set
 ADD CONSTRAINT game_name_pk PRIMARY KEY(game_name);

-- FOREIGN KEY 설정

ALTER TABLE userinfo
 ADD CONSTRAINT user_game_no_fk FOREIGN KEY(game_no) 
 REFERENCES game_room(game_no);

ALTER TABLE character
 ADD CONSTRAINT char_user_id_fk FOREIGN KEY(user_id) 
 REFERENCES userinfo(user_id);
 
ALTER TABLE room
 ADD CONSTRAINT room_user_id_fk FOREIGN KEY(user_id) 
 REFERENCES userinfo(user_id);

 
ALTER TABLE game_room
 ADD CONSTRAINT game_game_name_fk FOREIGN KEY(game_name) 
 REFERENCES game_set(game_name);
 
ALTER TABLE log 
 ADD CONSTRAINT log_user_id_fk FOREIGN KEY(user_id)
 REFERENCES userinfo(user_id);
 