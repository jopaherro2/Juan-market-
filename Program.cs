using System;
using System.Collections.Generic;
using System.Text;

namespace DonJuanMarketCore
{
    // =========================================================================
    // MÓDULO 1: GESTIÓN DE INVENTARIO CON ÁRBOL AVL
    // =========================================================================

    /**
     * Representa un producto individual dentro del inventario del supermercado Don Juan Market.
     */
    public class Producto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public double Precio { get; set; }
        public int Stock { get; set; }

        public Producto(string codigo, string nombre, string categoria, double precio, int stock)
        {
            Codigo = codigo;
            Nombre = nombre;
            Categoria = categoria;
            Precio = precio;
            Stock = stock;
        }

        public override string ToString()
        {
            return $"{Codigo,-10} {Nombre,-22} {Categoria,-12} Q{Precio,8:N2} {Stock,6}";
        }
    }

    /**
     * Representa un nodo individual dentro de la estructura del Árbol AVL.
     * Almacena una instancia de Producto y referencias a sus hijos y altura.
     */
    public class NodoAVL
    {
        public Producto Payload;
        public NodoAVL HijoIzquierdo;
        public NodoAVL HijoDerecho;
        public int Altura;

        public NodoAVL(Producto producto)
        {
            Payload = producto;
            HijoIzquierdo = null;
            HijoDerecho = null;
            Altura = 1;
        }
    }

    /**
     * Implementación completa de un Árbol AVL auto-balanceado para la administración 
     * eficiente del inventario de productos.
     */
    public class ArbolAVL
    {
        private NodoAVL raiz;

        public bool EstaVacio() => raiz == null;

        private int ObtenerAltura(NodoAVL nodo) => nodo == null ? 0 : nodo.Altura;

        private int CalcularFactorBalance(NodoAVL nodo)
        {
            return nodo == null ? 0 : ObtenerAltura(nodo.HijoIzquierdo) - ObtenerAltura(nodo.HijoDerecho);
        }

        /**
         * Realiza una rotación simple a la derecha para rebalancear el subárbol.
         * @param y Nodo desbalanceado que pasará a ser hijo derecho.
         * @return Nueva raíz balanceada del subárbol.
         */
        private NodoAVL RotarDerecha(NodoAVL y)
        {
            NodoAVL x = y.HijoIzquierdo;
            NodoAVL T2 = x.HijoDerecho;

            x.HijoDerecho = y;
            y.HijoIzquierdo = T2;

            y.Altura = Math.Max(ObtenerAltura(y.HijoIzquierdo), ObtenerAltura(y.HijoDerecho)) + 1;
            x.Altura = Math.Max(ObtenerAltura(x.HijoIzquierdo), ObtenerAltura(x.HijoDerecho)) + 1;

            return x;
        }

        /**
         * Realiza una rotación simple a la izquierda para rebalancear el subárbol.
         * @param x Nodo desbalanceado que pasará a ser hijo izquierdo.
         * @return Nueva raíz balanceada del subárbol.
         */
        private NodoAVL RotarIzquierda(NodoAVL x)
        {
            NodoAVL y = x.HijoDerecho;
            NodoAVL T2 = y.HijoIzquierdo;

            y.HijoIzquierdo = x;
            x.HijoDerecho = T2;

            x.Altura = Math.Max(ObtenerAltura(x.HijoIzquierdo), ObtenerAltura(x.HijoDerecho)) + 1;
            y.Altura = Math.Max(ObtenerAltura(y.HijoIzquierdo), ObtenerAltura(y.HijoDerecho)) + 1;

            return y;
        }

        /**
         * Agrega un nuevo producto al inventario utilizando el código como clave de ordenación.
         * Si es necesario, aplica las rotaciones correspondientes para mantener el balance del Árbol AVL.
         * @param producto Instancia del producto a insertar.
         * @param mensaje Mensaje informativo sobre el resultado de la operación.
         * @return true si la inserción se realizó exitosamente; false si el código ya se encuentra registrado.
         */
        public bool Insertar(Producto producto, out string mensaje)
        {
            bool insertado = false;
            raiz = InsertarRecursivo(raiz, producto, ref insertado);
            if (insertado)
            {
                mensaje = $"Producto {producto.Codigo} insertado correctamente.";
            }
            else
            {
                mensaje = $"Error: Ya existe un producto con el código {producto.Codigo}.";
            }
            return insertado;
        }

        private NodoAVL InsertarRecursivo(NodoAVL nodo, Producto producto, ref bool insertado)
        {
            if (nodo == null)
            {
                insertado = true;
                return new NodoAVL(producto);
            }

            int comparacion = string.Compare(producto.Codigo, nodo.Payload.Codigo, StringComparison.Ordinal);

            if (comparacion < 0)
                nodo.HijoIzquierdo = InsertarRecursivo(nodo.HijoIzquierdo, producto, ref insertado);
            else if (comparacion > 0)
                nodo.HijoDerecho = InsertarRecursivo(nodo.HijoDerecho, producto, ref insertado);
            else
            {
                insertado = false;
                return nodo;
            }

            nodo.Altura = 1 + Math.Max(ObtenerAltura(nodo.HijoIzquierdo), ObtenerAltura(nodo.HijoDerecho));
            int balance = CalcularFactorBalance(nodo);

            if (balance > 1 && string.Compare(producto.Codigo, nodo.HijoIzquierdo.Payload.Codigo, StringComparison.Ordinal) < 0)
                return RotarDerecha(nodo);

            if (balance < -1 && string.Compare(producto.Codigo, nodo.HijoDerecho.Payload.Codigo, StringComparison.Ordinal) > 0)
                return RotarIzquierda(nodo);

            if (balance > 1 && string.Compare(producto.Codigo, nodo.HijoIzquierdo.Payload.Codigo, StringComparison.Ordinal) > 0)
            {
                nodo.HijoIzquierdo = RotarIzquierda(nodo.HijoIzquierdo);
                return RotarDerecha(nodo);
            }

            if (balance < -1 && string.Compare(producto.Codigo, nodo.HijoDerecho.Payload.Codigo, StringComparison.Ordinal) < 0)
            {
                nodo.HijoDerecho = RotarDerecha(nodo.HijoDerecho);
                return RotarIzquierda(nodo);
            }

            return nodo;
        }

        public void ActualizarDirecto(string codigo, string nombre, string categoria, double precio, int stock)
        {
            var item = Buscar(codigo);
            if (item != null)
            {
                item.Nombre = nombre;
                item.Categoria = categoria;
                item.Precio = precio;
                item.Stock = stock;
            }
        }

        /**
         * Localiza un producto dentro del inventario utilizando su código como criterio de búsqueda.
         * @param codigo Código único del producto a buscar.
         * @return Instancia del producto si se encuentra; de lo contrario, null.
         */
        public Producto Buscar(string codigo)
        {
            return BuscarRecursivo(raiz, codigo);
        }

        private Producto BuscarRecursivo(NodoAVL nodo, string codigo)
        {
            if (nodo == null || nodo.Payload.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                return nodo?.Payload;

            int comparacion = string.Compare(codigo, nodo.Payload.Codigo, StringComparison.Ordinal);
            if (comparacion < 0)
                return BuscarRecursivo(nodo.HijoIzquierdo, codigo);

            return BuscarRecursivo(nodo.HijoDerecho, codigo);
        }

        public void RecorridoInOrden(Action<Producto> accion)
        {
            InOrdenRecursivo(raiz, accion);
        }

        private void InOrdenRecursivo(NodoAVL nodo, Action<Producto> accion)
        {
            if (nodo != null)
            {
                InOrdenRecursivo(nodo.HijoIzquierdo, accion);
                accion(nodo.Payload);
                InOrdenRecursivo(nodo.HijoDerecho, accion);
            }
        }

        public void Eliminar(string codigo)
        {
            raiz = EliminarRecursivo(raiz, codigo);
        }

        private NodoAVL EliminarRecursivo(NodoAVL raizActual, string codigo)
        {
            if (raizActual == null) return raizActual;

            int comparacion = string.Compare(codigo, raizActual.Payload.Codigo, StringComparison.Ordinal);
            if (comparacion < 0)
                raizActual.HijoIzquierdo = EliminarRecursivo(raizActual.HijoIzquierdo, codigo);
            else if (comparacion > 0)
                raizActual.HijoDerecho = EliminarRecursivo(raizActual.HijoDerecho, codigo);
            else
            {
                if ((raizActual.HijoIzquierdo == null) || (raizActual.HijoDerecho == null))
                {
                    NodoAVL temp = raizActual.HijoIzquierdo ?? raizActual.HijoDerecho;
                    if (temp == null)
                    {
                        temp = raizActual;
                        raizActual = null;
                    }
                    else
                        raizActual = temp;
                }
                else
                {
                    NodoAVL temp = ObtenerNodoMinimo(raizActual.HijoDerecho);
                    raizActual.Payload = temp.Payload;
                    raizActual.HijoDerecho = EliminarRecursivo(raizActual.HijoDerecho, temp.Payload.Codigo);
                }
            }

            if (raizActual == null) return raizActual;

            raizActual.Altura = Math.Max(ObtenerAltura(raizActual.HijoIzquierdo), ObtenerAltura(raizActual.HijoDerecho)) + 1;
            int balance = CalcularFactorBalance(raizActual);

            if (balance > 1 && CalcularFactorBalance(raizActual.HijoIzquierdo) >= 0)
                return RotarDerecha(raizActual);

            if (balance > 1 && CalcularFactorBalance(raizActual.HijoIzquierdo) < 0)
            {
                raizActual.HijoIzquierdo = RotarIzquierda(raizActual.HijoIzquierdo);
                return RotarDerecha(raizActual);
            }

            if (balance < -1 && CalcularFactorBalance(raizActual.HijoDerecho) <= 0)
                return RotarIzquierda(raizActual);

            if (balance < -1 && CalcularFactorBalance(raizActual.HijoDerecho) > 0)
            {
                raizActual.HijoDerecho = RotarDerecha(raizActual.HijoDerecho);
                return RotarIzquierda(raizActual);
            }

            return raizActual;
        }

        private NodoAVL ObtenerNodoMinimo(NodoAVL nodo)
        {
            NodoAVL actual = nodo;
            while (actual.HijoIzquierdo != null)
                actual = actual.HijoIzquierdo;
            return actual;
        }
    }


    // =========================================================================
    // MÓDULO 2: COLA DE PRIORIDAD MEDIANTE HEAP
    // =========================================================================

    /**
     * Representa un cliente en espera para ser atendido en caja, almacenando
     * su prioridad, categoría, cantidad de productos y hora de llegada.
     */
    public class Cliente
    {
        public string NombreCliente { get; set; }
        public int PesoPrioridad { get; set; }
        public string TipoCliente { get; set; }
        public int CantidadProductos { get; set; }
        public DateTime HoraLlegada { get; set; }

        public Cliente(string nombreCliente, int pesoPrioridad, string tipoCliente, int cantidadProductos)
        {
            NombreCliente = nombreCliente;
            PesoPrioridad = pesoPrioridad;
            TipoCliente = tipoCliente;
            CantidadProductos = cantidadProductos;
            HoraLlegada = DateTime.Now;
        }

        public override string ToString()
        {
            return $" | {NombreCliente,-18} | {TipoCliente,-14} | Productos: {CantidadProductos,3} | Hora: {HoraLlegada:HH:mm:ss}";
        }
    }

    /**
     * Administrador de turnos basado en Min-Heap para gestionar la Cola de Prioridad
     * de los clientes según su categoría (VIP, Adulto Mayor, Regular).
     */
    public class ColaPrioridad
    {
        private List<Cliente> listaHeap = new List<Cliente>();

        public int Conteo => listaHeap.Count;
        public bool EstaVacia() => listaHeap.Count == 0;

        /**
         * Agrega un nuevo cliente a la cola de prioridad aplicando el proceso de "Bubble Up".
         * @param nombre Nombre del cliente.
         * @param peso Peso numérico de prioridad (1 para VIP, 2 para Adulto Mayor, 3 para Regular).
         * @param tipo Nombre textual de la categoría.
         * @param productos Cantidad de artículos que lleva el cliente.
         */
        public void Encolar(string nombre, int peso, string tipo, int productos)
        {
            Cliente cliente = new Cliente(nombre, peso, tipo, productos);
            listaHeap.Add(cliente);
            SubirEnHeap(listaHeap.Count - 1);
        }

        /**
         * Extrae y retorna el cliente con mayor prioridad (raíz del Heap) aplicando "Bubble Down".
         * @return Instancia del cliente atendido.
         */
        public Cliente Desencolar()
        {
            if (EstaVacia()) return null;

            Cliente clienteTop = listaHeap[0];
            int ultimoIndice = listaHeap.Count - 1;
            listaHeap[0] = listaHeap[ultimoIndice];
            listaHeap.RemoveAt(ultimoIndice);

            if (listaHeap.Count > 0)
            {
                BajarEnHeap(0);
            }

            return clienteTop;
        }

        private void SubirEnHeap(int indice)
        {
            while (indice > 0)
            {
                int padre = (indice - 1) / 2;
                if (DebeIntercambiar(indice, padre))
                {
                    Intercambiar(indice, padre);
                    indice = padre;
                }
                else
                {
                    break;
                }
            }
        }

        private void BajarEnHeap(int indice)
        {
            int ultimoIndice = listaHeap.Count - 1;
            while (true)
            {
                int hijoIzquierdo = 2 * indice + 1;
                int hijoDerecho = 2 * indice + 2;
                int mejorHijo = indice;

                if (hijoIzquierdo <= ultimoIndice && DebeIntercambiar(hijoIzquierdo, mejorHijo))
                    mejorHijo = hijoIzquierdo;

                if (hijoDerecho <= ultimoIndice && DebeIntercambiar(hijoDerecho, mejorHijo))
                    mejorHijo = hijoDerecho;

                if (mejorHijo != indice)
                {
                    Intercambiar(indice, mejorHijo);
                    indice = mejorHijo;
                }
                else
                {
                    break;
                }
            }
        }

        private bool DebeIntercambiar(int hijoIdx, int padreIdx)
        {
            var hijo = listaHeap[hijoIdx];
            var padre = listaHeap[padreIdx];

            if (hijo.PesoPrioridad < padre.PesoPrioridad) return true;
            if (hijo.PesoPrioridad == padre.PesoPrioridad && hijo.HoraLlegada < padre.HoraLlegada) return true;
            return false;
        }

        private void Intercambiar(int i, int j)
        {
            var temp = listaHeap[i];
            listaHeap[i] = listaHeap[j];
            listaHeap[j] = temp;
        }

        public List<Cliente> ObtenerCopia()
        {
            var clon = new List<Cliente>(listaHeap);
            clon.Sort((a, b) =>
            {
                int comp = a.PesoPrioridad.CompareTo(b.PesoPrioridad);
                if (comp != 0) return comp;
                return a.HoraLlegada.CompareTo(b.HoraLlegada);
            });
            return clon;
        }
    }


    // =========================================================================
    // MOTOR VISUAL NEÓN Y CARTEL ASCII
    // =========================================================================

    public static class VisualizadorNeon
    {
        public static void DibujarBanner()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
    ██████╗  ██████╗ ███╗   ██╗     ██╗██╗   ██╗ █████╗ ███╗   ██╗
    ██╔══██╗██╔═══██╗████╗  ██║     ██║██║   ██║██╔══██╗████╗  ██║
    ██║  ██║██║   ██║██╔██╗ ██║     ██║██║   ██║███████║██╔██╗ ██║
    ██║  ██║██║   ██║██║╚██╗██║██   ██║██║   ██║██╔══██║██║╚██╗██║
    ██████╔╝╚██████╔╝██║ ╚████║╚█████╔╝╚██████╔╝██║  ██║██║ ╚████║
    ╚═════╝  ╚═════╝ ╚═╝  ╚═══╝ ╚════╝  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚════╝");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("    ==============================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("             SISTEMA INTEGRAL DE GESTIÓN DE SUPERMERCADO          ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("    ==============================================================");
            Console.ResetColor();
        }

        public static void MostrarExito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  [OK] {mensaje}");
            Console.ResetColor();
        }

        public static void MostrarAlerta(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n  [AVISO] {mensaje}");
            Console.ResetColor();
        }
    }


    // =========================================================================
    // PROGRAMA PRINCIPAL
    // =========================================================================

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            ArbolAVL inventarioArbol = new ArbolAVL();
            ColaPrioridad colaClientes = new ColaPrioridad();

            CargarDatosIniciales(inventarioArbol, colaClientes);

            int opcion = 0;
            do
            {
                VisualizadorNeon.DibujarBanner();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n  ----------- INVENTARIO (AVL) -----------");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  1. Registrar producto");
                Console.WriteLine("  2. Buscar producto por código");
                Console.WriteLine("  3. Actualizar producto");
                Console.WriteLine("  4. Eliminar producto");
                Console.WriteLine("  5. Mostrar inventario ordenado (In-Orden)");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n  ------ ATENCIÓN DE CLIENTES (HEAP) -----");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  6. Agregar cliente a la cola");
                Console.WriteLine("  7. Atender siguiente cliente");
                Console.WriteLine("  8. Mostrar clientes en espera");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n  --------------- REPORTES ---------------");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  9. Mostrar estadísticas del sistema");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n  --------------- SISTEMA ----------------");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  10. Salir");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\n  Seleccione una opción numérica -> ");
                Console.ResetColor();

                if (!int.TryParse(Console.ReadLine(), out opcion))
                {
                    VisualizadorNeon.MostrarAlerta("Ingrese un valor numérico válido.");
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }

                switch (opcion)
                {
                    case 1:
                        ProcesarInsertar(inventarioArbol);
                        break;
                    case 2:
                        ProcesarBuscar(inventarioArbol);
                        break;
                    case 3:
                        ProcesarActualizar(inventarioArbol);
                        break;
                    case 4:
                        ProcesarEliminar(inventarioArbol);
                        break;
                    case 5:
                        ProcesarMostrarInventario(inventarioArbol);
                        break;
                    case 6:
                        ProcesarEncolarCliente(colaClientes);
                        break;
                    case 7:
                        ProcesarAtenderCliente(colaClientes);
                        break;
                    case 8:
                        ProcesarMostrarCola(colaClientes);
                        break;
                    case 9:
                        ProcesarEstadisticas(inventarioArbol, colaClientes);
                        break;
                    case 10:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("\n  ¡Saliendo del sistema de Don Juan Market con éxito!");
                        Console.ResetColor();
                        break;
                    default:
                        VisualizadorNeon.MostrarAlerta("Opción fuera de rango.");
                        Pausar();
                        break;
                }

            } while (opcion != 10);
        }

        static void CargarDatosIniciales(ArbolAVL arbol, ColaPrioridad cola)
        {
            string mensajeAux;
            arbol.Insertar(new Producto("PROD001", "Aceite de Oliva", "Alimentos", 45.99, 50), out mensajeAux);
            arbol.Insertar(new Producto("PROD002", "Leche Entera", "Bebidas", 12.50, 100), out mensajeAux);

            cola.Encolar("Juan Pérez", 1, "VIP", 5);
            cola.Encolar("María López", 2, "ADULTO_MAYOR", 3);
            cola.Encolar("Carlos Gómez", 3, "REGULAR", 8);
        }

        static string SolicitarCodigoProducto()
        {
            Console.Write("  Número de código (ej: escriba 1 para PROD001): ");
            if (int.TryParse(Console.ReadLine(), out int num))
            {
                return $"PROD{num:D3}";
            }
            return "PROD001";
        }

        static void ProcesarInsertar(ArbolAVL arbol)
        {
            VisualizadorNeon.DibujarBanner();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  === REGISTRAR NUEVO PRODUCTO ===");
            Console.ResetColor();

            string codigo = SolicitarCodigoProducto();

            var existente = arbol.Buscar(codigo);
            if (existente != null)
            {
                VisualizadorNeon.MostrarAlerta($"Error: Ya existe un producto con el código {codigo}.");
                Pausar();
                return;
            }

            string nombre;
            do
            {
                Console.Write("  Nombre del producto: ");
                nombre = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(nombre))
                {
                    VisualizadorNeon.MostrarAlerta("El nombre no puede estar vacío. Intente de nuevo.");
                }
            } while (string.IsNullOrEmpty(nombre));

            string categoria;
            do
            {
                Console.Write("  Categoría / Departamento: ");
                categoria = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(categoria))
                {
                    VisualizadorNeon.MostrarAlerta("La categoría no puede estar vacía. Intente de nuevo.");
                }
            } while (string.IsNullOrEmpty(categoria));

            double precio;
            Console.Write("  Precio (Qtz., ej: 45.99): ");
            if (!double.TryParse(Console.ReadLine(), out precio) || precio < 0) precio = 0;

            int stock;
            Console.Write("  Cantidad en Stock: ");
            if (!int.TryParse(Console.ReadLine(), out stock) || stock < 0) stock = 0;

            var nuevoProducto = new Producto(codigo, nombre, categoria, precio, stock);
            string mensajeResultado;
            arbol.Insertar(nuevoProducto, out mensajeResultado);

            VisualizadorNeon.MostrarExito(mensajeResultado);
            Pausar();
        }

        static void ProcesarBuscar(ArbolAVL arbol)
        {
            VisualizadorNeon.DibujarBanner();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  === BÚSQUEDA DE PRODUCTO ===");
            Console.ResetColor();

            string codigo = SolicitarCodigoProducto();
            var item = arbol.Buscar(codigo);
            if (item != null)
            {
                VisualizadorNeon.MostrarExito("Producto encontrado:");
                Console.WriteLine($"    Código    : {item.Codigo}");
                Console.WriteLine($"    Nombre    : {item.Nombre}");
                Console.WriteLine($"    Categoría : {item.Categoria}");
                Console.WriteLine($"    Precio    : Q{item.Precio:N2}");
                Console.WriteLine($"    Stock     : {item.Stock}");
            }
            else
            {
                VisualizadorNeon.MostrarAlerta("Producto no encontrado.");
            }
            Pausar();
        }

        static void ProcesarActualizar(ArbolAVL arbol)
        {
            VisualizadorNeon.DibujarBanner();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  === ACTUALIZAR PRODUCTO ===");
            Console.ResetColor();

            string codigo = SolicitarCodigoProducto();
            var item = arbol.Buscar(codigo);
            if (item != null)
            {
                Console.WriteLine($"  [Modificando el producto {codigo}. El código no se puede modificar]");

                string nombre;
                do
                {
                    Console.Write("  Nuevo Nombre: ");
                    nombre = Console.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(nombre)) VisualizadorNeon.MostrarAlerta("El nombre no puede estar vacío.");
                } while (string.IsNullOrEmpty(nombre));

                string categoria;
                do
                {
                    Console.Write("  Nueva Categoría: ");
                    categoria = Console.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(categoria)) VisualizadorNeon.MostrarAlerta("La categoría no puede estar vacía.");
                } while (string.IsNullOrEmpty(categoria));

                Console.Write("  Nuevo Precio (> 0): ");
                double.TryParse(Console.ReadLine(), out double precio);
                Console.Write("  Nuevo Stock (>= 0): ");
                int.TryParse(Console.ReadLine(), out int stock);

                arbol.ActualizarDirecto(codigo, nombre, categoria, precio, stock);
                VisualizadorNeon.MostrarExito("Producto actualizado correctamente.");
                Console.WriteLine($"    Código    : {codigo}");
                Console.WriteLine($"    Nombre    : {nombre}");
                Console.WriteLine($"    Categoría : {categoria}");
                Console.WriteLine($"    Precio    : Q{precio:N2}");
                Console.WriteLine($"    Stock     : {stock}");
            }
            else
            {
                VisualizadorNeon.MostrarAlerta($"Error: No existe un producto con el código {codigo}.");
            }
            Pausar();
        }

        static void ProcesarEliminar(ArbolAVL arbol)
        {
            VisualizadorNeon.DibujarBanner();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  === ELIMINAR PRODUCTO ===");
            Console.ResetColor();

            string codigo = SolicitarCodigoProducto();
            var item = arbol.Buscar(codigo);
            if (item != null)
            {
                arbol.Eliminar(codigo);
                VisualizadorNeon.MostrarExito($"Producto {codigo} eliminado correctamente.");
            }
            else
            {
                VisualizadorNeon.MostrarAlerta($"Error: No existe un producto con el código {codigo}.");
            }
            Pausar();
        }

        static void ProcesarMostrarInventario(ArbolAVL arbol)
        {
            VisualizadorNeon.DibujarBanner();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ================================================================");
            Console.WriteLine("                   INVENTARIO DEL SUPERMERCADO                    ");
            Console.WriteLine("  ================================================================");
            Console.ResetColor();

            if (arbol.EstaVacio())
            {
                Console.WriteLine("\n  El inventario se encuentra vacío.");
            }
            else
            {
                Console.WriteLine($"  {"Código",-10} {"Nombre",-22} {"Categoría",-12} {"Precio",-10} {"Stock",-6}");
                Console.WriteLine(new string('-', 64));
                arbol.RecorridoInOrden(item => Console.WriteLine($"  {item}"));
            }
            Pausar();
        }

        static void ProcesarEncolarCliente(ColaPrioridad cola)
        {
            VisualizadorNeon.DibujarBanner();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  === REGISTRAR CLIENTE EN COLA DE PRIORIDAD ===");
            Console.ResetColor();

            string nombre;
            do
            {
                Console.Write("  Nombre del cliente: ");
                nombre = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(nombre))
                {
                    VisualizadorNeon.MostrarAlerta("El nombre del cliente no puede estar vacío. Intente de nuevo.");
                }
            } while (string.IsNullOrEmpty(nombre));

            Console.WriteLine("\n  Seleccione Tipo de Cliente:");
            Console.WriteLine("    1. VIP (Prioridad 1 - Premium)");
            Console.WriteLine("    2. ADULTO_MAYOR (Prioridad 2 - Mayores de 65 años)");
            Console.WriteLine("    3. REGULAR (Prioridad 3 - Estándar)");
            Console.Write("  Opción (1-3): ");
            if (!int.TryParse(Console.ReadLine(), out int tipoOpcion)) tipoOpcion = 3;

            Console.Write("  Cantidad de productos que va a comprar: ");
            if (!int.TryParse(Console.ReadLine(), out int productos) || productos <= 0) productos = 1;

            int peso = 3;
            string tipo = "REGULAR";
            if (tipoOpcion == 1) { peso = 1; tipo = "VIP"; }
            else if (tipoOpcion == 2) { peso = 2; tipo = "ADULTO_MAYOR"; }

            cola.Encolar(nombre, peso, tipo, productos);
            VisualizadorNeon.MostrarExito($"Cliente {nombre} agregado correctamente.");
            Console.WriteLine($"  Posición estimada en la cola: {cola.Conteo}");
            Pausar();
        }

        static void ProcesarAtenderCliente(ColaPrioridad cola)
        {
            VisualizadorNeon.DibujarBanner();
            var siguiente = cola.Desencolar();
            if (siguiente != null)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ========================================");
                Console.WriteLine("            ATENDIENDO CLIENTE            ");
                Console.WriteLine("  ========================================");
                Console.ResetColor();
                Console.WriteLine($"  Nombre    : {siguiente.NombreCliente}");
                Console.WriteLine($"  Tipo      : {siguiente.TipoCliente}");
                Console.WriteLine($"  Productos : {siguiente.CantidadProductos}");
                Console.WriteLine($"  Ingreso   : {siguiente.HoraLlegada:HH:mm:ss}");
                VisualizadorNeon.MostrarExito("Cliente atendido correctamente.");
            }
            else
            {
                VisualizadorNeon.MostrarAlerta("No hay clientes en espera actualmente.");
            }
            Pausar();
        }

        static void ProcesarMostrarCola(ColaPrioridad cola)
        {
            VisualizadorNeon.DibujarBanner();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ================================================================");
            Console.WriteLine("                     === CLIENTES EN ESPERA ===                   ");
            Console.WriteLine("  ================================================================");
            Console.ResetColor();

            if (cola.EstaVacia())
            {
                Console.WriteLine("\n  No hay clientes en espera.");
            }
            else
            {
                var copia = cola.ObtenerCopia();
                int pos = 1;
                foreach (var ticket in copia)
                {
                    Console.WriteLine($"  Posición {pos++}: {ticket}");
                }
            }
            Pausar();
        }

        static void ProcesarEstadisticas(ArbolAVL arbol, ColaPrioridad cola)
        {
            VisualizadorNeon.DibujarBanner();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("  === REPORTE Y ESTADÍSTICAS DEL SISTEMA ===");
            Console.ResetColor();
            int contadorProductos = 0;
            arbol.RecorridoInOrden(_ => contadorProductos++);

            Console.WriteLine($"  • Total de productos en inventario (Árbol AVL): {contadorProductos}");
            Console.WriteLine($"  • Clientes en fila de espera (Heap de Prioridad): {cola.Conteo}");
            Console.WriteLine("  • Estado general del sistema: Operativo, balanceado y sincronizado.");
            Pausar();
        }

        static void Pausar()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n  Presione cualquier tecla para continuar...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}