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

create table Acontecimento
(
CodAcontecimento int identity(1,1) primary key,
Tipo int not null,
Data date not null,
Titulo varchar(30) not null,
Descricao ntext,
CodUsuarioCriador int not null,
constraint fkAcontecimentoUsuario foreign key(CodUsuarioCriador) references Usuario(CodUsuario)
)