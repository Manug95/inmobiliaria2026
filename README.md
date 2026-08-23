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
10. Verificar que aparezca la base `dbinmobiliaria` con las tablas `propietarios` e `inquilinos`.

#### Opción 2: importar con phpMyAdmin

1. Abrir el panel de control de XAMPP.
2. Iniciar los módulos **Apache** y **MySQL**.
3. Presionar **Admin** junto al módulo MySQL para abrir phpMyAdmin.
4. Seleccionar la pestaña **Importar**.
5. Elegir el archivo `dbinmobiliaria.sql` del proyecto.
6. Mantener el formato SQL y presionar **Continuar**.
7. Verificar que aparezca la base `dbinmobiliaria` con las tablas `propietarios` e `inquilinos`.