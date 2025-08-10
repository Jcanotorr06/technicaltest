import "./index.css";

import { QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { createRouter, RouterProvider } from "@tanstack/react-router";
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { Authorization } from "./components/organisms";
import { queryClient } from "./config";
import { AuthProvider, TokenProvider } from "./context";

// Import the generated route tree
import { routeTree } from "./routeTree.gen";

// Create a new router instance
const router = createRouter({ routeTree });

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
              <RouterProvider router={router} />
              {import.meta.env.DEV ? (
                <ReactQueryDevtools initialIsOpen={false} />
              ) : null}
            </QueryClientProvider>
          </Authorization>
        </TokenProvider>
      </AuthProvider>
    </StrictMode>
  );
}
