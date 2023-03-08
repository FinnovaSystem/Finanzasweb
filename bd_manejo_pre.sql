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

-- PROCEDURES ---------------------------

-- test para ejemplo --
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
end -- fin test para ejemplo --

-- para insertar nuevo tipo cuenta
CREATE OR ALTER PROCEDURE TiposCuentas_Insertar
	@Nombre nvarchar(50), --parametro de entrada
	@UsuarioId int --parametro de entrada
AS 
BEGIN 
	
	-- al insertar nuevo registro tipocuenta, campo orden debe tomar el "ultimo valor de orden + 1"
	DECLARE @Orden int; --parametro interno, variable de uso interno
	SELECT @Orden = COALESCE (MAX(Orden),0)+1 -- coalesce(x,y): permite tomar "x" cuando es != null, toma "y" cuando es null
	from TiposCuentas
	where UsuarioId = @UsuarioId
	
	insert into TiposCuentas(Nombre, UsuarioId, Orden)
	values(@Nombre, @UsuarioId, @Orden)
	
	select SCOPE_IDENTITY();
	
	SELECT COALESCE (MAX(Orden),0)+1 -- coalesce permite tomar un valor distinto a null y usarlo
	from TiposCuentas
	where UsuarioId = 1;
	
	
	
End--- fin insertar nuevo tipo cuenta --

-- test insertar transaccion
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
end -- fin test insertar transaccion


-- PROCEDURES ---------------------------

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


update TiposCuentas 
set nombre = @Nombre
where id = @id









