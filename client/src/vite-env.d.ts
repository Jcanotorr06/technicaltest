/// <reference types="vite/client" />

interface ViteTypeOptions {
	strictImportMetaEnv: unknown;
}

interface ImportMetaEnv {
	readonly VITE_API_URL: string;
	readonly VITE_FUNCTIONS_KEY: string;
	readonly VITE_APIM_KEY: string;
	readonly VITE_CLIENT_ID: string;
	readonly VITE_CLIENT_AUTHORITY: string;
	readonly VITE_REDIRECT_URI: string;
	readonly VITE_LOGOUT_URI: string;
}

interface ImportMeta {
	readonly env: ImportMetaEnv;
}
