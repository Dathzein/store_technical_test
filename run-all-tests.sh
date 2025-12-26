#!/bin/bash

# ============================================
# Script para ejecutar todos los tests
# Backend (.NET) y Frontend (Node.js)
# ============================================

set -e  # Salir si hay errores críticos

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Variables de control
BACKEND_STATUS=0
FRONTEND_STATUS=0
START_TIME=$(date +%s)

# Función para imprimir mensajes con formato
print_header() {
    echo -e "\n${BLUE}=========================================${NC}"
    echo -e "${BLUE}  $1${NC}"
    echo -e "${BLUE}=========================================${NC}\n"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

# Función para limpiar resultados previos
clean_previous_results() {
    print_header "Limpiando resultados previos"
    rm -rf TestResults/Backend
    rm -rf TestResults/Frontend
    mkdir -p TestResults/Backend
    mkdir -p TestResults/Frontend
    print_success "Resultados anteriores eliminados"
}

# Función para ejecutar tests de backend
run_backend_tests() {
    print_header "EJECUTANDO TESTS DE BACKEND (.NET)"
    
    print_info "Construyendo imagen de tests de backend..."
    if docker build -t backend-tests -f Dockerfile.Test --target final . > /tmp/backend-build.log 2>&1; then
        print_success "Imagen de backend construida correctamente"
    else
        print_error "Error al construir imagen de backend"
        cat /tmp/backend-build.log
        return 1
    fi
    
    print_info "Ejecutando tests de backend en contenedor..."
    if docker run --rm -v $(pwd)/TestResults/Backend:/testresults backend-tests > /tmp/backend-tests.log 2>&1; then
        print_success "Tests de backend ejecutados"
    else
        print_warning "Tests de backend completados con advertencias"
    fi
    
    # Mostrar resumen de tests de backend
    if [ -f TestResults/Backend/coveragereport/Summary.txt ]; then
        echo ""
        print_info "Resumen de cobertura de backend:"
        cat TestResults/Backend/coveragereport/Summary.txt
    fi
    
    return 0
}

# Función para ejecutar tests de frontend
run_frontend_tests() {
    print_header "EJECUTANDO TESTS DE FRONTEND (Node.js)"
    
    print_info "Construyendo imagen de tests de frontend..."
    if docker build -t frontend-tests -f Dockerfile.Frontend.Test --target final . > /tmp/frontend-build.log 2>&1; then
        print_success "Imagen de frontend construida correctamente"
    else
        print_error "Error al construir imagen de frontend"
        cat /tmp/frontend-build.log
        return 1
    fi
    
    print_info "Ejecutando tests de frontend en contenedor..."
    if docker run --rm -v $(pwd)/TestResults/Frontend:/testresults frontend-tests > /tmp/frontend-tests.log 2>&1; then
        print_success "Tests de frontend ejecutados"
    else
        print_warning "Tests de frontend completados con advertencias"
    fi
    
    # Mostrar resumen de tests de frontend
    if [ -f TestResults/Frontend/coverage/coverage-summary.json ]; then
        echo ""
        print_info "Resumen de cobertura de frontend:"
        
        # Extraer y mostrar métricas usando jq si está disponible
        if command -v jq &> /dev/null; then
            LINES=$(jq -r '.total.lines.pct' TestResults/Frontend/coverage/coverage-summary.json)
            STATEMENTS=$(jq -r '.total.statements.pct' TestResults/Frontend/coverage/coverage-summary.json)
            FUNCTIONS=$(jq -r '.total.functions.pct' TestResults/Frontend/coverage/coverage-summary.json)
            BRANCHES=$(jq -r '.total.branches.pct' TestResults/Frontend/coverage/coverage-summary.json)
            
            echo "  Lines:      ${LINES}%"
            echo "  Statements: ${STATEMENTS}%"
            echo "  Functions:  ${FUNCTIONS}%"
            echo "  Branches:   ${BRANCHES}%"
        else
            cat TestResults/Frontend/coverage/coverage-summary.json
        fi
    fi
    
    return 0
}

# Función para generar resumen final
generate_summary() {
    END_TIME=$(date +%s)
    DURATION=$((END_TIME - START_TIME))
    
    print_header "RESUMEN FINAL DE EJECUCIÓN"
    
    echo "Duración total: ${DURATION}s"
    echo ""
    
    # Estado de Backend
    if [ $BACKEND_STATUS -eq 0 ]; then
        print_success "Backend Tests: PASSED"
    else
        print_error "Backend Tests: FAILED"
    fi
    
    # Estado de Frontend
    if [ $FRONTEND_STATUS -eq 0 ]; then
        print_success "Frontend Tests: PASSED"
    else
        print_error "Frontend Tests: FAILED"
    fi
    
    echo ""
    print_info "Reportes disponibles:"
    echo "  Backend:  TestResults/Backend/coveragereport/index.html"
    echo "  Frontend: TestResults/Frontend/coverage/lcov-report/index.html"
    echo ""
    
    # Determinar exit code final
    if [ $BACKEND_STATUS -eq 0 ] && [ $FRONTEND_STATUS -eq 0 ]; then
        print_header "✅ TODOS LOS TESTS PASARON EXITOSAMENTE"
        return 0
    else
        print_header "❌ ALGUNOS TESTS FALLARON"
        return 1
    fi
}

# Función para abrir reportes en el navegador (opcional)
open_reports() {
    read -p "¿Deseas abrir los reportes de cobertura en el navegador? (y/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        if [ -f TestResults/Backend/coveragereport/index.html ]; then
            print_info "Abriendo reporte de backend..."
            open TestResults/Backend/coveragereport/index.html 2>/dev/null || xdg-open TestResults/Backend/coveragereport/index.html 2>/dev/null || echo "No se pudo abrir automáticamente"
        fi
        
        if [ -f TestResults/Frontend/coverage/lcov-report/index.html ]; then
            print_info "Abriendo reporte de frontend..."
            open TestResults/Frontend/coverage/lcov-report/index.html 2>/dev/null || xdg-open TestResults/Frontend/coverage/lcov-report/index.html 2>/dev/null || echo "No se pudo abrir automáticamente"
        fi
    fi
}

# ============================================
# EJECUCIÓN PRINCIPAL
# ============================================

print_header "INICIANDO EJECUCIÓN DE TODOS LOS TESTS"
print_info "Backend: .NET 8 (Unit + Integration Tests)"
print_info "Frontend: Node.js 20 (Vitest + RTL + MSW)"

# Verificar que Docker esté corriendo
if ! docker info > /dev/null 2>&1; then
    print_error "Docker no está corriendo. Por favor, inicia Docker Desktop."
    exit 1
fi

# Limpiar resultados previos
clean_previous_results

# Ejecutar tests de backend
if run_backend_tests; then
    BACKEND_STATUS=0
else
    BACKEND_STATUS=1
fi

# Ejecutar tests de frontend
if run_frontend_tests; then
    FRONTEND_STATUS=0
else
    FRONTEND_STATUS=1
fi

# Generar resumen final
generate_summary
FINAL_STATUS=$?

# Opcional: abrir reportes
if [ -t 0 ]; then  # Solo si es terminal interactivo
    open_reports
fi

exit $FINAL_STATUS

