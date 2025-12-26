import { z } from 'zod';

/**
 * Esquema de validación para el formulario de login
 */
export const loginSchema = z.object({
  username: z
    .string()
    .min(1, 'El usuario es requerido')
    .min(3, 'El usuario debe tener al menos 3 caracteres')
    .max(50, 'El usuario no puede exceder 50 caracteres')
    .trim(),
  password: z
    .string()
    .min(1, 'La contraseña es requerida')
    .min(6, 'La contraseña debe tener al menos 6 caracteres')
    .max(100, 'La contraseña no puede exceder 100 caracteres'),
});

/**
 * Tipo inferido del esquema de login
 */
export type LoginFormData = z.infer<typeof loginSchema>;

