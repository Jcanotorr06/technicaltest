import { QueryClient } from "@tanstack/react-query";

const queryClient = new QueryClient({
	defaultOptions: {
		queries: {
			staleTime: 1000 * 60 * 5, // 5 minutes
			refetchInterval: 1000 * 60, // 1 minute
			throwOnError: false,
			refetchOnWindowFocus: true,
			refetchOnReconnect: true,
		},
		mutations: {
			throwOnError: false,
		},
	},
});

export { queryClient };
