import { useToken } from "@/context";
import type {
	CreateTaskRequest,
	PagedList,
	Task,
	UpdateTaskRequest,
} from "@/types";
import {
	mutationOptions,
	type QueryClient,
	queryOptions,
	useMutation,
	useQuery,
	useQueryClient,
} from "@tanstack/react-query";
import { toast } from "sonner";

type QueryParams<T> = {
	limit: number;
	offset: number;
	sortBy: keyof T;
	order: "asc" | "desc";
};

export const getTaskQueryOptions = (id: string, headers: Headers) =>
	queryOptions({
		queryKey: ["task", id],
		queryFn: async () => {
			const url = `${import.meta.env.VITE_API_URL}/tasks/${id}`;
			const request = new Request(url, {
				method: "GET",
				headers,
			});
			const response = await fetch(request);
			if (!response.ok) {
				throw new Error("Failed to fetch task");
			}
			return response.json() as Promise<Task>;
		},
	});

export const getListTasksQueryOptions = (
	id: string,
	queryParams: QueryParams<Task>,
	headers: Headers,
) =>
	queryOptions({
		queryKey: ["tasks", "list", id],
		queryFn: async () => {
			const queryString = new URLSearchParams({
				limit: queryParams.limit.toString(),
				offset: queryParams.offset.toString(),
				sortBy: "Order",
				order: queryParams.order,
			}).toString();
			const url = `${
				import.meta.env.VITE_API_URL
			}/list/${id}/tasks?${queryString}`;
			const request = new Request(url, {
				method: "GET",
				headers,
			});
			const response = await fetch(request);
			if (!response.ok) {
				throw new Error("Failed to fetch tasks");
			}
			return response.json() as Promise<PagedList<Task>>;
		},
	});

export const getTodayTasksQueryOptions = (
	queryParams: QueryParams<Task>,
	headers: Headers,
) =>
	queryOptions({
		queryKey: ["tasks", "today"],
		queryFn: async () => {
			const queryString = new URLSearchParams({
				limit: queryParams.limit.toString(),
				offset: queryParams.offset.toString(),
				sortBy: "Order",
				order: queryParams.order,
			}).toString();
			const url = `${import.meta.env.VITE_API_URL}/tasks/today?${queryString}`;
			const request = new Request(url, {
				method: "GET",
				headers,
			});
			const response = await fetch(request);
			if (!response.ok) {
				throw new Error("Failed to fetch tasks");
			}
			return response.json() as Promise<PagedList<Task>>;
		},
	});

export const getUpcomingTasksQueryOptions = (
	queryParams: QueryParams<Task>,
	headers: Headers,
) =>
	queryOptions({
		queryKey: ["tasks", "upcoming"],
		queryFn: async () => {
			const queryString = new URLSearchParams({
				limit: queryParams.limit.toString(),
				offset: queryParams.offset.toString(),
				sortBy: "Order",
				order: queryParams.order,
			}).toString();
			const url = `${
				import.meta.env.VITE_API_URL
			}/tasks/upcoming?${queryString}`;
			const request = new Request(url, {
				method: "GET",
				headers,
			});
			const response = await fetch(request);
			if (!response.ok) {
				throw new Error("Failed to fetch tasks");
			}
			return response.json() as Promise<PagedList<Task>>;
		},
	});

export const getCompletedTasksQueryOptions = (
	queryParams: QueryParams<Task>,
	headers: Headers,
) =>
	queryOptions({
		queryKey: ["tasks", "completed"],
		queryFn: async () => {
			const queryString = new URLSearchParams({
				limit: queryParams.limit.toString(),
				offset: queryParams.offset.toString(),
				sortBy: "Order",
				order: queryParams.order,
			}).toString();
			const url = `${
				import.meta.env.VITE_API_URL
			}/tasks/completed?${queryString}`;
			const request = new Request(url, {
				method: "GET",
				headers,
			});
			const response = await fetch(request);
			if (!response.ok) {
				throw new Error("Failed to fetch tasks");
			}
			return response.json() as Promise<PagedList<Task>>;
		},
	});

export const getUserTasksQueryOptions = (search: string, headers: Headers) =>
	queryOptions({
		queryKey: ["user", search],
		enabled: !!search && search.length > 0,
		queryFn: async () => {
			const queryString = new URLSearchParams({
				search,
			}).toString();
			const url = `${import.meta.env.VITE_API_URL}/user/tasks?${queryString}`;
			const request = new Request(url, {
				method: "GET",
				headers,
			});
			const response = await fetch(request);
			if (!response.ok) {
				throw new Error("Failed to fetch tasks");
			}
			return response.json() as Promise<Task[]>;
		},
	});

export const createTaskMutationOptions = (
	headers: Headers,
	queryClient: QueryClient,
) =>
	mutationOptions({
		mutationKey: ["createTask"],
		mutationFn: async (data: CreateTaskRequest) => {
			const url = `${import.meta.env.VITE_API_URL}/tasks`;
			const request = new Request(url, {
				method: "POST",
				headers,
				body: JSON.stringify(data),
			});
			const response = await fetch(request);
			if (!response.ok) {
				throw new Error("Failed to create task");
			}
			return response.json() as Promise<Task>;
		},
		onSettled: (data) => {
			queryClient.invalidateQueries({
				queryKey: ["tasks", "list", data?.listId],
			});
			queryClient.invalidateQueries({
				queryKey: ["tasks"],
			});
			queryClient.invalidateQueries({
				queryKey: ["task", data?.id],
			});
		},
		onSuccess: (data) => {
			toast.success(`Task "${data.title}" created successfully`);
		},
		onError: () => {
			toast.error(`Failed to create task`);
		},
	});

export const updateTaskMutationOptions = (
	headers: Headers,
	queryClient: QueryClient,
) =>
	mutationOptions({
		mutationKey: ["updateTask"],
		mutationFn: async (data: UpdateTaskRequest) => {
			const url = `${import.meta.env.VITE_API_URL}/tasks`;
			const request = new Request(url, {
				method: "PUT",
				headers,
				body: JSON.stringify(data),
			});
			const response = await fetch(request);
			if (!response.ok) {
				throw new Error("Failed to update task");
			}
			return response.json() as Promise<Task>;
		},
		onSettled: (data) => {
			queryClient.invalidateQueries({
				queryKey: ["tasks", "list", data?.listId],
			});
			queryClient.invalidateQueries({
				queryKey: ["tasks"],
			});
			queryClient.invalidateQueries({
				queryKey: ["task", data?.id],
			});
		},
		onSuccess: (data) => {
			toast.success(`Task "${data.title}" updated successfully`);
		},
		onError: () => {
			toast.error(`Failed to update task`);
		},
	});

export const completeTaskMutationOptions = (
	id: string,
	headers: Headers,
	queryClient: QueryClient,
) =>
	mutationOptions({
		mutationKey: ["completeTask", id],
		mutationFn: async () => {
			const url = `${import.meta.env.VITE_API_URL}/tasks/${id}/complete`;
			const request = new Request(url, {
				method: "POST",
				headers,
			});
			const response = await fetch(request);
			if (!response.ok) {
				throw new Error("Failed to create task");
			}
			return response.json() as Promise<Task>;
		},
		onSettled: (data) => {
			queryClient.invalidateQueries({
				queryKey: ["tasks", "list", data?.listId],
			});
			queryClient.invalidateQueries({
				queryKey: ["tasks"],
			});
			queryClient.invalidateQueries({
				queryKey: ["task", data?.id],
			});
		},
		onSuccess: (data) => {
			toast.success(`Task "${data.title}" completed successfully`);
		},
		onError: () => {
			toast.error(`Failed to complete task`);
		},
	});

export const deleteTaskMutationOptions = (
	id: string,
	headers: Headers,
	queryClient: QueryClient,
) =>
	mutationOptions({
		mutationKey: ["deleteTask", id],
		mutationFn: async () => {
			const url = `${import.meta.env.VITE_API_URL}/tasks/${id}`;
			const request = new Request(url, {
				method: "DELETE",
				headers,
			});
			const response = await fetch(request);
			if (!response.ok || response.status !== 204) {
				throw new Error("Failed to delete task");
			}
			return response.json() as Promise<Task>;
		},
		onSettled: () => {
			queryClient.invalidateQueries({
				queryKey: ["tasks"],
			});
			queryClient.invalidateQueries({
				queryKey: ["tasks", id],
			});
			queryClient.invalidateQueries({
				queryKey: ["lists"],
			});
		},
		onSuccess: (data) => {
			toast.success(`Task "${data.title}" deleted successfully`);
		},
		onError: () => {
			toast.error(`Failed to delete task`);
		},
	});

/**
 * Custom hook to get a task by ID
 * @param id - The ID of the task to retrieve
 * @returns The task data
 */
export const useGetTask = (id: string) => {
	const { headers } = useToken();
	return useQuery(getTaskQueryOptions(id, headers));
};

/**
 * Custom hook to list tasks for a specific list
 * @param id - The ID of the list to retrieve tasks for
 * @param queryParams - The query parameters for the task list
 * @returns The task list data
 */
export const useListTasks = (id: string, queryParams: QueryParams<Task>) => {
	const { headers } = useToken();
	return useQuery(getListTasksQueryOptions(id, queryParams, headers));
};

/**
 * Custom hook to list upcoming tasks
 * @param queryParams - The query parameters for the upcoming tasks
 * @returns The upcoming tasks data
 */
export const useUpcomingTasks = (queryParams: QueryParams<Task>) => {
	const { headers } = useToken();
	return useQuery(getUpcomingTasksQueryOptions(queryParams, headers));
};

/**
 * Custom hook to list today's tasks
 * @param queryParams - The query parameters for the today's tasks
 * @returns The today's tasks data
 */
export const useTodayTasks = (queryParams: QueryParams<Task>) => {
	const { headers } = useToken();
	return useQuery(getTodayTasksQueryOptions(queryParams, headers));
};

/**
 * Custom hook to list completed tasks
 * @param queryParams - The query parameters for the completed tasks
 * @returns The completed tasks data
 */
export const useCompletedTasks = (queryParams: QueryParams<Task>) => {
	const { headers } = useToken();
	return useQuery(getCompletedTasksQueryOptions(queryParams, headers));
};

export const useUserTasks = (search: string) => {
	const { headers } = useToken();
	return useQuery(getUserTasksQueryOptions(search, headers));
};

/**
 * Custom hook to create a new task
 * @returns The mutation object
 */
export const useCreateTask = () => {
	const { headers } = useToken();
	const queryClient = useQueryClient();
	return useMutation(createTaskMutationOptions(headers, queryClient));
};

/**
 * Custom hook to update a task
 * @returns The mutation object
 */
export const useUpdateTask = () => {
	const { headers } = useToken();
	const queryClient = useQueryClient();
	return useMutation(updateTaskMutationOptions(headers, queryClient));
};

/**
 * Custom hook to complete a task
 * @param id - The ID of the task to complete
 * @returns The mutation object
 */
export const useCompleteTask = (id: string) => {
	const { headers } = useToken();
	const queryClient = useQueryClient();
	return useMutation(completeTaskMutationOptions(id, headers, queryClient));
};

/**
 * Custom hook to delete a task
 * @param id - The ID of the task to delete
 * @returns The mutation object
 */
export const useDeleteTask = (id: string) => {
	const { headers } = useToken();
	const queryClient = useQueryClient();
	return useMutation(deleteTaskMutationOptions(id, headers, queryClient));
};
