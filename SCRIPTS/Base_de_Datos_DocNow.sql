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


CREATE TABLE Paciente (
    idPaciente INT IDENTITY(1,1) PRIMARY KEY,
    idUsuario INT,
    alergia NVARCHAR(255),
    medicacion NVARCHAR(255),
    FOREIGN KEY (idUsuario) REFERENCES Usuario(idUsuario)
);


CREATE TABLE Medico (
    idMedico INT IDENTITY(1,1) PRIMARY KEY,
    numCedula NVARCHAR(20),
    especialidad NVARCHAR(100),
    idUsuario INT,
    FOREIGN KEY (idUsuario) REFERENCES Usuario(idUsuario)
);


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