// See https://aka.ms/new-console-template for more information
using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Linq;
using System.Text;


using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;



class Program
{
    static string connectionString = "Server=localhost;Port=3306;Database=transporte_publico;Uid=root;Pwd=1511;";

    static void Main(string[] args)
    {
        MostrarMenu();
    }

    static void MostrarMenu(){
                while (true)
        {
            Console.WriteLine("1. Crear usuario");
            Console.WriteLine("2. Borrar usuario");
            Console.WriteLine("3. Actualizar usuario");
            Console.WriteLine("4. Consultar usuarios");
            Console.WriteLine("5. Otrogar privilegio");
            Console.WriteLine("6. Revocar privilegio");
            Console.WriteLine("7. Consultar privilegios de usuarios");
            Console.WriteLine("8. Consultar todos los privilegio");
            Console.WriteLine("9. Resplado base de datos");
            Console.WriteLine("10. Restaurar base de datos");
            Console.WriteLine("11. Consultar todas las tablas");
            Console.WriteLine("12. Consultar atributos por entidades");
            Console.WriteLine("13. Agregar entidades con atributos ");
            Console.WriteLine("14. CURD");
            Console.WriteLine("15. Salir");
            Console.WriteLine("16. Elaborar PDF");
            Console.WriteLine("Selecciona una opción:");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    CrearUsuario();
                    break;
                case "2":
                    BorrarUsuario();
                    break;
                case "3":
                    ActualizarUsuario();
                    break;
                case "4":
                    ConsultarUsuarios();
                    break;
                case "5":
                    OtorgarPrivilegio();
                    break;
                case "6":
                    RevocarPrivilegio();
                    break;
                case "7":
                    ConsultarPrivilegiosUsuario();
                    break;
                case "8":
                    ConsultarPrivilegios();
                    break;
                case "9":
                    RespaldarBaseDeDatos();
                    break;
                case "10":
                    RestaurarBaseDeDatos();
                    break;
                case "11":
                    ConsultarTodasLasTablas();
                    break;
                case "12":
                    ConsultarAtributosPorEntidad();
                    break;
                case "13":
                    AgregarNuevaTabla();
                    break;
                case "14":
                    GenerarScriptCRUD();
                    break;
                case "15":
                    break;
                case "16":
                    GenerarReportePDF();
                    return;
            }

            Console.WriteLine();
        }
    }
    
    static void CrearUsuario()
    {
        Console.WriteLine("Ingrese el nombre del nuevo usuario:");
        string nombreUsuario = Console.ReadLine();

        Console.WriteLine("Ingrese la contraseña del nuevo usuario:");
        string contraseña = Console.ReadLine();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand($"CREATE USER '{nombreUsuario}'@'localhost' IDENTIFIED BY '{contraseña}'", connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Usuario creado exitosamente.");
        }
}

    static void BorrarUsuario()
    {
        Console.WriteLine("Ingrese el nombre del usuario a borrar:");
        string nombreUsuario = Console.ReadLine();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand($"DROP USER '{nombreUsuario}'@'localhost'", connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Usuario borrado exitosamente.");
        }
    }
 
    static void ActualizarUsuario()
    {
        Console.WriteLine("Ingrese el nombre del usuario a actualizar:");
        string nombreUsuario = Console.ReadLine();

        Console.WriteLine("Ingrese el nuevo nombre de usuario:");
        string nuevoNombreUsuario = Console.ReadLine();

        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"RENAME USER '{nombreUsuario}'@'localhost' TO '{nuevoNombreUsuario}'@'localhost'", connection);
                command.ExecuteNonQuery();
                Console.WriteLine("Usuario actualizado exitosamente.");
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Error al actualizar el usuario: " + ex.Message);
        }
}

    static void ConsultarUsuarios()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("SELECT User FROM mysql.user WHERE Host='localhost'", connection);
            MySqlDataReader reader = command.ExecuteReader();

            Console.WriteLine("Usuarios:");
            while (reader.Read())
            {
                Console.WriteLine(reader["User"]);
            }
        }
    }
    
    static void OtorgarPrivilegio()
    {
        try
        {
            Console.WriteLine("Ingrese el nombre del usuario:");
            string nombreUsuario = Console.ReadLine();

            Console.WriteLine("Ingrese el privilegio (por ejemplo, SUPER):");
            string privilegio = Console.ReadLine();

            string query = $"GRANT {privilegio} ON *.* TO '{nombreUsuario}'@'localhost';";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                command.ExecuteNonQuery();
                Console.WriteLine($"Privilegio {privilegio} otorgado a {nombreUsuario} exitosamente.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al otorgar el privilegio: " + ex.Message);
        }
    }
    
    static void RevocarPrivilegio()
    {
        try
        {
            Console.WriteLine("Ingrese el nombre del usuario:");
            string nombreUsuario = Console.ReadLine();

            Console.WriteLine("Ingrese el privilegio a revocar (por ejemplo, SUPER):");
            string privilegio = Console.ReadLine();

            string query = $"REVOKE {privilegio} ON *.* FROM '{nombreUsuario}'@'localhost';";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                command.ExecuteNonQuery();
                Console.WriteLine($"Privilegio {privilegio} revocado de {nombreUsuario} exitosamente.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al revocar el privilegio: " + ex.Message);
        }
    }
    
    static void ConsultarPrivilegiosUsuario()
    {
        try
        {
            Console.WriteLine("Ingrese el nombre del usuario:");
            string nombreUsuario = Console.ReadLine();

            string query = $"SHOW GRANTS FOR '{nombreUsuario}'@'localhost';";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                Console.WriteLine($"Privilegios del usuario '{nombreUsuario}':");
                while (reader.Read())
                {
                    string grant = reader.GetString(0);
                    string[] parts = grant.Split(new[] { " ON " }, StringSplitOptions.RemoveEmptyEntries);
                    Console.WriteLine(parts[0]);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al consultar los privilegios del usuario: " + ex.Message);
        }
    }
    
    static void ConsultarPrivilegios()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT " +
                    "Select_priv, Insert_priv, " +
                    "Update_priv, Delete_priv, " +
                    "Create_priv, Drop_priv, " +
                    "Reload_priv, Shutdown_priv, " +
                    "Process_priv, File_priv, " +
                    "Grant_priv, References_priv, " +
                    "Index_priv, Alter_priv, " +
                    "Show_db_priv, Super_priv, " +
                    "Create_tmp_table_priv, " +
                    "Lock_tables_priv, Execute_priv, " +
                    "Repl_slave_priv, Repl_client_priv " +
                    "FROM mysql.user", connection);
                MySqlDataReader reader = command.ExecuteReader();

                Console.WriteLine("Privilegios:");
                bool printed = false;
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader[i].ToString() == "Y")
                        {
                            Console.WriteLine(reader.GetName(i));
                            printed = true;
                        }
                    }
                    if (printed)
                    {
                        break; // Detiene el bucle después de imprimir los privilegios una vez
                    }
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Error al consultar los privilegios: " + ex.Message);
        }
    }
    
    static void RespaldarBaseDeDatos()
    {
        try
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string backupFileName = Path.Combine(desktopPath, "Gestion_respaldo.sql");

            string mysqldumpPath = @"C:\Program Files\MariaDB 11.3\bin\mysqldump.exe"; // Cambia esta ruta según tu instalación

            string server = "localhost";
            string database = "transporte_publico";
            string user = "root";
            string password = "1511";

            string backupCommand = $"\"{mysqldumpPath}\" -h {server} -u {user} -p{password} {database} > \"{backupFileName}\"";

            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", $"/c {backupCommand}");
            processInfo.RedirectStandardInput = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;

            using (Process process = Process.Start(processInfo))
            {
                process.WaitForExit();
                Console.WriteLine("Respaldo de base de datos completado exitosamente.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al respaldar la base de datos: " + ex.Message);
        }
    }
    
    static void RestaurarBaseDeDatos()
    {
        try
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string backupFileName = Path.Combine(desktopPath, "Gestion_respaldo.sql");

            string mysqlPath = @"C:\Program Files\MariaDB 11.3\bin\mysql.exe"; // Cambia esta ruta según tu instalación

            string server = "localhost";
            string database = "transporte_publico";
            string user = "root";
            string password = "1511";

            string restoreCommand = $"\"{mysqlPath}\" -h {server} -u {user} -p{password} {database} < \"{backupFileName}\"";

            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", $"/c {restoreCommand}");
            processInfo.RedirectStandardInput = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;

            using (Process process = Process.Start(processInfo))
            {
                process.WaitForExit();
                Console.WriteLine("Restauración de base de datos completada exitosamente.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al restaurar la base de datos: " + ex.Message);
        }
    }
    
    static void ConsultarTodasLasTablas()
        {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("SHOW TABLES;", connection);
                MySqlDataReader reader = command.ExecuteReader();

                Console.WriteLine("Tablas disponibles:");
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetString(0));
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Error al consultar las tablas: " + ex.Message);
        }
    }
    
    static void ConsultarAtributosPorEntidad()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                Console.WriteLine("Ingrese el nombre de la entidad (tabla) para consultar sus atributos:");
                string entidad = Console.ReadLine();

                // Consultar la estructura de la tabla especificada
                string query = $@"
                    SELECT column_name, data_type
                    FROM information_schema.columns
                    WHERE table_schema = 'transporte_publico'
                    AND table_name = '{entidad}'";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                Console.WriteLine($"Atributos de la entidad '{entidad}':");
                while (reader.Read())
                {
                    string columnName = reader["column_name"].ToString();
                    string dataType = reader["data_type"].ToString();
                    Console.WriteLine($"Atributo: {columnName}, Tipo de dato: {dataType}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al consultar los atributos de la entidad: " + ex.Message);
        }
    }
   
    static void AgregarNuevaTabla()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                Console.WriteLine("Ingrese el nombre de la nueva tabla:");
                string nombreTabla = Console.ReadLine();

                Console.WriteLine("Ingrese los atributos de la nueva tabla en el formato 'nombre1 tipo1, nombre2 tipo2, ...':");
                string atributos = Console.ReadLine();

                // Construir la consulta SQL para crear la nueva tabla
                string query = $@"
                    CREATE TABLE {nombreTabla} (
                        {atributos}
                    )";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Tabla '{nombreTabla}' creada exitosamente con los atributos: {atributos}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al crear la nueva tabla: " + ex.Message);
        }
    }

    public static void GenerarScriptCRUD()
    {
        using (MySqlConnection con = ObtenerConexion())
        {
            con.Open();
            string obtenerTablasQuery = "SHOW TABLES;";
            MySqlCommand obtenerTablasCmd = new MySqlCommand(obtenerTablasQuery, con);
            MySqlDataReader tablasReader = obtenerTablasCmd.ExecuteReader();

            List<string> tablas = new List<string>();

            while (tablasReader.Read())
            {
                tablas.Add(tablasReader[0].ToString());
            }

            tablasReader.Close();

            foreach (string tabla in tablas)
            {
                // Generar procedimientos almacenados CRUD para cada tabla
                string insertProcedure = GenerarInsertProcedure(con, tabla);
                EjecutarScript(con, insertProcedure);

                string updateProcedure = GenerarUpdateProcedure(con, tabla);
                EjecutarScript(con, updateProcedure);

                string deleteProcedure = GenerarDeleteProcedure(con, tabla);
                EjecutarScript(con, deleteProcedure);

                string selectProcedure = GenerarSelectProcedure(tabla);
                EjecutarScript(con, selectProcedure);
            }
        }
        Console.WriteLine("Se ha generado CRUD correctamente");
        //Console.ReadKey();
        MostrarMenu();
    }

    private static MySqlConnection ObtenerConexion()
    {
        // Configura tu cadena de conexión a la base de datos MariaDB
        string connectionString = "Server=localhost;Port=3306;Database=transporte_publico;Uid=root;Pwd=1511;";
        return new MySqlConnection(connectionString);
    }

    private static string GenerarInsertProcedure(MySqlConnection con, string tabla)
    {
        StringBuilder insertProcedure = new StringBuilder();
        insertProcedure.AppendLine($"DROP PROCEDURE IF EXISTS Insert_{tabla};");
        insertProcedure.AppendLine($"CREATE PROCEDURE Insert_{tabla}(");

        // Obtener los atributos de la tabla
        List<string> atributos = new List<string>();

        string describeQuery = $"DESCRIBE {tabla};";
        MySqlCommand describeCmd = new MySqlCommand(describeQuery, con);
        MySqlDataReader describeReader = describeCmd.ExecuteReader();

        while (describeReader.Read())
        {
            string columna = describeReader["Field"].ToString();
            string tipoDato = describeReader["Type"].ToString();
            if (!columna.Equals("ID", StringComparison.OrdinalIgnoreCase) && !tipoDato.ToLower().Contains("auto_increment"))
            {
                atributos.Add($"{columna} {tipoDato}");
            }
        }

        describeReader.Close();

        // Concatenar los parámetros
        insertProcedure.AppendLine(string.Join(", ", atributos.Select(a => $"IN p_{a.Split(' ')[0]} {a.Split(' ')[1]}")));
        insertProcedure.AppendLine(")");
        insertProcedure.AppendLine("BEGIN");
        insertProcedure.AppendLine($"INSERT INTO {tabla}");

        // Lista de columnas
        insertProcedure.AppendLine($"({string.Join(", ", atributos.Select(a => a.Split(' ')[0]))})");

        // Lista de valores
        insertProcedure.AppendLine($"VALUES ({string.Join(", ", atributos.Select(a => $"p_{a.Split(' ')[0]}"))});");
        insertProcedure.AppendLine("END;");

        return insertProcedure.ToString();
    }

    private static string GenerarUpdateProcedure(MySqlConnection con, string tabla)
    {
        StringBuilder updateProcedure = new StringBuilder();
        updateProcedure.AppendLine($"DROP PROCEDURE IF EXISTS Update_{tabla};");
        updateProcedure.AppendLine($"CREATE PROCEDURE Update_{tabla}(");

        // Obtener los atributos de la tabla
        List<string> atributos = new List<string>();
        string clavePrimaria = string.Empty;

        string describeQuery = $"DESCRIBE {tabla};";
        MySqlCommand describeCmd = new MySqlCommand(describeQuery, con);
        MySqlDataReader describeReader = describeCmd.ExecuteReader();

        while (describeReader.Read())
        {
            string columna = describeReader["Field"].ToString();
            string tipoDato = describeReader["Type"].ToString();
            string key = describeReader["Key"].ToString();

            if (key == "PRI")
            {
                clavePrimaria = columna;
            }
            else
            {
                atributos.Add($"{columna} {tipoDato}");
            }
        }

        describeReader.Close();

        // Concatenar los parámetros
        updateProcedure.AppendLine(string.Join(", ", atributos.Select(a => $"IN p_{a.Split(' ')[0]} {a.Split(' ')[1]}")));
        updateProcedure.AppendLine($", IN p_{clavePrimaria} INT");
        updateProcedure.AppendLine(")");
        updateProcedure.AppendLine("BEGIN");
        updateProcedure.AppendLine($"UPDATE {tabla} SET");

        // Lista de asignaciones de columnas
        updateProcedure.AppendLine(string.Join(", ", atributos.Select(a => $"{a.Split(' ')[0]} = p_{a.Split(' ')[0]}")));

        // Condición para la clave primaria
        updateProcedure.AppendLine($"WHERE {clavePrimaria} = p_{clavePrimaria};");
        updateProcedure.AppendLine("END;");

        return updateProcedure.ToString();
    }

    private static string GenerarDeleteProcedure(MySqlConnection con, string tabla)
    {
        StringBuilder deleteProcedure = new StringBuilder();
        deleteProcedure.AppendLine($"DROP PROCEDURE IF EXISTS Delete_{tabla};");
        deleteProcedure.AppendLine($"CREATE PROCEDURE Delete_{tabla}(");

        string clavePrimaria = string.Empty;
        string describeQuery = $"DESCRIBE {tabla};";
        MySqlCommand describeCmd = new MySqlCommand(describeQuery, con);
        MySqlDataReader describeReader = describeCmd.ExecuteReader();

        while (describeReader.Read())
        {
            string columna = describeReader["Field"].ToString();
            string key = describeReader["Key"].ToString();

            if (key == "PRI")
            {
                clavePrimaria = columna;
                break;
            }
        }

        describeReader.Close();

        if (!string.IsNullOrEmpty(clavePrimaria))
        {
            deleteProcedure.AppendLine($"IN p_{clavePrimaria} INT");
            deleteProcedure.AppendLine(")");
            deleteProcedure.AppendLine($"BEGIN");
            deleteProcedure.AppendLine($"DELETE FROM {tabla} WHERE {clavePrimaria} = p_{clavePrimaria};");
            deleteProcedure.AppendLine($"END;");

        }
        else
        {
            deleteProcedure.AppendLine(")");
            deleteProcedure.AppendLine("BEGIN");
            deleteProcedure.AppendLine("-- Error: No se encontró una clave primaria en la tabla.");
            deleteProcedure.AppendLine("END;");
        }

        return deleteProcedure.ToString();
    }

    private static string GenerarSelectProcedure(string tabla)
    {
        StringBuilder selectProcedure = new StringBuilder();
        selectProcedure.AppendLine($"DROP PROCEDURE IF EXISTS Select_{tabla};");
        selectProcedure.AppendLine($"CREATE PROCEDURE Select_{tabla}()");
        selectProcedure.AppendLine("BEGIN");
        selectProcedure.AppendLine($"SELECT * FROM {tabla};");
        selectProcedure.AppendLine("END;");

        return selectProcedure.ToString();
    }

    private static void GenerarReportePDF()
    {
        List<string> entidades = ObtenerEntidades();
        if (entidades.Count == 0)
        {
            Console.WriteLine("No se encontraron entidades disponibles.");
            return;
        }

        Console.WriteLine("Entidades disponibles:");
        for (int i = 0; i < entidades.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {entidades[i]}");
        }

        Console.Write("Selecciona el número de la entidad que deseas incluir en el reporte: ");
        if (!int.TryParse(Console.ReadLine(), out int seleccion) || seleccion < 1 || seleccion > entidades.Count)
        {
            Console.WriteLine("Selección no válida.");
            return;
        }

        string entidadSeleccionada = entidades[seleccion - 1];
        List<Atributo> atributos = ObtenerAtributos(entidadSeleccionada);
        if (atributos.Count == 0)
        {
            Console.WriteLine($"No se encontraron atributos para la entidad '{entidadSeleccionada}'.");
            return;
        }

        Console.WriteLine("Atributos disponibles:");
        for (int i = 0; i < atributos.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {atributos[i].Nombre}");
        }

        Console.WriteLine("Selecciona los números de los atributos que deseas incluir en el reporte, separados por comas:");
        string seleccionAtributos = Console.ReadLine();
        List<int> indicesAtributos = seleccionAtributos.Split(',').Select(s => int.Parse(s.Trim()) - 1).ToList();

        if (indicesAtributos.Any(i => i < 0 || i >= atributos.Count))
        {
            Console.WriteLine("Una o más selecciones no son válidas.");
            return;
        }

        List<Atributo> atributosSeleccionados = indicesAtributos.Select(i => atributos[i]).ToList();
        List<Dictionary<string, string>> datosTabla = ObtenerDatosDeEntidad(entidadSeleccionada, atributosSeleccionados);
        if (datosTabla.Count == 0)
        {
            Console.WriteLine($"No se encontraron datos para la entidad '{entidadSeleccionada}'.");
            return;
        }

        // Generar el PDF
        string rutaDirectorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string rutaArchivoPDF = $@"{rutaDirectorio}\Reporte_{entidadSeleccionada}.pdf";
        using (FileStream fs = new FileStream(rutaArchivoPDF, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            // Agregar título
            doc.Add(new Paragraph($"Reporte de la entidad: {entidadSeleccionada}", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD)));
            doc.Add(new Paragraph("\n"));

            // Agregar tabla con los datos de la entidad seleccionada
            PdfPTable table = new PdfPTable(atributosSeleccionados.Count);
            table.WidthPercentage = 100;

            // Encabezados de la tabla (nombres de las columnas)
            foreach (var atributo in atributosSeleccionados)
            {
                table.AddCell(new PdfPCell(new Phrase(atributo.Nombre, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            }

            // Agregar los datos de la tabla
            foreach (var fila in datosTabla)
            {
                foreach (var atributo in atributosSeleccionados)
                {
                    table.AddCell(new PdfPCell(new Phrase(fila[atributo.Nombre])));
                }
            }

            doc.Add(table);
            doc.Close();
            writer.Close();
        }

        Console.WriteLine($"Reporte generado con éxito: {rutaArchivoPDF}");
        Console.ReadKey();
    }

    private static List<string> ObtenerEntidades()
    {
        List<string> entidades = new List<string>();

        using (MySqlConnection con = ObtenerConexionDB())
        {
            con.Open();
            string query = "SHOW TABLES;";

            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                entidades.Add(reader[0].ToString());
            }

            reader.Close();
        }

        return entidades;
    }

    private static List<Atributo> ObtenerAtributos(string entidad)
    {
        List<Atributo> atributos = new List<Atributo>();

        using (MySqlConnection con = ObtenerConexionDB())
        {
            con.Open();
            string query = $"DESCRIBE {entidad};";

            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                atributos.Add(new Atributo
                {
                    Nombre = reader["Field"].ToString(),
                    Tipo = reader["Type"].ToString()
                });
            }

            reader.Close();
        }

        return atributos;
    }

    private static List<Dictionary<string, string>> ObtenerDatosDeEntidad(string entidad, List<Atributo> atributosSeleccionados)
    {
        List<Dictionary<string, string>> datos = new List<Dictionary<string, string>>();

        using (MySqlConnection con = ObtenerConexionDB())
        {
            con.Open();
            string columnas = string.Join(", ", atributosSeleccionados.Select(a => a.Nombre));
            string query = $"SELECT {columnas} FROM {entidad};";

            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Dictionary<string, string> fila = new Dictionary<string, string>();
                foreach (var atributo in atributosSeleccionados)
                {
                    fila[atributo.Nombre] = reader[atributo.Nombre].ToString();
                }
                datos.Add(fila);
            }

            reader.Close();
        }

        return datos;
    }

    private static MySqlConnection ObtenerConexionDB()
    {
        return new MySqlConnection(connectionString);
    }

    private class Atributo
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
    }

















    private static void EjecutarScript(MySqlConnection con, string script)
    {
        try
        {
            string tempFileName = Path.GetTempFileName();
            File.WriteAllText(tempFileName, script);

            string mysqlPath = @"C:\Program Files\MariaDB 11.3\bin\mysql.exe"; // Cambia esta ruta según tu instalación
            string server = "localhost";
            string database = "transporte_publico";
            string user = "root";
            string password = "1511";

            string restoreCommand = $"\"{mysqlPath}\" -h {server} -u {user} -p{password} {database} < \"{tempFileName}\"";

            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", $"/c {restoreCommand}");
            processInfo.RedirectStandardInput = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;

            using (Process process = Process.Start(processInfo))
            {
                process.WaitForExit();
                Console.WriteLine("Script SQL ejecutado correctamente.");
            }

            // Borramos el archivo temporal después de usarlo
            File.Delete(tempFileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al ejecutar el script SQL: {ex.Message}");
        }
    }



}

