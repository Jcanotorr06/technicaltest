import { Separator } from "@/components/atoms";
import { AddTaskForm, TaskCard } from "@/components/organisms";
import { useToken } from "@/context";
import { getUpcomingTasksQueryOptions } from "@/services";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/upcoming/")({
  component: Upcoming,
  loader: ({ context }) => {
    const { queryClient, tokenContext } = context;
    if (tokenContext.headers.has("Authorization")) {
      queryClient.ensureQueryData(
        getUpcomingTasksQueryOptions(
          {
            limit: 100,
            offset: 0,
            sortBy: "order",
            order: "desc",
          },
          tokenContext.headers
        )
      );
    }
    return {
      crumb: "Upcoming Tasks",
    };
  },
});

function Upcoming() {
  const { headers } = useToken();
  const upcomingTasksQuery = useSuspenseQuery(
    getUpcomingTasksQueryOptions(
      {
        limit: 100,
        offset: 0,
        sortBy: "order",
        order: "desc",
      },
      headers
    )
  );

  return (
    <div className="flex flex-col flex-1 mx-auto px-2 md:px-14 w-full max-w-[800px]">
      <h1 className="text-2xl font-bold mb-2">Upcoming Tasks</h1>
      {upcomingTasksQuery.data?.items.map((task) => (
        <TaskCard key={task.id} task={task} />
      ))}
      {upcomingTasksQuery.data?.items.length > 0 ? (
        <Separator className="my-2" />
      ) : null}
      <AddTaskForm />
    </div>
  );
}
