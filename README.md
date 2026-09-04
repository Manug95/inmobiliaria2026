# inmobiliaria2026

> Proyecto Inmobiliaria para la materia Laboratorio II.

---

## 👥 Integrantes del Grupo

* **Manuel Gutierrez** - [@Manug95](https://github.com/Manug95)

---

## 📐 Modelado de Datos

A continuación se presenta el esquema del modelo de datos correspondiente a la aplicación:

### Diagrama Entidad-Relación (DER) / Diagrama de Clases

![Diagrama del Proyecto](./DER_inmobiliaria.jpg)

### 🗄️ Crear e Inicializar la Base de Datos
El archivo "dbinmobiliaria.sql" contiene la creación de la BD en MySql 8.0.43 y la creación de las tablas necesarias para la aplicación.

#### Opción 1: importar con MySQL Workbench

1. Abrir MySQL Workbench.
2. Iniciar en su instancia local.
3. En el panel izquierdo, seleccionar la pestaña **Administration**.
4. En la secci'on 'MANAGEMENT' seleccionar **Data import/restore**.
5. Seleccionar el radio button **Import from Self-Contained File**.
6. Buscar y elegir el archivo `dbinmobiliaria.sql` de este proyecto.
7. Presionar el boton **Start Import**.
8. En el panel izquierdo, ir a la pestaña **Schemas**
9. Click derecho en cualquier lugar del panel y seleccionar **Refresh All**
10. Verificar que aparezca la base `dbinmobiliaria` con las tablas `propietarios`, `inquilinos`, `inmuebles`, `tipos_inmueble`, `imagenes` y `reservas`.

#### Opción 2: importar con phpMyAdmin

1. Abrir el panel de control de XAMPP.
2. Iniciar los módulos **Apache** y **MySQL**.
3. Presionar **Admin** junto al módulo MySQL para abrir phpMyAdmin.
4. Seleccionar la pestaña **Importar**.
5. Elegir el archivo `dbinmobiliaria.sql` del proyecto.
6. Mantener el formato SQL y presionar **Continuar**.
7. Verificar que aparezca la base `dbinmobiliaria` con las tablas `propietarios`, `inquilinos`, `inmuebles`, `tipos_inmueble`, `imagenes` y `reservas`.

# 🚀 Guía para ejecutar el proyecto
## 1. Clonar el repositorio
```bash
git clone https://github.com/Manug95/inmobiliaria2026.git
cd inmobiliaria2026
```
## 2. Revisar configuración
En el archivo `appsettings.json` en la raiz del proyecto, asegurarse de tener su usuario y contraseña de su gestor de base de datos en `ConnectionStrings:MySql`.
## 3. Restaurar dependencias, compilar y ejecutar
```bash
dotnet restore
dotnet build
dotnet run
```

Abrir en el navegador en http://localhost:5140

## Advertencia
Ejecutar la app con `dotnet run` porque al ejecutar con `dotnet watch` crashea al cargar las imagenes de los inmuebles

### 🏠 Realizar una Reserva

#### Opción 1
1. En el menú de navegacion Inmuebles -> Reservar Inmueble
2. Rellenar el fomulario con los campo requeridos y deseados
3. Si se encontro el inmueble deseado, clickear el icono de Reservar Inmueble
4. Rellenar el formulario de la reserva

#### Opción 2
1. En el menú de navegacion Inquilinos
2. Buscar el Inquilino que quiere hacer la reserva
3. Clickear el icono de Reservar Inmueble que tiene cada inquilino
4. Rellenar el formulario de la reserva