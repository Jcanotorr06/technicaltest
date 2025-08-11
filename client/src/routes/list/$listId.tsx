import { Separator } from "@/components/atoms";
import { AddTaskForm, TaskCard } from "@/components/organisms";
import { useToken } from "@/context";
import { getListQueryOptions, getListTasksQueryOptions } from "@/services";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/list/$listId")({
	component: List,
	loader: ({ context, params }) => {
		const { queryClient, tokenContext } = context;
		if (tokenContext.headers.has("Authorization")) {
			const listPromise = queryClient.ensureQueryData(
				getListQueryOptions(params.listId, tokenContext.headers),
			);
			queryClient.ensureQueryData(
				getListTasksQueryOptions(
					params.listId,
					{
						limit: 100,
						offset: 0,
						sortBy: "order",
						order: "desc",
					},
					tokenContext.headers,
				),
			);
			return {
				crumb: listPromise.then((list) => list.name),
			};
		}
		return {
			crumb: "",
		};
	},
});

function List() {
	const { headers } = useToken();
	const { listId } = Route.useParams();
	const listQuery = useSuspenseQuery(getListQueryOptions(listId, headers));
	const listTasksQuery = useSuspenseQuery(
		getListTasksQueryOptions(
			listId,
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
			<h1 className="text-2xl font-bold mb-2">{listQuery.data?.name}</h1>
			{listTasksQuery.data?.items.map((task) => (
				<TaskCard key={task.id} task={task} />
			))}
			{listTasksQuery.data?.items.length > 0 ? (
				<Separator className="my-2" />
			) : null}
			<AddTaskForm key={listId} />
		</div>
	);
}
