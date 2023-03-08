drop database if exists cnstring
go
create database cnstring
go
use cnstring
go
create table student
(
Id varchar(100) primary key,
Name varchar(100),
Fee money,
joindate datetime,
Photo image,
stringphoto varchar (100),
)
go 
create  table fees(
vno varchar(100),
slno int,
headname varchar(100),
amount money,
studentid varchar(100) references student(id),
primary key(vno,slno)
)
select * from student