import { QueryClient } from "@tanstack/react-query";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutes
      throwOnError: false,
      refetchOnWindowFocus: true,
      refetchOnReconnect: true,
    },
  },
});

export { queryClient };
