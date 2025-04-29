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

            CreateFile($"{baseName}.models.ts", baseName, moduloName, "Models.ts", orden);

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
using PgxAPI.Models.ERROR;

namespace PgxAPI.BusinessLogic
{{
    public class {baseName}BL
    {{
        private readonly IConfiguration? _config;
        {baseName}DB {baseFormat}DB;

        public {baseName}BL(IConfiguration config)
        {{
            _config = config;
            {baseFormat}DB = new {baseName}DB(_config);
        }}
    }}
}}";
                    }
                    else
                    {
                        return $@"using PgxAPI.DataBaseTier;
using PgxAPI.Models.ERROR;
using PgxAPI.Models{orden};

namespace PgxAPI.BusinessLogic
{{
    public class {baseName}BL
    {{
        private readonly IConfiguration? _config;
        {baseName}DB {baseFormat}DB;

        public {baseName}BL(IConfiguration config)
        {{
            _config = config;
            {baseFormat}DB = new {baseName}DB(_config);
        }}
    }}
}}";
                    }                 
                case "DB":

                    if(orden == "")
                    {
                        return $@"using PgxAPI.Models.ERROR;
using Microsoft.Data.SqlClient;
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
                        return $@"using PgxAPI.Models.ERROR;
using PgxAPI.Models{orden};
using Microsoft.Data.SqlClient;
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
using PgxAPI.Models.ERROR;

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
using PgxAPI.Models.ERROR;
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
    import {{ MenuService }} from 'src/app/backend/services/menu.service';

    @Component({{
        selector: 'app-{baseName}',
        templateUrl: './{baseName}.component.html',
        providers: [ConfirmationService, MessageService],
    }})

    export class {baseName} implements OnInit, OnDestroy {{

        public _unsubscribeAll!: Subject<any>;
        public UsuarioSesion: UsuarioLogeadoDTO = new UsuarioLogeadoDTO();
        manualID: string = null;

        constructor(private router: Router,
            private _{baseName}Srv: {baseName}Service,
            private messageService: MessageService,
            private _Server: MenuService) {{
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

        abrirManual(): void {{
            if(this.manualID == null){{
                this.manualID = this._Server._keyFormulario.value;
            }}
            window.open(this.manualID, '_blank');
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
    import {{ MenuService }} from 'src/app/backend/services/menu.service';

    @Component({{
        selector: 'app-{baseName}',
        templateUrl: './{baseName}.component.html',
        providers: [ConfirmationService, MessageService],
    }})

    export class {baseName} implements OnInit, OnDestroy {{

        public _unsubscribeAll!: Subject<any>;
        public UsuarioSesion: UsuarioLogeadoDTO = new UsuarioLogeadoDTO();
        manualID: string = null;

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

        abrirManual(): void {{
            if(this.manualID == null){{
                this.manualID = this._Server._keyFormulario.value;
            }}
            window.open(this.manualID, '_blank');
        }}

    }}";
                    }   
                case "ComponentHtml":
                    return $@"<div class=""grid"">

<p-toast></p-toast>

<div class=""col-12"">

    <h5><span class=""fa-solid fa-check-to-slot""></span> Título de pantalla</h5>

    <div class=""formgroup-inline flex justify-content-between flex-column sm:flex-row "">

    <div class=""flex gap-1 align-items-center justify-content-center"">

        <button pTooltip=""Ayuda"" (click)=""abrirManual()"" pButton pRipple type=""button"" icon=""pi pi-question-circle"" 
        class=""p-button-raised p-button-rounded p-button-secondary p-button-outlined mr-2 mt-3""></button>

    </div>

    <div class=""flex flex-column gap-2 align-items-center justify-content-center sm:flex-row"">
        <div class=""flex gap-1 align-items-center justify-content-center"">
        </div>
    </div>

    </div>
     
</div>

</div>";
                case "Service":
                    return $@"import {{ Injectable }} from '@angular/core';
import {{ BehaviorSubject, catchError, Observable }} from 'rxjs';
import {{ environment }} from 'src/environments/environment';
import {{ HttpClient }} from '@angular/common/http';
import {{ ErrorHandlerService }} from '../../ErrorHandlerService.service';
import {{ HeadersService }} from '../../HeadersService.service';
import {{ Error_DTO,ErrorDTO }} from 'src/app/backend/models/errores.models';
//import {{  }} from '../models/{baseName}.models';

@Injectable({{providedIn: 'root'
}})

export class {baseName}Service {{

    private apiUrl = environment.apiUrl + '{baseName}/';
                                
    constructor(
        private http: HttpClient,
        private errorHandler: ErrorHandlerService,
        private headersService: HeadersService,
    ) {{ }}

                               

                                

    // AQUI VAN LOS METODOS DEL SERVICIO, Utilizar comentarios como el siguiente en cada método:

        /**
        * Obtiene una factura por su ID
        * @param CodEmpresa Código de la empresa seleccionada
        * @param id         ID de la factura a consultar
        * @param jwtToken   Token de autorización para la API
        * @returns  Detalle de factura 
        */

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
