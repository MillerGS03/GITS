create table Item
(
CodItem int primary key,
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

alter table Usuario
alter column CodUsuario varchar(30)