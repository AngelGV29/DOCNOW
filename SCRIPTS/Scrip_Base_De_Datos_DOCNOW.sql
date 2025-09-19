CREATE TABLE NuevoUsuario (
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

CREATE TABLE Paciente (
    idPaciente INT IDENTITY(1,1) PRIMARY KEY,
    idUsuario INT,
    alergia NVARCHAR(255),
    medicacion NVARCHAR(255),
    FOREIGN KEY (idUsuario) REFERENCES NuevoUsuario(idUsuario)
);

CREATE TABLE Medico (
    idMedico INT IDENTITY(1,1) PRIMARY KEY,
    numCedula NVARCHAR(20),
    especialidad NVARCHAR(100)
);

CREATE TABLE Consultorio (
    idConsultorio INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100),
    direccion NVARCHAR(255),
    telefono NVARCHAR(20)
);


INSERT INTO NuevoUsuario (nombre, apellidoPaterno, apellidoMaterno, correo, contrasenia, telefono, fechaNac, sexo, rol, fechaCreacion, ultimaModSesion)
VALUES 
    ('Juan', 'Pérez', 'García', 'juan.perez@gmail.com', 'contraseñaSegura123', '687-555-1234', '1990-05-15', 'M', 'Paciente', GETDATE(), NULL),
    ('María', 'López', 'Rodríguez', 'maria.lopez@gmail.com', 'miPass456', '687-555-5678', '1985-08-22', 'F', 'Paciente', GETDATE(), NULL),
    ('Carlos', 'Gómez', 'Martínez', 'carlos.gomez@gmail.com', 'docPass789', '687-555-9012', '1978-03-10', 'M', 'Médico', GETDATE(), NULL);

-- Verifica:
SELECT * FROM NuevoUsuario;


INSERT INTO Paciente (idUsuario, alergia, medicacion)
VALUES 
    (1, 'Alergia a penicilina', 'Paracetamol diario'),
    (2, 'Ninguna alergia conocida', 'Insulina para diabetes');

-- Verifica:
SELECT * FROM Paciente;

INSERT INTO Medico (numCedula, especialidad)
VALUES 
    ('1234567890', 'Cardiología'),
    ('0987654321', 'Pediatría');

INSERT INTO Consultorio (nombre, direccion, telefono)
VALUES 
    ('Consultorio Central', 'Calle Principal 123, Ciudad', '667-555-0001'),
    ('Clínica Especializada', 'Avenida Sur 456, Barrio Norte', '667-555-0002');



-- Nota: El SELECT a continuación fallará si no hay datos. Inserta datos primero si lo necesitas.
SELECT * FROM NuevoUsuario;
SELECT * FROM Paciente;
SELECT * FROM Medico;
SELECT * FROM Consultorio;


--18/09/2025
--Se le agregaron 3 tabla más a la base de datos y se hicieron las relaciones del usuario con paciente y doctor
ALTER TABLE Medico
ADD idUsuario INT;


ALTER TABLE Medico
ADD CONSTRAINT FK_Medico_NuevoUsuario
FOREIGN KEY (idUsuario)
REFERENCES NuevoUsuario (idUsuario);


SELECT * FROM Medico;
SELECT * FROM NuevoUsuario;
SELECT * FROM Paciente;


UPDATE Medico
SET idUsuario = 3
WHERE idMedico = 1; -- Asocia el médico con numCedula '1234567890' a Carlos Gómez
INSERT INTO NuevoUsuario (nombre, apellidoPaterno, apellidoMaterno, correo, contrasenia, telefono, fechaNac, sexo, rol, ultimaModSesion)
VALUES ('Pedro', 'Ramírez', 'González', 'pedro.ramirez@gmail.com', 'passPedro123', '687-555-7890', '1982-09-10', 'M', 'Médico', NULL);

UPDATE Medico
SET idUsuario = (SELECT idUsuario FROM NuevoUsuario WHERE correo = 'pedro.ramirez@gmail.com')
WHERE idMedico = 2; -- Asocia el segundo médico a este nuevo usuario

ALTER TABLE Medico
ALTER COLUMN idUsuario INT NOT NULL;

SELECT * FROM NuevoUsuario WHERE idUsuario = 4;
INSERT INTO Paciente (idUsuario, alergia, medicacion)
VALUES (4, 'Ninguna alergia conocida', 'Sin medicación regular');
SELECT * FROM Paciente WHERE idUsuario = 4;