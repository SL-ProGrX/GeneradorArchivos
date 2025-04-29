using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreadorArchivos
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            bool showMenu = true;
            while (showMenu)
            {
                showMenu = await MainMenu();
            }
        }

        private static async Task<bool> MainMenu()
        {
            Console.Clear();
            Console.WriteLine("**Se puede regresar al menu principal al escribir {salir}+{Enter}**");
            Console.WriteLine("Elige una opción:");
            Console.WriteLine("1) Generar Componente");
            Console.WriteLine("2) Generar Modelo base");
            Console.WriteLine("3) Salir");
            Console.WriteLine("4) Limpiar Raiz");
            Console.Write("\r\nSelecciona una opción: ");
            

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Clear();
                    await GenerateComponent();
                    return true;
                case "2":
                    Console.Clear();
                    await  GenerateModel();
                    return true;
                case "3":
                    Console.Clear();
                    Environment.Exit(0);
                    return false;
                case "4":
                    //limpio archivos menos .exe
                    string[] files = Directory.GetFiles(AppContext.BaseDirectory);
                    foreach (string file in files)
                    {
                        if (file.Contains(".exe"))
                        {
                            continue;
                        }
                        File.Delete(file);
                    }
                    //borro subcarpetas
                    string[] dirs = Directory.GetDirectories(AppContext.BaseDirectory);
                    foreach (string dir in dirs)
                    {
                        Directory.Delete(dir, true);
                    }
                    return false;
                default:
                    break;
            }
            return true;
        }

        private static async Task GenerateComponent()
        {

            // Solicitar al usuario el nombre del modulo
            Console.Write("Nota: Puede dejarlo nulo para crearlo en la raiz...\n");
            Console.Write("Ingrese el nombre modulo V6 sin 'proGrx_' (ej: Beneficios): ");
            string moduloName = Console.ReadLine();

            if(moduloName == "salir")
            {
                Console.ReadKey();
                return;
            }

            if(moduloName == "")
            {
                moduloName = ""; 
            }
            else
            {
                moduloName = "proGrx_" + moduloName; // "proGrx_Beneficios
            }

           

            // Solicitar al usuario el nombre base
            Console.Write("Ingrese el nombre componente (ej: frmAF_Beneficios ) : ");
            string baseName = Console.ReadLine();

            if (baseName == "salir")
            {
                Console.ReadKey();
                return;
            }

            string orden = "";
            Console.Write("Ingrese el nombre carpeta namespace (ej: AF, PRE, etc ) : ");
            orden = Console.ReadLine();

            if (orden == "salir")
            {
                Console.ReadKey();
                return;
            }

            if(orden != "")
            {
                orden = "." + orden;
            }

            string directoryPath = AppContext.BaseDirectory;

            // Validar que la ruta exista
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("La ruta especificada no existe. Creando la ruta...");
                Directory.CreateDirectory(directoryPath);
            }

            // Definir los sufijos de los archivos
            string[] suffixes = { "DB", "BL", "Models", "Controller" };

            // Crear archivos con los nombres especificados y contenido base
            foreach (var suffix in suffixes)
            {
                string fileName = $"{baseName}{suffix}.cs";
                CreateFile(fileName, baseName, moduloName, suffix, orden);
            }

            // Crear carpeta y archivos adicionales para Angular dentro de la ruta especificada
            string angularFolderPath = Path.Combine(directoryPath, baseName);
            Directory.CreateDirectory(angularFolderPath);

            string componentTsFilePath = Path.Combine(angularFolderPath, $"{baseName}.component.ts");
            CreateFile(componentTsFilePath, baseName, moduloName, "ComponentTs", orden);

            string componentHtmlFilePath = Path.Combine(angularFolderPath, $"{baseName}.component.html");
            CreateFile(componentHtmlFilePath, baseName, moduloName, "ComponentHtml", orden);

            CreateFile($"{baseName}.service.ts", baseName, moduloName, "Service", orden);

            CreateFile($"{baseName}-models.ts", baseName, moduloName, "Models.ts", orden);

            // Crear archivo de instrucciones adicionales
            string instructionsFilePath = Path.Combine(directoryPath, "Instrucciones.txt");
            CreateFile(instructionsFilePath, baseName, moduloName, "Instructions", orden);

            Console.WriteLine("Archivos creados exitosamente.");

            Console.WriteLine("\r\nPulsa cualquier tecla para volver al menú principal...");
            Console.ReadKey();
        }

        private static async Task GenerateModel()
        {
            string exit = "";

            Console.Write("Seleccione un Nombre para el modelo (ej: AfbeneficiosDatos): ");
            string nombreClase = Console.ReadLine();

            if (nombreClase == "salir")
            {
                Console.ReadKey();
                return;
            }

            Console.Clear();

            // Solicitar al usuario el nombre del modulo
            Console.Write("Pega la tabla SQL (Ctr +  C , Ctr + V del resultado): ");
            string Tabla = Console.ReadLine();

            if (Tabla == "salir")
            {
                Console.ReadKey();
                return;
            }

            Console.Clear();

            string [] colName = Tabla.Split('\t');

            string modelo = "\n\npublic class " + nombreClase + "\n{\n";
            for (int i = 0; i < colName.Length ; i++)
            {
                if (colName[i].Contains("FECHA"))
                {
                    modelo += "    public DateTime " + colName[i].ToLowerInvariant() + " { get; set; }\n";
                }
                else
                {
                    modelo += "    public string " + colName[i].ToLowerInvariant() + " { get; set; }\n";
                }
            }

            string modeloAngular = "\n\nexport class " + nombreClase + " {\n";

            for (int i = 0; i < colName.Length; i++)
            {
                if (colName[i].Contains("FECHA"))
                {
                    modeloAngular += "    " + colName[i].ToLowerInvariant() + ": Date;\n";
                }
                else
                {
                    modeloAngular += "    " + colName[i].ToLowerInvariant() + ": string;\n";
                }
            }

            ClearConsoleInputBuffer();
            

            Console.Write("Recuerda cambiar el tipo de dato si es necesario: ");
            Console.Write("\nModelo C#: ");
            Console.WriteLine(modelo + "\n}");
            Console.WriteLine("\n");
            Console.Write("Modelo Angular: ");
            Console.WriteLine(modeloAngular + "\n}");

            Console.WriteLine("\r\nEscriba {salir} para regresar al menu...");
            // Limpiar el buffer de entrada
            

            exit = Console.ReadLine();

            if (exit == "salir")
            {
                Console.ReadKey();
                return;
            }
        }

        static void CreateFile(string fileName, string baseName, string moduleName, string suffix, string orden)
        {
            try
            {
                // Crear el archivo con el contenido base
                using (StreamWriter sw = File.CreateText(fileName))
                {
                    string content = GetFileContent(baseName, moduleName,suffix, orden.ToUpper());
                    sw.Write(content);
                }
                Console.WriteLine($"Archivo {fileName} creado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el archivo {fileName}: {ex.Message}");
            }
        }

        static string GetFileContent(string baseName, string moduleName ,string suffix, string orden)
        {

            string[] parts = baseName.Split(new char[] { '_' }, 2); // El 2 indica que se divide en dos partes
            string firstPart, remainingText = "";
            string baseFormat = "";
            if (parts.Length > 0)
            {
                firstPart = parts[0]; // "frmAF"
                remainingText = parts[1]; // "BenePruebaBL"
            }

            if(remainingText != "")
            {
                baseFormat = remainingText;
            }
            else
            {
                baseFormat = baseName.Replace("frm", "");
            }
            

            switch (suffix)
            {
                case "Models":
                    return $"namespace PgxAPI.Models{orden}\n{{\n}}\n";
                case "BL":

                    if(orden == "")
                    {
                        return $@"using PgxAPI.DataBaseTier;
using PgxAPI.Models;

namespace PgxAPI.BusinessLogic
{{
    public class {baseName}BL
    {{
        private readonly IConfiguration? _config;
        {baseName}DB {baseFormat}Db;

        public {baseName}BL(IConfiguration config)
        {{
            _config = config;
            {baseFormat}Db = new {baseName}DB(_config);
        }}
    }}
}}";
                    }
                    else
                    {
                        return $@"using PgxAPI.DataBaseTier;
using PgxAPI.Models;
using PgxAPI.Models{orden};

namespace PgxAPI.BusinessLogic
{{
    public class {baseName}BL
    {{
        private readonly IConfiguration? _config;
        {baseName}DB {baseFormat}Db;

        public {baseName}BL(IConfiguration config)
        {{
            _config = config;
            {baseFormat}Db = new {baseName}DB(_config);
        }}
    }}
}}";
                    }                 
                case "DB":

                    if(orden == "")
                    {
                        return $@"using PgxAPI.Models;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace PgxAPI.DataBaseTier
{{
    public class {baseName}DB
    {{
        private readonly IConfiguration? _config;

        public {baseName}DB(IConfiguration config)
        {{
            _config = config;
        }}
    }}
}}";
                    }
                    else
                    {
                        return $@"using PgxAPI.Models;
using PgxAPI.Models{orden};
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace PgxAPI.DataBaseTier
{{
    public class {baseName}DB
    {{
        private readonly IConfiguration? _config;

        public {baseName}DB(IConfiguration config)
        {{
            _config = config;
        }}
    }}
}}";
                    }               
                case "Controller":

                    if (orden == "")
                    {
                        return $@"using Microsoft.AspNetCore.Mvc;
using PgxAPI.BusinessLogic;
using PgxAPI.Models;

namespace PgxAPI.Controllers
{{
    [Route(""api/[controller]"")]
    [ApiController]
    public class {baseName}Controller : Controller
    {{
        private readonly IConfiguration? _config;
         {baseName}BL {baseFormat}BL;
        public {baseName}Controller(IConfiguration config)
        {{
            _config = config;
            {baseFormat}BL = new {baseName}BL(_config);
        }}
    }}
}}";
                    }
                    else
                    {
                        return $@"using Microsoft.AspNetCore.Mvc;
using PgxAPI.BusinessLogic;
using PgxAPI.Models;
using PgxAPI.Models{orden};

namespace PgxAPI.Controllers
{{
    [Route(""api/[controller]"")]
    [ApiController]
    public class {baseName}Controller : Controller
    {{
        private readonly IConfiguration? _config;
         {baseName}BL {baseFormat}BL;
        public {baseName}Controller(IConfiguration config)
        {{
            _config = config;
            {baseFormat}BL = new {baseName}BL(_config);
        }}
    }}
}}";
                    }                
                case "ComponentTs":
                    if(moduleName != "")
                    {
                        return $@"
import {{ Component, OnInit, OnDestroy }} from '@angular/core';
import {{ Router }} from '@angular/router';
import {{ ConfirmationService, MessageService }} from 'primeng/api';
import {{ Subject }} from 'rxjs';
import {{ UsuarioLogeadoDTO }} from '../../../models/logon.model';
import {{ {baseName}Service }} from '../../../services/{moduleName}/{baseName}.service';

@Component({{
    selector: 'app-{baseName}',
    templateUrl: './{baseName}.component.html',
    providers: [ConfirmationService, MessageService],
}})

export class {baseName} implements OnInit, OnDestroy {{

    public _unsubscribeAll!: Subject<any>;
    public UsuarioSesion: UsuarioLogeadoDTO = new UsuarioLogeadoDTO();

    constructor(private router: Router,
        private _{baseName}Srv: {baseName}Service,
        private messageService: MessageService) {{
        this._unsubscribeAll = new Subject();
    }}

    ngOnDestroy(): void {{
        this._unsubscribeAll.unsubscribe();
    }}

    ngOnInit(): void {{
        if (sessionStorage.getItem('UsuarioInfo') != null) {{
            this.UsuarioSesion = JSON.parse(sessionStorage.getItem('UsuarioInfo')!);
        }}
        else {{
            this.router.navigate(['auth/login']);
        }}
    }}

}}";
                    }
                    else
                    {
                        return $@"
import {{ Component, OnInit, OnDestroy }} from '@angular/core';
import {{ Router }} from '@angular/router';
import {{ ConfirmationService, MessageService }} from 'primeng/api';
import {{ Subject }} from 'rxjs';
import {{ UtilitiesService }} from 'src/app/backend/services/utilities.service';
import {{ UsuarioLogeadoDTO }} from '../../models/logon.model';
import {{ {baseName}Service }} from '../../services/{baseName}.service';

@Component({{
    selector: 'app-{baseName}',
    templateUrl: './{baseName}.component.html',
    providers: [ConfirmationService, MessageService],
}})

export class {baseName} implements OnInit, OnDestroy {{

    public _unsubscribeAll!: Subject<any>;
    public UsuarioSesion: UsuarioLogeadoDTO = new UsuarioLogeadoDTO();

    constructor(private router: Router,
        private _{baseName}Srv: {baseName}Service,
        private messageService: MessageService) {{
        this._unsubscribeAll = new Subject();
    }}

    ngOnDestroy(): void {{
        this._unsubscribeAll.unsubscribe();
    }}

    ngOnInit(): void {{
        if (sessionStorage.getItem('UsuarioInfo') != null) {{
            this.UsuarioSesion = JSON.parse(sessionStorage.getItem('UsuarioInfo')!);
        }}
        else {{
            this.router.navigate(['auth/login']);
        }}
    }}

}}";
                    }   
                case "ComponentHtml":
                    return $@"<div class=""grid"">

  <p-toast></p-toast>

  <div class=""col-12"">

      <h5><span class=""fa-solid fa-check-to-slot""></span> frmFSL_Consulta</h5>
     
  </div>

</div>";
                case "Service":
                    return $@"import {{ Injectable }} from '@angular/core';
import {{ throwError }} from 'rxjs';
import {{ environment }} from 'src/environments/environment';
import {{ HttpClient, HttpHeaders }} from '@angular/common/http';
import Swal from 'sweetalert2';
//import {{  }} from '../models/{baseName}.models';

@Injectable({{providedIn: 'root'
}})

export class {baseName}Service {{

    private apiUrl = environment.apiUrl + '{baseName}/';
    constructor(private http: HttpClient) {{ }}

    private obtenerHeaders(jwtToken: string): HttpHeaders {{
        return new HttpHeaders({{
            Authorization: 'Bearer ' + jwtToken,
            'Content-Type': 'application/json'
        }});
    }}

    handleError(error: any) {{
        let errorMessage = '';
        console.log(error);
        if (error.error instanceof ErrorEvent) {{
            // Get client-side error
            errorMessage = error.error.message;
        }} else {{
            // Get server-side error
            errorMessage = `Error Code: ${{error.status}}\nMessage: ${{error.message}}`;
        }}
        //window.alert(errorMessage);
        Swal.fire({{
            title: ""Error de Servidor"",
            html: 'Por favor envíe el siguiente mensaje al administrador del sistema: ' + '<b>' + errorMessage + '</b>',
            icon: ""error"",
            width: 600,
            padding: ""3em"",
            confirmButtonColor: ""#28a745"",
            confirmButtonText: ""Aceptar"",
        }});
        return throwError(errorMessage);
    }}

    // AQUI VAN LOS METODOS DEL SERVICIO

}}
";
                case "Models.ts":
                    return $@"export class {baseName} {{}}";
                case "Instructions":

                    if(moduleName == "proGrx_")
                    {
                        return $@"EN INICIO.COMPONENT.TS : 
import {{ {baseName} }} from '../{baseName}/{baseName}.component';

this.componentRegistry.registerComponent('{baseName}', {baseName});

EN INICIO MODULE
import {{ {baseName} }} from '../{baseName}/{baseName}.component';
";
                    }
                    else
                    {
                        return $@"EN INICIO.COMPONENT.TS : 
import {{ {baseName} }} from '../{moduleName}/{baseName}/{baseName}.component';

this.componentRegistry.registerComponent('{baseName}', {baseName});

EN INICIO MODULE
import {{ {baseName} }} from '../{moduleName}/{baseName}/{baseName}.component';
";
                    }
      
                default:
                    return $"// Este es un archivo de ejemplo\n// Nombre del archivo: \n";
            }
        }

        static void ClearConsoleInputBuffer()
        {
            while (Console.KeyAvailable)
            {
                Console.ReadLine();
            }
            Console.Clear();
        }
    }
}
