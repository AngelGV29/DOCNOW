-- TABLA: Usuario
CREATE TABLE Usuario (
    idUsuario INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    apellPaterno NVARCHAR(100) NOT NULL,
    apellMaterno NVARCHAR(100),
    correo NVARCHAR(150) UNIQUE NOT NULL,
    contrasenia NVARCHAR(255) NOT NULL,
    telefono NVARCHAR(20),
    fechaNac DATE,
    sexo NVARCHAR(10),
    rol NVARCHAR(20) NOT NULL
);

-- TABLA: Paciente
CREATE TABLE Paciente (
    idPaciente INT IDENTITY(1,1) PRIMARY KEY,
    alergia NVARCHAR(255),
    medicacion NVARCHAR(255),
    ultimaVisita DATE,
    idUsuario INT NOT NULL,
    FOREIGN KEY (idUsuario) REFERENCES Usuario(idUsuario) ON DELETE CASCADE
);


-- TABLA: EspecialidadMedico
CREATE TABLE EspecialidadMedico (
    idEspecialidad INT IDENTITY(1,1) PRIMARY KEY,
    nombreEspecialidad NVARCHAR(100) NOT NULL
);


-- TABLA: Medico
CREATE TABLE Medico (
    idMedico INT IDENTITY(1,1) PRIMARY KEY,
    numCedula NVARCHAR(50) UNIQUE NOT NULL,
    idEspecialidad INT,
    idUsuario INT,
    FOREIGN KEY (idEspecialidad) REFERENCES EspecialidadMedico(idEspecialidad),
    FOREIGN KEY (idUsuario) REFERENCES Usuario(idUsuario) ON DELETE CASCADE
);


-- TABLA: Consultorio
CREATE TABLE Consultorio (
    idConsultorio INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    calle NVARCHAR(150),
    numeroInterior NVARCHAR(50),
    numeroExterior NVARCHAR(50),
    colonia NVARCHAR(100),
    codigoPostal NVARCHAR(10),
    telefono NVARCHAR(20)
);


-- TABLA: MedicoConsultorio 

CREATE TABLE MedicoConsultorio (
    idMedicoConsultorio INT IDENTITY(1,1) PRIMARY KEY,
    idMedico INT NOT NULL,
    idConsultorio INT NOT NULL,
    FOREIGN KEY (idMedico) REFERENCES Medico(idMedico),
    FOREIGN KEY (idConsultorio) REFERENCES Consultorio(idConsultorio)
);


-- TABLA: AgendaDisponibilidad
CREATE TABLE AgendaDisponibilidad (
    idAgenda INT IDENTITY(1,1) PRIMARY KEY,
    idMedicoConsultorio INT NOT NULL,
    trabajaLunes BIT NOT NULL DEFAULT 0,
    trabajaMartes BIT NOT NULL DEFAULT 0,
    trabajaMiercoles BIT NOT NULL DEFAULT 0,
    trabajaJueves BIT NOT NULL DEFAULT 0,
    trabajaViernes BIT NOT NULL DEFAULT 0,
    trabajaSabado BIT NOT NULL DEFAULT 0,
    trabajaDomingo BIT NOT NULL DEFAULT 0,
    horaInicioJornada TIME NOT NULL,
    horaFinJornada TIME NOT NULL,
    horaInicioDescanso TIME NULL,
    horaFinDescanso TIME NULL,
    duracionSlotMinutos INT NOT NULL,
    estado NVARCHAR(50) NOT NULL,
    fechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    fechaActualizacion DATETIME NULL,
    FOREIGN KEY (idMedicoConsultorio) REFERENCES MedicoConsultorio(idMedicoConsultorio)
);


-- TABLA: Turno
CREATE TABLE Turno (
    idTurno INT IDENTITY(1,1) PRIMARY KEY,
    idAgenda INT NOT NULL,
    fecha DATE NOT NULL,
    horaInicio TIME NOT NULL,
    horaFinal TIME NOT NULL,
    estado NVARCHAR(20) NOT NULL,
    FOREIGN KEY (idAgenda) REFERENCES AgendaDisponibilidad(idAgenda)
);


-- TABLA: MotivoConsulta
CREATE TABLE MotivoConsulta (
    idMotivo INT IDENTITY(1,1) PRIMARY KEY,
    descripcion NVARCHAR(200) NOT NULL,
    instruccion NVARCHAR(MAX)
);

-- TABLA: Cita
CREATE TABLE Cita (
    idCita INT IDENTITY(1,1) PRIMARY KEY,
    idPaciente INT NOT NULL,
    idMedico INT NOT NULL,
    idTurno INT NOT NULL,
    idMotivo INT NOT NULL,
    estado NVARCHAR(20) NOT NULL,
    fechaCreacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (idPaciente) REFERENCES Paciente(idPaciente),
    FOREIGN KEY (idMedico) REFERENCES Medico(idMedico),
    FOREIGN KEY (idTurno) REFERENCES Turno(idTurno),
    FOREIGN KEY (idMotivo) REFERENCES MotivoConsulta(idMotivo)
);



--insertar valores

--Usuario
INSERT INTO Usuario (nombre, apellPaterno, apellMaterno, correo, contrasenia, telefono, fechaNac, sexo, rol)
VALUES
('Carlos', 'Ramírez', 'López', 'carlos.ramirez@gmail.com', '12345abcde', '6871123422', '1988-05-10', 'Masculino', 'Medico'),
('Laura', 'García', 'Mendoza', 'laura.garcia@hotmail.com', 'laurasexy123', '6673338494', '1992-11-22', 'Femenino', 'Paciente'),
('Mario', 'Pérez', 'Juárez', 'mario.perez@gmail.com', 'mariobros7621', '6671458580', '1985-02-18', 'Masculino', 'Medico'),
('Ana', 'Hernández', 'Díaz', 'ana.hernandez@gmail.com', 'anahernandez07', '6871320944', '2000-07-07', 'Femenino', 'Paciente');

--EspecialidadMedico
INSERT INTO EspecialidadMedico (nombreEspecialidad)
VALUES
('Cardiología'),
('Pediatría'),
('Dermatología');

--Medico
--Asociamos los médicos con usuarios y especialidades
INSERT INTO Medico (numCedula, idEspecialidad, idUsuario)
VALUES
('CED12345', 1, 1),   -- Carlos (Cardiólogo)
('CED67890', 3, 3);   -- Mario (Dermatólogo)


--Paciente
INSERT INTO Paciente (alergia, medicacion, ultimaVisita, idUsuario)
VALUES
('Penicilina', 'Paracetamol', '2025-09-01', 2),
('Ninguna', 'Ibuprofeno', '2025-10-01', 4);


--Consultorio
INSERT INTO Consultorio (nombre, calle, numeroInterior, numeroExterior, colonia, codigoPostal, telefono)
VALUES
('Consultorio Central', 'Av. Reforma', '1', '101', 'Centro', '06000', '6873420941'),
('Clínica del Sol', 'Calle Luna', '2', '202', 'Roma Norte', '06700', '6879983421');


--MedicoConsultorio
INSERT INTO MedicoConsultorio (idMedico, idConsultorio)
VALUES
(1, 1),  -- Carlos en Consultorio Central
(2, 2);  -- Mario en Clínica del Sol


--AgendaDisponibilidad
INSERT INTO AgendaDisponibilidad (
    idMedicoConsultorio,
    trabajaLunes, trabajaMartes, trabajaMiercoles, trabajaJueves, trabajaViernes,
    horaInicioJornada, horaFinJornada,
    duracionSlotMinutos, estado
)
VALUES
(1, 1, 1, 1, 1, 0, '08:00', '14:00', 30, 'Activo'),
(2, 1, 1, 1, 0, 0, '10:00', '16:00', 45, 'Activo');


--Turno
INSERT INTO Turno (idAgenda, fecha, horaInicio, horaFinal, estado)
VALUES
(1, '2025-10-20', '08:00', '08:30', 'Disponible'),
(1, '2025-10-20', '08:30', '09:00', 'Reservado'),
(2, '2025-10-21', '10:00', '10:45', 'Disponible');

--MotivoConsulta
INSERT INTO MotivoConsulta (descripcion, instruccion)
VALUES
('Chequeo general', 'El paciente debe llegar 10 minutos antes.'),
('Dolor de cabeza', 'Evitar café antes de la cita.'),
('Revisión de piel', 'Traer resultados de exámenes anteriores.');



--Cita
INSERT INTO Cita (idPaciente, idMedico, idTurno, idMotivo, estado)
VALUES
(1, 1, 2, 1, 'Confirmada'),
(2, 2, 3, 3, 'Pendiente');


SELECT * FROM MotivoConsulta;
