import "./index.css";

import { QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { createRouter } from "@tanstack/react-router";
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { Authorization } from "./components/organisms";
import { queryClient } from "./config";
import { AuthProvider, TokenProvider } from "./context";

// Import the generated route tree
import App from "./App";
import { routeTree } from "./routeTree.gen";
import { Toaster } from "./components/atoms";

// Create a new router instance
export const router = createRouter({
	routeTree,
	defaultPreload: "intent",
	defaultPreloadStaleTime: 0,
	scrollRestoration: true,
	context: {
		queryClient,
		// biome-ignore lint/style/noNonNullAssertion: Initial value is undefined because the TokenProvider is not yet mounted
		tokenContext: undefined!,
	},
});

// Register the router instance for type safety
declare module "@tanstack/react-router" {
	interface Register {
		router: typeof router;
	}
}

const rootElement = document.getElementById("root");

// Render the app
if (rootElement && !rootElement?.innerHTML) {
	const root = createRoot(rootElement);
	root.render(
		<StrictMode>
			<AuthProvider>
				<TokenProvider>
					<Authorization>
						<QueryClientProvider client={queryClient}>
							<App />
							<Toaster />
							{import.meta.env.DEV ? (
								<ReactQueryDevtools initialIsOpen={false} />
							) : null}
						</QueryClientProvider>
					</Authorization>
				</TokenProvider>
			</AuthProvider>
		</StrictMode>,
	);
}
