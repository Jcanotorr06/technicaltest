import { Separator } from "@/components/atoms";
import { AddTaskForm, TaskCard } from "@/components/organisms";
import { useToken } from "@/context";
import { getTodayTasksQueryOptions } from "@/services";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/today/")({
  component: Today,
  loader: ({ context }) => {
    const { queryClient, tokenContext } = context;
    if (tokenContext.headers.has("Authorization")) {
      queryClient.ensureQueryData(
        getTodayTasksQueryOptions(
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
      crumb: "Tasks due today",
    };
  },
});

function Today() {
  const { headers } = useToken();
  const todayTasksQuery = useSuspenseQuery(
    getTodayTasksQueryOptions(
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
      <h1 className="text-2xl font-bold mb-2">Tasks due today</h1>
      {todayTasksQuery.data?.items.length === 0 ? (
        <>
          <AddTaskForm />
          <div className="flex flex-col items-center justify-center h-full text-center">
            <img src="/travel.png" alt="No tasks due today" />
            <p className="text-sm text-muted-foreground">
              No tasks due today. Enjoy your day!
            </p>
          </div>
        </>
      ) : (
        todayTasksQuery.data?.items.map((task) => (
          <TaskCard key={task.id} task={task} />
        ))
      )}
      {todayTasksQuery.data?.items.length > 0 ? (
        <>
          <Separator className="my-2" />
          <AddTaskForm />
        </>
      ) : null}
    </div>
  );
}
