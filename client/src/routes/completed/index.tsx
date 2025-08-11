import { Separator } from "@/components/atoms";
import { TaskCard } from "@/components/organisms";
import { useToken } from "@/context";
import { getCompletedTasksQueryOptions } from "@/services";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/completed/")({
	component: Completed,
	loader: ({ context }) => {
		const { queryClient, tokenContext } = context;
		if (tokenContext.headers.has("Authorization")) {
			queryClient.ensureQueryData(
				getCompletedTasksQueryOptions(
					{
						limit: 100,
						offset: 0,
						sortBy: "order",
						order: "desc",
					},
					tokenContext.headers,
				),
			);
		}
		return {
			crumb: "Completed Tasks",
		};
	},
});

function Completed() {
	const { headers } = useToken();
	const completedTasksQuery = useSuspenseQuery(
		getCompletedTasksQueryOptions(
			{
				limit: 100,
				offset: 0,
				sortBy: "order",
				order: "desc",
			},
			headers,
		),
	);

	return (
		<div className="flex flex-col flex-1 mx-auto px-2 md:px-14 w-full max-w-[800px]">
			<h1 className="text-2xl font-bold mb-2">Completed Tasks</h1>
			{completedTasksQuery.data?.items.length === 0 ? (
				<div className="flex flex-col items-center justify-center h-full text-center">
					<img src="/cup.png" alt="No completed tasks" />
					<p className="text-sm text-muted-foreground">No tasks to display</p>
				</div>
			) : (
				completedTasksQuery.data?.items.map((task) => (
					<TaskCard key={task.id} task={task} />
				))
			)}
			{completedTasksQuery.data?.items.length > 0 ? (
				<Separator className="my-2" />
			) : null}
		</div>
	);
}
