CREATE TABLE Usuario (
    idUsuario INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(50),
    apellidoPaterno NVARCHAR(50),
    apellidoMaterno NVARCHAR(50),
    correo NVARCHAR(100) UNIQUE, -- Restricción UNIQUE para evitar duplicados
    contrasenia NVARCHAR(100), -- Considera encriptación en la aplicación
    telefono NVARCHAR(20),
    fechaNac DATE,
    sexo CHAR(1),
    rol NVARCHAR(50),
    fechaCreacion DATETIME DEFAULT GETDATE(),
    ultimaModSesion DATETIME
);

--Crear Usuarios experimentales
INSERT INTO Usuario (nombre, apellidoPaterno, apellidoMaterno, correo, contrasenia, telefono, fechaNac, sexo, rol, ultimaModSesion)
VALUES 
('Angel Joaquin', 'García', 'Velázquez', 'angelgarciavelazquez29@gmail.com', '29092006', '6871167363', '2006-09-29', 'M', 'ADMIN', GETDATE()),
('Pablo', 'Hernandez', 'Gutierrez', 'pablo@hotmail.com', 'pablo123', '0123456789', '2000-01-01', 'M', 'PACIENTE', GETDATE()),
('Laura', 'Garza', 'Vazquez', 'laura@gmail.com', 'laura456', '9876543210', '2005-06-06', 'F', 'MEDICO', GETDATE());

CREATE TABLE Paciente (
    idPaciente INT IDENTITY(1,1) PRIMARY KEY,
    idUsuario INT,
    alergia NVARCHAR(255),
    medicacion NVARCHAR(255),
    FOREIGN KEY (idUsuario) REFERENCES Usuario(idUsuario)
);


CREATE TABLE Medico (
    idMedico INT IDENTITY(1,1) PRIMARY KEY,
    idUsuario INT,
    numCedula NVARCHAR(20),
    especialidad NVARCHAR(100),
    FOREIGN KEY (idUsuario) REFERENCES Usuario(idUsuario)
);



CREATE TABLE Admin (
    idAdmin INT IDENTITY(1,1) PRIMARY KEY,
    idUsuario INT,
    FOREIGN KEY (idUsuario) REFERENCES Usuario(idUsuario)
);

INSERT INTO Admin (idUsuario) Values 
(1);

CREATE TABLE Consultorio (
    idConsultorio INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100),
    direccion NVARCHAR(255),
    telefono NVARCHAR(20)
);


CREATE TABLE AgendaDisponibilidad (
    idAgenda INT IDENTITY(1,1) PRIMARY KEY,
    idMedico INT NOT NULL,
    idConsultorio INT NOT NULL,
    fechaInicio DATETIME NOT NULL,
    fechaFin DATETIME NOT NULL,
    FOREIGN KEY (idMedico) REFERENCES Medico(idMedico),
    FOREIGN KEY (idConsultorio) REFERENCES Consultorio(idConsultorio)
);


CREATE TABLE Turno (
    idTurno INT IDENTITY(1,1) PRIMARY KEY,
    idAgenda INT NOT NULL,
    fecha DATE NOT NULL,
    horaInicio TIME NOT NULL,
    horaFinal TIME NOT NULL,
    estado VARCHAR(20) NOT NULL,
    FOREIGN KEY (idAgenda) REFERENCES AgendaDisponibilidad(idAgenda)
);


CREATE TABLE MotivoConsulta (
    idMotivo INT IDENTITY(1,1) PRIMARY KEY,
    descripcion VARCHAR(200) NOT NULL,
    instruccion NVARCHAR(MAX)
);


CREATE TABLE Cita (
    idCita INT IDENTITY(1,1) PRIMARY KEY,
    idPaciente INT NOT NULL,
    idMedico INT NOT NULL,
    idTurno INT NOT NULL,
    idMotivo INT NOT NULL,
    estado VARCHAR(20) NOT NULL,
    fechaCreacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (idPaciente) REFERENCES Paciente(idPaciente),
    FOREIGN KEY (idMedico) REFERENCES Medico(idMedico),
    FOREIGN KEY (idTurno) REFERENCES Turno(idTurno),
    FOREIGN KEY (idMotivo) REFERENCES MotivoConsulta(idMotivo)
);

--Modificaciones a la base de datos 11-10-2025

-- Modificación 1: Agregar campo ultimaVisita a la tabla Paciente
ALTER TABLE Paciente
ADD ultimaVisita DATE;

--Creación de paciente experimental
INSERT INTO Paciente (idUsuario, alergia, medicacion, ultimaVisita) VALUES
(2, 'Ninguna', 'Ninguna', GETDATE());


-- Modificación 2: Crear tabla EspecialidadMedico y reemplazar campo especialidad en Medico
-- Primero, crear la tabla EspecialidadMedico
CREATE TABLE EspecialidadMedico (
    idEspecialidad INT IDENTITY(1,1) PRIMARY KEY,
    nombreEspecialidad NVARCHAR(100) NOT NULL
);

-- Luego, agregar columna idEspecialidad a Medico y hacer la FK
ALTER TABLE Medico
ADD idEspecialidad INT;

ALTER TABLE Medico
ADD FOREIGN KEY (idEspecialidad) REFERENCES EspecialidadMedico(idEspecialidad);



-- Finalmente, eliminar la columna especialidad original de Medico
ALTER TABLE Medico
DROP COLUMN especialidad;

INSERT INTO Consultorio (nombre, direccion, telefono)
VALUES 
('Consultorio Central', 'Av. Principal #100', '687-855-0601'),
('Consultorio Norte', 'Calle Norte #200', '687-335-8902'),
('Consultorio Sur', 'Boulevard Sur #300', '687-555-6423');


-- Modificación 4: Normalizar Consultorio dividiendo dirección
-- Primero, agregar nuevas columnas para la normalización
ALTER TABLE Consultorio
ADD calle NVARCHAR(150),
    numeroInterior NVARCHAR(50),
    numeroExterior NVARCHAR(50),
    colonia NVARCHAR(100),
    codigoPostal NVARCHAR(10);



-- Luego, actualizar los datos existentes si hay (ejemplo manual, ajusta según tus datos)
UPDATE Consultorio
SET calle = 'Av. Principal', numeroExterior = '#100', colonia = 'Centro', codigoPostal = '80000'
WHERE idConsultorio = 1;  -- Ajusta IDs según tus datos

UPDATE Consultorio
SET calle = 'Calle Norte', numeroExterior = '#200', colonia = 'Norte', codigoPostal = '80010'
WHERE idConsultorio = 2;

UPDATE Consultorio
SET calle = 'Boulevard Sur', numeroExterior = '#300', colonia = 'Sur', codigoPostal = '80020'
WHERE idConsultorio = 3;

-- Finalmente, eliminar la columna direccion original
ALTER TABLE Consultorio
DROP COLUMN direccion;



-- Modificación 2: Crear tabla EspecialidadMedico y reemplazar campo especialidad en Medico
-- Crear la tabla EspecialidadMedico
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EspecialidadMedico')
BEGIN
    CREATE TABLE EspecialidadMedico (
        idEspecialidad INT IDENTITY(1,1) PRIMARY KEY,
        nombreEspecialidad NVARCHAR(100) NOT NULL
    );
END;


-- Insertar datos iniciales en EspecialidadMedico (ejemplo)
INSERT INTO EspecialidadMedico (nombreEspecialidad)
VALUES ('Cardiología'), ('Pediatría'), ('Dermatología');


-- Verificar si la columna idEspecialidad existe en Medico antes de agregar
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Medico' AND COLUMN_NAME = 'idEspecialidad')
BEGIN
    ALTER TABLE Medico
    ADD idEspecialidad INT;
END;



-- Modificación 5: Crear tabla MedicoConsultorio si no existe y agregar datos
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'MedicoConsultorio')
BEGIN
    CREATE TABLE MedicoConsultorio (
        idMedicoConultorio INT IDENTITY(1,1) PRIMARY KEY,
        idMedico INT NOT NULL,
        idConsultorio INT NOT NULL,
        FOREIGN KEY (idMedico) REFERENCES Medico(idMedico),
        FOREIGN KEY (idConsultorio) REFERENCES Consultorio(idConsultorio)
    );
END;

SELECT idMedico FROM Medico;

--Creación de medico experimental
INSERT INTO Medico (idUsuario, numCedula, idEspecialidad) VALUES 
(3, '14789256', 1);


-- Verificar los IDs generados
SELECT idMedico, idUsuario, numCedula, idEspecialidad FROM Medico;

-- Insertar datos de ejemplo en MedicoConsultorio (ajusta IDs según tus datos)
INSERT INTO MedicoConsultorio (idMedico, idConsultorio)
VALUES 
(1, 1),  -- Médico 1 en Consultorio 1
(1, 2);  -- Médico 1 en Consultorio 2