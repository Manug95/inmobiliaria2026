CREATE DATABASE  IF NOT EXISTS `dbinmobiliaria` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `dbinmobiliaria`;
-- MySQL dump 10.13  Distrib 8.0.43, for Win64 (x86_64)
--
-- Host: localhost    Database: dbinmobiliaria
-- ------------------------------------------------------
-- Server version	8.0.43

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `imagenes`
--

DROP TABLE IF EXISTS `imagenes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `imagenes` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ruta` varchar(255) NOT NULL,
  `inmuebleId` int unsigned NOT NULL,
  PRIMARY KEY (`id`),
  KEY `inmuebleId` (`inmuebleId`),
  CONSTRAINT `imagenes_ibfk_1` FOREIGN KEY (`inmuebleId`) REFERENCES `inmuebles` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `imagenes`
--

LOCK TABLES `imagenes` WRITE;
/*!40000 ALTER TABLE `imagenes` DISABLE KEYS */;
INSERT INTO `imagenes` VALUES (1,'/Uploads\\Inmuebles\\3\\46ebaa5b-1cda-459a-84a2-2a43104cf09d.jpg',3),(4,'/Uploads\\Inmuebles\\3\\f521f819-e117-41ce-9a30-3ff4bafc8f3e.jpg',3);
/*!40000 ALTER TABLE `imagenes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inmuebles`
--

DROP TABLE IF EXISTS `inmuebles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inmuebles` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `idPropietario` int unsigned NOT NULL,
  `idTipoInmueble` tinyint unsigned NOT NULL,
  `calle` varchar(100) NOT NULL,
  `latitud` decimal(10,8) DEFAULT NULL,
  `longitud` decimal(11,8) DEFAULT NULL,
  `precio` decimal(10,2) unsigned NOT NULL,
  `disponible` tinyint(1) NOT NULL DEFAULT '1',
  `borrado` tinyint(1) NOT NULL DEFAULT '0',
  `nroCalle` mediumint unsigned NOT NULL,
  `foto` varchar(255) DEFAULT NULL,
  `cupo` int unsigned NOT NULL,
  `senia` int unsigned NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idPropietario` (`idPropietario`),
  KEY `idTipoInmueble` (`idTipoInmueble`),
  CONSTRAINT `inmuebles_ibfk_1` FOREIGN KEY (`idPropietario`) REFERENCES `propietarios` (`id`),
  CONSTRAINT `inmuebles_ibfk_2` FOREIGN KEY (`idTipoInmueble`) REFERENCES `tipos_inmueble` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=55 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inmuebles`
--

LOCK TABLES `inmuebles` WRITE;
/*!40000 ALTER TABLE `inmuebles` DISABLE KEYS */;
INSERT INTO `inmuebles` VALUES (1,3,2,'CALLE',12.21310000,123.12314000,125.00,1,0,321,'/Uploads\\Inmuebles\\portada_1.jpg',4,20),(2,6,3,'Villarino',12.34500000,67.78900000,500.00,1,0,338,'/Uploads\\Inmuebles\\portada_2.jpg',2,5),(3,4,4,'Mitre',21.54300000,76.98700000,130.00,1,0,618,NULL,3,15),(4,3,2,'lolol',12.23452460,54.42523500,654321.00,0,1,1237,NULL,0,0),(5,3,2,'callerina',12.23452460,54.42523500,654321.00,1,1,1234,NULL,0,0),(6,7,3,'Buenos Aires',12.34560000,12.65430000,400.00,1,0,464,NULL,5,20),(7,5,3,'Gral. Pinto',13.45670000,13.76540000,123.00,1,0,846,NULL,6,10),(8,7,1,'Buenos Aires',12.34568000,12.86540000,650.00,1,0,465,'/Uploads\\Inmuebles\\portada_8.jpg',1,25),(9,4,1,'Mitre',21.54400000,76.98800000,123.00,1,0,620,NULL,1,5),(10,8,3,'Entre Ríos',15.45345000,15.63463000,450.00,1,0,432,NULL,8,35),(11,6,2,'Irigoyen',23.32523500,124.25562000,370.00,1,0,100,NULL,2,50),(12,4,4,'Illia',53.25623000,45.32423000,235.00,1,0,587,NULL,3,23),(13,10,1,'Corrientes',53.32532000,143.62622300,350.00,1,0,346,NULL,1,20),(14,4,4,'Belgrano',57.43563400,21.25626000,520.00,0,0,240,NULL,3,15),(15,1,1,'San Luis',45.15610000,15.45400000,999.99,1,0,123,'/Uploads\\Inmuebles\\portada_15.jpg',2,33);
/*!40000 ALTER TABLE `inmuebles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inquilinos`
--

DROP TABLE IF EXISTS `inquilinos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inquilinos` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `apellido` varchar(50) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `dni` varchar(13) NOT NULL,
  `telefono` varchar(25) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_dni` (`dni`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inquilinos`
--

LOCK TABLES `inquilinos` WRITE;
/*!40000 ALTER TABLE `inquilinos` DISABLE KEYS */;
INSERT INTO `inquilinos` VALUES (1,'Larrea','Fausto Antón','22222221','2222-111111','anton_rally@mail.com',1),(2,'Alunda','Agustín','22222222','2222-111112','agustin_alunda@mail.com',1),(3,'García','Brian','22222223','2222-111113','xBrian@mail.com',1),(4,'Correa','Juan Manuel','22222224','2222-111114','juan_correa@mail.com',0),(5,'Manesse','Matías','22222225','2222-111115','matias_manesse@mail.com',1),(6,'Piva','Valentina','22222226','2222-111116','valentina_piva@mail.com',1),(7,'Gutierrez','Marcos','22222227','2222-111117','marcos_gutierrez@mail.com',1),(8,'Piva','Candela','12222228','2122-111118','cande_piva@mail.com',1),(9,'Labaronie','Martina','22222229','2222-111119','martina_labaronie@mail.com',1),(10,'Mari','Matías','22222210','2222-111120','matias_mari@mail.com',1),(11,'Gutierrez','Agustina','22222211','2222-111121','agus_gutierrez@mail.com',1),(12,'Bernasconi','Nicolás','22222212','2222-111122','nico_bernasconi@mail.com',1),(13,'Toledo','Branko','22222213','2222-111123','toledin@mail.com',1),(14,'Serrani','Rodrigo','22222214','2222-111124','serra@mail.com',1),(15,'Gutierrez','Lucía','22222215','2222-111125','lucy@mail.com',1),(16,'Iuri','Enzo','22222216','2222-111126','enzo_iuri@mail.com',1),(17,'Labaronie','Trinidad','22222217','2222-111127','trini_labaronie@mail.com',1),(18,'Tripode','Tomás','22222218','2222-111128','tripa@mail.com',1),(19,'Gutierrez','Delfina','22222219','2222-111129','titi@mail.com',1),(20,'Della Croce','Mario','22222230','2222-111130','marito@mail.com',1),(21,'Longo','Ramiro','22222231','2222-111131','rama@mail.com',1),(22,'Palacios','Ignacio','22222232','2222-111132','nachito@mail.com',1),(23,'apellidoModificado','nombreMOdificado','11111119','1111-234566','example@mail.com',0),(24,'bc','ac','10000001','1234-567891','asd@mail.com',0);
/*!40000 ALTER TABLE `inquilinos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pagos`
--

DROP TABLE IF EXISTS `pagos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pagos` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `reservaId` bigint unsigned NOT NULL,
  `fecha` date NOT NULL DEFAULT (curdate()),
  `importe` decimal(10,2) NOT NULL,
  `concepto` varchar(255) DEFAULT NULL,
  `anulado` tinyint(1) NOT NULL DEFAULT '0',
  `idUsuarioCobrador` int unsigned NOT NULL,
  `idUsuarioAnulador` int unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `reservaId` (`reservaId`),
  KEY `idUsuarioCobrador` (`idUsuarioCobrador`),
  KEY `idUsuarioAnulador` (`idUsuarioAnulador`),
  CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`reservaId`) REFERENCES `reservas` (`id`),
  CONSTRAINT `pagos_ibfk_2` FOREIGN KEY (`idUsuarioCobrador`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `pagos_ibfk_3` FOREIGN KEY (`idUsuarioAnulador`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pagos`
--

LOCK TABLES `pagos` WRITE;
/*!40000 ALTER TABLE `pagos` DISABLE KEYS */;
INSERT INTO `pagos` VALUES (1,1,'2026-09-02',487.50,'primer pago asd',0,1,NULL),(2,3,'2026-09-07',325.00,'multa por terminar la reserva',0,1,NULL);
/*!40000 ALTER TABLE `pagos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propietarios`
--

DROP TABLE IF EXISTS `propietarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propietarios` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `apellido` varchar(50) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `dni` varchar(13) NOT NULL,
  `telefono` varchar(25) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_propietarios_dni` (`dni`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propietarios`
--

LOCK TABLES `propietarios` WRITE;
/*!40000 ALTER TABLE `propietarios` DISABLE KEYS */;
INSERT INTO `propietarios` VALUES (1,'Gutierrez','Manuel','12345678','2364123456','yo@mail.com',1),(2,'Torres','Roberto','11111112','1111-131313','roberto_torres@mail.com',1),(3,'Martínez','Ana','11111113','1111-141414','ana_martinez@mail.com',1),(4,'Salmeron','José','11111114','1111-151515','jose_salmeron@mail.com',1),(5,'Alonso','Marcela','11111115','1111-161616','marcela_alonso@mail.com',1),(6,'Gutiérrez','Ernesto Darío','11111116','1321-171717','dario_gutierrez@mail.com',1),(7,'López','Raúl','11111117','1111-181818','raul_lopez@mail.com',1),(8,'Morichetti','María Luisa','11111118','1111-191919','marisa_morichetti@mail.com',1),(9,'Gutierrez','Nerea','11111119','1111-101919','nerea_gutierrez@mail.com',1),(10,'Rato','Miriam','11111110','1111-101010','miriam_rato@mail.com',1),(11,'apeProp11','nomProp11','11111199','1111-111010','nomProp11@mail.com',1),(12,'apeProp12','nomProp12','11111121','1111-111012','nomProp12@mail.com',1),(13,'apeProp13','nomProp13','11111211','1111-131313','nomProp13@mail.com',1),(14,'apeProp14','nomProp14','11111141','1111-131314','nomProp14@mail.com',1),(15,'apeProp15','nomProp15','11111151','1111-131315','nomProp15@mail.com',1),(16,'apeProp16','nomProp16','11111161','1111-131316','nomProp16@mail.com',1),(17,'apeProp17','nomProp17','11111171','1111-131317','nomProp17@mail.com',1),(18,'apeProp18','nomProp18','11111181','1111-131318','nomProp18@mail.com',1),(19,'apeProp19','nomProp19','11111191','1111-131319','nomProp19@mail.com',1),(20,'apeProp20','nomProp20','11111120','1111-131320','nomProp20@mail.com',1),(21,'apeProp21','nomProp21','11112111','1111-131321','nomProp21@mail.com',1),(22,'apeProp22','nomProp22','11111122','1111-111122','nomProp22@mail.com',1),(23,'apeProp23','nomProp23','11111123','1111-111123','nomProp23@mail.com',1),(24,'apeProp24','nomProp24','11111124','1111-111124','nomProp24@mail.com',1),(25,'apeProp25','nomProp25','11111125','1111-111125','nomProp25@mail.com',1),(26,'apeProp26','nomProp26','11111126','1111-111126','nomProp26@mail.com',1),(27,'apeProp27','nomProp27','11111127','1111-111127','nomProp27@mail.com',1),(28,'apeProp28','nomProp28','11111128','1111-111128','nomProp28@mail.com',1),(29,'apeProp29','nomProp29','11111129','1111-111129','nomProp29@mail.com',1),(30,'apeProp30','nomProp30','11111130','1111-111130','nomProp30@mail.com',1),(31,'apeProp31','nomProp31','11111131','1111-111131','nomProp31@mail.com',1),(32,'apeProp32','nomProp32','11111132','1111-111132','nomProp32@mail.com',0),(33,'apeProp33','nomProp33','11111133','1111-111133','nomProp33@mail.com',1),(34,'apeProp34','nomProp34','11111134','1111-111134','nomProp34@mail.com',1),(35,'apePrueba','nomPrueba','00200001','1234-567899','prueba@mail.com',0),(36,'bc','ac','10000001','1234-567891','asd@mail.com',0);
/*!40000 ALTER TABLE `propietarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reservas`
--

DROP TABLE IF EXISTS `reservas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reservas` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `idInquilino` int unsigned NOT NULL,
  `idInmueble` int unsigned NOT NULL,
  `monto` decimal(10,2) NOT NULL,
  `fechaInicio` date NOT NULL,
  `fechaFin` date NOT NULL,
  `fechaTerminado` date DEFAULT NULL,
  `borrado` tinyint(1) NOT NULL DEFAULT '0',
  `idUsuarioReservador` int unsigned NOT NULL,
  `idUsuarioTerminador` int unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idInquilino` (`idInquilino`),
  KEY `idInmueble` (`idInmueble`),
  KEY `idUsuarioReservador` (`idUsuarioReservador`),
  KEY `idUsuarioTerminador` (`idUsuarioTerminador`),
  CONSTRAINT `reservas_ibfk_1` FOREIGN KEY (`idInquilino`) REFERENCES `inquilinos` (`id`),
  CONSTRAINT `reservas_ibfk_2` FOREIGN KEY (`idInmueble`) REFERENCES `inmuebles` (`id`),
  CONSTRAINT `reservas_ibfk_3` FOREIGN KEY (`idUsuarioReservador`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `reservas_ibfk_4` FOREIGN KEY (`idUsuarioTerminador`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reservas`
--

LOCK TABLES `reservas` WRITE;
/*!40000 ALTER TABLE `reservas` DISABLE KEYS */;
INSERT INTO `reservas` VALUES (1,1,8,650.00,'2026-09-02','2026-09-05',NULL,0,1,NULL),(2,2,8,650.00,'2026-09-06','2026-09-10',NULL,0,1,NULL),(3,2,8,650.00,'2026-09-11','2026-09-12','2026-09-07',0,1,NULL),(4,3,1,125.00,'2025-09-15','2025-09-22',NULL,0,1,NULL),(5,7,1,125.00,'2025-10-01','2025-10-10',NULL,0,1,NULL),(6,1,1,125.00,'2025-11-15','2025-11-25',NULL,0,1,NULL),(7,12,1,125.00,'2026-01-10','2026-01-18',NULL,0,1,NULL),(8,5,1,125.00,'2026-04-05','2026-04-15',NULL,0,1,NULL),(9,2,2,500.00,'2025-09-20','2025-09-27',NULL,0,1,NULL),(10,9,2,500.00,'2025-11-01','2025-11-08',NULL,0,1,NULL),(11,14,2,500.00,'2026-02-10','2026-02-17',NULL,0,1,NULL),(12,11,2,500.00,'2026-06-01','2026-06-10',NULL,0,1,NULL),(13,3,12,235.00,'2025-10-12','2025-10-20',NULL,0,1,NULL),(14,8,12,235.00,'2025-12-20','2025-12-30',NULL,0,1,NULL),(15,15,12,235.00,'2026-03-15','2026-03-22',NULL,0,1,NULL),(16,6,3,130.00,'2025-10-05','2025-10-12',NULL,0,1,NULL),(17,10,3,130.00,'2026-05-10','2026-05-18',NULL,0,1,NULL),(18,13,8,650.00,'2026-01-15','2026-01-22',NULL,0,1,NULL),(19,1,1,125.00,'2024-05-01','2024-05-10',NULL,0,1,NULL),(20,5,5,320.00,'2024-08-15','2024-08-22',NULL,0,1,NULL),(21,9,7,123.00,'2025-01-10','2025-01-20',NULL,0,1,NULL),(22,3,7,123.00,'2026-09-08','2026-09-13',NULL,0,1,NULL),(23,14,2,500.00,'2026-09-10','2026-09-15',NULL,0,1,NULL),(24,1,15,999.99,'2026-09-09','2026-09-16',NULL,0,1,NULL),(25,22,6,400.00,'2026-09-11','2026-09-18',NULL,0,1,NULL),(26,9,11,370.00,'2026-09-07','2026-09-20',NULL,0,1,NULL);
/*!40000 ALTER TABLE `reservas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tipos_inmueble`
--

DROP TABLE IF EXISTS `tipos_inmueble`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipos_inmueble` (
  `id` tinyint unsigned NOT NULL AUTO_INCREMENT,
  `tipo` varchar(50) NOT NULL,
  `descripcion` varchar(255) DEFAULT 'SIN DESCRIPCIÓN',
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `tipo` (`tipo`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tipos_inmueble`
--

LOCK TABLES `tipos_inmueble` WRITE;
/*!40000 ALTER TABLE `tipos_inmueble` DISABLE KEYS */;
INSERT INTO `tipos_inmueble` VALUES (1,'MONOAMBIENTE',NULL,1),(2,'LOFT',NULL,1),(3,'CASA','asd',1),(4,'DEPARTAMENTO',NULL,1),(5,'wasd','epa',0);
/*!40000 ALTER TABLE `tipos_inmueble` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuarios`
--

DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `email` varchar(100) NOT NULL,
  `pass` varchar(60) NOT NULL,
  `rol` enum('ADMIN','EMPLEADO') NOT NULL DEFAULT 'EMPLEADO',
  `nombre` varchar(60) DEFAULT NULL,
  `apellido` varchar(60) DEFAULT NULL,
  `avatar` varchar(255) DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios`
--

LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
INSERT INTO `usuarios` VALUES (1,'manuel@mail.com','5cPdoAYcEWknZJd5+8/2Yiwcz8G8tKpHrq8dLpCc/zU=','ADMIN','Manuel','Gutierrez','/Uploads\\Avatares\\avatar_1.png',1),(2,'a@mail.com','5cPdoAYcEWknZJd5+8/2Yiwcz8G8tKpHrq8dLpCc/zU=','EMPLEADO','Carlitos','el empleado','/Uploads\\Avatares\\avatar_2.png',1),(3,'b@mail.com','5cPdoAYcEWknZJd5+8/2Yiwcz8G8tKpHrq8dLpCc/zU=','EMPLEADO','Maria','la del barrio',NULL,0);
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-16 20:11:47
