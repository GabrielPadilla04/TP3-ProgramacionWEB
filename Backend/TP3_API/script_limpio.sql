CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Productos` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `CodigoBarras` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Nombre` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `PrecioCosto` decimal(18,2) NOT NULL,
    `PrecioVenta` decimal(18,2) NOT NULL,
    `StockActual` int NOT NULL,
    `StockMinimo` int NOT NULL,
    CONSTRAINT `PK_Productos` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Roles` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Nombre` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Roles` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Usuarios` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `NombreUsuario` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `PasswordHash` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Email` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `RolId` int NOT NULL,
    CONSTRAINT `PK_Usuarios` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Usuarios_Roles_RolId` FOREIGN KEY (`RolId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `EntradasMercaderia` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Fecha` datetime(6) NOT NULL,
    `Proveedor` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `UsuarioId` int NOT NULL,
    CONSTRAINT `PK_EntradasMercaderia` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_EntradasMercaderia_Usuarios_UsuarioId` FOREIGN KEY (`UsuarioId`) REFERENCES `Usuarios` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `LogsAuditoria` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Fecha` datetime(6) NOT NULL,
    `Nivel` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Mensaje` longtext CHARACTER SET utf8mb4 NOT NULL,
    `StackTrace` longtext CHARACTER SET utf8mb4 NULL,
    `UsuarioId` int NULL,
    CONSTRAINT `PK_LogsAuditoria` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_LogsAuditoria_Usuarios_UsuarioId` FOREIGN KEY (`UsuarioId`) REFERENCES `Usuarios` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Ventas` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Fecha` datetime(6) NOT NULL,
    `Total` decimal(18,2) NOT NULL,
    `UsuarioId` int NOT NULL,
    CONSTRAINT `PK_Ventas` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Ventas_Usuarios_UsuarioId` FOREIGN KEY (`UsuarioId`) REFERENCES `Usuarios` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `EntradaMercaderiaDetalles` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `EntradaId` int NOT NULL,
    `ProductoId` int NOT NULL,
    `Cantidad` int NOT NULL,
    `CostoUnitario` decimal(18,2) NOT NULL,
    CONSTRAINT `PK_EntradaMercaderiaDetalles` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_EntradaMercaderiaDetalles_EntradasMercaderia_EntradaId` FOREIGN KEY (`EntradaId`) REFERENCES `EntradasMercaderia` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_EntradaMercaderiaDetalles_Productos_ProductoId` FOREIGN KEY (`ProductoId`) REFERENCES `Productos` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `VentaDetalles` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `VentaId` int NOT NULL,
    `ProductoId` int NOT NULL,
    `Cantidad` int NOT NULL,
    `PrecioUnitario` decimal(18,2) NOT NULL,
    `Subtotal` decimal(18,2) NOT NULL,
    CONSTRAINT `PK_VentaDetalles` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_VentaDetalles_Productos_ProductoId` FOREIGN KEY (`ProductoId`) REFERENCES `Productos` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_VentaDetalles_Ventas_VentaId` FOREIGN KEY (`VentaId`) REFERENCES `Ventas` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_EntradaMercaderiaDetalles_EntradaId` ON `EntradaMercaderiaDetalles` (`EntradaId`);

CREATE INDEX `IX_EntradaMercaderiaDetalles_ProductoId` ON `EntradaMercaderiaDetalles` (`ProductoId`);

CREATE INDEX `IX_EntradasMercaderia_UsuarioId` ON `EntradasMercaderia` (`UsuarioId`);

CREATE INDEX `IX_LogsAuditoria_UsuarioId` ON `LogsAuditoria` (`UsuarioId`);

CREATE INDEX `IX_Usuarios_RolId` ON `Usuarios` (`RolId`);

CREATE INDEX `IX_VentaDetalles_ProductoId` ON `VentaDetalles` (`ProductoId`);

CREATE INDEX `IX_VentaDetalles_VentaId` ON `VentaDetalles` (`VentaId`);

CREATE INDEX `IX_Ventas_UsuarioId` ON `Ventas` (`UsuarioId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261025020845_InitialCreate', '8.0.8');

COMMIT;

START TRANSACTION;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261025021244_Create2', '8.0.8');

COMMIT;

