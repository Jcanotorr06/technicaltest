import { type ClassValue, clsx } from "clsx";
import { twMerge } from "tailwind-merge";

/**
 * Combines class names into a single string.
 *
 * Utilizes clsx and tailwind-merge for class name management.
 *
 * @param inputs - The class names to combine.
 * @returns The combined class names.
 *
 * @example
 * // Combining class names
 * const buttonClass = cn("btn", { "btn-primary": isPrimary });
 *
 */
export function cn(...inputs: ClassValue[]) {
	return twMerge(clsx(inputs));
}
