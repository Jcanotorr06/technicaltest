import { useToken } from "@/context";
import type { CreateListRequest, List } from "@/types";
import {
	mutationOptions,
	type QueryClient,
	queryOptions,
	useMutation,
	useQuery,
	useQueryClient,
} from "@tanstack/react-query";
import { toast } from "sonner";

export const getListsQueryOptions = (headers: Headers) =>
	queryOptions({
		queryKey: ["lists"],
		queryFn: async () => {
			console.log("Fetching user lists", headers);
			const url = `${import.meta.env.VITE_API_URL}/user/lists`;
			const request = new Request(url, {
				method: "GET",
				headers: headers,
			});

			const response = await fetch(request);

			if (!response.ok) {
				throw new Error("Failed to fetch lists");
			}
			return response.json() as Promise<List[]>;
		},
	});

export const getPublicListsQueryOptions = (headers: Headers) =>
	queryOptions({
		queryKey: ["publicLists"],
		queryFn: async () => {
			const url = `${import.meta.env.VITE_API_URL}/public/lists`;
			const request = new Request(url, {
				method: "GET",
				headers: headers,
			});

			const response = await fetch(request);

			if (!response.ok) {
				throw new Error("Failed to fetch public lists");
			}
			return response.json() as Promise<List[]>;
		},
	});

export const getListQueryOptions = (id: string, headers: Headers) =>
	queryOptions({
		queryKey: ["list", id],
		queryFn: async () => {
			const url = `${import.meta.env.VITE_API_URL}/lists/${id}`;
			const request = new Request(url, {
				method: "GET",
				headers: headers,
			});

			const response = await fetch(request);

			if (!response.ok) {
				throw new Error("Failed to fetch list");
			}
			return response.json() as Promise<List>;
		},
	});

export const createListMutationOptions = (
	headers: Headers,
	queryClient: QueryClient,
) =>
	mutationOptions({
		mutationKey: ["createList"],
		mutationFn: async (payload: CreateListRequest) => {
			const url = `${import.meta.env.VITE_API_URL}/lists`;
			const request = new Request(url, {
				method: "POST",
				headers: headers,
				body: JSON.stringify(payload),
			});

			const response = await fetch(request);

			if (!response.ok) {
				throw new Error("Failed to create list");
			}
			return response.json() as Promise<List>;
		},
		onSettled: (_data, _error, variables) => {
			queryClient.invalidateQueries({
				queryKey: [variables.isPublic ? "publicLists" : "lists"],
			});
		},
		onSuccess: (data) => {
			toast.success(`List "${data.name}" created successfully!`);
		},
		onError: () => {
			toast.error(`Failed to create list`);
		},
	});

/**
 * Custom hook to fetch lists.
 * @returns {ReturnType<typeof useQuery>} - The resulting query object
 *
 * See https://tanstack.com/query/latest/docs/framework/react/reference/useQuery for API details.
 */
export const useGetLists = () => {
	const { headers } = useToken();
	return useQuery(getListsQueryOptions(headers));
};

/**
 * Custom hook to fetch public lists.
 * @returns {ReturnType<typeof useQuery>} - The resulting query object
 *
 * See https://tanstack.com/query/latest/docs/framework/react/reference/useQuery for API details.
 */
export const useGetPublicLists = () => {
	const { headers } = useToken();
	return useQuery(getPublicListsQueryOptions(headers));
};

/**
 * Custom hook to fetch a single list.
 * @param {string} id The ID of the list to fetch.
 * @returns {ReturnType<typeof useQuery>} - The resulting query object
 *
 * See https://tanstack.com/query/latest/docs/framework/react/reference/useQuery for API details.
 */
export const useGetList = (id: string) => {
	const { headers } = useToken();
	return useQuery(getListQueryOptions(id, headers));
};

/**
 * Custom hook to create a new list.
 * @param {CreateListRequest} payload The data for the new list.
 * @returns {ReturnType<typeof useMutation>} - The resulting mutation object
 *
 * See https://tanstack.com/query/latest/docs/framework/react/reference/useMutation for API details.
 */
export const useCreateList = () => {
	const { headers } = useToken();
	const queryClient = useQueryClient();
	return useMutation(createListMutationOptions(headers, queryClient));
};
