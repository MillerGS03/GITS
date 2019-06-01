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

select * from UsuarioTarefa

alter proc adicionarTarefa
@dat date, --1
@tit varchar(65),
@desc ntext,
@dif int,
@cre int,
@met int,
@recomp int,
@cria date,
@xp int
as
insert into Tarefa values(@dat, @tit, @desc, @dif, @cre, @recomp, @cria, @xp)
declare @id int
set @id = SCOPE_IDENTITY();
insert into UsuarioTarefa values(@cre, @id, 1, 0)
insert into AdminTarefa values(@cre, @id)
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

create proc comprarItem_sp
@usu int,
@codItem int
as
declare @din int
select @din = Dinheiro from Usuario where Id = @usu
declare @val int
select @val = Valor from Item where CodItem = @codItem
set @din = @din - @val
update Usuario set Dinheiro = @din where Id = @usu
insert into UsuarioItem values(@usu, @codItem)

create table AdminTarefa
(
CodAdminTarefa int primary key identity(1, 1),
IdAdmin int,
CodTarefa int,
constraint fkIdAdmin foreign key (IdAdmin) references Usuario(Id),
constraint fkAdminTarefaCodTarefa foreign key(CodTarefa) references Tarefa(CodTarefa)
)

create table UsuarioAcontecimento 
(
CodUsuarioAcontecimento int primary key identity(1, 1),
IdUsuario int,
CodAcontecimento int,
constraint fkUsuarioAcontecimentoIdUsuario foreign key(IdUsuario) references Usuario(Id),
constraint fkUsuarioAcontecimentoCodAcontecimento foreign key(CodAcontecimento) references Acontecimento(CodAcontecimento)
)

create table AdminAcontecimento
(
CodAdminAcontecimento int primary key identity(1, 1),
IdUsuario int,
CodAcontecimento int,
constraint fkAdminAcontecimentoIdUsuario foreign key(IdUsuario) references Usuario(Id),
constraint fkAdminAcontecimentoCodAcontecimento foreign key(CodAcontecimento) references Acontecimento(CodAcontecimento)
)

alter proc adicionarAcontecimento_sp
@tit varchar(65),
@desc ntext,
@data date,
@tipo int,
@creator int
as
insert into Acontecimento values(@tipo, @data, @tit, @desc, @creator)
declare @id int
set @id = SCOPE_IDENTITY();
insert into UsuarioAcontecimento values(@creator, @id)
insert into AdminAcontecimento values(@creator, @id)
select @id as 'id'

create proc adicionarDinheiro_sp
@idUsuario int,
@valor float
as
declare @atual int
select @atual = Dinheiro from Usuario where Id = @idUsuario
set @atual = @atual + @valor
update Usuario set Dinheiro = @atual where Id = @idUsuario

create proc adicionarXP_sp
@idUsuario int,
@valor float
as
declare @atual int
select @atual = XP from Usuario where Id = @idUsuario
set @atual = @atual + @valor
update Usuario set XP = @atual where Id = @idUsuario

update Item set Conteudo = 'black #feed{background:#232323;} #feed h5{color: lightgray;} .containerPost{filter: invert(1);} .containerPost img{filter: invert(1);} #listaMetas{filter: invert(1);} #tarefas{filter: invert(1);} #tarefas a {filter: invert(1)} #tarefas h3 {filter: invert(1) !important;} #loja{background: #232323;} #metasObjetivos{background:#232323;} .tabs{background:#303030;} .conteudoLoja .collection-item{color: black !important;} .tituloPublicacao .linkSemDecoracao{filter:invert(1);} #infoUsuario{background-color:gray;} #conjuntoFotoXP{background-color:#232323;} #feed h4, #metasObjetivos h5{filter:invert(1);} .conteudoLoja .collection-item.active{color: white !important;} #slideEsquerda {color: white; background: #191919 !important;} #triggerEsquerda {background: #191919 !important;} #triggerEsquerda i {color: rgb(38, 166, 154) !important;} .inverter {color: white;} #containerConteudo {background: #232323 !important;} .principal {color: white !important;} h3 {color: white !important;} .displayTarefa {filter: invert(1);} #main{background: gray !important;} .displayTarefa .chip img {filter: invert(1);} #operacaoPrincipal {filter: invert(1);} .displayAcontecimento {filter: invert(1);} .displayAcontecimento .chip img, h3#nome, .colecaoResultado, .avatar img, .resultados, .fotoUsuarioCriadorEvento, .resultados .tabs {filter: invert(1);} .resultados #publicacoes .containerPost {filter: unset !important;} #tabAgenda {background: rgb(217, 217, 217); filter: invert(1);} .fc-day-grid-event, .fc-h-event, .fc-event, .fc-start, .fc-end {filter: invert(1);} #btnExcluirEvento {filter: invert(1);}' where CodItem = 4
select * from Item

select * from UsuarioTarefa
select * from AdminTarefa
select * from TarefaMeta
select * from Tarefa
select * from Acontecimento
select * from Notificacao