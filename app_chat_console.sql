create database app_chat_console
use app_chat_console
go
create table channel
(
	channel_name varchar(50) primary  key,
	pass varchar(20),
	total_user int,
)
create table mess
(
	id_mess int identity(1,1) primary key,
	channel_name varchar(50) foreign key references channel(channel_name) on update  cascade on delete cascade,
	username varchar(50),
	content nvarchar(100)
)