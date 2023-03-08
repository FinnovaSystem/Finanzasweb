use ManejoPresupuesto;
-- TABLAS --
CREATE TABLE Transacciones (
	Id int IDENTITY(1,1) NOT NULL,
	UsuarioId nvarchar(450) NOT NULL,
	FechaTransaccion datetime NOT NULL,
	Monto decimal(18,2) NOT NULL,
	TipoOperacionId int,  -- representa si es ingreso o gasto
	Nota nvarchar(1000)
	
	CONSTRAINT transacciones_PK PRIMARY KEY (Id)
);

--drop table Transacciones;
--drop table TiposOperaciones;

alter table Transacciones
add constraint transacciones_tipo_operaciones_fk 
foreign key (TipoOperacionId) references TiposOperaciones(Id);

CREATE TABLE TiposOperaciones (
	Id int IDENTITY(1,1) NOT NULL,
	Descripcion nvarchar(50) NOT NULL,
	
	CONSTRAINT tipos_operaciones_PK PRIMARY KEY (Id)
);

CREATE TABLE TiposCuentas (
	Id int IDENTITY(1,1) NOT NULL,
	Nombre nvarchar(50) NOT NULL,
	UsuarioId int NOT NULL,
	Orden int not null
	
	CONSTRAINT tipos_cuentas_PK PRIMARY KEY (Id)
);

CREATE TABLE Cuentas (
	Id int IDENTITY(1,1) NOT NULL,
	Nombre nvarchar(50) NOT NULL,
	TipoCuentaId int NOT NULL,
	Balance decimal(18,2) not null,
	Descripcion nvarchar(1000)
	
	CONSTRAINT cuentas_PK PRIMARY KEY (Id)
);

CREATE TABLE Usuarios (
	Id int IDENTITY(1,1) NOT NULL,
	Email nvarchar(256) NOT NULL,
	EmailNormalizado nvarchar(256) NOT NULL, -- util para comparaciones omitiendo transformaciones upper/lowercase
	PasswordHash nvarchar(max) not null
	
	CONSTRAINT usuarios_PK PRIMARY KEY (Id)
);

CREATE TABLE Categorias (
	Id int IDENTITY(1,1) NOT NULL,
	Nombre nvarchar(50) NOT NULL,
	TipoOperacionId int NOT NULL,
	UsuarioId int not null
	
	CONSTRAINT categorias_PK PRIMARY KEY (Id)
);


alter table Cuentas
add constraint cuentas_tipo_cuentas_fk
foreign key (TipoCuentaId) references TiposCuentas(Id);

----------- FIN TABLAS ------------

update Transacciones 
set nota = 'nota actualizada'
where id = 1;

-- procedures

create or alter procedure transacciones_selectConTipoOperacion
	@fecha DATE
AS 
BEGIN 
	select transacciones.id, UsuarioId , monto, nota, descripcion
	from transacciones
	join TiposOperaciones
	on(transacciones.TipoOperacionId = tiposOperaciones.id)
	where fechatransaccion = @fecha
	;
End

CREATE or alter PROCEDURE dbo.Transacciones_insertar
	@UsuarioId nvarchar(450),
	@FechaTransaccion date,
	@Monto decimal (18,2),
	@TipoOperacionId int,
	@Nota nvarchar(1000)
AS 
begin
	insert into transacciones(UsuarioId,FechaTransaccion,Monto,TipoOperacionId,Nota)
	values(@UsuarioId,@FechaTransaccion,@Monto,@TipoOperacionId,@Nota)
end

-- ejemplos ejecuciones de procedures
exec transacciones_selectConTipoOperacion '2023-03-01';
exec Transacciones_insertar 'peter','2023-03-01',899.69,1,'nota ejemplo'; 

insert into TiposCuentas (Nombre, UsuarioId, Orden)
values (Nombre, UsuarioId, Orden)

select 1
from TiposCuentas
where Nombre = @Nombre and UsuarioId = @UsuarioId
--where Nombre = 'Ahorro' and UsuarioId = 0


select *
from TiposCuentas;








