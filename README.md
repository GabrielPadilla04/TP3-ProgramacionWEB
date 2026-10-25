# 🛒 Sistema POS & Gestión Comercial - Trabajo Práctico N°3 (ITES)

Sistema Integral de Punto de Venta (POS) desarrollado con **ASP.NET Core Web API** en el backend y **Blazor WebAssembly (Standalone)** en el frontend, orientado a la administración y control de stock de negocios minoristas.

---

## 👥 Integrantes del Equipo
* **Gabriel Padilla**
* **Valentin Montes**

---
Paso a Paso para Ejecutar el Sistema
Prerrequisitos previos
Antes de comenzar, asegúrate de tener instalado en tu computadora:
.NET 9 SDK
MariaDB corriendo localmente o en un servidor accesible.
Visual Studio 2022 o Visual Studio Code con la extensión de C#.
Un gestor de bases de datos.

Paso 1 Clonar el repositorio y cambiar a la rama 

git clone https://github.com/GabrielPadilla04/TP3-ProgramacionWEB
cd <nombre_de_la_carpeta>

Paso 2: Configurar la Base de Datos MariaDB.

Creá una base de datos vacía llamada exactamente tp3_puntoventa_db, entrá a la carpeta del backend (TP3_API) y buscá el archivo de configuración appsettings.json.

Asegurate de que la cadena de conexión (ConexionDB) apunte correctamente a tu servidor local de MariaDB con tu usuario y contraseña configurados anteriormente:
Paso 3: Aplicar las Migraciones (Entity Framework Core)

Para crear automáticamente todas las tablas normalizadas en tu base de datos ubícate en la terminal dentro de la carpeta del proyecto backend y corre

cd TP3_API

Ejecuta el comando de Entity Framework y actualiza/crea la base de datos:
dotnet ef database update

Paso 4: Ejecutar el Backend (ASP.NET Web API)

Desde la misma carpeta TP3_API, ejecuta el servidor de la API:

dotnet run

Una vez ejecutado, la terminal te indicará en qué puertos está corriendo 

Paso 5: Ejecutar el Frontend

Abrir otra pestaña o ventana de la terminal y navegar hasta la carpeta del proyecto de Blazor (TP3_Blazor o el nombre que tenga tu cliente):

cd ../TP3_Blazor

Ejecutar la aplicación frontend:
dotnet run
Y felicitaciones! ya puedes navegar por el sistema

## 🚀 Stack Tecnológico
* **Backend:** ASP.NET Core 9 Web API, Entity Framework Core.
* **Base de Datos:** MariaDB (Normalizada hasta la 3FN).
* **Frontend:** Blazor WebAssembly (Standalone) con componentes interactivos.
* **Generación de Reportes:** QuestPDF (para reportes consolidados en formato PDF).
* **Logging y Auditoría:** Serilog (archivos físicos) + Middleware global personalizado con persistencia en Base de Datos (`LogAuditoria`).
* **Seguridad:** Autenticación y Autorización basada en Tokens **JWT** con control de acceso por roles (RBAC).

---

## ⚙️ Características Principales
1. **Punto de Venta (POS):** Registro de ventas de forma segura para usuarios autenticados, descontando stock de forma transaccional.
2. **Gestión de Inventario y Compras:** Registro de entradas de mercadería por proveedor para mantener el stock actualizado.
3. **Panel de Administración Exclusivo:** Panel protegido mediante claims de rol ("Administrador") para control global.
4. **Reportes en PDF:** Endpoint unificado que genera informes detallados de stock, ventas e historial de ingresos.
5. **Auditoría de Errores y Seguridad:** Middleware global que captura excepciones no controladas, responde con un JSON normalizado (Código 500) y almacena el rastro técnico en la base de datos y en logs físicos.

—

## Diagrama ER

<img width="5397" height="6725" alt="Inventory Sales Management System-2026-10-25-234951" src="https://github.com/user-attachments/assets/9b7e45f3-1da9-4d95-b35c-77f5974e5b66" />
