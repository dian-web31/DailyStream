use dailystream;

drop table if exists dbo.dislike;
drop table if exists dbo.[like];
drop table if exists dbo.comentario;
drop table if exists dbo.usuario;

create table dbo.usuario 
(
  idusuario   int identity(1,1) not null
  , username    varchar(50)     not null
  , password    varchar(255)    not null
  , constraint pk_usuario primary key (idusuario)
  , constraint uq_usuario_username unique (username)
);
go 

create table dbo.comentario
(
    id          int           identity(1,1) not null
  , idusuario   int           not null
  , idvideo     varchar(50)   not null
  , contenido   text          not null
  , fecha       date          not null
  , constraint pk_comentario primary key (id)
  , constraint fk_comentario_usuario foreign key (idusuario)
        references dbo.usuario(idusuario)
        on update cascade
        on delete cascade
);
go

create table dbo.[like]
(
    idusuario   int         not null
  , idvideo     varchar(50) not null
  , constraint pk_like primary key (idusuario, idvideo)
  , constraint fk_like_usuario foreign key (idusuario)
        references dbo.usuario(idusuario)
        on update cascade
        on delete cascade
);
go

create table dbo.dislike
(
    idusuario   int         not null
  , idvideo     varchar(50) not null
  , constraint pk_dislike primary key (idusuario, idvideo)
  , constraint fk_dislike_usuario foreign key (idusuario)
        references dbo.usuario(idusuario)
        on update cascade
        on delete cascade
);
go

