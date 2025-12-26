#!/bin/bash

# Script de inicio rápido para ServerCloudStore
# Este script facilita el inicio del proyecto en modo desarrollo

set -e

echo "🚀 ServerCloudStore - Inicio Rápido"
echo "===================================="
echo ""

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Función para imprimir mensajes de éxito
success() {
    echo -e "${GREEN}✓${NC} $1"
}

# Función para imprimir mensajes de error
error() {
    echo -e "${RED}✗${NC} $1"
}

# Función para imprimir mensajes de advertencia
warning() {
    echo -e "${YELLOW}!${NC} $1"
}

# Verificar si Docker está instalado
check_docker() {
    if ! command -v docker &> /dev/null; then
        error "Docker no está instalado"
        echo "Por favor instala Docker desde: https://www.docker.com/get-started"
        exit 1
    fi
    success "Docker está instalado"
}

# Verificar si Docker Compose está instalado
check_docker_compose() {
    if ! command -v docker-compose &> /dev/null; then
        error "Docker Compose no está instalado"
        echo "Por favor instala Docker Compose desde: https://docs.docker.com/compose/install/"
        exit 1
    fi
    success "Docker Compose está instalado"
}

# Verificar si .NET está instalado
check_dotnet() {
    if ! command -v dotnet &> /dev/null; then
        warning ".NET SDK no está instalado"
        echo "Para desarrollo local, instala .NET 8 desde: https://dotnet.microsoft.com/download"
        return 1
    fi
    success ".NET SDK está instalado"
    return 0
}

# Verificar si Node.js está instalado
check_node() {
    if ! command -v node &> /dev/null; then
        warning "Node.js no está instalado"
        echo "Para desarrollo local, instala Node.js desde: https://nodejs.org/"
        return 1
    fi
    success "Node.js está instalado"
    return 0
}

# Menú principal
show_menu() {
    echo ""
    echo "Selecciona una opción:"
    echo "1) 🐳 Iniciar con Docker (Recomendado)"
    echo "2) 💻 Iniciar en modo desarrollo local"
    echo "3) 🛑 Detener servicios Docker"
    echo "4) 🗑️  Limpiar contenedores y volúmenes"
    echo "5) 📊 Ver logs de servicios"
    echo "6) ❌ Salir"
    echo ""
    read -p "Opción: " choice
}

# Iniciar con Docker
start_docker() {
    echo ""
    echo "🐳 Iniciando servicios con Docker..."
    echo ""
    
    check_docker
    check_docker_compose
    
    echo ""
    echo "Construyendo imágenes..."
    docker-compose build
    
    echo ""
    echo "Iniciando contenedores..."
    docker-compose up -d
    
    echo ""
    success "Servicios iniciados correctamente"
    echo ""
    echo "📍 URLs disponibles:"
    echo "   Frontend:  http://localhost:3000"
    echo "   Backend:   http://localhost:5000"
    echo "   Swagger:   http://localhost:5000/swagger"
    echo "   PostgreSQL: localhost:5432"
    echo ""
    echo "👤 Credenciales de prueba:"
    echo "   Usuario:    admin"
    echo "   Contraseña: admin123"
    echo ""
    echo "Para ver los logs: docker-compose logs -f"
}

# Iniciar en modo desarrollo
start_dev() {
    echo ""
    echo "💻 Iniciando en modo desarrollo local..."
    echo ""
    
    # Verificar dependencias
    DOTNET_OK=false
    NODE_OK=false
    
    if check_dotnet; then
        DOTNET_OK=true
    fi
    
    if check_node; then
        NODE_OK=true
    fi
    
    if [ "$DOTNET_OK" = false ] || [ "$NODE_OK" = false ]; then
        error "Faltan dependencias necesarias para modo desarrollo"
        exit 1
    fi
    
    # Verificar PostgreSQL
    echo ""
    warning "Asegúrate de que PostgreSQL esté en ejecución en localhost:5432"
    warning "Base de datos: servercloudstore"
    echo ""
    read -p "¿Continuar? (y/n): " confirm
    
    if [ "$confirm" != "y" ]; then
        echo "Cancelado"
        exit 0
    fi
    
    # Iniciar backend en background
    echo ""
    echo "Iniciando backend..."
    cd ServerCloudStore.API
    dotnet restore
    dotnet run &
    BACKEND_PID=$!
    cd ..
    
    # Esperar a que el backend inicie
    sleep 5
    
    # Iniciar frontend
    echo ""
    echo "Iniciando frontend..."
    cd frontend
    npm install
    npm run dev &
    FRONTEND_PID=$!
    cd ..
    
    echo ""
    success "Servicios iniciados en modo desarrollo"
    echo ""
    echo "📍 URLs disponibles:"
    echo "   Frontend:  http://localhost:3000"
    echo "   Backend:   http://localhost:5000"
    echo "   Swagger:   http://localhost:5000/swagger"
    echo ""
    echo "PIDs de procesos:"
    echo "   Backend:  $BACKEND_PID"
    echo "   Frontend: $FRONTEND_PID"
    echo ""
    echo "Para detener los servicios: kill $BACKEND_PID $FRONTEND_PID"
    echo ""
    
    # Esperar a que el usuario presione Ctrl+C
    trap "kill $BACKEND_PID $FRONTEND_PID; exit" INT
    wait
}

# Detener servicios Docker
stop_docker() {
    echo ""
    echo "🛑 Deteniendo servicios Docker..."
    docker-compose down
    success "Servicios detenidos"
}

# Limpiar contenedores y volúmenes
clean_docker() {
    echo ""
    echo "🗑️  Limpiando contenedores y volúmenes..."
    warning "Esto eliminará todos los datos de la base de datos"
    read -p "¿Estás seguro? (y/n): " confirm
    
    if [ "$confirm" = "y" ]; then
        docker-compose down -v
        success "Limpieza completada"
    else
        echo "Cancelado"
    fi
}

# Ver logs
view_logs() {
    echo ""
    echo "📊 Mostrando logs..."
    echo "Presiona Ctrl+C para salir"
    echo ""
    docker-compose logs -f
}

# Bucle principal
while true; do
    show_menu
    
    case $choice in
        1)
            start_docker
            ;;
        2)
            start_dev
            ;;
        3)
            stop_docker
            ;;
        4)
            clean_docker
            ;;
        5)
            view_logs
            ;;
        6)
            echo "Adiós!"
            exit 0
            ;;
        *)
            error "Opción inválida"
            ;;
    esac
done

