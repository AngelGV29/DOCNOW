CREATE TABLE NuevoUsuario (
    idUsuario INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(50),
    apellidoPaterno NVARCHAR(50),
    apellidoMaterno NVARCHAR(50),
    correo NVARCHAR(100) UNIQUE, -- Restricci�n UNIQUE para evitar duplicados
    contrasenia NVARCHAR(100), -- Considera encriptaci�n en la aplicaci�n
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
    ('Juan', 'P�rez', 'Garc�a', 'juan.perez@gmail.com', 'contrase�aSegura123', '687-555-1234', '1990-05-15', 'M', 'Paciente', GETDATE(), NULL),
    ('Mar�a', 'L�pez', 'Rodr�guez', 'maria.lopez@gmail.com', 'miPass456', '687-555-5678', '1985-08-22', 'F', 'Paciente', GETDATE(), NULL),
    ('Carlos', 'G�mez', 'Mart�nez', 'carlos.gomez@gmail.com', 'docPass789', '687-555-9012', '1978-03-10', 'M', 'M�dico', GETDATE(), NULL);

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
    ('1234567890', 'Cardiolog�a'),
    ('0987654321', 'Pediatr�a');

INSERT INTO Consultorio (nombre, direccion, telefono)
VALUES 
    ('Consultorio Central', 'Calle Principal 123, Ciudad', '667-555-0001'),
    ('Cl�nica Especializada', 'Avenida Sur 456, Barrio Norte', '667-555-0002');



-- Nota: El SELECT a continuaci�n fallar� si no hay datos. Inserta datos primero si lo necesitas.
SELECT * FROM NuevoUsuario;
SELECT * FROM Paciente;
SELECT * FROM Medico;
SELECT * FROM Consultorio;
