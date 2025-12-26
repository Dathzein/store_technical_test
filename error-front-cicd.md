Run npm run build

> frontend@0.0.0 build
> tsc -b && vite build

Error: src/__tests__/App.test.tsx(34,3): error TS2304: Cannot find name 'beforeEach'.
Error: src/guards/__tests__/AuthGuard.test.tsx(28,3): error TS2304: Cannot find name 'beforeEach'.
Error: src/mocks/handlers.ts(219,11): error TS6133: 'id' is declared but its value is never read.
Error: src/mocks/handlers.ts(311,11): error TS6133: 'id' is declared but its value is never read.
Error: src/pages/__tests__/ProductDetail.test.tsx(4,25): error TS6133: 'Route' is declared but its value is never read.
Error: src/pages/__tests__/ProductDetail.test.tsx(4,32): error TS6133: 'Routes' is declared but its value is never read.
Error: src/pages/__tests__/ProductDetail.test.tsx(5,1): error TS6133: 'server' is declared but its value is never read.
Error: src/pages/__tests__/ProductDetail.test.tsx(6,1): error TS6192: All imports in import declaration are unused.
Error: src/services/__tests__/categoryService.test.ts(5,15): error TS6196: 'Response' is declared but never used.
Error: src/services/__tests__/categoryService.test.ts(5,25): error TS6196: 'CategoryDto' is declared but never used.
Error: Process completed with exit code 2.