#!/bin/bash

# Script para ejecutar tests con cobertura de código

echo "=========================================="
echo "  Tests Unitarios con Cobertura"
echo "=========================================="

# Limpiar resultados previos
rm -rf TestResults

# Ejecutar tests unitarios con cobertura
echo ""
echo "Ejecutando tests unitarios..."
dotnet test ServerCloudStore.Tests.Unit/ServerCloudStore.Tests.Unit.csproj \
    --collect:"XPlat Code Coverage" \
    --results-directory ./TestResults/Unit \
    --verbosity minimal

echo ""
echo "=========================================="
echo "  Reporte de Cobertura"
echo "=========================================="

# Instalar reportgenerator si no existe
if ! command -v reportgenerator &> /dev/null
then
    echo "Instalando reportgenerator..."
    dotnet tool install --global dotnet-reportgenerator-globaltool
fi

# Generar reporte HTML
echo ""
echo "Generando reporte HTML de cobertura..."
reportgenerator \
    -reports:"TestResults/**/coverage.cobertura.xml" \
    -targetdir:"TestResults/CoverageReport" \
    -reporttypes:"Html;TextSummary"

# Mostrar resumen
echo ""
echo "=========================================="
echo "  Resumen de Cobertura"
echo "=========================================="
cat TestResults/CoverageReport/Summary.txt

echo ""
echo "=========================================="
echo "Reporte completo disponible en:"
echo "TestResults/CoverageReport/index.html"
echo "=========================================="

# Abrir reporte en el navegador (opcional, comentado por defecto)
# open TestResults/CoverageReport/index.html

