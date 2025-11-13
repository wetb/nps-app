# NPS Application - Guía de Inicio

Esta guía proporciona los pasos necesarios para levantar y configurar la aplicación NPS (Net Promoter Score) con Docker, SQL Server y Angular.

## 1. Construir y Levantar los Contenedores

### 1.1 Construir las imágenes Docker
```powershell
docker-compose build
```

### 1.2 Levantar los servicios en modo detached
```powershell
docker-compose up -d
```

### 1.3 Ver los logs en tiempo real
```powershell
docker-compose logs -f
```

**Nota:** Presiona `Ctrl + C` para salir de los logs sin detener los contenedores.

---

## 2. Configurar la Base de Datos en SQL Server Management Studio

### 2.1 Conectar a SQL Server
1. Abre **SQL Server Management Studio**
2. En **Connect to Server**, completa los siguientes datos:
   - **Server name:** `localhost, 1433`
   - **Authentication:** SQL Server Authentication
   - **Login:** `sa`
   - **Password:** `Password123!`
   - **Encryption:** Selecciona **Obligatorio**
   - **Confiar en el cifrado del servidor:** Marca ✓

3. Haz clic en **Connect**

### 2.2 Crear la base de datos
1. En SSMS, abre el archivo `Backend/Database/CreateDatabase.sql`
2. **NO ejecutes aún** - primero debes obtener los hashes de las contraseñas (ver sección 3)

---

## 3. Generar Hashes de Contraseñas

### 3.1 Ejecutar HashPasswords
```powershell
cd Backend/HashPasswords
dotnet build
dotnet run
```

**Nota:** 
- `dotnet build` compila el proyecto
- `dotnet run` ejecuta la aplicación para generar los hashes

### 3.2 Reemplazar los hashes en CreateDatabase.sql

Cuando ejecutes `HashPasswords`, obtendrás dos hashes para:
- **Usuario:** `admin` (Contraseña: `Admin123!`)
- **Usuario:** `voter1` (Contraseña: `Voter123!`)

Ejemplo de salida esperada:
```
Contraseña: Admin123!
Hash BCrypt: $2a$11$XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

Contraseña: Voter123!
Hash BCrypt: $2a$11$YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
```

### 3.3 Actualizar CreateDatabase.sql
1. Abre `Backend/Database/CreateDatabase.sql` en SSMS o en tu editor
2. Busca la sección **"Datos Iniciales"** al final del archivo
3. Reemplaza los hashes en las líneas correspondientes:

**Para el usuario admin:**
```sql
INSERT INTO Users (Username, PasswordHash, Role, CreatedAt)
VALUES ('admin', '$2a$11$XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX', 2, GETUTCDATE());
```

**Para el usuario voter1:**
```sql
INSERT INTO Users (Username, PasswordHash, Role, CreatedAt)
VALUES 
    ('voter1', '$2a$11$YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY', 1, GETUTCDATE());
```

---

## 4. Ejecutar el Script de Base de Datos

### 4.1 Ejecutar en SSMS
1. En SSMS, con el archivo `CreateDatabase.sql` abierto y los hashes actualizados
2. Presiona `Ctrl + A` para seleccionar todo
3. Presiona `Ctrl + E` o haz clic en **Execute**
4. Espera a que se completen todas las operaciones

**Salida esperada:**
```
Base de datos creada exitosamente
```

---

## 5. Acceder a la Aplicación Angular

### 5.1 Abrir el navegador
1. Abre tu navegador web
2. Navega a: `http://localhost:4200/`

### 5.2 Flujo para Usuario Admin
1. **Login:**
   - Username: `admin`
   - Password: `Admin123!`
2. Accede al dashboard de administración
3. Verifica las opciones disponibles

### 5.3 Flujo para Usuario Votante (Voter1)
1. **Logout** del usuario admin (si es necesario)
2. **Login con voter1:**
   - Username: `voter1`
   - Password: `Voter123!`
3. Completa el formulario de votación NPS:
   - Selecciona una puntuación del 0-10
   - Confirma tu voto
4. Verifica que el voto se registre correctamente

---

## Resumen del Flujo Completo

```
1. docker-compose build
   ↓
2. docker-compose up -d
   ↓
3. docker-compose logs -f (para monitorear)
   ↓
4. Conectar a SQL Server Management Studio
   ↓
5. dotnet run (HashPasswords)
   ↓
6. Actualizar hashes en CreateDatabase.sql
   ↓
7. Ejecutar CreateDatabase.sql en SSMS
   ↓
8. Abrir http://localhost:4200/
   ↓
9. Login usuario admin (Admin123!)
   ↓
10. Logout y login usuario voter1 (Voter123!)
    ↓
11. Completar formulario de votación NPS
```

---

## Solución de Problemas

### Puerto 1433 ya está en uso
```powershell
netstat -ano | findstr :1433
# Identifica el PID y termina el proceso si es necesario
taskkill /PID <PID> /F
```

### Puerto 4200 ya está en uso
```powershell
netstat -ano | findstr :4200
# Identifica el PID y termina el proceso si es necesario
taskkill /PID <PID> /F
```

### Detener todos los contenedores
```powershell
docker-compose down
```

### Ver logs de un servicio específico
```powershell
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f sqlserver
```

---

## Credenciales de Acceso

| Usuario | Contraseña | Rol |
|---------|-----------|-----|
| admin | Admin123! | Administrador |
| voter1 | Voter123! | Votante |
| sa | Password123! | SQL Server |

---

## Variables de Entorno

La aplicación utiliza las siguientes configuraciones:
- **SQL Server:** localhost:1433
- **Base de datos:** NPSApplicationDB
- **Frontend:** http://localhost:4200/
- **Backend API:** http://localhost:5000/

---
