create table Item
(
CodItem int primary key identity(0, 1),
Nome varchar(20),
Descricao nvarchar(max),
Valor float,
Tipo int,
MetodoObtencao int,
Conteudo varchar(30)
)
create table Usuario
(
Id int primary key identity(1,1),
CodUsuario nvarchar(max),
Email varchar(35),
FotoPerfil varchar(100),
XP float,
_Status varchar(30),
Insignia int,
Titulo varchar(15),
Decoracao int,
TemaSite int,
Dinheiro float
)

create table Acontecimento
(
CodAcontecimento int identity(1,1) primary key,
Tipo int not null,
Data date not null,
Titulo varchar(30) not null,
Descricao ntext,
CodUsuarioCriador int not null,
constraint fkAcontecimentoUsuario foreign key(CodUsuarioCriador) references Usuario(Id)
)

create table Meta
(
CodMeta int identity(1, 1) primary key,
Titulo varchar(30) not null,
Descricao ntext,
Data date not null,
Progresso int not null,
UltimaInteracao date
)

create table Tarefa
(
CodTarefa int identity(1,1) primary key,
Urgencia int not null,
Data date not null,
Titulo varchar(30) not null,
Descricao ntext,
Dificuldade int not null
)

create table TarefaMeta
(
CodTarefa int not null,
CodMeta int not null,
constraint fkTarefaMetaCodTarefa foreign key(CodTarefa) references Tarefa(CodTarefa),
constraint fkTarefaMetaCodMeta foreign key(CodMeta) references Meta(CodMeta)
)

create table Amizade
(
CodAmizade int primary key identity(1,1),
CodUsuario1 int,
CodUsuario2 int,
constraint fkAmizadeUsuario1 foreign key(CodUsuario1) references Usuario(Id),
constraint fkAmizadeUsuario2 foreign key(CodUsuario2) references Usuario(Id)
)

create table AcontecimentoUsuario
(
CodAcontecimentoUsuario int primary key,
CodAcontecimento int,
CodUsuario int,
constraint fkAcontecimentoUsuarioAcontecimento foreign key(CodAcontecimento) references Acontecimento(CodAcontecimento),
constraint fkAcontecimentoUsuarioUsuario foreign key(CodUsuario) references Usuario(Id)
)

create table Publicacao
(
CodPublicacao int primary key identity(1,1),
CodUsuario int,
Titulo varchar(50),
Descricao nvarchar(max),
Data date,
constraint fkPublicacaoUsuario foreign key(CodUsuario) references Usuario(Id)
)

create table UsuarioTarefa(
CodUsuarioTarefa int primary key identity(1,1),
IdUsuario int not null,
CodTarefa int not null,
constraint fkUsuarioTarefaUsuario foreign key(IdUsuario) references Usuario(Id),
constraint fkUsuarioTarefaTarefa foreign key(CodTarefa) references Tarefa(CodTarefa)
)

create table UsuarioMeta
(
CodUsuarioMeta int primary key identity(1,1),
IdUsuario int,
CodMeta int,
constraint fkUsuarioMetaUsuario foreign key(IdUsuario) references Usuario(Id),
constraint fkUsuarioMetaMeta foreign key(CodMeta) references Meta(CodMeta)
)


alter proc removerUsuario
@id int
as
delete from Amizade where CodUsuario1 = @id or CodUsuario2 = @id
delete from UsuarioTarefa where IdUsuario = @id
delete from Publicacao where CodUsuario = @id
delete from AcontecimentoUsuario where CodUsuario = @id
delete from Acontecimento where CodUsuarioCriador = @id
delete from UsuarioTarefa where IdUsuario = @id
delete from UsuarioMeta where IdUsuario = @id
delete from Usuario where Id = @id

select * from Tarefa
select * from Usuario
insert into UsuarioTarefa values(12, 34)
select * from TarefaMeta


alter proc adicionarTarefa
@ur int,
@dat date,
@tit varchar(65),
@desc ntext,
@dif int,
@cre int,
@met int
as
insert into Tarefa values(@ur, @dat, @tit, @desc, @dif, @cre)
declare @id int
set @id = SCOPE_IDENTITY();
insert into UsuarioTarefa values(@cre, @id, 1)
if @met <> 0
	insert into TarefaMeta values(@id, @met)
select @id as 'id'

create table Notificacao(
Id int primary key identity(1,1),
IdUsuarioReceptor int,
IdUsuarioTransmissor int,
Tipo int,
IdCoisa int,
JaViu bit,
constraint fkReceptorNotificacao foreign key(IdUsuarioReceptor) references Usuario(Id),
constraint fkTransmissorNotificacao foreign key(IdUsuarioTransmissor) references Usuario(Id)
)

create table UsuarioItem
(
IdUsuarioItem int primary key identity (1, 1),
IdUsuario int,
CodItem int,
constraint fkUsuarioItemUsuario foreign key(IdUsuario) references Usuario(Id),
constraint fkUsuarioItemItem foreign key(CodItem) references Item(CodItem)
)

sp_help Item

select * from UsuarioItem
select * from Usuario
select * from Item
update Usuario set Titulo = '3 R B' where Id = 8
update Item set Conteudo = 'Desenvolvedor' where CodItem = 3
select * from Item
insert into UsuarioItem values(8, 3)
insert into UsuarioItem values(12, 3)
insert into Item values('Estudo Negro', 'Sua urgência é tamanha que nem a procrastinação escapa', 0, 0, 0, 'black')