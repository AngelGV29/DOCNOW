-- TABLA: Usuario
CREATE TABLE Usuario (
    idUsuario INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(50) NOT NULL,
    apellPaterno NVARCHAR(50) NOT NULL,
    apellMaterno NVARCHAR(50) NOT NULL,
    correo NVARCHAR(100) UNIQUE NOT NULL,
    contrasenia NVARCHAR(100) NOT NULL,
    telefono NVARCHAR(10) NOT NULL,
    fechaNac DATE NOT NULL,
    sexo NCHAR(1) NOT NULL,
    rol NVARCHAR(20) NOT NULL,
    fechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    ultimoLogin DATETIME NOT NULL DEFAULT GETDATE()
);

-- TABLA: Administrador
CREATE TABLE Administrador (
    idAdmin INT IDENTITY(1,1) PRIMARY KEY,
    idUsuario INT NOT NULL,
    FOREIGN KEY (idUsuario) REFERENCES Usuario(idUsuario) ON DELETE CASCADE
);

-- TABLA: Paciente
CREATE TABLE Paciente (
    idPaciente INT IDENTITY(1,1) PRIMARY KEY,
    idUsuario INT NOT NULL,
    alergia NVARCHAR(255),
    medicacion NVARCHAR(255),
    ultimaVisita DATE NOT NULL,
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
    idUsuario INT NOT NULL,
    numCedula NVARCHAR(20) UNIQUE NOT NULL,
    idEspecialidad INT NOT NULL,
    FOREIGN KEY (idEspecialidad) REFERENCES EspecialidadMedico(idEspecialidad),
    FOREIGN KEY (idUsuario) REFERENCES Usuario(idUsuario) ON DELETE CASCADE
);

-- TABLA: Consultorio
CREATE TABLE Consultorio (
    idConsultorio INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(60) NOT NULL,
    telefono NVARCHAR(10) NOT NULL,
    calle NVARCHAR(150) NOT NULL,
    numeroInterior NVARCHAR(10),
    numeroExterior NVARCHAR(10) NOT NULL,
    colonia NVARCHAR(100) NOT NULL,
    codigoPostal NVARCHAR(10)
);

-- TABLA: MedicoConsultorio 
CREATE TABLE MedicoConsultorio (
    idMedicoConsultorio INT IDENTITY(1,1) PRIMARY KEY,
    idMedico INT NOT NULL,
    idConsultorio INT NOT NULL,
    FOREIGN KEY (idMedico) REFERENCES Medico(idMedico),
    FOREIGN KEY (idConsultorio) REFERENCES Consultorio(idConsultorio)
);

-- TABLA: Dia
CREATE TABLE Dia(
    idDia INT PRIMARY KEY,
    nombreDia VARCHAR(10) NOT NULL
);

-- TABLA: AgendaDisponibilidad
CREATE TABLE AgendaDisponibilidad (
    idAgendaDisponibilidad INT IDENTITY(1,1) PRIMARY KEY,
    idMedico INT NOT NULL,
    idConsultorio INT NOT NULL,
    idDia INT NOT NULL,
    horaInicioJornada TIME(0) NOT NULL,
    horaFinJornada TIME(0) NOT NULL,
    duracionSlotMinutos INT NOT NULL DEFAULT 30,
    agendaActiva BIT NOT NULL DEFAULT 0,
    fechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    fechaModificacion DATETIME NULL DEFAULT GETDATE(),
    FOREIGN KEY (idMedico) REFERENCES Medico(idMedico),
    FOREIGN KEY (idConsultorio) REFERENCES Consultorio(idConsultorio),
    FOREIGN KEY (idDia) REFERENCES Dia(idDia)
);

-- TABLA: MotivoConsulta
CREATE TABLE MotivoConsulta (
    idMotivo INT IDENTITY(1,1) PRIMARY KEY,
    idMedico INT NOT NULL,
    descripcion NVARCHAR(255) NOT NULL,
    instruccion NVARCHAR(255) DEFAULT 'Sin instrucciones.',
    FOREIGN KEY (idMedico) REFERENCES Medico(idMedico) ON DELETE CASCADE
);

-- ============================================================
-- TABLA MODIFICADA: Cita (actualizada para usar AgendaDisponibilidad)
-- ============================================================
CREATE TABLE Cita (
    idCita INT IDENTITY(1,1) PRIMARY KEY,
    idPaciente INT NOT NULL,
    idMedico INT NOT NULL,
    idConsultorio INT NOT NULL,
    idMotivo INT NOT NULL,
    fechaCita DATE NOT NULL,
    horaInicio TIME(0) NOT NULL,
    horaFin TIME(0) NOT NULL,
    estadoCita NVARCHAR(20) NOT NULL,
    notas NVARCHAR(255) DEFAULT 'Cita agendada.',
    fechaCreacion DATETIME DEFAULT GETDATE(),
    fechaModificacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (idPaciente) REFERENCES Paciente(idPaciente) ON DELETE CASCADE,
    FOREIGN KEY (idMedico) REFERENCES Medico(idMedico),
    FOREIGN KEY (idConsultorio) REFERENCES Consultorio(idConsultorio),
    FOREIGN KEY (idMotivo) REFERENCES MotivoConsulta(idMotivo)
);

-- ============================================================
-- ELIMINADA: TABLA Turno (ya no existe)
-- ============================================================


-- ============================================================
-- INSERTS ORIGINALES (NO MODIFICADOS)
-- ============================================================

-- Usuario
INSERT INTO Usuario (nombre, apellPaterno, apellMaterno, correo, contrasenia, telefono, fechaNac, sexo, rol, fechaCreacion, ultimoLogin)
VALUES
('ANGEL JOAQUIN', 'GARCIA', 'VELAZQUEZ', 'angelgarciavelazquez29@gmail.com', '29092006', '6871167363', '2006-09-29', 'M', 'ADMIN', GETDATE(), GETDATE()),
('CARLOS', 'RAMIREZ', 'LÓPEZ', 'carlosramirez@gmail.com', '12345abcde', '6871123422', '1988-05-10', 'M', 'MEDICO', GETDATE(), GETDATE()),
('MARIO', 'PEREZ', 'JUAREZ', 'marioperez@gmail.com', 'mariobros1234', '6671458580', '1985-02-18', 'M', 'MEDICO', GETDATE(), GETDATE()),
('ANA', 'HERNANDEZ', 'DIAZ', 'anahernandez@gmail.com', 'anahernandez07', '6871320944', '2000-07-07', 'F', 'PACIENTE', GETDATE(), GETDATE()),
('RAUL', 'VALENZUELA', 'MACIAS', 'raulvalenzuela@gmail.com', 'val789', '6871234560', '2001-06-01', 'M', 'PACIENTE', GETDATE(), GETDATE());

-- Administrador
INSERT INTO Administrador (idUsuario) Values (1);

-- EspecialidadMedico
INSERT INTO EspecialidadMedico (nombreEspecialidad)
VALUES
('General'),
('Cardiología'),
('Pediatría'),
('Dermatología');

-- Medico
INSERT INTO Medico (numCedula, idEspecialidad, idUsuario)
VALUES
('CED12345', 2, 2),
('CED67890', 4, 3);

-- Paciente
INSERT INTO Paciente (alergia, medicacion, ultimaVisita, idUsuario)
VALUES
('Penicilina', 'Paracetamol', '2025-09-01', 4),
('Ninguna', 'Ninguna', '2025-10-30', 5);

-- Consultorio
INSERT INTO Consultorio (nombre, calle, numeroInterior, numeroExterior, colonia, codigoPostal, telefono)
VALUES
('Consultorio Central', 'Av. Reforma', '1', '101', 'Centro', '06000', '6873420941'),
('Clínica del Sol', 'Calle Luna', null, '202', 'Roma Norte', '06700', '6879983421');

-- MedicoConsultorio
INSERT INTO MedicoConsultorio (idMedico, idConsultorio)
VALUES
(1, 1), -- Médico Carlos trabaja en el Consultorio Central
(1, 2), -- Médico Carlos trabaja en la Clínica del Sol
(2, 2); -- Médico Mario trabaja en la Clínica del sol

-- Dia
INSERT INTO Dia (idDia, nombreDia)
VALUES
(1, 'Lunes'),
(2, 'Martes'),
(3, 'Miércoles'),
(4, 'Jueves'),
(5, 'Viernes'),
(6, 'Sábado'),
(7, 'Domingo');

-- AgendaDisponibilidad
INSERT INTO AgendaDisponibilidad (idMedico, idConsultorio, idDia, horaInicioJornada, horaFinJornada, duracionSlotMinutos, agendaActiva)
VALUES
-- Franjas de disponibilidad del Médico Carlos en el Consultorio Central
(1, 1, 1, '08:00', '12:00', 30, 1),
(1, 1, 2, '08:00', '12:00', 30, 1),
(1, 1, 3, '08:00', '12:00', 30, 1),
(1, 1, 4, '08:00', '12:00', 30, 1),
(1, 1, 5, '08:00', '12:00', 30, 1),
(1, 1, 6, '08:00', '12:00', 15, 1),
(1, 1, 6, '12:00', '18:00', 15, 1),
(1, 1, 7, '09:00', '13:00', 30, 0),

-- Franjas de disponibilidad del Médico Carlos en la Clínica del Sol
(1, 2, 1, '14:00', '18:00', 30, 1),
(1, 2, 2, '14:00', '18:00', 30, 1),
(1, 2, 3, '14:00', '18:00', 30, 1),
(1, 2, 4, '14:00', '18:00', 30, 1),
(1, 2, 5, '14:00', '18:00', 30, 1),
(1, 2, 7, '15:00', '18:00', 15, 1),

-- Franjas de disponibilidad del Médico Mario en la Clínica del Sol
(2, 2, 1, '12:00', '17:00', 15, 0),
(2, 2, 3, '12:00', '17:00', 15, 0),
(2, 2, 4, '12:00', '17:00', 15, 0),
(2, 2, 5, '12:00', '17:00', 15, 0),
(2, 2, 6, '07:00', '12:00', 15, 1),
(2, 2, 6, '13:00', '17:00', 15, 1),
(2, 2, 7, '07:00', '12:00', 15, 1),
(2, 2, 7, '13:00', '17:00', 15, 1);

INSERT INTO MotivoConsulta (idMedico, descripcion, instruccion)
VALUES
-- Motivos de consulta con el médico Carlos (Cardiólogo)
(1, 'Otros', 'Sin instrucciones.'),
(1, 'Revisión de presión arterial', 'Venir en ayunas.'),
(1, 'Consulta por dolores en el pecho', 'Sin instrucciones.'),
(1, 'Chequeo de colesterol y triglicéridos', 'Venir con 8 horas de ayunas; Traer resultados de laboratorios anteriores si los tiene.'),

-- Motivos de consulta con el médico Mario (Dermatólogo)
(2, 'Otros', 'Sin instrucciones.'),
(2, 'Evaluación de manchas o lunares', 'No aplicar creamas ni maquillaje el día de la consulta.'),
(2, 'Tratamiento de acné', 'Sin instrucciones.'),
(2, 'Consulta por caída excesiva de cabello', 'No lavar el cabello el día de la consulta; Traer estudios médicos previos si los tiene.');


INSERT INTO Cita (idPaciente, idMedico, idConsultorio, idMotivo, fechaCita, horaInicio, horaFin, estadoCita, notas)
VALUES
-- La paciente Ana tiene dos citas agendadas
(1, 1, 1, 2, '2025-12-15', '08:30', '09:00', 'AGENDADA', 'Cita agendada.'),
(1, 2, 2, 8, '2025-12-13', '16:30', '16:45', 'AGENDADA', 'Cita agendada.'),

-- El paciente Raul tiene una cita agendada
(2, 1, 2, 4, '2025-11-30', '15:00', '15:15', 'AGENDADA', 'Cita agendada.'),
(2, 2, 2, 7, '2025-12-28', '07:30', '07:45', 'CANCELADA','Cita cancelada por el paciente.');



-- PRUEBAS AISLADAS
/*
select idAgendaDisponibilidad, idMedico, idConsultorio, idDia, 
                horaInicioJornada, horaFinJornada, duracionSlotMinutos, agendaActiva from AgendaDisponibilidad
                where idMedico = 1 and idConsultorio = 1
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


SELECT * FROM AgendaDisponibilidad;
*/

--hola xddddddd
-- hola ._.